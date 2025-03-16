using Commons.Domain.Models.User;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Commons.Persistence.Configurations.Users
{
    public class UserUnitConfiguration : IEntityTypeConfiguration<UserUnit>
    {
        public void Configure(EntityTypeBuilder<UserUnit> builder)
        {
            builder.HasKey(uu => new { uu.UserId, uu.UnitId });

            builder.HasOne(uu => uu.User)
                   .WithMany(u => u.UserUnits)
                   .HasForeignKey(uu => uu.UserId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(uu => uu.Unit)
                   .WithMany()
                   .HasForeignKey(uu => uu.UnitId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }

}
