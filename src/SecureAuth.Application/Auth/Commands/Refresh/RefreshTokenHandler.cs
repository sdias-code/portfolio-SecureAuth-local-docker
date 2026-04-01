using MediatR;
using SecureAuth.Application.Auth.DTOs;
using SecureAuth.Application.Security.Interfaces;
using SecureAuth.Domain.Entities;
using SecureAuth.Domain.Repositories;

namespace SecureAuth.Application.Auth.Commands.Refresh;

public class RefreshTokenHandler : IRequestHandler<RefreshTokenCommand, AuthResponse>
{
    private readonly IRefreshTokenRepository _repository;
    private readonly IJwtService _jwtService;
    private readonly IRefreshTokenService _service;

    public RefreshTokenHandler(
        IRefreshTokenRepository repository,
        IJwtService jwtService,
        IRefreshTokenService service)
    {
        _repository = repository;
        _jwtService = jwtService;
        _service = service;
    }

    public async Task<AuthResponse> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        // 🔥 1. Buscar todos tokens ativos do usuário
        var token = await _repository.GetByHashWithUserAsync(_service.Hash(request.RefreshToken));

        // ❌ Token inválido
        if (token == null || !_service.Verify(request.RefreshToken, token.TokenHash))
            throw new UnauthorizedAccessException("Token inválido");

        // 💣 Reuse detection
        if (!token.IsActiveToken())
        {
            await _repository.RevokeAllByUserAsync(token.UserId);
            throw new UnauthorizedAccessException("Token reutilizado detectado. Sessão encerrada.");
        }

        // ❌ Expirado
        if (token.ExpiresAt < DateTime.UtcNow)
            throw new UnauthorizedAccessException("Token expirado");

        // 🔄 ROTATION: revogar token antigo
        token.Revoke();

        // 🔄 Criar novo refresh token RAW + hash
        var newTokenRaw = _service.Generate();
        var newHash = _service.Hash(newTokenRaw);

        var newRefreshToken = new RefreshToken
        {
            Id = Guid.NewGuid(),
            UserId = token.UserId,
            TokenHash = newHash,
            ExpiresAt = DateTime.UtcNow.AddDays(7),
            ReplacedByTokenHash = newHash
        };

        // 🔥 Salvar novo token
        await _repository.CreateAsync(newRefreshToken);

        // 🔥 Persistir revogação do token antigo
        await _repository.UpdateAsync();

        // 🔄 Gera novo JWT
        var jwt = await _jwtService.GenerateToken(token.User);

        // 🔥 Retornar JWT + novo refresh token RAW
        return new AuthResponse(jwt, newTokenRaw);
    }
}