using MediatR;

namespace Commons.Application.Features.Queries.Package
{
    public class GetPackageByIdQueryRequest : IRequest<Commons.Domain.Models.Packages.Package>
    {
        public string Id { get; set; }
    }
}
