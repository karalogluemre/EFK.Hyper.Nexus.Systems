using Commons.Domain.Models.Groups;
using Commons.Domain.Models.Units;

namespace Commons.Domain.Models.Departments
{
    public class Department : BaseEntity
    {
        public Guid GroupId { get; set; }
        public Group Group { get; set; }

        public ICollection<Unit> Units { get; set; } = new List<Unit>(); 

    }
}
