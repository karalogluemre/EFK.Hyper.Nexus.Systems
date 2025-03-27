using Commons.Domain.Models;
using MediatR;

namespace Commons.Application.Features.Commands.PackageMenu.Update
{
    public class UpdatePackageMenuCommandRequest : IRequest<BaseResponse>
    {
        public string Id { get; set; } 
        public List<PackageMenuItemDto> PackageMenus { get; set; } 
    }

    public class PackageMenuItemDto
    {
        public string PackageId { get; set; }
        public string MenuId { get; set; }
    }
}