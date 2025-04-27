using Commons.Domain.Models.Branches;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Commons.Persistence.Configurations.Branches
{
    public class BranchMenuConfiguration : IEntityTypeConfiguration<BranchMenu>
    {
        public void Configure(EntityTypeBuilder<BranchMenu> builder)
        {
            builder.HasKey(pm => pm.Id);

            builder.Property(pm => pm.Id)
                   .ValueGeneratedNever(); //  Guid'leri biz atayacağız 

            builder.HasOne(bm => bm.Branch)
                   .WithMany(x=>x.BranchMenus)
                   .HasForeignKey(bm => bm.BranchId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(bm => bm.Menu)
                   .WithMany(x => x.BranchMenus)
                   .HasForeignKey(bm => bm.MenuId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }

}
