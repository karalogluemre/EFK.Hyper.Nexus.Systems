using Commons.Application.Repositories.Queries;
using Commons.Domain.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Commons.Application.Features.Queries.Company
{
    public class GetAllCompaniesQueryHandler<TDbContext>(
      IElasticSearchReadRepository<TDbContext, Commons.Domain.Models.Companies.Company> elasticReadRepository
  ) : IRequestHandler<GetAllCompaniesQueryRequest, BaseResponse> where TDbContext : DbContext
    {
        private readonly IElasticSearchReadRepository<TDbContext, Commons.Domain.Models.Companies.Company> elasticSearchReadRepository = elasticReadRepository;
        public async Task<BaseResponse> Handle(GetAllCompaniesQueryRequest request, CancellationToken cancellationToken)
        {
            var allPackages = await this.elasticSearchReadRepository.GetAllFromElasticSearchAsync();
            return new BaseResponse
            {
                Succeeded = true,
                Message = "Paket menüleri başarıyla getirildi.",
                Data = allPackages.Where(p => p.IsDeleted != true).ToList()
            };
        }
    }
}
