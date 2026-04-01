using MediatR;
using SecureAuth.Application.Security.Interfaces;
using SecureAuth.Domain.Exceptions;
using SecureAuth.Domain.Repositories;

namespace SecureAuth.Application.Auth.Commands.Logout;

public class LogoutCommandHandler : IRequestHandler<LogoutCommand, Unit>
{
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly IRefreshTokenService _refreshTokenService;

    public LogoutCommandHandler(
        IRefreshTokenRepository refreshTokenRepository,
        IRefreshTokenService refreshTokenService)
    {
        _refreshTokenRepository = refreshTokenRepository;
        _refreshTokenService = refreshTokenService;
    }

    public async Task<Unit> Handle(LogoutCommand request, CancellationToken cancellationToken)
    {
        // 🔥 1. Hash do token recebido
        var hash = _refreshTokenService.Hash(request.RefreshToken);

        // 🔥 2. Buscar token
        var token = await _refreshTokenRepository.GetByHashWithUserAsync(hash);

        if (token == null)
            throw new BusinessException("Token inválido");

        // 🔥 3. Logout global (opcional)
        if (request.LogoutAll)
        {
            await _refreshTokenRepository.RevokeAllByUserAsync(token.UserId);
            return Unit.Value;
        }

        // 🔥 4. Idempotência
        if (!token.IsActiveToken())
            return Unit.Value;

        // 🔥 5. Revogar token atual
        token.Revoke();

        // 🔥 6. Persistir
        await _refreshTokenRepository.UpdateAsync();

        return Unit.Value;
    }
}