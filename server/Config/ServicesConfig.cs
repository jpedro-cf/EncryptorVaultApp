using System.Security.Cryptography;
using EncryptionApp.Api.Factory;
using EncryptionApp.Api.Infra.Security;
using EncryptionApp.Api.Infra.Storage;
using EncryptionApp.Api.Services;
using EncryptionApp.Api.Workers;

namespace EncryptionApp.Config;

public static class ServicesConfig
{
    private static readonly RSA _privateKey = RSA.Create();
    private static readonly RSA _publicKey = RSA.Create();
    
        
    public static void AddServicesConfig(this WebApplicationBuilder builder)
    {
        _privateKey.ImportFromPem(File.ReadAllText(Environment.GetEnvironmentVariable("PRIVATE_KEY_PATH")!));
        _publicKey.ImportFromPem(File.ReadAllText(Environment.GetEnvironmentVariable("PUBLIC_KEY_PATH")!));
        
        builder.Services.AddSingleton<AmazonS3>();
        builder.Services.AddSingleton<ResponseFactory>();
        
        builder.Services.AddSingleton<BackgroundTaskQueue>();
        builder.Services.AddHostedService<DeletionBackgroundTask>();
        builder.Services.AddHostedService<PeriodicBackgroundTask>();
        
        builder.Services.AddTransient<StorageUsageService>();
        builder.Services.AddTransient<FilesService>();
        builder.Services.AddTransient<UploadsService>();
        builder.Services.AddTransient<UsersService>();
        builder.Services.AddTransient<AuthService>();
        builder.Services.AddTransient<FoldersService>();
        builder.Services.AddTransient<ShareService>();
        builder.Services.AddTransient<ItemsService>();

        builder.Services.AddSingleton(_privateKey);
        builder.Services.AddSingleton(_publicKey);
        builder.Services.AddSingleton<JwtTokenHandler>();
    }
}