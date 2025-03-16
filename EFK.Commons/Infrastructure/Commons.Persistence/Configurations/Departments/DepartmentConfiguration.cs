using Commons.Domain.Models.Departments;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Commons.Persistence.Configurations.Departments
{
    public class DepartmentConfiguration : IEntityTypeConfiguration<Department>
    {
        public void Configure(EntityTypeBuilder<Department> builder)
        {
            builder.HasKey(o => o.Id);

            builder.Property(o => o.Name)
                   .IsRequired()
                   .HasMaxLength(200);

            builder.HasOne(o => o.Group)
                   .WithMany(c => c.Departments)
                   .HasForeignKey(o => o.GroupId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(o => o.Units)
                   .WithOne(g => g.Department)
                   .HasForeignKey(g => g.DepartmentId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
