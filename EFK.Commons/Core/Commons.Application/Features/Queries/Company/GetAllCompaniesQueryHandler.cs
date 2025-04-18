using AutoMapper;
using Commons.Application.Abstract.Dto.Company;
using Commons.Application.Repositories.Queries;
using Commons.Domain.Models;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;

namespace Commons.Application.Features.Queries.Company
{
    public class GetAllCompaniesQueryHandler<TDbContext>(
      IElasticSearchReadRepository<TDbContext, Commons.Domain.Models.Companies.Company> elasticReadRepository,
      IMongoReadRepository<TDbContext> mongoReadRepository,
      IMapper mapper 

  ) : IRequestHandler<GetAllCompaniesQueryRequest, BaseResponse> where TDbContext : DbContext
    {
        private readonly IElasticSearchReadRepository<TDbContext, Commons.Domain.Models.Companies.Company> elasticSearchReadRepository = elasticReadRepository;
        readonly private IMongoReadRepository<TDbContext> mongoReadRepository = mongoReadRepository; 
        readonly private IMapper mapper = mapper;
        public async Task<BaseResponse> Handle(GetAllCompaniesQueryRequest request, CancellationToken cancellationToken)
        {
            var allCompanies = await this.elasticSearchReadRepository.GetAllFromElasticSearchAsync();
            var activeCompanies = allCompanies.Where(p => p.IsDeleted != true).ToList();

            var resultList = new List<CompanyDto>();

            foreach (var company in activeCompanies)
            {
                // Mapper ile dönüşüm
                var dto = this.mapper.Map<CompanyDto>(company);

                try
                {
                    var logoData = await this.mongoReadRepository.PreviewFileQueryResponse(company.Id.ToString());

                    if (logoData?.Stream != null)
                    {
                        using var memory = new MemoryStream();
                        await logoData.Stream.CopyToAsync(memory);
                        memory.Position = 0;

                        dto.LogoUrl = new FormFile(memory, 0, memory.Length, "logo", logoData.FileName)
                        {
                            Headers = new HeaderDictionary(),
                            ContentType = logoData.ContentType
                        };
                    }
                }
                catch
                {
                    // Logo yoksa geç
                }

                resultList.Add(dto);
            }

            return new BaseResponse
            {
                Succeeded = true,
                Message = "Firmalar başarıyla getirildi.",
                Data = resultList
            };
        }
    }
}
