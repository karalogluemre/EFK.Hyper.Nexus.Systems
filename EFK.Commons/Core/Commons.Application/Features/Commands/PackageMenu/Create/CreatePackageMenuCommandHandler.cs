using AutoMapper;
using Commons.Application.Repositories.Commands;
using Commons.Application.Repositories.Queries;
using Commons.Domain.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Commons.Application.Features.Commands.PackageMenu.Create
{
    public class CreatePackageMenuCommandHandler<TDbContext>(
    IWriteRepository<TDbContext, Commons.Domain.Models.Packages.PackageMenu> writeRepository,
    IReadRepository<TDbContext, Commons.Domain.Models.Menus.Menu> menuReadRepository,
    IMapper mapper
) : IRequestHandler<CreatePackageMenuCommandRequest, BaseResponse> where TDbContext : DbContext
    {
        readonly private IWriteRepository<TDbContext, Commons.Domain.Models.Packages.PackageMenu> writeRepository = writeRepository;
        IReadRepository<TDbContext, Commons.Domain.Models.Menus.Menu> menuReadRepository = menuReadRepository;

        readonly private IMapper mapper = mapper;

        public async Task<BaseResponse> Handle(CreatePackageMenuCommandRequest request, CancellationToken cancellationToken)
        {
            var packageMenus = new List<Commons.Domain.Models.Packages.PackageMenu>();
            var existingCombinations = new HashSet<(Guid PackageId, Guid MenuId)>();

            foreach (var packageIdString in request.PackageIds)
            {
                if (Guid.TryParse(packageIdString.ToString(), out Guid packageGuid))
                {
                    foreach (var menuIdString in request.MenuIds)
                    {
                        if (Guid.TryParse(menuIdString.ToString(), out Guid menuGuid))
                        {
                            var allRelatedMenuIds = new HashSet<Guid>();
                            await AddMenuWithParentsAsync(menuGuid, allRelatedMenuIds);

                            foreach (var relatedMenuId in allRelatedMenuIds)
                            {
                                var key = (PackageId: packageGuid, MenuId: relatedMenuId);

                                if (!existingCombinations.Contains(key))
                                {
                                    packageMenus.Add(new Commons.Domain.Models.Packages.PackageMenu
                                    {
                                        PackageId = packageGuid,
                                        MenuId = relatedMenuId
                                    });

                                    existingCombinations.Add(key);
                                }
                            }
                        }
                    }
                }
            }

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
