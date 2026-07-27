using Respawn;
using Npgsql;

namespace Booking.IntegrationTests;

public abstract class BaseIntegrationTest : IClassFixture<TestWebApplicationFactory>, IAsyncLifetime
{
    protected readonly HttpClient HttpClient;
    protected readonly TestWebApplicationFactory Factory;

    private static Respawner? _respawner;
    private static readonly SemaphoreSlim _semaphore = new(1, 1);

    protected BaseIntegrationTest(TestWebApplicationFactory factory)
    {
        Factory = factory;
        HttpClient = factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false
        });
    }

    public async Task InitializeAsync()
    {
        await ResetDatabaseAsync();
    }

    public Task DisposeAsync()
    {
        return Task.CompletedTask;
    }

    protected async Task ResetDatabaseAsync()
    {
        var connectionString = Factory.ConnectionString;

        await _semaphore.WaitAsync();
        try
        {
            if (_respawner == null)
            {
                await using var conn = new NpgsqlConnection(connectionString);
                await conn.OpenAsync();
                _respawner = await Respawner.CreateAsync(conn, new RespawnerOptions
                {
                    DbAdapter = DbAdapter.Postgres,
                    TablesToIgnore = ["__EFMigrationsHistory"]
                });
            }
        }
        finally
        {
            _semaphore.Release();
        }

        await using var dbConn = new NpgsqlConnection(connectionString);
        await dbConn.OpenAsync();
        await _respawner!.ResetAsync(dbConn);
    }
}
