using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Commons.Persistence.Configurations.Menu
{
    public class MenuConfiguration : IEntityTypeConfiguration<Commons.Domain.Models.Menus.Menu>
    {
        public void Configure(EntityTypeBuilder<Commons.Domain.Models.Menus.Menu> builder)
        {
            builder.HasOne(m => m.ParentMenu)
                   .WithMany(m => m.Items)
                   .HasForeignKey(m => m.MenuId)
                   .OnDelete(DeleteBehavior.NoAction);
        }
    }

}
