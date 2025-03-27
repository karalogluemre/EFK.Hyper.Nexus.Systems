using AutoMapper;
using Commons.Application.Repositories.Commands;
using Commons.Domain.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace Commons.Application.Features.Commands.Menu.Create
{
    public class CreateMenuCommandHandler<TDbContext>(
         IWriteRepository<TDbContext, Commons.Domain.Models.Menus.Menu> writeRepository,
            IMapper mapper
        ) : IRequestHandler<CreateMenuCommandRequest, BaseResponse>
    where TDbContext : DbContext
    {
        private readonly IWriteRepository<TDbContext, Commons.Domain.Models.Menus.Menu> writeRepository = writeRepository;
        private readonly IMapper mapper = mapper;

        public async Task<BaseResponse> Handle(CreateMenuCommandRequest request, CancellationToken cancellationToken)
        {
            string jsonFilePath = @"C:\Users\emrek\OneDrive\Masaüstü\Files\EFK.System.Projects\EFK.System\EFK.System.API\EFK.Hyper.Nexus.Systems\EFK.Commons\Core\Commons.Application\JsonMenus\humanResources.json";

            if (File.Exists(jsonFilePath))
            {
                string jsonContent = await File.ReadAllTextAsync(jsonFilePath);
                var menuData = JsonConvert.DeserializeObject<Commons.Domain.Models.Menus.Menu>(jsonContent);

                // Tüm alt öğeleri toplayıp listeye ekleyelim
                var menuList = new List<Commons.Domain.Models.Menus.Menu> { menuData };
                CollectAllMenus(menuData, menuList);

                return await this.writeRepository.AddBulkAsync(menuList);
            }
            return new BaseResponse { Message = "File not found", Succeeded = false };

        }
        private void CollectAllMenus(Commons.Domain.Models.Menus.Menu menu, List<Commons.Domain.Models.Menus.Menu> menuList)
        {
            if (menu == null || menu.Items == null || !menu.Items.Any())
                return;

            foreach (var subMenu in menu.Items)
            {
                menuList.Add(subMenu);
                CollectAllMenus(subMenu, menuList);
            }
        }
    }
}