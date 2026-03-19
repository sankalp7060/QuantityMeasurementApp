using System.Reflection;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.OpenApi.Models;

namespace QuantityMeasurementWebAPI.Extensions
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddWebApiServices(
            this IServiceCollection services,
            IConfiguration configuration
        )
        {
            // ==================== SWAGGER CONFIGURATION WITH AUTH ====================
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc(
                    "v1",
                    new OpenApiInfo
                    {
                        Title = "Quantity Measurement API",
                        Version = "v1",
                        Description =
                            "REST API for quantity measurement operations across Length, Weight, Volume, and Temperature units",
                        Contact = new OpenApiContact
                        {
                            Name = "Quantity Measurement Team",
                            Email = "api@quantitymeasurement.com",
                            Url = new Uri("https://github.com/yourusername/QuantityMeasurementApp"),
                        },
                        License = new OpenApiLicense
                        {
                            Name = "MIT License",
                            Url = new Uri("https://opensource.org/licenses/MIT"),
                        },
                    }
                );

                // Add JWT Authentication to Swagger
                c.AddSecurityDefinition(
                    "Bearer",
                    new OpenApiSecurityScheme
                    {
                        Name = "Authorization",
                        Type = SecuritySchemeType.Http,
                        Scheme = "bearer",
                        BearerFormat = "JWT",
                        In = ParameterLocation.Header,
                        Description =
                            "Enter 'Bearer' [space] and then your valid JWT token.\n\nExample: \"Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9\"",
                    }
                );

                c.AddSecurityRequirement(
                    new OpenApiSecurityRequirement
                    {
                        {
                            new OpenApiSecurityScheme
                            {
                                Reference = new OpenApiReference
                                {
                                    Type = ReferenceType.SecurityScheme,
                                    Id = "Bearer",
                                },
                            },
                            new string[] { }
                        },
                    }
                );

                // Include XML comments
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                if (File.Exists(xmlPath))
                {
                    c.IncludeXmlComments(xmlPath);
                }

                c.EnableAnnotations();
            });

            // ... rest of your existing code (CORS, Health Checks, etc.)
            services.AddCors(options =>
            {
                options.AddPolicy(
                    "AllowAll",
                    builder =>
                    {
                        builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
                    }
                );
            });

            var connectionString = configuration.GetConnectionString("DefaultConnection");
            if (!string.IsNullOrEmpty(connectionString))
            {
                services
                    .AddHealthChecks()
                    .AddSqlServer(
                        connectionString,
                        name: "SQL Server",
                        tags: new[] { "database", "sql" }
                    );
            }
            else
            {
                services
                    .AddHealthChecks()
                    .AddCheck(
                        "self",
                        () =>
                            Microsoft.Extensions.Diagnostics.HealthChecks.HealthCheckResult.Healthy(
                                "OK"
                            )
                    );
            }

            services.AddResponseCaching();
            services.AddHttpContextAccessor();

            return services;
        }
    }
}
