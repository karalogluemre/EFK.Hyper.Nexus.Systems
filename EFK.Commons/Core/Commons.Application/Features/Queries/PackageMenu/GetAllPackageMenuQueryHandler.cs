using Commons.Application.Repositories.Queries;
using Commons.Domain.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Commons.Application.Features.Queries.PackageMenu
{
    public class GetAllPackageMenuQueryHandler<TDbContext>(
      IElasticSearchReadRepository<TDbContext, Commons.Domain.Models.Packages.Package> packageElasticReadRepository,
      IElasticSearchReadRepository<TDbContext, Commons.Domain.Models.Packages.PackageMenu> packageMenuElasticReadRepository
  ) : IRequestHandler<GetAllPackageMenuQueryRequest, BaseResponse> where TDbContext : DbContext
    {
        readonly private IElasticSearchReadRepository<TDbContext, Commons.Domain.Models.Packages.Package> packageElasticReadRepository = packageElasticReadRepository;
        readonly private IElasticSearchReadRepository<TDbContext, Commons.Domain.Models.Packages.PackageMenu> packageMenuElasticReadRepository = packageMenuElasticReadRepository;
        public async Task<BaseResponse> Handle(GetAllPackageMenuQueryRequest request, CancellationToken cancellationToken)
        {
            var allPackageMenus = await this.packageMenuElasticReadRepository.GetAllFromElasticSearchAsync();
            var distinctPackageMenuPairs = allPackageMenus
                .GroupBy(pm => new { pm.PackageId, pm.MenuId,pm.Id })
                .Select(g => g.First())
                .ToList();

            var uniquePackageIds = distinctPackageMenuPairs.Select(pm => pm.PackageId).Distinct().ToList();
            var allPackages = await packageElasticReadRepository.GetAllFromElasticSearchAsync();

            var filteredPackages = allPackages
                .Where(p => uniquePackageIds.Contains(p.Id))
                .Select(p => new
                {
                    Id = p.Id,
                    PackageMenus = distinctPackageMenuPairs.Where(pm => pm.PackageId == p.Id).ToList()
                })
                .ToList();

            return new BaseResponse
            {
                Succeeded = true,
                Message = "Paket menüleri başarıyla getirildi.",
                Data = filteredPackages
            };
        }
    }
}
