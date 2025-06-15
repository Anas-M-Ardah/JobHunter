// Services/UserSeedService.cs
using Microsoft.AspNetCore.Identity;
using JobHunter.Models;

namespace JobHunter.Services
{
    public class UserSeedService
    {
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ILogger<UserSeedService> _logger;

        public UserSeedService(
            UserManager<User> userManager,
            RoleManager<IdentityRole> roleManager,
            ILogger<UserSeedService> logger)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _logger = logger;
        }

        public async Task SeedUsersAndRolesAsync()
        {
            try
            {
                // Create roles if they don't exist
                await CreateRoleIfNotExists("Admin");
                await CreateRoleIfNotExists("EndUser");

                // Create admin user if doesn't exist
                await CreateAdminUserIfNotExists();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while seeding users and roles");
                throw;
            }
        }

        private async Task CreateRoleIfNotExists(string roleName)
        {
            if (!await _roleManager.RoleExistsAsync(roleName))
            {
                await _roleManager.CreateAsync(new IdentityRole(roleName));
                _logger.LogInformation($"Role '{roleName}' created successfully");
            }
        }

        private async Task CreateAdminUserIfNotExists()
        {
            const string adminEmail = "admin@jobhunter.com";
            const string adminPassword = "Admin@123";

            var adminUser = await _userManager.FindByEmailAsync(adminEmail);

            if (adminUser == null)
            {
                adminUser = new Admin
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    FirstName = "System",
                    LastName = "Administrator",
                    EmailConfirmed = true,
                    CreatedDate = DateTime.UtcNow
                };

                var result = await _userManager.CreateAsync(adminUser, adminPassword);

                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(adminUser, "Admin");
                    _logger.LogInformation($"Admin user created successfully with email: {adminEmail}");
                }
                else
                {
                    var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                    _logger.LogError($"Failed to create admin user: {errors}");
                    throw new Exception($"Failed to create admin user: {errors}");
                }
            }
            else
            {
                // Ensure admin user has admin role
                if (!await _userManager.IsInRoleAsync(adminUser, "Admin"))
                {
                    await _userManager.AddToRoleAsync(adminUser, "Admin");
                    _logger.LogInformation("Admin role added to existing admin user");
                }
            }
        }
    }
}