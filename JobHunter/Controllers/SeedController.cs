// Controllers/SeedController.cs
using Microsoft.AspNetCore.Mvc;
using JobHunter.Services;

namespace JobHunter.Controllers
{
    public class SeedController : Controller
    {
        private readonly UserSeedService _userSeedService;
        private readonly IWebHostEnvironment _environment;

        public SeedController(UserSeedService userSeedService, IWebHostEnvironment environment)
        {
            _userSeedService = userSeedService;
            _environment = environment;
        }

        // Only allow this in development
        public async Task<IActionResult> CreateAdmin()
        {
            if (!_environment.IsDevelopment())
            {
                return NotFound();
            }

            try
            {
                await _userSeedService.SeedUsersAndRolesAsync();
                return Json(new { success = true, message = "Admin user and roles created successfully" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
    }
}