using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using FluentAssertions;
using SecureAuth.Application.Auth.DTOs;
using SecureAuth.IntegrationTests.Fixture;

namespace SecureAuth.IntegrationTests
{

    public class AuthControllerTests
        : IClassFixture<PostgresContainerFixture>
    {
        private readonly HttpClient _client;
        private readonly PostgresContainerFixture _fixture;

        public AuthControllerTests(PostgresContainerFixture fixture)
        {
            _fixture = fixture;

            var factory = new CustomWebApplicationFactory(_fixture.ConnectionString);
            _client = factory.CreateClient();
        }

        private RegisterRequest CreateRegisterRequest(string email = "test@test.com")
        {
            return new RegisterRequest
            {
                Email = email,
                Password = "123456"
            };
        }

        [Fact]
        public async Task Register_ShouldCreateUser()
        {
            await _fixture.ResetDatabaseAsync();

            var request = CreateRegisterRequest("test@test.com");

            var response = await _client.PostAsJsonAsync("/api/auth/register", request);

            response.StatusCode.Should().Be(System.Net.HttpStatusCode.Created);
        }

        [Fact]
        public async Task Login_ShouldReturnToken()
        {
            await _fixture.ResetDatabaseAsync();

            var request = CreateRegisterRequest("test@test.com");

            await _client.PostAsJsonAsync("/api/auth/register", request);

            var response = await _client.PostAsJsonAsync("/api/auth/login", new LoginRequest
            {
                Email = "test@test.com",
                Password = "123456"
            });

            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadFromJsonAsync<AuthResponse>();

            result.Should().NotBeNull();
            result!.Token.Should().NotBeNullOrEmpty();
            result.RefreshToken.Should().NotBeNullOrEmpty();
        }

        [Fact]
        public async Task Login_ShouldFail_WithInvalidCredentials()
        {
            await _fixture.ResetDatabaseAsync();

            var response = await _client.PostAsJsonAsync("/api/auth/login", new LoginRequest
            {
                Email = "wrong@test.com",
                Password = "123456"
            });

            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task Refresh_ShouldReturnNewToken()
        {
            await _fixture.ResetDatabaseAsync();

            await _client.PostAsJsonAsync("/api/auth/register", new RegisterRequest
            {
                Email = "test@test.com",
                Password = "123456"
            });

            var login = await _client.PostAsJsonAsync("/api/auth/login", new LoginRequest
            {
                Email = "test@test.com",
                Password = "123456"
            });

            var loginResult = await login.Content.ReadFromJsonAsync<AuthResponse>();

            // importante: imprimir o token para facilitar debug em caso de falha
            var token = loginResult!.Token;
            Console.WriteLine(token);

            var refresh = await _client.PostAsJsonAsync("/api/auth/refresh", new RefreshTokenRequest(loginResult!.RefreshToken));

            refresh.EnsureSuccessStatusCode();

            var refreshResult = await refresh.Content.ReadFromJsonAsync<AuthResponse>();

            refreshResult.Should().NotBeNull();
            refreshResult!.Token.Should().NotBeNullOrEmpty();
        }

        [Fact]
        public async Task Logout_ShouldSucceed()
        {
            await _fixture.ResetDatabaseAsync();

            await _client.PostAsJsonAsync("/api/auth/register", new RegisterRequest
            {
                Email = "test@test.com",
                Password = "123456"
            });

            var login = await _client.PostAsJsonAsync("/api/auth/login", new LoginRequest
            {
                Email = "test@test.com",
                Password = "123456"
            });

            var loginResult = await login.Content.ReadFromJsonAsync<AuthResponse>();

            // importante: imprimir o token para facilitar debug em caso de falha
            var token = loginResult!.Token;
            Console.WriteLine(token);

            var logout = await _client.PostAsJsonAsync("/api/auth/logout", new LogoutRequest(loginResult!.RefreshToken, false));

            logout.EnsureSuccessStatusCode();
        }

        [Fact]
        public async Task Me_ShouldReturnUser_WhenAuthenticated()
        {
            // Arrange
            await _fixture.ResetDatabaseAsync();

            var email = "test@test.com";
            var password = "123456";

            // 🔹 Register
            var registerResponse = await _client.PostAsJsonAsync("/api/auth/register", new RegisterRequest
            {
                Email = email,
                Password = password
            });

            registerResponse.EnsureSuccessStatusCode();

            // 🔹 Login
            var loginResponse = await _client.PostAsJsonAsync("/api/auth/login", new LoginRequest
            {
                Email = email,
                Password = password
            });

            // 🔥 garante que login NÃO falhou silenciosamente
            loginResponse.EnsureSuccessStatusCode();

            var loginResult = await loginResponse.Content.ReadFromJsonAsync<AuthResponse>();

            // 🔥 valida retorno
            loginResult.Should().NotBeNull();
            loginResult!.Token.Should().NotBeNullOrEmpty();

            // 🔥 DEBUG FORTE (CI friendly)
            Console.WriteLine("========== TOKEN DEBUG ==========");
            Console.WriteLine(loginResult.Token);
            Console.WriteLine($"Length: {loginResult.Token.Length}");
            Console.WriteLine($"Has dots: {loginResult.Token.Count(c => c == '.')}");
            Console.WriteLine("=================================");

            // 🔥 garante formato JWT válido
            loginResult.Token.Count(c => c == '.').Should().Be(2, "Token JWT deve ter 3 partes");

            // 🔹 Auth header
            _client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", loginResult.Token);

            // Act
            var response = await _client.GetAsync("/api/auth/me");

            // 🔥 debug se falhar
            if (!response.IsSuccessStatusCode)
            {
                var body = await response.Content.ReadAsStringAsync();
                Console.WriteLine("========== RESPONSE ERROR ==========");
                Console.WriteLine(body);
                Console.WriteLine($"Status: {response.StatusCode}");
                Console.WriteLine("===================================");
            }

            // Assert
            response.EnsureSuccessStatusCode();
        }

        [Fact]
        public async Task Me_ShouldReturnUnauthorized_WhenTokenInvalid()
        {
            _client.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", "token-invalido");

            var response = await _client.GetAsync("/api/auth/me");

            response.StatusCode.Should().Be(System.Net.HttpStatusCode.Unauthorized);
        } 

    }
}