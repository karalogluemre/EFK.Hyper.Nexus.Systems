using Commons.Domain.Models.Organizations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Commons.Persistence.Configurations.Organizations
{
    public class OrganizationConfiguration : IEntityTypeConfiguration<Organization>
    {
        public void Configure(EntityTypeBuilder<Organization> builder)
        {
            builder.HasKey(o => o.Id);

            builder.Property(o => o.Name)
                   .IsRequired()
                   .HasMaxLength(200);

            builder.HasOne(o => o.Branch)
                   .WithMany(c => c.Organizations)
                   .HasForeignKey(o => o.BranchId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(o => o.Groups)
                   .WithOne(g => g.Organization)
                   .HasForeignKey(g => g.OrganizationId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
