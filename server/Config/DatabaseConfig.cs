using Microsoft.EntityFrameworkCore;

namespace EncryptionApp.Config;

public static class DatabaseConfig
{
    public static void AddDbContext(this WebApplicationBuilder builder)
    {
        var connectionString = Environment.GetEnvironmentVariable("DATABASE_CONNECTION");
        
        if (string.IsNullOrEmpty(connectionString))
        {
            throw new InvalidOperationException(
                "The DATABASE_CONNECTION environment variable is not set."
            );
        }

        try
        {
            builder.Services.AddDbContext<AppDbContext>(options => 
                options.UseLazyLoadingProxies()
                    .UseNpgsql(connectionString));
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}