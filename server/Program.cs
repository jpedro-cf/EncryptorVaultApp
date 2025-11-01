using System.Security.Cryptography;
using Microsoft.EntityFrameworkCore;
using EncryptionApp.Api.Global.Exceptions;
using EncryptionApp.Config;

var builder = WebApplication.CreateBuilder(args);

var connectionString = Environment.GetEnvironmentVariable("DATABASE_CONNECTION");
builder.Services.AddDbContext<AppDbContext>(options => 
    options.UseLazyLoadingProxies()
        .UseNpgsql(connectionString));

builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var privateKey = RSA.Create();
var publicKey = RSA.Create();
privateKey.ImportFromPem(File.ReadAllText(Environment.GetEnvironmentVariable("PRIVATE_KEY_PATH")!));
publicKey.ImportFromPem(File.ReadAllText(Environment.GetEnvironmentVariable("PUBLIC_KEY_PATH")!));

builder.AddSecurityConfig();
builder.AddIdentityConfig();
builder.AddAuthConfig(publicKey);
builder.Services.AddAuthorization();

builder.AddServicesConfig(publicKey, privateKey);
builder.Services.AddControllers();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseHsts();
} else
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseExceptionHandler();
app.UseRouting();   
app.UseCors("AllowSpecificOrigin");
app.UseAuthorization();
app.MapControllers();

app.Run();
