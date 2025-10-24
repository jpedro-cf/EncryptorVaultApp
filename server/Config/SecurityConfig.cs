namespace EncryptionApp.Config;

public static class SecurityConfig
{
    public static void AddSecurityConfig(this WebApplicationBuilder builder, IConfiguration configuration)
    {
        builder.Services.AddCors(options =>
        {
            options.AddPolicy("AllowSpecificOrigin", policy =>
            {
                var origins = configuration.GetSection("Cors:AllowedOrigins").Get<string[]>();
                policy
                    .WithOrigins(origins ?? ["http://localhost:5173"])
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials();
            });
        });
    }
}