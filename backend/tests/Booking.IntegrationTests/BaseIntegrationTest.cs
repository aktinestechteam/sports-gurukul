using Xunit;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net.Http;
using System.Threading.Tasks;
using SportsGurukul.Api;
using FluentAssertions;
using Respawn;
using Npgsql;
using Testcontainers.PostgreSql;
using Xunit.Sdk;

namespace Booking.IntegrationTests
{
    public abstract class BaseIntegrationTest : IClassFixture<TestWebApplicationFactory>, IAsyncLifetime
    {
        private readonly WebApplicationFactory<Program> _factory;
        protected readonly HttpClient HttpClient;
        private readonly PostgreSqlContainer _postgreSqlContainer;
        private static Respawner _respawner = null!;

        protected BaseIntegrationTest(TestWebApplicationFactory factory)
        {
            _factory = factory;
            HttpClient = _factory.CreateClient();
            _postgreSqlContainer = factory.PostgreSqlContainer;
        }

        public async Task InitializeAsync()
        {
            await _postgreSqlContainer.StartAsync();

            await InitializeRespawner();

            await ResetDatabase();
        }

        public async Task DisposeAsync()
        {
            await _postgreSqlContainer.StopAsync();
        }

        private async Task InitializeRespawner()
        {
            if (_respawner == null)
            {
                var connection = new NpgsqlConnection(_postgreSqlContainer.GetConnectionString());
                await connection.OpenAsync();

                _respawner = await Respawner.CreateAsync(connection, new RespawnerOptions
                {
                    DbAdapter = DbAdapter.Postgres,
                    TablesToIgnore = new Respawn.Graph.Table[] { "__EFMigrationsHistory" }
                });

                await connection.CloseAsync();
            }
        }

        protected async Task ResetDatabase()
        {
            var connection = new NpgsqlConnection(_postgreSqlContainer.GetConnectionString());
            await connection.OpenAsync();
            await _respawner.ResetAsync(connection);
            await connection.CloseAsync();
        }
    }
}
