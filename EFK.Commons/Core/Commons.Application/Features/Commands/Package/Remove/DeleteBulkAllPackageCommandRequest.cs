using Commons.Domain.Models;
using MediatR;

namespace Commons.Application.Features.Commands.Package.Remove
{
    public class DeleteBulkAllPackageCommandRequest : IRequest<BaseResponse>
    {
        public List<string> Ids { get; set; } = new();
    }
}
