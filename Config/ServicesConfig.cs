using System.Security.Cryptography;
using MyMVCProject.Api.Infra.Security;
using MyMVCProject.Api.Services;

namespace MyMVCProject.Config;

public static class ServicesConfig
{
    public static void AddServicesConfig(this WebApplicationBuilder builder, RSA privateKey, RSA publicKey)
    {
        builder.Services.AddTransient<UsersService>();
        builder.Services.AddTransient<AuthService>();
        
        builder.Services.AddSingleton(privateKey);
        builder.Services.AddSingleton(publicKey);
        builder.Services.AddSingleton<JwtTokenHandler>();
    }
}