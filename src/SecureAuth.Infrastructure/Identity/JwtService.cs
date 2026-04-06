using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using SecureAuth.Application.Common.Settings;
using SecureAuth.Application.Security.Interfaces;
using SecureAuth.Domain.Entities;
using SecureAuth.Domain.Exceptions;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace SecureAuth.Infrastructure.Identity;

public class JwtService : IJwtService
{
    private readonly JwtSettings _jwtSettings;

    public JwtService(IOptions<JwtSettings> jwtOptions)
    {
        _jwtSettings = jwtOptions.Value;
    }

    public Task<string> GenerateToken(User user)
    {
        // Debug: Verificar as configurações de JWT
        Console.WriteLine("JWT Configurações: _jwtSettings.ExpirationMinutes");
        Console.WriteLine(_jwtSettings.ExpirationMinutes);
        Console.WriteLine("JWT Configurações: _jwtSettings.Key.Length");
        Console.WriteLine(_jwtSettings.Key.Length);

        var key = _jwtSettings.Key
            ?? throw new BusinessException("JWT Key não configurada");

        if (key.Length < 32)
            throw new BusinessException("JWT Key muito curta");

        var keyBytes = Encoding.UTF8.GetBytes(key);

        var claims = new[]
{
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var creds = new SigningCredentials(
            new SymmetricSecurityKey(keyBytes),
            SecurityAlgorithms.HmacSha256
        );

        var token = new JwtSecurityToken(
            issuer: _jwtSettings.Issuer,
            audience: _jwtSettings.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(_jwtSettings.ExpirationMinutes),
            signingCredentials: creds
        );

        return Task.FromResult(new JwtSecurityTokenHandler().WriteToken(token));
    }
}