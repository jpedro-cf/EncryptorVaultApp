namespace EncryptionApp.Config;

public static class SecurityConfig
{
    public static void AddSecurityConfig(this WebApplicationBuilder builder)
    {
        builder.Services.AddCors(options =>
        {
            options.AddPolicy("AllowSpecificOrigin", policy =>
            {
                var origin = Environment.GetEnvironmentVariable("CLIENT_URL");
                policy
                    .WithOrigins(origin != null ? [origin] : ["http://localhost:5173"])
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials();
            });
        });
    }
}