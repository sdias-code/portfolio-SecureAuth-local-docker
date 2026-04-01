namespace SecureAuth.Application.Auth.Commands.Login
{
    using MediatR;
    using SecureAuth.Application.Auth.DTOs;

    public record LoginCommand(string Email, string Password) : IRequest<AuthResponse>;
}
