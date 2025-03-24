using MediatR;

namespace Commons.Application.Features.Queries.Package
{
    public class GetAllPackagesQueryRequest : IRequest<List<Commons.Domain.Models.Packages.Package>>
    {
    }
}
