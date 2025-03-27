using Commons.Domain.Models;
using MediatR;

namespace Commons.Application.Features.Commands.PackageMenu.Create
{
    public class CreatePackageMenuCommandRequest : IRequest<BaseResponse>
    {
        public List<dynamic> PackageIds { get; set; }
        public List<dynamic> MenuIds { get; set; }
    }
}