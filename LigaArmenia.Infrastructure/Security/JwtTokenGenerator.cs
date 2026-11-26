using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using LigaArmenia.Application.DTOs.Auth;
using LigaArmenia.Application.Interfaces.Security;
using LigaArmenia.Infrastructure.Security.Options;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace LigaArmenia.Infrastructure.Security;

public sealed class JwtTokenGenerator : IJwtTokenGenerator
{
    private readonly JwtOptions _options;

    public JwtTokenGenerator(IOptions<JwtOptions> options)
    {
        _options = options.Value;
    }

    public bool IsConfigured => _options.IsValid;

    public JwtTokenResultDto GenerateToken(AuthenticatedUserDto user)
    {
        if (!_options.IsValid)
        {
            throw new InvalidOperationException(
                "La configuración JWT (Jwt:Key, Jwt:Issuer, Jwt:Audience) no está configurada correctamente en User Secrets o variables de entorno.");
        }

        var now = DateTime.UtcNow;
        var expires = now.AddMinutes(_options.ExpirationMinutes);

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Name, user.Nombres),
            new(ClaimTypes.Email, user.Correo),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        if (user.VocalId.HasValue)
            claims.Add(new Claim("vocal_id", user.VocalId.Value.ToString()));

        foreach (var role in user.Roles)
            claims.Add(new Claim(ClaimTypes.Role, role));

        var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.Key));
        var credentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _options.Issuer,
            audience: _options.Audience,
            claims: claims,
            notBefore: now,
            expires: expires,
            signingCredentials: credentials);

        var encodedToken = new JwtSecurityTokenHandler().WriteToken(token);

        return new JwtTokenResultDto
        {
            AccessToken = encodedToken,
            TokenType = "Bearer",
            ExpiresAtUtc = expires
        };
    }
}
