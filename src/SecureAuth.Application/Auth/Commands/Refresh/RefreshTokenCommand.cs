namespace SecureAuth.Application.Auth.Commands.Refresh
{
    using MediatR;
    using SecureAuth.Application.Auth.DTOs;

    public record RefreshTokenCommand(string RefreshToken) : IRequest<AuthResponse>;
}
