using HsaLedger.Server.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace HsaLedger.Server.Infrastructure.Persistence;

public class ApplicationDbContextInitializer
{
    private readonly ILogger<ApplicationDbContextInitializer> _logger;
    private readonly ApplicationDbContext _context;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly UserManager<User> _userManager;

    public ApplicationDbContextInitializer(ILogger<ApplicationDbContextInitializer> logger,
        ApplicationDbContext context, RoleManager<IdentityRole> roleManager, UserManager<User> userManager)
    {
        _logger = logger;
        _context = context;
        _roleManager = roleManager;
        _userManager = userManager;
    }

    public async Task SeedAsync()
    {
        try
        {
            await TrySeedAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while seeding the database");
            throw;
        }
    }

    private async Task TrySeedAsync()
    {
        string[] roles = ["Administrator", "Operations", "Service"];

        foreach (var role in roles)
        {
            if (!await _roleManager.RoleExistsAsync(role))
            {
                await _roleManager.CreateAsync(new IdentityRole(role));
            }
        }

        // default admin user
        var user = await _userManager.FindByNameAsync("admin");
        if (user == null)
        {
            user = new User
            {
                UserName = "admin"
            };
            var result = await _userManager.CreateAsync(user, "Admin123#!");
            if (result.Succeeded)
            {
                user = await _userManager.FindByNameAsync("admin");
                if (user != null)
                {
                    user.IsEnabled = true;
                    await _userManager.UpdateAsync(user);
                    await _userManager.AddToRolesAsync(user, ["Administrator"]);
                }
            }
        }
    }
}