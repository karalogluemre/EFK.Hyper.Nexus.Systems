using Commons.Application.Repositories.Commands;
using Commons.Application.Repositories.Queries;
using Commons.Domain.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Commons.Application.Features.Commands.PackageMenu.Update
{
    public class UpdatePackageMenuCommandHandler<TDbContext>(
    IWriteRepository<TDbContext, Commons.Domain.Models.Packages.PackageMenu> writeRepository,
    IReadRepository<TDbContext, Commons.Domain.Models.Packages.PackageMenu> packageMenuReadRepository,
    IReadRepository<TDbContext, Commons.Domain.Models.Menus.Menu> menuReadRepository
) : IRequestHandler<UpdatePackageMenuCommandRequest, BaseResponse> where TDbContext : DbContext
    {
        readonly private IWriteRepository<TDbContext, Commons.Domain.Models.Packages.PackageMenu> writeRepository = writeRepository;
        readonly private IReadRepository<TDbContext, Commons.Domain.Models.Packages.PackageMenu> packageMenuReadRepository = packageMenuReadRepository;

        readonly private IReadRepository<TDbContext, Commons.Domain.Models.Menus.Menu> menuReadRepository = menuReadRepository;

        public async Task<BaseResponse> Handle(UpdatePackageMenuCommandRequest request, CancellationToken cancellationToken)
        {
            var packageMenus = new List<Commons.Domain.Models.Packages.PackageMenu>();
            var existingCombinations = new HashSet<(Guid PackageId, Guid MenuId)>();

            foreach (var item in request.PackageMenus)
            {
                if (Guid.TryParse(item.PackageId, out Guid packageGuid) && Guid.TryParse(item.MenuId, out Guid menuGuid))
                {
                    var allRelatedMenuIds = new HashSet<Guid>();
                    await AddMenuWithParentsAsync(menuGuid, allRelatedMenuIds);

                    foreach (var relatedMenuId in allRelatedMenuIds)
                    {
                        var key = (PackageId: packageGuid, MenuId: relatedMenuId);

                        if (!existingCombinations.Contains(key))
                        {
                            existingCombinations.Add(key);
                            packageMenus.Add(new Commons.Domain.Models.Packages.PackageMenu
                            {
                                PackageId = packageGuid,
                                MenuId = relatedMenuId
                            });
                        }
                    }
                }
            }


            // Önce mevcut verileri sil
            if (request.PackageMenus.Any())
            {
                var packageId = Guid.Parse(request.PackageMenus.First().PackageId);
                var existingItems = packageMenuReadRepository.GetWhere(pm => pm.PackageId == packageId).ToList();

                // Yeni: RemoveRangeAsync ile toplu silme
                if (existingItems.Any())
                    await this.writeRepository.RemoveRangeAsync(existingItems);
            }

            // Yeni verileri ekle
            return await this.writeRepository.AddBulkAsync(packageMenus);
        }

        private async Task AddMenuWithParentsAsync(Guid menuId, HashSet<Guid> collectedIds)
        {
            if (!collectedIds.Add(menuId)) return;

            var menu = await this.menuReadRepository.GetByIdAsync(menuId.ToString());
            if (menu?.MenuId != null)
            {
                await AddMenuWithParentsAsync(menu.MenuId.Value, collectedIds);
            }
        }
    }
}