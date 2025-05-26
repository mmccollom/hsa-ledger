using System.Text;
using FluentValidation.AspNetCore;
using HsaLedger.Server.Identity;
using HsaLedger.Server.Identity.Model;
using HsaLedger.Server.Infrastructure.Identity;
using HsaLedger.Server.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

namespace HsaLedger.Server;

public static class ConfigureServices
{
    public static IServiceCollection AddClientServices(this IServiceCollection services,
        IConfiguration configuration)
    {
        var jwtConfigurationModel = configuration.GetSection("Jwt").Get<JwtConfigurationModel>() ?? throw new Exception("Jwt Configuration not found");
        services.AddSingleton(jwtConfigurationModel);
        services.AddHealthChecks();
        services.AddAuthorization();
        services.AddAuthentication(options =>
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
                    ValidIssuer = jwtConfigurationModel.Issuer,
                    ValidAudience = jwtConfigurationModel.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtConfigurationModel.Key)),
                    ClockSkew = TimeSpan.Zero // Ensure no clock skew
                };
            });

        services.AddIdentityCore<User>()
            .AddRoles<IdentityRole>() // if using roles
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders()
            .AddSignInManager();

        services.AddScoped<IdentityQueries>();
        services.AddScoped<IdentityCommands>();
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();
        services.AddHttpContextAccessor();
        services.AddFluentValidationAutoValidation();
        services.AddControllers();
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();

        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "HSA Ledger API",
                Version = "v1",
                Contact = new OpenApiContact { Email = "_", Name = "Developer" }
            });

            c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Type = SecuritySchemeType.Http,
                Scheme = "Bearer",
                BearerFormat = "JWT",
                Description = "Type into the textbox: Bearer {your JWT token}."
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
                    []
                }
            });

            c.DocumentFilter<RoleDocumentFilter>();
        });

        return services;
    }
}