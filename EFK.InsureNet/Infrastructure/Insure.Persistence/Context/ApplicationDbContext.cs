using Commons.Domain.Models.Adreses;
using Commons.Domain.Models.Branches;
using Commons.Domain.Models.Companies;
using Commons.Domain.Models.Departments;
using Commons.Domain.Models.Groups;
using Commons.Domain.Models.Organizations;
using Commons.Domain.Models.Packages;
using Commons.Domain.Models.Role;
using Commons.Domain.Models.Units;
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

        #region Adreses
        public DbSet<Province> Provinces { get; set; }
        public DbSet<District> Districts { get; set; }
        #endregion

        #region Pack - Firm - Org -  
        public DbSet<Package> Packages { get; set; }
        public DbSet<PackageMenu> PackageMenus { get; set; }
        public DbSet<Company> Companies { get; set; }
        public DbSet<Branch> Branches { get; set; }
        public DbSet<BranchMenu> BranchMenus { get; set; }
        public DbSet<Organization> Organizations { get; set; }
        public DbSet<OrganizationMenu> OrganizationMenus { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<Group> Groups { get; set; }
        public DbSet<Unit> Units { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }
        public DbSet<UserMenuPermission> UserMenuPermissions { get; set; }
        public DbSet<UserDepartment> UserDepartments { get; set; }
        public DbSet<UserOrganization> UserOrganizations { get; set; }
        public DbSet<UserGroup> UserGroups { get; set; }
        public DbSet<UserUnit> UserUnits { get; set; }
        #endregion
    }
}
