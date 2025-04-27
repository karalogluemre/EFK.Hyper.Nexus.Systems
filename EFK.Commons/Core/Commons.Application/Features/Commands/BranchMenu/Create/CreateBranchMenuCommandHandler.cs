using AutoMapper;
using Commons.Application.Repositories.Commands;
using Commons.Application.Repositories.Queries;
using Commons.Domain.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Commons.Application.Features.Commands.BranchMenu.Create
{
    public class CreateBranchMenuCommandHandler<TDbContext>(
    IWriteRepository<TDbContext, Commons.Domain.Models.Branches.BranchMenu> writeRepository,
    IReadRepository<TDbContext, Commons.Domain.Models.Branches.BranchMenu> readRepository,
    IReadRepository<TDbContext, Commons.Domain.Models.Menus.Menu> menuReadRepository,
    IMapper mapper
) : IRequestHandler<CreateBranchMenuCommandRequest, BaseResponse> where TDbContext : DbContext
    {
        readonly private IWriteRepository<TDbContext, Commons.Domain.Models.Branches.BranchMenu> writeRepository = writeRepository;
        readonly private IReadRepository<TDbContext, Commons.Domain.Models.Menus.Menu> menuReadRepository = menuReadRepository;
        readonly private IReadRepository<TDbContext, Commons.Domain.Models.Branches.BranchMenu> readRepository = readRepository;

        readonly private IMapper mapper = mapper;

        public async Task<BaseResponse> Handle(CreateBranchMenuCommandRequest request, CancellationToken cancellationToken)
        {
            if (request.BranchIds == null || !request.BranchIds.Any())
                return new BaseResponse { Succeeded = false, Message = "En az bir şube seçilmelidir." };

            if (request.MenuIds == null || !request.MenuIds.Any())
                return new BaseResponse { Succeeded = false, Message = "En az bir menü seçilmelidir." };

            // Tüm menüleri getir (id, menuId, ... diğer alanlar)
            var allMenus = await this.menuReadRepository
                .GetAll()
                .ToListAsync(cancellationToken);

            //  Tüm parent'larıyla birlikte seçilen menuId'leri genişlet
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

            //  Eski PackageMenu kayıtlarını sil
            var existingEntities = await this.readRepository
                .GetWhere(pm => request.BranchIds.Contains(pm.BranchId))
                .ToListAsync(cancellationToken);

            if (existingEntities.Any())
            {
                await this.writeRepository.RemoveRangeAsync(existingEntities);
            }

            // 4️⃣ Yeni kayıtları oluştur
            var newEntities = request.BranchIds
                .SelectMany(pkgId => allMenuIdsToAssign.Select(menuId => new Commons.Domain.Models.Branches.BranchMenu
                {
                    Id = Guid.NewGuid(),
                    BranchId = pkgId,
                    MenuId = menuId
                }))
                .DistinctBy(pm => new { pm.BranchId, pm.MenuId })
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