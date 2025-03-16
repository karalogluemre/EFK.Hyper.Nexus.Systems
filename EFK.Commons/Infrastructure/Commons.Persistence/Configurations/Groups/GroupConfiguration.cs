using Commons.Domain.Models.Groups;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Commons.Persistence.Configurations.Groups
{
    public class GroupConfiguration : IEntityTypeConfiguration<Group>
    {
        public void Configure(EntityTypeBuilder<Group> builder)
        {
            builder.HasKey(o => o.Id);

            builder.Property(o => o.Name)
                   .IsRequired()
                   .HasMaxLength(200);

            builder.HasOne(o => o.Organization)
                   .WithMany(c => c.Groups)
                   .HasForeignKey(o => o.OrganizationId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(o => o.Departments)
                   .WithOne(g => g.Group)
                   .HasForeignKey(g => g.GroupId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
