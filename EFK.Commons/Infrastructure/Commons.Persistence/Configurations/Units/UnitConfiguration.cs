using Commons.Domain.Models.Units;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Commons.Persistence.Configurations.Units
{
    public class UnitConfiguration : IEntityTypeConfiguration<Unit>
    {
        public void Configure(EntityTypeBuilder<Unit> builder)
        {
            builder.HasKey(o => o.Id);

            builder.Property(o => o.Name)
                   .IsRequired()
                   .HasMaxLength(200);

            builder.HasOne(o => o.Department)
                   .WithMany(c => c.Units)
                   .HasForeignKey(o => o.DepartmentId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
