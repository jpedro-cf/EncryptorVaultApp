using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using Microsoft.IdentityModel.Tokens;
using MyMVCProject.Api.Entities;

namespace MyMVCProject.Api.Infra.Security;

public class JwtTokenHandler(IConfiguration config, RSA privateKey, RSA publicKey)
{
    private readonly string _issuer = config["Jwt:Issuer"] ?? "";
    private readonly string _audience = config["Jwt:Audience"] ?? "";

    public string Encode(User user, DateTime expiresAt)
    {
        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email ?? ""),
        };

        return Encode(claims, expiresAt);

    }

    public string Encode(List<Claim> claims, DateTime expiresAt)
    {
        var creds = new SigningCredentials(new RsaSecurityKey(privateKey), 
            SecurityAlgorithms.RsaSha256);

        claims.Add(new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()));
        
        var token = new JwtSecurityToken(
            issuer: _issuer,
            audience: _audience,
            claims: claims,
            notBefore: DateTime.Now,
            expires: expiresAt,
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public ClaimsPrincipal Decode(string token)
    {
        var validationParams = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = _issuer,
            ValidateAudience = true,
            ValidAudience = _audience,
            ValidateLifetime = true,
            RequireExpirationTime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new RsaSecurityKey(publicKey)
        };

        return new JwtSecurityTokenHandler().ValidateToken(token, validationParams, out _);
    }
}