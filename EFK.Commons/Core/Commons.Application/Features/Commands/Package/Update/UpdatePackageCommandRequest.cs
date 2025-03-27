using Commons.Domain.Models;
using MediatR;

namespace Commons.Application.Features.Commands.Package.Update
{
    public class UpdatePackageCommandRequest : IRequest<BaseResponse>
    {
        public string Id { get; set; }
        public string Name { get; set; }
    }
}
