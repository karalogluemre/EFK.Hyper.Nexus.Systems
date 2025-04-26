using Commons.Domain.Models;
using MediatR;

namespace Commons.Application.Features.Commands.Branch.Remove
{
    public class RemoveBranchCommandRequest : IRequest<BaseResponse>
    {
        public string Id { get; set; }
    }
}
