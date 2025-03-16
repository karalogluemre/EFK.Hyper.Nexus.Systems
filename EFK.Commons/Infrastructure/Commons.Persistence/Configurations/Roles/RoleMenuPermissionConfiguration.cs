using Commons.Domain.Models.Role;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Commons.Persistence.Configurations.Roles
{
    public class RoleMenuPermissionConfiguration : IEntityTypeConfiguration<RoleMenuPermission>
    {
        public void Configure(EntityTypeBuilder<RoleMenuPermission> builder)
        {
            builder.HasKey(rmp => new { rmp.RoleId, rmp.MenuId });

            builder.HasOne(rmp => rmp.AppRole)
                   .WithMany()
                   .HasForeignKey(rmp => rmp.RoleId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(rmp => rmp.Menu)
                   .WithMany()
                   .HasForeignKey(rmp => rmp.MenuId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.Property(rmp => rmp.CanCreate).IsRequired();
            builder.Property(rmp => rmp.CanRead).IsRequired();
            builder.Property(rmp => rmp.CanUpdate).IsRequired();
            builder.Property(rmp => rmp.CanDelete).IsRequired();
        }
    }

}
