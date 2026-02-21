using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Red_Code.Application.Interfaces;
using Red_Code.Application.Services;
using Red_Code.Domain.Entities;
using Red_Code.Infrastructure.Data;
using Red_Code.Infrastructure.Repositories;

namespace Red_Code
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            // Load environment-specific variables from .env.* before building configuration
            var environmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            var contentRoot = System.IO.Directory.GetCurrentDirectory();

            if (string.Equals(environmentName, "Development", StringComparison.OrdinalIgnoreCase))
            {
                var envFilePath = System.IO.Path.Combine(contentRoot, ".env.development");
                LoadEnvFile(envFilePath);
            }
            else if (string.Equals(environmentName, "Production", StringComparison.OrdinalIgnoreCase))
            {
                var envFilePath = System.IO.Path.Combine(contentRoot, ".env.production");
                LoadEnvFile(envFilePath);
            }

            var builder = WebApplication.CreateBuilder(args);

            // When running on platforms like Railway, the port is provided via the PORT environment variable
            var port = Environment.GetEnvironmentVariable("PORT");
            if (!string.IsNullOrWhiteSpace(port))
            {
                builder.WebHost.UseUrls($"http://0.0.0.0:{port}");
            }

            // Build connection string and
            // falling back to individual DB_* environment variables
            var dbHost = builder.Configuration["DB_HOST"];
            var dbPort = builder.Configuration["DB_PORT"] ?? "5432";
            var dbName = builder.Configuration["DB_NAME"];
            var dbUser = builder.Configuration["DB_USER"];
            var dbPassword = builder.Configuration["DB_PASSWORD"];

            if (new[] { dbHost, dbName, dbUser, dbPassword }.Any(string.IsNullOrWhiteSpace))
            {
                throw new Exception("Database environment variables are missing");
            }

            var connectionString =
                $"Host={dbHost};" +
                $"Port={dbPort};" +
                $"Database={dbName};" +
                $"Username={dbUser};" +
                $"Password={dbPassword};" +
                $"SSL Mode=Require;Trust Server Certificate=true;" +
                $"Prefer IPv6=false;";

            // Override the connection string from appsettings.json
            builder.Configuration["ConnectionStrings:DefaultConnection"] = connectionString;

            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowFrontend", policy =>
                {
                    policy
                        .WithOrigins("http://localhost:4200")
                        .AllowAnyHeader()
                        .AllowAnyMethod();
                });
            });

            // Configure Entity Framework and PostgreSQL
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

            // Add ASP.NET Core Identity
            builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
            {
                options.SignIn.RequireConfirmedAccount = false;
                
                //Consciously closed to make the register easier

                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequiredLength = 3;
            })
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();

            // Add JWT Authentication
            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = builder.Configuration["Jwt:Issuer"],
                    ValidAudience = builder.Configuration["Jwt:Audience"],
                    IssuerSigningKey = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(
                    System.Text.Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"] ?? ""))
                };
            });

            builder.Services.AddControllers();
            builder.Services.AddOpenApi();

            // Register application services
            builder.Services.AddScoped<IBookService, BookService>();
            builder.Services.AddScoped<IBookRepository, BookRepository>();

            builder.Services.AddScoped<IQuotationService, QuotationService>();
            builder.Services.AddScoped<IQuotationRepository, QuotationRepository>();

            var app = builder.Build();

            // Seed the database
            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                var logger = services.GetRequiredService<ILogger<Program>>();
                try
                {
                    logger.LogInformation("Starting database migration and seeding...");
                    var context = services.GetRequiredService<ApplicationDbContext>();
                    await DbInitializer.SeedAsync(context);
                    logger.LogInformation("Database migration and seeding completed successfully.");
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "An error occurred while seeding the database. Details: {Message}", ex.Message);
                    logger.LogError("Inner Exception: {InnerException}", ex.InnerException?.Message ?? "None");
                    // Re-throw to make the error visible
                    throw;
                }
            }

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
            }
            app.UseCors("AllowFrontend");

            app.UseHttpsRedirection();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            await app.RunAsync();
        }

        private static string BuildConnectionStringFromDatabaseUrl(string databaseUrl)
        {
            // Expected format: postgres://user:password@host:port/database
            if (string.IsNullOrWhiteSpace(databaseUrl))
            {
                throw new ArgumentException("DATABASE_URL configuration is empty.", nameof(databaseUrl));
            }

            var uri = new Uri(databaseUrl);

            var userInfo = uri.UserInfo.Split(':', 2);
            var user = Uri.UnescapeDataString(userInfo[0]);
            var password = userInfo.Length > 1 ? Uri.UnescapeDataString(userInfo[1]) : string.Empty;

            var host = uri.Host;
            var port = uri.Port > 0 ? uri.Port : 5432;
            var database = uri.AbsolutePath.TrimStart('/');

            // Basic Npgsql connection string; additional options (like SSL) can be appended via env if needed
            return $"Host={host};Port={port};Database={database};Username={user};Password={password}";
        }

        private static void LoadEnvFile(string path)
        {
            if (!System.IO.File.Exists(path))
            {
                return;
            }

            foreach (var line in System.IO.File.ReadAllLines(path))
            {
                var trimmed = line.Trim();

                // Skip empty lines and comments
                if (string.IsNullOrEmpty(trimmed) || trimmed.StartsWith("#", StringComparison.Ordinal))
                {
                    continue;
                }

                var separatorIndex = trimmed.IndexOf('=', StringComparison.Ordinal);
                if (separatorIndex <= 0)
                {
                    continue;
                }

                var key = trimmed.Substring(0, separatorIndex).Trim();
                var value = trimmed.Substring(separatorIndex + 1).Trim();

                if (key.Length == 0)
                {
                    continue;
                }

                // Do not allow env files to change the ASP.NET Core environment
                if (string.Equals(key, "ASPNETCORE_ENVIRONMENT", StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                // Values from env files override any existing process-level values for that key
                Environment.SetEnvironmentVariable(key, value);
            }
        }
    }
}
