using Commons.Domain.Models.Branches;
using Commons.Domain.Models.Packages;

namespace Commons.Domain.Models.Companies
{
    public class Company : BaseEntity
    {
        public Guid PackageId { get; set; } 
        public Package Package { get; set; }

        public ICollection<Branch> Branches { get; set; } = new List<Branch>();
    }
}
