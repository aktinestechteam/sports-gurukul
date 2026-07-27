using Respawn;
using Npgsql;

namespace Booking.IntegrationTests;

public sealed class DatabaseResetFixture : IAsyncDisposable
{
    private readonly string _connectionString;
    private Respawner? _respawner;

    public DatabaseResetFixture(string connectionString)
    {
        _connectionString = connectionString;
    }

    public async Task InitializeAsync()
    {
        await using var conn = new NpgsqlConnection(_connectionString);
        await conn.OpenAsync();
        _respawner = await Respawner.CreateAsync(conn, new RespawnerOptions
        {
            DbAdapter = DbAdapter.Postgres,
            TablesToIgnore = ["__EFMigrationsHistory"]
        });
    }

    public async Task ResetAsync()
    {
        if (_respawner == null)
            await InitializeAsync();

        await using var dbConn = new NpgsqlConnection(_connectionString);
        await dbConn.OpenAsync();
        await _respawner!.ResetAsync(dbConn);
    }

    public async ValueTask DisposeAsync()
    {
        if (_respawner != null)
        {
            await Task.CompletedTask;
        }
    }
}
