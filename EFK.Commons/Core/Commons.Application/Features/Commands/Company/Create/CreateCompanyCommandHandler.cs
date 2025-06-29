using AutoMapper;
using Commons.Application.Repositories.Commands;
using Commons.Application.Repositories.Queries;
using Commons.Domain.Models;
using Commons.Domain.Models.Companies;
using Commons.Domain.MongoFile;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Commons.Application.Features.Commands.Company.Create
{
    public class CreateCompanyCommandHandler<TDbContext>(
        IWriteRepository<TDbContext, Commons.Domain.Models.Companies.Company> writeRepository,
        IWriteRepository<TDbContext, Commons.Domain.MongoFile.FileMetaData> fileMetaDataWriteRepository,
         IReadRepository<TDbContext, CompanyFile> companyFileReadRepository,
        IReadRepository<TDbContext, Commons.Domain.Models.Companies.Company> readRepository,
        IReadRepository<TDbContext, Commons.Domain.MongoFile.FileMetaData> fileMetaDataReadRepository,
        IMongoWriteRepository mongoWriteRepository,
        IMapper mapper,
        IWriteRepository<TDbContext, CompanyFile> companyFileWriteRepository

        ) : IRequestHandler<CreateCompanyCommandRequest, BaseResponse> where TDbContext : DbContext
    {
        readonly IWriteRepository<TDbContext, Commons.Domain.Models.Companies.Company> writeRepository = writeRepository;
        readonly IReadRepository<TDbContext, Commons.Domain.Models.Companies.Company> readRepository = readRepository;
        readonly IMapper mapper = mapper;
        readonly IMongoWriteRepository mongoWriteRepository = mongoWriteRepository;
        readonly IWriteRepository<TDbContext, CompanyFile> companyFileWriteRepository = companyFileWriteRepository;
        readonly IReadRepository<TDbContext, CompanyFile> companyFileReadRepository = companyFileReadRepository;
        readonly IWriteRepository<TDbContext, Commons.Domain.MongoFile.FileMetaData> fileMetaDataWriteRepository = fileMetaDataWriteRepository;
        readonly IReadRepository<TDbContext, Commons.Domain.MongoFile.FileMetaData> fileMetaDataReadRepository = fileMetaDataReadRepository;

        public async Task<BaseResponse> Handle(CreateCompanyCommandRequest request, CancellationToken cancellationToken)
        {
            try
            {
                var company = this.mapper.Map<Commons.Domain.Models.Companies.Company>(request);
                var isCompanyExists = await this.readRepository
                    .GetAll().Where(c => c.Id == Guid.Parse(request.Id))
                    .FirstOrDefaultAsync(cancellationToken);
                if (isCompanyExists != null)
                {
                    var isUpdate = company.Id == isCompanyExists.Id;

                    if (isUpdate)
                    {
                        var companyId = company.Id;

                        var existingFileMetas = await this.fileMetaDataReadRepository
                            .GetWhere(f => f.ReferenceId == companyId)
                            .ToListAsync(cancellationToken);

                        foreach (var fileMeta in existingFileMetas)
                        {
                            await this.mongoWriteRepository.DeleteFileAsync<FileMetaData>(fileMeta.ObjectId.ToString());
                        }

                        if (existingFileMetas.Any())
                            await this.fileMetaDataWriteRepository.RemoveAsync(existingFileMetas);

                        var existingCompanyFiles = await this.companyFileReadRepository
                            .GetWhere(cf => cf.CompanyId == companyId)
                            .ToListAsync(cancellationToken);

                        if (existingCompanyFiles.Any())
                            await this.companyFileWriteRepository.RemoveAsync(existingCompanyFiles);
                    }
                }
                if (request.LogoUrl != null)
                {
                    var file = await this.mongoWriteRepository.UploadFileAsync<FileMetaData>(new FileMetaData
                    {
                        FileName = request.LogoUrl.FileName,
                        ContentType = request.LogoUrl.ContentType,
                        UploadedAt = DateTime.UtcNow,
                        ReferenceId = company.Id,
                        ReferenceType = company.GetType().Name,
                        Tag = "Logo",
                        Description = request.LogoUrl.ContentDisposition
                    });

                    var metadata = new FileMetaData
                    {
                        Id = Guid.NewGuid(),
                        ObjectId = file.ToString(),
                        ReferenceId = company.Id,
                        ReferenceType = company.GetType().Name,
                        FileName = request.LogoUrl.FileName,
                        ContentType = request.LogoUrl.ContentType,
                        UploadedAt = DateTime.UtcNow,
                        Tag = "Logo",
                        Description = request.LogoUrl.ContentDisposition
                    };

                    await this.fileMetaDataWriteRepository.AddOrUpdateBulkAsync(new List<FileMetaData> { metadata });

                    var companyFile = new CompanyFile
                    {
                        CompanyId = company.Id,
                        FileId = file,
                        FileType = "Logo"
                    };

                    await this.companyFileWriteRepository.AddOrUpdateBulkAsync(new List<CompanyFile> { companyFile });
                }

                return await this.writeRepository.AddOrUpdateBulkAsync(new List<Commons.Domain.Models.Companies.Company> { company });
            }
            catch (Exception ex)
            {
                return new BaseResponse
                {
                    Succeeded = false,
                    Message = $"Firma kaydı sırasında hata: {ex.Message}"
                };
            }
        }
    }
}