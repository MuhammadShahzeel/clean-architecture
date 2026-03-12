using Clean.Application.Settings;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

namespace Clean.WebApi.Extensions
{
    public static class ServiceExtensions
    {

        //swagger configuration with jwt authentication to show the authorize button in swagger ui 
        public static void AddSwaggerWithJwt(this IServiceCollection services)
        {
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Clean Architecture",
                    Version = "v1"
                });

                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "Enter your JWT token below. Do not include 'Bearer' prefix.",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    BearerFormat = "JWT"
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
                        },
                        Array.Empty<string>()
                    }
                });
            });
        }

        // JWT authentication configuration with custom error responses for authentication failures, challenges, and forbidden access.
        public static void AddJwtAuthentication(this IServiceCollection services, IConfiguration configuration)
        {
            var jwtSection = configuration.GetSection("JwtSettings");
            services.Configure<JwtSettings>(jwtSection); // to make JwtSettings available for injection via IOptions<JwtSettings>

            var jwtSettings = jwtSection.Get<JwtSettings>() ?? new JwtSettings(); 
            var keyBytes = Encoding.UTF8.GetBytes(jwtSettings.Key ?? string.Empty);

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = false;
                options.SaveToken = false;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(keyBytes),
                    ValidateIssuer = true,
                    ValidIssuer = jwtSettings.Issuer,
                    ValidateAudience = true,
                    ValidAudience = jwtSettings.Audience,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.FromSeconds(30)
                };

                options.Events = new JwtBearerEvents
                {
                    OnAuthenticationFailed = async context =>
                    {
                        context.NoResult();
                        var response = context.Response;
                        response.StatusCode = StatusCodes.Status401Unauthorized;
                        response.ContentType = "application/problem+json";

                        string detail = "Authentication failed.";
                        string error = "invalid_token";

                        if (context.Exception is SecurityTokenExpiredException)
                        {
                            detail = "The access token has expired.";
                        }

                        response.Headers["WWW-Authenticate"] = $"Bearer error=\"{error}\", error_description=\"{detail}\"";

                        await response.WriteAsJsonAsync(new Microsoft.AspNetCore.Mvc.ProblemDetails
                        {
                            Type = "https://tools.ietf.org/html/rfc6750",
                            Title = "Authentication Failed",
                            Status = StatusCodes.Status401Unauthorized,
                            Detail = detail
                        });
                    },

                    OnChallenge = async context =>
                    {
                        context.HandleResponse();
                        var response = context.Response;
                        response.StatusCode = StatusCodes.Status401Unauthorized;
                        response.ContentType = "application/problem+json";

                        var description = context.ErrorDescription ?? "Authentication is required to access this resource.";
                        response.Headers["WWW-Authenticate"] = $"Bearer error=\"invalid_token\", error_description=\"{description}\"";

                        await response.WriteAsJsonAsync(new Microsoft.AspNetCore.Mvc.ProblemDetails
                        {
                            Type = "https://tools.ietf.org/html/rfc6750",
                            Title = "Unauthorized",
                            Status = StatusCodes.Status401Unauthorized,
                            Detail = description
                        });
                    },

                    OnForbidden = async context =>
                    {
                        var response = context.Response;
                        response.StatusCode = StatusCodes.Status403Forbidden;
                        response.ContentType = "application/problem+json";

                        await response.WriteAsJsonAsync(new Microsoft.AspNetCore.Mvc.ProblemDetails
                        {
                            Title = "Forbidden",
                            Status = StatusCodes.Status403Forbidden,
                            Detail = "You do not have permission to access this resource."
                        });
                    }
                }; 
            });    
        }         
    }             
}                  