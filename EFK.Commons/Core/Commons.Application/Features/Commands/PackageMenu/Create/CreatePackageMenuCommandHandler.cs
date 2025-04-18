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
    IReadRepository<TDbContext, Commons.Domain.Models.Packages.PackageMenu> readRepository,
    IReadRepository<TDbContext, Commons.Domain.Models.Menus.Menu> menuReadRepository,
    IMapper mapper
) : IRequestHandler<CreatePackageMenuCommandRequest, BaseResponse> where TDbContext : DbContext
    {
        readonly private IWriteRepository<TDbContext, Commons.Domain.Models.Packages.PackageMenu> writeRepository = writeRepository;
        readonly private IReadRepository<TDbContext, Commons.Domain.Models.Menus.Menu> menuReadRepository = menuReadRepository;
        readonly private IReadRepository<TDbContext, Commons.Domain.Models.Packages.PackageMenu> readRepository = readRepository;

        readonly private IMapper mapper = mapper;

        public async Task<BaseResponse> Handle(CreatePackageMenuCommandRequest request, CancellationToken cancellationToken)
        {
            if (request.PackageIds == null || !request.PackageIds.Any())
                return new BaseResponse { Succeeded = false, Message = "En az bir paket seçilmelidir." };

            if (request.MenuIds == null || !request.MenuIds.Any())
                return new BaseResponse { Succeeded = false, Message = "En az bir menü seçilmelidir." };

            // 1️⃣ Tüm menüleri getir (id, menuId, ... diğer alanlar)
            var allMenus = await this.menuReadRepository
                .GetAll()
                .ToListAsync(cancellationToken);

            // 2️⃣ Tüm parent'larıyla birlikte seçilen menuId'leri genişlet
            var allMenuIdsToAssign = new HashSet<Guid>();

            foreach (var menuId in request.MenuIds)
            {
                allMenuIdsToAssign.Add(menuId);

                var current = allMenus.FirstOrDefault(m => m.Id == menuId);

                while (current?.MenuId != null && current.MenuId != Guid.Empty)
                {
                    if (!allMenuIdsToAssign.Add(current.MenuId.Value))
                        break; // zaten eklenmişse döngüyü kır

                    current = allMenus.FirstOrDefault(m => m.Id == current.MenuId.Value);
                    if (current == null)
                        break;
                }
            }

            // 3️⃣ Eski PackageMenu kayıtlarını sil
            var existingEntities = await this.readRepository
                .GetWhere(pm => request.PackageIds.Contains(pm.PackageId))
                .ToListAsync(cancellationToken);

            if (existingEntities.Any())
            {
                await this.writeRepository.RemoveRangeAsync(existingEntities);
            }

            // 4️⃣ Yeni kayıtları oluştur
            var newEntities = request.PackageIds
                .SelectMany(pkgId => allMenuIdsToAssign.Select(menuId => new Commons.Domain.Models.Packages.PackageMenu
                {
                    Id = Guid.NewGuid(),
                    PackageId = pkgId,
                    MenuId = menuId
                }))
                .DistinctBy(pm => new { pm.PackageId, pm.MenuId })
                .ToList();

            if (newEntities.Any())
            {
                return await this.writeRepository.AddOrUpdateBulkAsync(newEntities);
            }

            return new BaseResponse
            {
                Succeeded = true,
                Message = "Tüm eski kayıtlar silindi, yeni ekleme yapılmadı."
            };
        }

    }
}
