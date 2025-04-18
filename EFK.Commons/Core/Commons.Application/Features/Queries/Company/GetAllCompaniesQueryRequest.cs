using Commons.Domain.Models;
using MediatR;

namespace Commons.Application.Features.Queries.Company
{
    public class GetAllCompaniesQueryRequest : IRequest<BaseResponse>
    {
    }
}
