using Commons.Domain.Models.Packages;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Commons.Persistence.Configurations.Packages
{
    public class PackageMenuConfiguration : IEntityTypeConfiguration<PackageMenu>
    {
        public void Configure(EntityTypeBuilder<PackageMenu> builder)
        {
            builder.HasKey(pm => pm.Id);

            builder.Property(pm => pm.Id)
                   .ValueGeneratedNever(); //  Guid'leri biz atayacağız 

            builder.HasOne(pm => pm.Package)
                   .WithMany(x => x.PackageMenus)
                   .HasForeignKey(pm => pm.PackageId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(pm => pm.Menu)
                   .WithMany(x => x.PackageMenus)
                   .HasForeignKey(pm => pm.MenuId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
