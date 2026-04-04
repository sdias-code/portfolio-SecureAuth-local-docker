using Testcontainers.PostgreSql;

namespace SecureAuth.IntegrationTests.Fixture
{
    public class PostgresContainerFixture : IAsyncLifetime
    {
        private readonly PostgreSqlContainer _container;

        public string ConnectionString => _container.GetConnectionString();

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
        }

        public async Task DisposeAsync()
        {
            await _container.DisposeAsync();
        }
    }
}