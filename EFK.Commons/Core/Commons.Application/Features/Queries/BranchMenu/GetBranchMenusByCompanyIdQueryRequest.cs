using Commons.Domain.Models;
using MediatR;

namespace Commons.Application.Features.Queries.BranchMenu
{
    public class GetBranchMenusByCompanyIdQueryRequest : IRequest<BaseResponse>
    {
        public string CompanyId { get; set; }
    }
}
