using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using SecureAuth.Domain.Entities;
using SecureAuth.Infrastructure.Persistence;
using SecureAuth.Infrastructure.Repositories;
using SecureAuth.IntegrationTests.Factory;
using SecureAuth.IntegrationTests.Fixture;

namespace SecureAuth.IntegrationTests.Repositories
{
    public class UserRepositoryIntegrationTests
        : IClassFixture<PostgresContainerFixture>
    {
        private readonly PostgresContainerFixture _fixture;

        public UserRepositoryIntegrationTests(PostgresContainerFixture fixture)
        {
            _fixture = fixture;
        }

        private User CreateUser(string email = "user@test.com")
        {
            return new User
            {
                Id = Guid.NewGuid(),
                Email = email,
                PasswordHash = "hash"
            };
        }

        private async Task ResetDatabase() =>
            await _fixture.ResetDatabaseAsync();

        private async Task<AppDbContext> CreateContext() =>
            await DbContextFactory.CreateAsync(_fixture.ConnectionString);


        [Fact]
        public async Task AddAsync_ShouldPersistUser_InDatabase()
        {
            // Arrange
            await ResetDatabase();

            var context = await CreateContext();

            var repository = new UserRepository(context);

            var user = CreateUser("test@test.com");

            // Act
            await repository.AddAsync(user);

            // Assert (novo contexto!)
            var validationContext = await DbContextFactory.CreateAsync(_fixture.ConnectionString);          

            // importante: desabilitar tracking para garantir que estamos lendo do banco, não do cache
            validationContext.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;

            var saved = await validationContext.Users
                .FirstOrDefaultAsync(u => u.Email == "test@test.com");

            saved.Should().NotBeNull();
            saved!.Email.Should().Be("test@test.com");
        }

        [Fact]
        public async Task ExistsByEmailAsync_ShouldReturnTrue_WhenUserExists()
        {
            // Arrange
            await ResetDatabase();

            var context = await CreateContext();

            var repository = new UserRepository(context);

            var user = CreateUser("exists@test.com");
            await repository.AddAsync(user);

            // Act
            var exists = await repository.ExistsByEmailAsync("exists@test.com");

            // Assert
            exists.Should().BeTrue();
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnCorrectUser()
        {
            // Arrange
            await ResetDatabase();

            var context = await CreateContext();

            var repository = new UserRepository(context);

            var user = CreateUser();
            await repository.AddAsync(user);

            // Act
            var result = await repository.GetByIdAsync(user.Id);

            // Assert
            result.Should().NotBeNull();
            result!.Id.Should().Be(user.Id);
            result.Email.Should().Be(user.Email);
        }

        [Fact]
        public async Task AddAsync_ShouldNotAllowDuplicateEmail()
        {
            await ResetDatabase();

            var context = await CreateContext();

            var repository = new UserRepository(context);

            var user1 = CreateUser("test@test.com");
            var user2 = CreateUser("test@test.com");

            await repository.AddAsync(user1);

            // Act
            Func<Task> act = async () => await repository.AddAsync(user2);

            // Assert            
            await act.Should().ThrowAsync<DbUpdateException>();
        }

        [Fact]
        public async Task ExistsByEmailAsync_ShouldReturnFalse_WhenUserDoesNotExist()
        {
            await ResetDatabase();

            var context = await CreateContext();

            var repository = new UserRepository(context);

            var exists = await repository.ExistsByEmailAsync("notfound@test.com");

            exists.Should().BeFalse();            
        }
    }
}