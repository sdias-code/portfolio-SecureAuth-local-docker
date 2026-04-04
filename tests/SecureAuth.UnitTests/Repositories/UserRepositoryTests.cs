using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using SecureAuth.Domain.Entities;
using SecureAuth.Infrastructure.Persistence;
using SecureAuth.Infrastructure.Repositories;

namespace SecureAuth.Tests.Repositories
{
    public class UserRepositoryTests
    {
        private AppDbContext CreateDbContext()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString()) // banco isolado por teste
                .Options;

            return new AppDbContext(options);
        }

        private User CreateUser(string email = "test@email.com")
        {
            return new User
            {
                Id = Guid.NewGuid(),
                Email = email,
                PasswordHash = "hash_fake"
            };
        }

        [Fact]
        public async Task ExistsByEmailAsync_ShouldReturnTrue_WhenUserExists()
        {
            // Arrange
            var context = CreateDbContext();
            var repository = new UserRepository(context);

            var user = CreateUser("test@email.com");
            await repository.AddAsync(user);

            // Act
            var exists = await repository.ExistsByEmailAsync("test@email.com");

            // Assert
            exists.Should().BeTrue();
        }

        [Fact]
        public async Task ExistsByEmailAsync_ShouldReturnFalse_WhenUserDoesNotExist()
        {
            // Arrange
            var context = CreateDbContext();
            var repository = new UserRepository(context);

            // Act
            var exists = await repository.ExistsByEmailAsync("notfound@email.com");

            // Assert
            exists.Should().BeFalse();
        }

        [Fact]
        public async Task GetByEmailAsync_ShouldReturnUser_WhenExists()
        {
            // Arrange
            var context = CreateDbContext();
            var repository = new UserRepository(context);

            var user = CreateUser("user@email.com");
            await repository.AddAsync(user);

            // Act
            var result = await repository.GetByEmailAsync("user@email.com");

            // Assert
            result.Should().NotBeNull();
            result!.Email.Should().Be("user@email.com");
        }

        [Fact]
        public async Task GetByEmailAsync_ShouldReturnNull_WhenNotFound()
        {
            // Arrange
            var context = CreateDbContext();
            var repository = new UserRepository(context);

            // Act
            var result = await repository.GetByEmailAsync("none@email.com");

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnUser_WhenExists()
        {
            // Arrange
            var context = CreateDbContext();
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
        public async Task AddAsync_ShouldPersistUser_WithNormalizedEmail()
        {
            // Arrange
            var context = CreateDbContext();
            var repository = new UserRepository(context);

            var user = CreateUser("TEST@EMAIL.COM ");

            // Act
            await repository.AddAsync(user);

            var savedUser = await context.Users.FirstOrDefaultAsync();

            // Assert
            savedUser.Should().NotBeNull();
            savedUser!.Email.Should().Be("test@email.com");
        }
    }
}