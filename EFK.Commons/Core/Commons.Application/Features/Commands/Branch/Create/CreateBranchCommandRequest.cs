using Commons.Domain.Models;
using MediatR;

namespace Commons.Application.Features.Commands.Branch.Create
{
    public class CreateBranchCommandRequest : IRequest<BaseResponse>
    {
        public Guid Id { get; set; }
        public Guid CompanyId { get; set; }
        public string Name { get; set; }
        public string BranchCode { get; set; }

        public string PhoneNumber { get; set; }
        public string Email { get; set; }

        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string City { get; set; }
        public string District { get; set; }
        public string PostalCode { get; set; }
        public string Country { get; set; }

        public decimal? Latitude { get; set; }
        public decimal? Longitude { get; set; }

        public string ManagerName { get; set; }
        public string ManagerPhone { get; set; }

    }
}
