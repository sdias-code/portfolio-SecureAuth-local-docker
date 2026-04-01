using MediatR;
using SecureAuth.Application.Auth.DTOs;
using SecureAuth.Application.Security.Interfaces;
using SecureAuth.Domain.Entities;
using SecureAuth.Domain.Repositories;

namespace SecureAuth.Application.Auth.Commands.Login;

public class LoginCommandHandler : IRequestHandler<LoginCommand, AuthResponse>
{
    private readonly IUserRepository _userRepository;
    private readonly IJwtService _jwtService;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly IRefreshTokenService _refreshTokenService;

    public LoginCommandHandler(
        IUserRepository userRepository,
        IJwtService jwtService,
        IPasswordHasher passwordHasher,
        IRefreshTokenRepository refreshTokenRepository,
        IRefreshTokenService refreshTokenService)
    {
        _userRepository = userRepository;
        _jwtService = jwtService;
        _passwordHasher = passwordHasher;
        _refreshTokenRepository = refreshTokenRepository;
        _refreshTokenService = refreshTokenService;
    }

    public async Task<AuthResponse> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        // 🔥 1. Buscar usuário
        var user = await _userRepository.GetByEmailAsync(request.Email);

        // 🔥 2. Validar credenciais
        if (user == null || !_passwordHasher.Verify(request.Password, user.PasswordHash))
            throw new UnauthorizedAccessException("Credenciais inválidas");

        // 🔥 3. Gerar JWT
        var jwt = await _jwtService.GenerateToken(user);

        // 🔥 4. Gerar refresh token RAW
        var refreshTokenRaw = _refreshTokenService.Generate();

        // 🔥 5. Gerar hash do refresh token
        var hash = _refreshTokenService.Hash(refreshTokenRaw);

        // 🔥 6. Criar entidade RefreshToken
        var refreshTokenEntity = new RefreshToken
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            TokenHash = hash,
            ExpiresAt = DateTime.UtcNow.AddDays(7)
        };

        // 🔥 7. Salvar no banco
        await _refreshTokenRepository.CreateAsync(refreshTokenEntity);

        // 🔥 8. Retornar JWT + token RAW
        return new AuthResponse(jwt, refreshTokenRaw);
    }
}