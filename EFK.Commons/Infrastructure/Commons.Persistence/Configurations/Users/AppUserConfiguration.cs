using Commons.Domain.Models.User;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace Commons.Persistence.Configurations.Users
{
    public class AppUserConfiguration : IEntityTypeConfiguration<AppUser>
    {
        public void Configure(EntityTypeBuilder<AppUser> builder)
        {

            builder
                .HasOne(u => u.Role)
                .WithMany(r => r.Users)
                .HasForeignKey(u => u.RoleId)
                .OnDelete(DeleteBehavior.NoAction);

            builder
                .HasMany(rt => rt.RefreshTokens)
                .WithOne(au => au.AppUser)
                .HasForeignKey(rt => rt.AppUserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
