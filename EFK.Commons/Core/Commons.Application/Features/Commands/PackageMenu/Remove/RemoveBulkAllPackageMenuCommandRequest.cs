using Commons.Domain.Models;
using MediatR;

namespace Commons.Application.Features.Commands.PackageMenu.Remove
{
    public class RemoveBulkAllPackageMenuCommandRequest : IRequest<BaseResponse>
    {
        public List<string> Ids { get; set; } 
    }
}
