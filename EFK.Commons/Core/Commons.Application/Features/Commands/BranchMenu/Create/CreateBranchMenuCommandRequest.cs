using Commons.Domain.Models;
using MediatR;

namespace Commons.Application.Features.Commands.BranchMenu.Create
{
    public class CreateBranchMenuCommandRequest : IRequest<BaseResponse>
    {
        public Guid Id { get; set; }
        public Guid CompanyId { get; set; }
        public List<Guid> BranchIds { get; set; }
        public List<Guid> MenuIds { get; set; }
    }
}
