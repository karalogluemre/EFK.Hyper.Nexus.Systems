using Commons.Application.Repositories.Queries;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Commons.Application.Features.Queries.Package
{
    public class GetPackageByIdQueryHandler<TDbContext>(
      IElasticSearchReadRepository<TDbContext, Commons.Domain.Models.Packages.Package> elasticReadRepository
  ) : IRequestHandler<GetPackageByIdQueryRequest, Commons.Domain.Models.Packages.Package> where TDbContext : DbContext
    {
        readonly IElasticSearchReadRepository<TDbContext, Commons.Domain.Models.Packages.Package> elasticReadRepository = elasticReadRepository;

        public async Task<Commons.Domain.Models.Packages.Package> Handle(GetPackageByIdQueryRequest request, CancellationToken cancellationToken)
        {
            var allPackages = await this.elasticReadRepository.GetAllFromElasticSearchAsync();
            return allPackages.FirstOrDefault(p => p.Id == Guid.Parse(request.Id));
        }
    }

}
