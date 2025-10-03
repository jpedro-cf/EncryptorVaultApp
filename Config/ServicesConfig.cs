using System.Security.Cryptography;
using Amazon.S3;
using MyMVCProject.Api.Infra.Security;
using MyMVCProject.Api.Infra.Storage;
using MyMVCProject.Api.Services;

namespace MyMVCProject.Config;

public static class ServicesConfig
{
    public static void AddServicesConfig(this WebApplicationBuilder builder, RSA privateKey, RSA publicKey)
    {
        builder.Services.AddSingleton<AmazonS3>();
        
        builder.Services.AddTransient<UsersService>();
        builder.Services.AddTransient<AuthService>();
        builder.Services.AddTransient<FoldersService>();
        builder.Services.AddTransient<FilesService>();

        builder.Services.AddSingleton(privateKey);
        builder.Services.AddSingleton(publicKey);
        builder.Services.AddSingleton<JwtTokenHandler>();
    }
}