using Commons.Domain.Models;
using MediatR;

namespace Commons.Application.Features.Commands.PackageMenu.Remove
{
    public class RemovePackageMenuCommandRequest : IRequest<BaseResponse>
    {
        public string Id { get; set; }
    }
}