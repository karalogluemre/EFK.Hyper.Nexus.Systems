using Commons.Domain.Models;
using MediatR;

namespace Commons.Application.Features.Commands.Branch.Remove
{
    public class RemoveBulkAllBranchesCommandRequest : IRequest<BaseResponse>
    {
        public List<string> Ids { get; set; } = new();
    }
}
