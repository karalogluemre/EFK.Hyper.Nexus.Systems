using Commons.Domain.Models.User;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Commons.Persistence.Configurations.Users
{
    public class UserDepartmentConfiguration : IEntityTypeConfiguration<UserDepartment>
    {
        public void Configure(EntityTypeBuilder<UserDepartment> builder)
        {
            builder.HasKey(ud => new { ud.UserId, ud.DepartmentId });

            builder.HasOne(ud => ud.User)
                   .WithMany(u => u.UserDepartments)
                   .HasForeignKey(ud => ud.UserId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(ud => ud.Department)
                   .WithMany()
                   .HasForeignKey(ud => ud.DepartmentId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
