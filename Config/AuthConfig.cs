using System.Security.Cryptography;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

namespace MyMVCProject.Config;

public static class AuthConfig
{
    public static void AddAuthConfig(this WebApplicationBuilder builder, IConfiguration configuration, RSA publicKey)
    {
        builder.Services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = configuration["Jwt:Issuer"],
                ValidateAudience = true,
                ValidAudience = configuration["Jwt:Audience"],
                ValidateLifetime = true,
                RequireExpirationTime = true,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new RsaSecurityKey(publicKey)
            };

            options.Events = new JwtBearerEvents
            {
                OnMessageReceived = context =>
                {
                    var cookieJwt = context.Request.Cookies["accessToken"];
                    if (!string.IsNullOrWhiteSpace(cookieJwt))
                    {
                        context.Token = cookieJwt;
                    }

                    return Task.CompletedTask;
                }
            };
        });
    }
}