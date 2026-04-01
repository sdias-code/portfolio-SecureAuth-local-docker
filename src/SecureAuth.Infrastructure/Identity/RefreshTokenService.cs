using System.Security.Cryptography;
using System.Text;
using SecureAuth.Application.Security.Interfaces;

namespace SecureAuth.Application.Security.Services;

public class RefreshTokenService : IRefreshTokenService
{
    private const int TokenSize = 64; // 64 bytes → 512 bits

    /// <summary>
    /// Gera um token RAW (base64) seguro e aleatório
    /// </summary>
    public string Generate()
    {
        var bytes = RandomNumberGenerator.GetBytes(TokenSize);
        return Convert.ToBase64String(bytes);
    }

    /// <summary>
    /// Gera hash SHA256 do token RAW
    /// </summary>
    public string Hash(string token)
    {
        using var sha256 = SHA256.Create();
        var bytes = Encoding.UTF8.GetBytes(token);
        var hashBytes = sha256.ComputeHash(bytes);
        return Convert.ToBase64String(hashBytes);
    }

    /// <summary>
    /// Verifica se o token RAW confere com o hash armazenado
    /// </summary>
    public bool Verify(string token, string tokenHash)
    {
        var computedHash = Hash(token);
        return CryptographicOperations.FixedTimeEquals(
            Encoding.UTF8.GetBytes(computedHash),
            Encoding.UTF8.GetBytes(tokenHash)
        );
    }
}