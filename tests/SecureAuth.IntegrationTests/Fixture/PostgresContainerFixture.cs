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

        private DbConnection _connection;

        public string ConnectionString => _container.GetConnectionString();

        private Respawner _respawner;

        public PostgresContainerFixture()
        {
            _container = new PostgreSqlBuilder()
                .WithImage("postgres:15")
                .WithDatabase("secureauth_test")
                .WithUsername("postgres")
                .WithPassword("postgres")
                .Build();
        }

        public async Task InitializeAsync()
        {
            await _container.StartAsync();

            _connection = new NpgsqlConnection(ConnectionString);
            await _connection.OpenAsync();

            // 🔥 GARANTE QUE AS TABELAS EXISTEM
            var context = await DbContextFactory.CreateAsync(ConnectionString);
            await context.Database.MigrateAsync();

            // 🔥 cria o Respawner (configuração importante)
            _respawner = await Respawner.CreateAsync(_connection, new RespawnerOptions
            {
                DbAdapter = DbAdapter.Postgres,

                // ignora tabelas de migrations do EF
                TablesToIgnore = new Table[]
                    {
                        new Table("__EFMigrationsHistory", "public")
                    }
            });
        }

        public async Task ResetDatabaseAsync()
        {
            await _respawner.ResetAsync(_connection);
        }

        public async Task DisposeAsync()
        {
            await _connection.DisposeAsync();
            await _container.DisposeAsync();
        }
    }
}