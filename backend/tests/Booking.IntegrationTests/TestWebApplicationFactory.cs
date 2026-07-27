using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Testcontainers.PostgreSql;
using SportsGurukul.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using SportsGurukul.Api;

namespace Booking.IntegrationTests
{
    public class TestWebApplicationFactory : WebApplicationFactory<Program>, IAsyncLifetime
    {
        public PostgreSqlContainer PostgreSqlContainer { get; private set; }

        public TestWebApplicationFactory()
        {
            PostgreSqlContainer = new PostgreSqlBuilder()
                .WithImage("postgres:latest")
                .WithDatabase("SportsGurukulTestDb")
                .WithUsername("admin")
                .WithPassword("admin")
                .Build();
        }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureAppConfiguration((context, config) =>
            {
                config.AddInMemoryCollection(new Dictionary<string, string?>
                {
                    {"ConnectionStrings:DefaultConnection", PostgreSqlContainer.GetConnectionString()}
                });
            });

            builder.ConfigureServices(services =>
            {
                // Remove the existing DbContext registration
                var descriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(DbContextOptions<ApplicationDbContext>));

                if (descriptor != null)
                {
                    services.Remove(descriptor);
                }

                // Add DbContext using the Testcontainers connection string
                services.AddDbContext<ApplicationDbContext>(options =>
                {
                    options.UseNpgsql(PostgreSqlContainer.GetConnectionString());
                });

                var sp = services.BuildServiceProvider();

                using (var scope = sp.CreateScope())
                {
                    var scopedServices = scope.ServiceProvider;
                    var db = scopedServices.GetRequiredService<ApplicationDbContext>();
                    db.Database.Migrate(); // Apply migrations
                }
            });
        }

        public async Task InitializeAsync()
        {
            await PostgreSqlContainer.StartAsync();
        }

        public new async Task DisposeAsync()
        {
            await PostgreSqlContainer.StopAsync();
            await base.DisposeAsync();
        }
    }
}
