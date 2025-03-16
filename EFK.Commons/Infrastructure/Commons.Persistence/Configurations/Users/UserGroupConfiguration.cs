using Commons.Domain.Models.User;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Commons.Persistence.Configurations.Users
{
    public class UserGroupConfiguration : IEntityTypeConfiguration<UserGroup>
    {
        public void Configure(EntityTypeBuilder<UserGroup> builder)
        {
            builder.HasKey(ug => new { ug.UserId, ug.GroupId });

            builder.HasOne(ug => ug.User)
                   .WithMany(u => u.UserGroups)
                   .HasForeignKey(ug => ug.UserId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(ug => ug.Group)
                   .WithMany(x=>x.UserGroups)
                   .HasForeignKey(ug => ug.GroupId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
