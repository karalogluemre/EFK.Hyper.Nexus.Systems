using Commons.Domain.Models.Organizations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Commons.Persistence.Configurations.Organizations
{
    public class OrganizationMenuConfiguration : IEntityTypeConfiguration<OrganizationMenu>
    {
        public void Configure(EntityTypeBuilder<OrganizationMenu> builder)
        {
            builder.HasKey(om => new { om.OrganizationId, om.MenuId });

            builder.HasOne(om => om.Organization)
                   .WithMany()
                   .HasForeignKey(om => om.OrganizationId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(om => om.Menu)
                   .WithMany()
                   .HasForeignKey(om => om.MenuId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }

}
