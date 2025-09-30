using System.Security.Cryptography;
using Microsoft.EntityFrameworkCore;
using MyMVCProject.Api.Global.Exceptions;
using MyMVCProject.Config;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<AppDbContext>(options => options.UseNpgsql(connectionString));

builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var privateKey = RSA.Create();
var publicKey = RSA.Create();
privateKey.ImportFromPem(File.ReadAllText(builder.Configuration["Keys:PrivateKeyPath"]!));
publicKey.ImportFromPem(File.ReadAllText(builder.Configuration["Keys:PublicKeyPath"]!));

builder.AddIdentityConfig();
builder.AddAuthConfig(builder.Configuration, publicKey);
builder.Services.AddAuthorization();

builder.AddServicesConfig(publicKey, privateKey);

builder.Services.AddControllersWithViews()
    .AddRazorOptions(options =>
    {
        options.ViewLocationFormats.Clear();
        options.ViewLocationFormats.Add("/Web/Views/{1}/{0}.cshtml");
        options.ViewLocationFormats.Add("/Web/Views/Shared/{0}.cshtml");
    });
builder.Services.AddControllers();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
} else
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.MapSwagger();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseExceptionHandler();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
