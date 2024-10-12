using Battleships.DAL.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Battleships.DAL.DbFactory
{
    public class DbContextFactory : IDbContextFactory
    {
        private readonly IConfiguration _configuration;

        public DbContextFactory(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        // Configure EF Core with InMemory database for testing.
        public BattleshipDbContext CreateDbContext()
        {
            var optionsBuilder = new DbContextOptionsBuilder<BattleshipDbContext>();

            // Access the "UseInMemoryDatabase" configuration section.
            var useInMemorySection = _configuration.GetSection("UseInMemoryDatabase");
            var useInMemory = useInMemorySection.Value != null && bool.Parse(useInMemorySection.Value);

            if (useInMemory)
            {
                optionsBuilder.UseInMemoryDatabase("BattleshipDb");
            }
            else
            {
                var connectionString = _configuration.GetConnectionString("DefaultConnection");
                optionsBuilder.UseSqlServer(connectionString, sqlOptions =>
                {
                    sqlOptions.EnableRetryOnFailure(
                        maxRetryCount: 5,
                        maxRetryDelay: TimeSpan.FromSeconds(10),
                        errorNumbersToAdd: null);
                });
            }

            return new BattleshipDbContext(optionsBuilder.Options);
        }
    }
}
