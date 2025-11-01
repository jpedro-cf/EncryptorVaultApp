using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using EncryptionApp.Api.Entities;
using EncryptionApp.Api.Global;
using EncryptionApp.Api.Global.Errors;
using Microsoft.IdentityModel.Tokens;
using File = System.IO.File;

namespace EncryptionApp.Api.Infra.Security;

public class JwtTokenHandler
{
    private readonly RSA _privateKey = RSA.Create();
    private readonly RSA _publicKey = RSA.Create();
    
    private readonly string _issuer = Environment.GetEnvironmentVariable("SERVER_URL") ?? "";
    private readonly string _audience = Environment.GetEnvironmentVariable("CLIENT_URL") ?? "";

    private readonly ILogger<JwtTokenHandler> _logger;

    public JwtTokenHandler (ILogger<JwtTokenHandler> logger)
    {
        var privateKeyPath = Environment.GetEnvironmentVariable("PRIVATE_KEY_PATH");
        var publicKeyPath = Environment.GetEnvironmentVariable("PUBLIC_KEY_PATH");

        if (string.IsNullOrEmpty(privateKeyPath) || string.IsNullOrEmpty(publicKeyPath))
        {
            throw new ArgumentNullException(privateKeyPath != null ? nameof(_publicKey) : nameof(_privateKey));
        }
        
        _privateKey.ImportFromPem(File.ReadAllText(privateKeyPath));
        _publicKey.ImportFromPem(File.ReadAllText(publicKeyPath));
        _logger = logger;
    }

    public string Encode(User user, DateTime expiresAt)
    {
        try
        {
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email ?? ""),
            };

            return Encode(claims, expiresAt);

        }
        catch (Exception e)
        {
            _logger.LogError($"Error while encoding JWT: {e}");
            throw;
        }

    }

    public string Encode(List<Claim> claims, DateTime expiresAt)
    {
        try
        {
            var creds = new SigningCredentials(new RsaSecurityKey(_privateKey), 
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
        catch (Exception e)
        {
            _logger.LogError($"Error while encoding JWT: {e}");
            throw;
        }
    }

    public Result<ClaimsPrincipal> Decode(string token)
    {

        try
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
                IssuerSigningKey = new RsaSecurityKey(_publicKey)
            };
            var claims = new JwtSecurityTokenHandler().ValidateToken(token, validationParams, out _);
            return Result<ClaimsPrincipal>.Success(claims);
        }
        catch (Exception e)
        {
            _logger.LogError($"Error while decoding JWT: {e}");
            return Result<ClaimsPrincipal>.Failure(new UnauthorizedError("Token failed."));
        }
    }
}