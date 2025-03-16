using Commons.Domain.Models.Departments;

namespace Commons.Domain.Models.Units
{
    public class Unit : BaseEntity
    {
        public Guid DepartmentId { get; set; } 
        public Department Department { get; set; }
    }
}
