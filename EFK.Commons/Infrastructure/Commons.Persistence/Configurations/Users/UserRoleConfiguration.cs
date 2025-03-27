using Commons.Domain.Models.User;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Commons.Persistence.Configurations.Users
{
    public class UserRoleConfiguration : IEntityTypeConfiguration<UserRole>
    {
        public void Configure(EntityTypeBuilder<UserRole> builder)
        {
            builder.HasKey(ur => new { ur.AppUserId, ur.AppRoleId });

            builder.HasOne(ur => ur.AppUser)
                   .WithMany(x => x.UserRoles)
                   .HasForeignKey(ur => ur.AppUserId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(ur => ur.AppRole)
                   .WithMany(x=>x.UserRoles)
                   .HasForeignKey(ur => ur.AppRoleId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}