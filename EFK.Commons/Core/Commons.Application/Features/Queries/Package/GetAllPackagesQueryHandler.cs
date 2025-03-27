using Commons.Application.Repositories.Queries;
using Commons.Domain.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Commons.Application.Features.Queries.Package
{
    public class GetAllPackagesQueryHandler<TDbContext>(
      IElasticSearchReadRepository<TDbContext, Commons.Domain.Models.Packages.Package> elasticReadRepository
  ) : IRequestHandler<GetAllPackagesQueryRequest, BaseResponse> where TDbContext : DbContext
    {
        private readonly IElasticSearchReadRepository<TDbContext, Commons.Domain.Models.Packages.Package> elasticSearchReadRepository = elasticReadRepository;
        public async Task<BaseResponse> Handle(GetAllPackagesQueryRequest request, CancellationToken cancellationToken)
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
