using SecureAuth.Domain.Entities;

namespace SecureAuth.Application.Security.Interfaces
{
    public interface IJwtService
    {
        Task<string> GenerateToken(User user);
    }
}
