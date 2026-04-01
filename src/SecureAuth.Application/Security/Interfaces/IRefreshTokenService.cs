namespace SecureAuth.Application.Security.Interfaces
{
    public interface IRefreshTokenService
    {
        string Generate();
        string Hash(string token);
        bool Verify(string token, string tokenHash);
    }
}
