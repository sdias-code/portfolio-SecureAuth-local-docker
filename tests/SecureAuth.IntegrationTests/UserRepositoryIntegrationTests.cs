using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using SecureAuth.Domain.Entities;
using SecureAuth.Infrastructure.Repositories;
using SecureAuth.IntegrationTests.Factory;
using SecureAuth.IntegrationTests.Fixture;
using SecureAuth.IntegrationTests.Helper;

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

        [Fact]
        public async Task AddAsync_ShouldPersistUser_InDatabase()
        {
            // Arrange
            var context = DbContextFactory.Create(_fixture.ConnectionString);

            await DatabaseHelper.ResetDatabaseAsync(context);

            var repository = new UserRepository(context);

            var user = CreateUser("test@test.com");

            // Act
            await repository.AddAsync(user);

            // Assert (novo contexto!)
            var validationContext = DbContextFactory.Create(_fixture.ConnectionString);

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
            var context = DbContextFactory.Create(_fixture.ConnectionString);

            await DatabaseHelper.ResetDatabaseAsync(context);

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
            var context = DbContextFactory.Create(_fixture.ConnectionString);

            await DatabaseHelper.ResetDatabaseAsync(context);

            var repository = new UserRepository(context);

            var user = CreateUser();
            await repository.AddAsync(user);

            // Act
            var result = await repository.GetByIdAsync(user.Id);

            // Assert
            result.Should().NotBeNull();
            result!.Id.Should().Be(user.Id);
        }

        [Fact]
        public async Task AddAsync_ShouldNotAllowDuplicateEmail()
        {
            var context = DbContextFactory.Create(_fixture.ConnectionString);
            await DatabaseHelper.ResetDatabaseAsync(context);

            var repository = new UserRepository(context);

            var user1 = CreateUser("test@test.com");
            var user2 = CreateUser("test@test.com");

            await repository.AddAsync(user1);

            // Act
            Func<Task> act = async () => await repository.AddAsync(user2);

            // Assert
            await act.Should().ThrowAsync<Exception>(); // ideal: custom exception
        }
    }
}