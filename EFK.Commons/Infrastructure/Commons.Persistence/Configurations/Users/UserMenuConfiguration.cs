using Commons.Domain.Models.User;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Commons.Persistence.Configurations.Users
{
    public class UserMenuConfiguration : IEntityTypeConfiguration<UserMenuPermission>
    {
        public void Configure(EntityTypeBuilder<UserMenuPermission> builder)
        {
            builder.HasKey(ump => new { ump.UserId, ump.MenuId });

            builder.HasOne(ump => ump.AppUser)
                   .WithMany()
                   .HasForeignKey(ump => ump.UserId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(ump => ump.Menu)
                   .WithMany()
                   .HasForeignKey(ump => ump.MenuId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.Property(ump => ump.CanCreate).IsRequired();
            builder.Property(ump => ump.CanRead).IsRequired();
            builder.Property(ump => ump.CanUpdate).IsRequired();
            builder.Property(ump => ump.CanDelete).IsRequired();
        }
    }
}
