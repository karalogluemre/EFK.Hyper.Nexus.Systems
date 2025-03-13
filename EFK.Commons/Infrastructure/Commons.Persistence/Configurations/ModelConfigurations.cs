using Commons.Persistence.Configurations.Users;
using Microsoft.EntityFrameworkCore;

namespace Commons.Persistence.Configurations
{
    public static class ModelConfigurations
    {
        public static void ApplyAllConfigurations(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new AppUserConfiguration());

        }
    }
}