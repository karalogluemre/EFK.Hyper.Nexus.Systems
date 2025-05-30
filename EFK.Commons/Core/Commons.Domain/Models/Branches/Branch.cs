﻿using Commons.Domain.Models.Companies;
using Commons.Domain.Models.Organizations;

namespace Commons.Domain.Models.Branches
{
    public class Branch : BaseEntity
    {
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
        
        public Company Company { get; set; }
        public ICollection<BranchMenu> BranchMenus { get; set; } = new List<BranchMenu>();
        public ICollection<Organization> Organizations { get; set; } = new List<Organization>();
    }
}
