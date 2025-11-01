using System.Security.Cryptography;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

namespace EncryptionApp.Config;

public static class AuthConfig
{
    public static void AddAuthConfig(this WebApplicationBuilder builder, RSA publicKey)
    {
        builder.Services.ConfigureApplicationCookie(options =>
        {
            options.LoginPath = "/Auth"; // redirect here if unauthorized
        });
        
        builder.Services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = Environment.GetEnvironmentVariable("SERVER_URL"),
                ValidateAudience = true,
                ValidAudience = Environment.GetEnvironmentVariable("CLIENT_URL"),
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