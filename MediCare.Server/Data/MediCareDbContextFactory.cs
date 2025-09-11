using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace MediCare.Server.Data
{
    public class MediCareDbContextFactory : IDesignTimeDbContextFactory<MediCareDbContext>
    {
        public MediCareDbContext CreateDbContext(string[] args)
        {
            // Load configuration from z appsettings.json
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false)
                .Build();

            // get connection string from section ConnectionStrings
            var connectionString = configuration.GetConnectionString("DefaultConnection");

            var optionsBuilder = new DbContextOptionsBuilder<MediCareDbContext>();
            optionsBuilder.UseNpgsql(connectionString);

            return new MediCareDbContext(optionsBuilder.Options);
        }
    }
}
