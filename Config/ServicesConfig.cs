using System.Security.Cryptography;
using EncryptionApp.Api.Infra.Security;
using EncryptionApp.Api.Infra.Storage;
using EncryptionApp.Api.Services;

namespace EncryptionApp.Config;

public static class ServicesConfig
{
    public static void AddServicesConfig(this WebApplicationBuilder builder, RSA privateKey, RSA publicKey)
    {
        builder.Services.AddSingleton<AmazonS3>();
        
        builder.Services.AddTransient<StorageUsageService>();
        builder.Services.AddTransient<FilesService>();
        builder.Services.AddTransient<UsersService>();
        builder.Services.AddTransient<AuthService>();
        builder.Services.AddTransient<FoldersService>();

        builder.Services.AddSingleton(privateKey);
        builder.Services.AddSingleton(publicKey);
        builder.Services.AddSingleton<JwtTokenHandler>();
    }
}