using Commons.Persistence.Configurations.Branches;
using Commons.Persistence.Configurations.Companies;
using Commons.Persistence.Configurations.Departments;
using Commons.Persistence.Configurations.Groups;
using Commons.Persistence.Configurations.Organizations;
using Commons.Persistence.Configurations.Packages;
using Commons.Persistence.Configurations.Roles;
using Commons.Persistence.Configurations.Units;
using Commons.Persistence.Configurations.Users;
using Microsoft.EntityFrameworkCore;

namespace Commons.Persistence.Configurations
{
    public static class ModelConfigurations
    {
        public static void ApplyAllConfigurations(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new AppUserConfiguration());
            modelBuilder.ApplyConfiguration(new UserMenuConfiguration());
            modelBuilder.ApplyConfiguration(new UserRoleConfiguration());
            modelBuilder.ApplyConfiguration(new PackageConfiguration());
            modelBuilder.ApplyConfiguration(new PackageMenuConfiguration());
            modelBuilder.ApplyConfiguration(new CompanyConfiguration());
            modelBuilder.ApplyConfiguration(new BranchConfiguration());
            modelBuilder.ApplyConfiguration(new BranchMenuConfiguration());
            modelBuilder.ApplyConfiguration(new OrganizationConfiguration());
            modelBuilder.ApplyConfiguration(new UserOrganizationConfiguration());
            modelBuilder.ApplyConfiguration(new OrganizationMenuConfiguration());
            modelBuilder.ApplyConfiguration(new GroupConfiguration());
            modelBuilder.ApplyConfiguration(new UserGroupConfiguration());
            modelBuilder.ApplyConfiguration(new DepartmentConfiguration());
            modelBuilder.ApplyConfiguration(new UserDepartmentConfiguration());
            modelBuilder.ApplyConfiguration(new UnitConfiguration());
            modelBuilder.ApplyConfiguration(new UserUnitConfiguration());
            modelBuilder.ApplyConfiguration(new RoleMenuPermissionConfiguration());
        }
    }
}