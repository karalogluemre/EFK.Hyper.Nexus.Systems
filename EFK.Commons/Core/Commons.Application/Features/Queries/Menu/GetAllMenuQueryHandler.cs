using Commons.Application.Repositories.Queries;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Commons.Application.Features.Queries.Menu
{
    public class GetAllMenuQueryHandler
    <TDbContext>(
        IElasticSearchReadRepository<TDbContext, Commons.Domain.Models.Menus.Menu> elasticSearchReadRepository,
        IReadRepository<TDbContext, Commons.Domain.Models.Menus.Menu> readRepository
        ) : IRequestHandler<GetAllMenuQueryRequest, List<Commons.Domain.Models.Menus.Menu>> where TDbContext : DbContext
    {
        readonly IElasticSearchReadRepository<TDbContext, Domain.Models.Menus.Menu> elasticSearchReadRepository = elasticSearchReadRepository;
        readonly IReadRepository<TDbContext, Commons.Domain.Models.Menus.Menu> readRepository = readRepository;

        public async Task<List<Commons.Domain.Models.Menus.Menu>> Handle(GetAllMenuQueryRequest request, CancellationToken cancellationToken)
        {
            var data =  await this.elasticSearchReadRepository.GetAllFromElasticSearchAsync();
            data = data.DistinctBy(m => m.Id).ToList();

            return BuildMenuHierarchy(data);

        }
        public List<Commons.Domain.Models.Menus.Menu> BuildMenuHierarchy(List<Commons.Domain.Models.Menus.Menu> menus)
        {
            var menuLookup = menus.ToDictionary(m => m.Id);
            var rootMenus = new List<Commons.Domain.Models.Menus.Menu>();

            foreach (var menu in menus)
            {
                if (menu.MenuId == null || !menuLookup.ContainsKey(menu.MenuId.Value))
                {
                    if (!rootMenus.Any(m => m.Id == menu.Id)) // 🔹 Eğer daha önce eklenmişse tekrar ekleme
                    {
                        rootMenus.Add(menu);
                    }
                }
                else
                {
                    var parentMenu = menuLookup[menu.MenuId.Value];
                    if (parentMenu.Items == null)
                        parentMenu.Items = new List<Commons.Domain.Models.Menus.Menu>();

                    if (!parentMenu.Items.Any(m => m.Id == menu.Id)) // 🔹 Eğer zaten ekliyse tekrar ekleme
                    {
                        parentMenu.Items.Add(menu);
                    }
                }
            }

            return rootMenus.OrderBy(m => m.Key).ToList();
        }

    }
}
