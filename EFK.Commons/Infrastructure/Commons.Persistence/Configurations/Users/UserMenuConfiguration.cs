﻿using Commons.Domain.Models.User;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Commons.Persistence.Configurations.Users
{
    public class UserMenuConfiguration : IEntityTypeConfiguration<UserMenuPermission>
    {
        public void Configure(EntityTypeBuilder<UserMenuPermission> builder)
        {
            builder.HasKey(ump => new { ump.AppUserId, ump.MenuId });

            builder.HasOne(ump => ump.AppUser)
                   .WithMany(x=>x.UserMenuPermissions)
                   .HasForeignKey(ump => ump.AppUserId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(ump => ump.Menu)
                   .WithMany(x=>x.UserMenuPermissions)
                   .HasForeignKey(ump => ump.MenuId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.Property(ump => ump.CanCreate).IsRequired();
            builder.Property(ump => ump.CanRead).IsRequired();
            builder.Property(ump => ump.CanUpdate).IsRequired();
            builder.Property(ump => ump.CanDelete).IsRequired();
        }
    }
}
