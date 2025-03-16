using Commons.Domain.Models.User;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Commons.Persistence.Configurations.Users
{
    public class UserOrganizationConfiguration : IEntityTypeConfiguration<UserOrganization>
    {
        public void Configure(EntityTypeBuilder<UserOrganization> builder)
        {
            builder.HasKey(uo => new { uo.UserId, uo.OrganizationId });

            builder.HasOne(uo => uo.User)
                   .WithMany(u => u.UserOrganizations)
                   .HasForeignKey(uo => uo.UserId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(uo => uo.Organization)
                   .WithMany()
                   .HasForeignKey(uo => uo.OrganizationId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
