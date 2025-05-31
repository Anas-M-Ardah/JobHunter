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
        public async Task<IActionResult> Create(PortfolioCreateEditDTO portfolioCreateEditDTO)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(User);
                _portfolioRepository.CreatePortfolio(portfolioCreateEditDTO, user);
                return RedirectToAction("Index");
            }
            else
            {
                var errors = ModelState
           .Where(x => x.Value.Errors.Count > 0)
           .Select(x => new {
               Field = x.Key,
               Errors = x.Value.Errors.Select(e => e.ErrorMessage)
           });

                // Set breakpoint here or log the errors
                foreach (var error in errors)
                {
                    Console.WriteLine($"Field: {error.Field}");
                    foreach (var msg in error.Errors)
                    {
                        Console.WriteLine($"  Error: {msg}");
                    }
                }
                return View(portfolioCreateEditDTO);
            }
        }
    }
}
