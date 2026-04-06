using Microsoft.EntityFrameworkCore;
using Npgsql;
using Respawn;
using Respawn.Graph;
using SecureAuth.IntegrationTests.Factory;
using System.Data.Common;
using Testcontainers.PostgreSql;

namespace SecureAuth.IntegrationTests.Fixture
{
    public class PostgresContainerFixture : IAsyncLifetime
    {
        private readonly PostgreSqlContainer _container;

        private DbConnection? _connection;
        private Respawner? _respawner;

        public string ConnectionString => _container.GetConnectionString();

        public PostgresContainerFixture()
        {
            _container = new PostgreSqlBuilder("postgres:15")
                .WithDatabase("secureauth_test")
                .WithUsername("postgres")
                .WithPassword("postgres")
                .Build();
        }

        public async Task InitializeAsync()
        {
            // 🔹 Start container
            await _container.StartAsync();

            // 🔹 Apply migrations FIRST (escopo isolado)
            await using (var context = await DbContextFactory.CreateAsync(ConnectionString))
            {
                await context.Database.MigrateAsync();
            }

            // 🔹 Open persistent connection for Respawn
            _connection = new NpgsqlConnection(ConnectionString);
            await _connection.OpenAsync();

            // 🔹 Configure Respawner
            _respawner = await Respawner.CreateAsync(_connection, new RespawnerOptions
            {
                DbAdapter = DbAdapter.Postgres,

                SchemasToInclude = new[] { "public" },

                TablesToIgnore = new Table[]
                {
                    new Table("__EFMigrationsHistory", "public")
                }
            });
        }

        public async Task ResetDatabaseAsync()
        {
            if (_respawner is null)
                throw new InvalidOperationException("Respawner is not initialized.");

            if (_connection is null)
                throw new InvalidOperationException("Database connection is not initialized.");

            await _respawner.ResetAsync(_connection);
        }

        public async Task DisposeAsync()
        {
            if (_connection is not null)
            {
                await _connection.CloseAsync();
                await _connection.DisposeAsync();
            }

            // 🔥 parada limpa do container
            await _container.StopAsync();
            await _container.DisposeAsync();
        }
    }
}