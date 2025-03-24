using Commons.Application.Repositories.Queries;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Commons.Application.Features.Queries.Package
{
    public class GetAllPackagesQueryHandler<TDbContext>(
      IElasticSearchReadRepository<TDbContext, Commons.Domain.Models.Packages.Package> elasticReadRepository
  ) : IRequestHandler<GetAllPackagesQueryRequest, List<Commons.Domain.Models.Packages.Package>> where TDbContext : DbContext
    {
        private readonly IElasticSearchReadRepository<TDbContext, Commons.Domain.Models.Packages.Package> elasticSearchReadRepository = elasticReadRepository;
        public async Task<List<Commons.Domain.Models.Packages.Package>> Handle(GetAllPackagesQueryRequest request, CancellationToken cancellationToken)
        {
            var allPackages = await this.elasticSearchReadRepository.GetAllFromElasticSearchAsync();
            return allPackages.Where(p => p.IsDeleted != true).ToList();
        }
    }
}
