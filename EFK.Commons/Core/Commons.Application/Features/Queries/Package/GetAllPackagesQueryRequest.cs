using Commons.Domain.Models;
using MediatR;

namespace Commons.Application.Features.Queries.Package
{
    public class GetAllPackagesQueryRequest : IRequest<BaseResponse>
    {
    }
}
