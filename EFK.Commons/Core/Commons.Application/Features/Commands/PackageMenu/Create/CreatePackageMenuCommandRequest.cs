using Commons.Domain.Models;
using MediatR;

namespace Commons.Application.Features.Commands.PackageMenu.Create
{
    public class CreatePackageMenuCommandRequest : IRequest<BaseResponse>
    {
        public Guid Id { get; set; }
        public List<Guid> PackageIds { get; set; }
        public List<Guid> MenuIds { get; set; }
    }
}