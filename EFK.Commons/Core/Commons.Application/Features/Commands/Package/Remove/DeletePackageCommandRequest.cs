using Commons.Domain.Models;
using MediatR;

namespace Commons.Application.Features.Commands.Package.Remove
{
    public class DeletePackageCommandRequest : IRequest<BaseResponse>
    {
        public string Id { get; set; }
        public string Name { get; set; }
    }
}
