using Commons.Domain.Models.Companies;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Commons.Persistence.Configurations.Companies
{
    public class CompanyConfiguration : IEntityTypeConfiguration<Company>
    {
        public void Configure(EntityTypeBuilder<Company> builder)
        {
            builder.HasKey(c => c.Id);

            builder.Property(c => c.Name)
                   .IsRequired()
                   .HasMaxLength(200);

            builder.HasOne(c => c.Packages)
                   .WithMany(o => o.Companies)
                   .HasForeignKey(o => o.PackageId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(c => c.Branches)
                   .WithOne(o => o.Company)
                   .HasForeignKey(o => o.CompanyId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(c => c.CompanyFiles)
               .WithOne(cf => cf.Company)
               .HasForeignKey(cf => cf.CompanyId)
               .OnDelete(DeleteBehavior.Cascade); // Şirket silinirse dosyalar da silinir

        }
    }
}