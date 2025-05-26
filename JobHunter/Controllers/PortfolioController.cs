using JobHunter.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using JobHunter.Models;
using JobHunter.DTOs;
namespace JobHunter.Controllers
{
    public class PortfolioController : BaseController
    {
        private readonly IPortfolioRepository _portfolioRepository;
        private readonly UserManager<User> _userManager;

        public PortfolioController(IPortfolioRepository portfolioRepository, UserManager<User> userManager)
        {
            _portfolioRepository = portfolioRepository;
            _userManager = userManager;
        }


        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(PortfolioCreateEditDTO portfolioCreateEditDTO)
        {
            if (ModelState.IsValid)
            {
                return RedirectToAction("Index");
            }
            else
            {
                return View(portfolioCreateEditDTO);
            }
        }
    }
}
