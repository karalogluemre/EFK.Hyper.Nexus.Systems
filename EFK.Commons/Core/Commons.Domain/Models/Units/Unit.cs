using Commons.Domain.Models.Departments;
using Commons.Domain.Models.User;

namespace Commons.Domain.Models.Units
{
    public class Unit : BaseEntity
    {
        public Guid DepartmentId { get; set; } 
        public Department Department { get; set; }
        public ICollection<UserUnit> UserUnits { get; set; } = new List<UserUnit>();

    }
}
