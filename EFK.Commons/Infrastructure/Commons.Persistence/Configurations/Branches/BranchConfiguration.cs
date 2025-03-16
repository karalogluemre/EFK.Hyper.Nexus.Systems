using Commons.Domain.Models.Branches;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Commons.Persistence.Configurations.Branches
{
    public class BranchConfiguration : IEntityTypeConfiguration<Branch>
    {
        public void Configure(EntityTypeBuilder<Branch> builder)
        {
            builder.HasKey(c => c.Id);

            builder.Property(c => c.Name)
                   .IsRequired()
                   .HasMaxLength(200);

            builder.HasOne(c => c.Company)
                               .WithMany(o => o.Branches)
                               .HasForeignKey(o => o.CompanyId)
                               .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(c => c.Organizations)
                               .WithOne(o => o.Branch)
                               .HasForeignKey(o => o.BranchId)
                               .OnDelete(DeleteBehavior.Cascade);

        }
    }
}