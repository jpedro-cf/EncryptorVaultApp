using Microsoft.AspNetCore.Identity;
using MyMVCProject.Api.Entities;

namespace MyMVCProject.Config;

public static class IdentityConfig
{
    public static void AddIdentityConfig(this WebApplicationBuilder builder)
    {
        builder.Services.AddIdentity<User, IdentityRole<Guid>>()
            .AddEntityFrameworkStores<AppDbContext>()
            .AddSignInManager()
            .AddDefaultTokenProviders();
        
        builder.Services.Configure<IdentityOptions>(options =>
        {
            options.User.RequireUniqueEmail = true;
            options.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789_@.+";
        });

        builder.Services.Configure<IdentityOptions>(options =>
        {
            options.Password.RequiredLength = 8;            // The minimum length.
            options.Password.RequireDigit = true;           // Requires a number between 0-9.
            options.Password.RequireLowercase = true;       // Requires a lowercase character.
            options.Password.RequireUppercase = true;       // Requires an uppercase character.
            options.Password.RequiredUniqueChars = 1;       // Requires the minimum number of distinct characters.
            options.Password.RequireNonAlphanumeric = true; // Requires a non-alphanumeric character (@, %, #, !, &, $, ...).
        });
    }   
}