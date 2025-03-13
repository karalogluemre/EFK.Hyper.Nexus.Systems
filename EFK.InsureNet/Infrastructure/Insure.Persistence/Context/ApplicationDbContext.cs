using Commons.Domain.Models.User;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Insure.Persistence.Context
{
    public class ApplicationDbContext : IdentityDbContext<AppUser, AppRole, Guid>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            Commons.Persistence.Configurations.ModelConfigurations.ApplyAllConfigurations(modelBuilder);
            Configurations.ModelConfigurations.ApplyAllConfigurations(modelBuilder);
            base.OnModelCreating(modelBuilder);
        }

        #region DbSets
        public DbSet<AppUser> AppUsers { get; set; }
        public DbSet<AppRole> AppRole { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }
        #endregion
    }
}
