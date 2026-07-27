using Testcontainers.PostgreSql;

namespace Booking.IntegrationTests;

public sealed class PostgreSqlContainerFixture : IAsyncLifetime
{
    public PostgreSqlContainer Container { get; }

    public string ConnectionString => Container.GetConnectionString();

    public PostgreSqlContainerFixture()
    {
        Container = new PostgreSqlBuilder("postgres:16-alpine")
            .WithDatabase("SportsGurukulTestDb")
            .WithUsername("test")
            .WithPassword("test")
            .WithPortBinding(5432, true)
            .Build();
    }

    public async Task InitializeAsync()
    {
        await Container.StartAsync();
    }

    public async Task DisposeAsync()
    {
        await Container.DisposeAsync();
    }
}
