using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Testcontainers.PostgreSql;
using SportsGurukul.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using SportsGurukul.Api;

namespace Booking.IntegrationTests;

public class TestWebApplicationFactory : WebApplicationFactory<ApiMarker>, IAsyncLifetime
{
    private readonly PostgreSqlContainer _postgreSqlContainer;

    public string ConnectionString => _postgreSqlContainer.GetConnectionString();

    public PostgreSqlContainer PostgreSqlContainer => _postgreSqlContainer;

    public TestWebApplicationFactory()
    {
        _postgreSqlContainer = new PostgreSqlBuilder("postgres:16-alpine")
            .WithDatabase("SportsGurukulTestDb")
            .WithUsername("test")
            .WithPassword("test")
            .WithPortBinding(5432, true)
            .Build();
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureAppConfiguration((context, config) =>
        {
            config.AddInMemoryCollection(new Dictionary<string, string?>
            {
                {"ConnectionStrings:DefaultConnection", _postgreSqlContainer.GetConnectionString()}
            });
        });

        builder.ConfigureServices(services =>
        {
            var descriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<ApplicationDbContext>));

            if (descriptor != null)
            {
                services.Remove(descriptor);
            }

            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseNpgsql(_postgreSqlContainer.GetConnectionString());
            });

            var sp = services.BuildServiceProvider();

            using (var scope = sp.CreateScope())
            {
                var scopedServices = scope.ServiceProvider;
                var db = scopedServices.GetRequiredService<ApplicationDbContext>();
                db.Database.Migrate();
            }
        });
    }

    public async Task InitializeAsync()
    {
        await _postgreSqlContainer.StartAsync();
    }

    public new async Task DisposeAsync()
    {
        await _postgreSqlContainer.StopAsync();
        await base.DisposeAsync();
    }
}
