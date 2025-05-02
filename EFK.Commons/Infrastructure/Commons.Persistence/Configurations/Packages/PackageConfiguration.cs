using Commons.Domain.Models.Packages;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Commons.Persistence.Configurations.Packages
{
    public class PackageConfiguration : IEntityTypeConfiguration<Package>
    {
        public void Configure(EntityTypeBuilder<Package> builder)
        {
            builder.HasKey(c => c.Id);

            builder.Property(c => c.Name)
                   .IsRequired()
                   .HasMaxLength(200);

            builder.HasMany(c => c.Companies)
                   .WithOne(o => o.Package)
                   .HasForeignKey(o => o.PackageId)
                   .OnDelete(DeleteBehavior.Cascade);

        }
    }
}