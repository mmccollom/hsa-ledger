using FluentValidation.AspNetCore;
using HsaLedger.Application;
using HsaLedger.Server;
using HsaLedger.Server.Common.Extensions;
using HsaLedger.Server.Identity;
using HsaLedger.Server.Infrastructure;
using HsaLedger.Server.Infrastructure.Identity;
using HsaLedger.Server.Infrastructure.Persistence;
using Microsoft.AspNetCore.Identity;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddAuthorization();
builder.Services.AddAuthentication(IdentityConstants.BearerScheme)
    .AddCookie(IdentityConstants.ApplicationScheme)
    .AddBearerToken(IdentityConstants.BearerScheme);
builder.Services.AddIdentityCore<User>()
    .AddRoles<IdentityRole>() // if using roles
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddSignInManager(); // This ensures SignInManager is added
    //.AddApiEndpoints();

builder.Services.AddScoped<JwtTokenGenerator>();
builder.Services.AddInfrastructureServices(builder.Configuration);
builder.Services.AddApplicationServices();

builder.Services.AddHttpContextAccessor();
builder.Services.AddFluentValidationAutoValidation();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSwaggerGen(c =>
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
        Scheme = IdentityConstants.BearerScheme,
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

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    
    app.ApplyMigrations();
}

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

//app.UseHealthChecks("/health");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");


//app.UseHttpsRedirection();

//app.MapIdentityApi<User>();

app.Run();
