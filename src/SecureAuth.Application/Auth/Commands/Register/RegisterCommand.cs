namespace SecureAuth.Application.Auth.Commands.Register
{
    using MediatR;

    public record RegisterCommand(string Email, string Password) : IRequest<Unit>;
}
