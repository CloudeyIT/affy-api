using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Affy.Core.Framework.Identity.Configuration;
using Affy.Core.Framework.Identity.Entities;
using Affy.Core.Framework.Identity.Types;
using Microsoft.IdentityModel.Tokens;

namespace Affy.Core.Framework.Identity.Services;

public class TokenService
{
    private readonly IdentityConfiguration _configuration;

    public TokenService (IdentityConfiguration configuration)
    {
        _configuration = configuration;
    }

    public string GenerateToken (User user)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(
                new[]
                {
                    new Claim(JwtRegisteredClaimNames.Email, user.Email),
                    new Claim(JwtRegisteredClaimNames.UniqueName, user.UserName),
                    new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                }
            ),
            Audience = _configuration.Audience,
            Issuer = _configuration.Issuer,
            IssuedAt = DateTime.UtcNow,
            Expires = DateTime.UtcNow.AddMinutes(_configuration.ExpirationMinutes),
            SigningCredentials = KeyService.GetSigningCredentials(_configuration.SigningKey),
        };
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
}