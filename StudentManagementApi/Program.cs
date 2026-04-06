using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using StudentManagementApi.Data;
using StudentManagementApi.Middleware;
using StudentManagementApi.Repository;
using StudentManagementApi.Service;
using System.Text;

namespace StudentManagementApi
{
    // Main entry point for the application
    public class Program
    {
        // Application configuration
        public static void Main(string[] args)
        {
            // Configure Serilog for logging
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Information()
                .WriteTo.Console()
                .WriteTo.File("logs/studentmanagement-.log", 
                    rollingInterval: RollingInterval.Day,
                    retainedFileCountLimit: 7)
                .CreateLogger();

            try
            {
                Log.Information("Starting Student Management API");

                // Create and configure web application
                var builder = WebApplication.CreateBuilder(args);

                // Add Serilog to the application
                builder.Host.UseSerilog();

                // =====================================================
                // Add services to the container
                // =====================================================

                // Add controller services
                builder.Services.AddControllers();

                // Add CORS policy to allow all origins
                builder.Services.AddCors(options =>
                {
                    options.AddPolicy("AllowAll", policy =>
                    {
                        policy.AllowAnyOrigin()
                              .AllowAnyMethod()
                              .AllowAnyHeader();
                    });
                });

                // Add endpoint explorer for routing
                builder.Services.AddEndpointsApiExplorer();

                // Add Swagger with JWT authorization support
                builder.Services.AddSwaggerGen(c =>
                {
                    c.SwaggerDoc("v1", new OpenApiInfo
                    {
                        Title = "Student Management API",
                        Version = "v1",
                        Description = "API for managing students with JWT authentication"
                    });

                    // Configure Swagger to use JWT authorization
                    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                    {
                        Description = "JWT Authorization header using the Bearer scheme. Enter 'Bearer' [space] and then your token.",
                        Name = "Authorization",
                        In = ParameterLocation.Header,
                        Type = SecuritySchemeType.ApiKey,
                        Scheme = "Bearer"
                    });

                    c.AddSecurityRequirement(new OpenApiSecurityRequirement
                    {
                        {
                            new OpenApiSecurityScheme
                            {
                                Reference = new OpenApiReference
                                {
                                    Type = ReferenceType.SecurityScheme,
                                    Id = "Bearer"
                                }
                            },
                            Array.Empty<string>()
                        }
                    });
                });

                // =====================================================
                // Database Configuration
                // =====================================================

                // Get connection string from configuration
                var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
                
                // Add Entity Framework Core with SQL Server
                builder.Services.AddDbContext<ApplicationDbContext>(options =>
                    options.UseSqlServer(connectionString));

                // =====================================================
                // JWT Authentication Configuration
                // =====================================================

                // Get JWT settings from configuration
                var jwtKey = builder.Configuration["Jwt:Key"] ?? "YourSuperSecretKeyHere12345678901234567890";
                var jwtIssuer = builder.Configuration["Jwt:Issuer"] ?? "StudentManagementApi";
                var jwtAudience = builder.Configuration["Jwt:Audience"] ?? "StudentManagementApi";

                // Configure JWT authentication
                builder.Services.AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = jwtIssuer,
                        ValidAudience = jwtAudience,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
                    };
                });

                // =====================================================
                // Dependency Injection - Register services
                // =====================================================

                // Register repository
                builder.Services.AddScoped<IStudentRepository, StudentRepository>();

                // Register service
                builder.Services.AddScoped<IStudentService, StudentService>();

                // =====================================================
                // Build the application
                // =====================================================

                var app = builder.Build();

                // =====================================================
                // Configure middleware pipeline
                // =====================================================

                // Configure development environment
                if (app.Environment.IsDevelopment())
                {
                    // Enable Swagger in development
                    app.UseSwagger();
                    app.UseSwaggerUI(c =>
                    {
                        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Student Management API v1");
                    });
                }

                // Use global exception handler
                app.UseGlobalExceptionHandler();

                // Use CORS policy
                app.UseCors("AllowAll");

                // Serve static files from wwwroot
                app.UseStaticFiles();

                // Use routing
                app.UseRouting();

                // Use authentication middleware
                app.UseAuthentication();

                // Use authorization middleware
                app.UseAuthorization();

                // Map controller routes
                app.MapControllers();

                // Fallback to index.html for SPA (serve static files for any unmatched route)
                app.MapFallbackToFile("index.html");

                // =====================================================
                // Initialize and migrate database
                // =====================================================

                using (var scope = app.Services.CreateScope())
                {
                    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                    
                    // Ensure database is created
                    dbContext.Database.EnsureCreated();
                    Log.Information("Database initialized successfully");
                }

                // =====================================================
                // Run the application
                // =====================================================

                Log.Information("Application starting...");
                app.Run();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Application terminated unexpectedly");
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }
    }
}
