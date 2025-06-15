using JobHunter.Models.DTOs;
using JobHunter.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JobHunter.Controllers
{
    public class AdminController : Controller
    {
        private readonly IAdminRepository _adminRepository;
        public AdminController(IAdminRepository adminRepository)
        {
            _adminRepository = adminRepository;
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Index()
        {
            var allUsers = await _adminRepository.GetAllUsersAsync();
            var totalResumes = await _adminRepository.GetTotalResumesAsync();
            var totalPortfolios = await _adminRepository.GetTotalPortfoliosAsync();

            var dto = new AdminIndexDTO
            {
                AllUsers = allUsers,
                TotalUsers = allUsers.Count,
                TotalResumes = totalResumes,
                TotalPortfolios = totalPortfolios
            };

            return View(dto);
        }
    }
}
