using AutoMapper;
using Commons.Application.Repositories.Commands;
using Commons.Application.Repositories.Queries;
using Commons.Domain.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Commons.Application.Features.Commands.Company.Create
{
    public class CreateCompanyCommandHandler<TDbContext>(
        IWriteRepository<TDbContext, Commons.Domain.Models.Companies.Company> writeRepository,
        IReadRepository<TDbContext, Commons.Domain.Models.Companies.Company> readRepository,
        IMongoWriteRepository mongoWriteRepository,
        IMapper mapper
        ) : IRequestHandler<CreateCompanyCommandRequest, BaseResponse> where TDbContext : DbContext
    {
        readonly IWriteRepository<TDbContext, Commons.Domain.Models.Companies.Company> writeRepository = writeRepository;
        readonly IReadRepository<TDbContext, Commons.Domain.Models.Companies.Company> readRepository = readRepository;
        readonly IMapper mapper = mapper;
        readonly IMongoWriteRepository mongoWriteRepository = mongoWriteRepository;
        public async Task<BaseResponse> Handle(CreateCompanyCommandRequest request, CancellationToken cancellationToken)
        {
            try
            {
                var company = this.mapper.Map<Commons.Domain.Models.Companies.Company>(request);

                // Güncelleme ise eski dosyayı kontrol et ve sil
                if (company.Id != Guid.Empty)
                {
                    var existing = await this.readRepository.GetByIdAsync(company.Id.ToString());
                    if (existing != null && request.LogoUrl != null && !string.IsNullOrWhiteSpace(existing.LogoUrl))
                    {
                        await this.mongoWriteRepository.DeleteFileAsync(existing.LogoUrl);
                    }
                }

                // Yeni dosya varsa yükle
                if (request.LogoUrl != null)
                {
                    var objectId = await this.mongoWriteRepository.UploadFileAsync(request.LogoUrl);
                    company.LogoUrl = objectId.ToString();
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