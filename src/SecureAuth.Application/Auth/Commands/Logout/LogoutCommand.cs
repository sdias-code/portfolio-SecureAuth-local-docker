
using MediatR;

namespace SecureAuth.Application.Auth.Commands.Logout
{
    public record LogoutCommand(string RefreshToken, bool LogoutAll) : IRequest<Unit>;
}

