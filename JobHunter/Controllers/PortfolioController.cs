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


        public async Task<IActionResult> Index()
        {
            var portfolios = await _portfolioRepository.GetAllPortfoliosByUserId(await _userManager.GetUserAsync(User));
            return View(portfolios);
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
                await _portfolioRepository.CreatePortfolioAsync(portfolioCreateEditDTO, user);
                return RedirectToAction("Index");
            }
            else
            {
                var errors = ModelState
                   .Where(x => x.Value.Errors.Count > 0)
                   .Select(x => new
                   {
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

        public async Task<IActionResult> Edit(Guid portfolioId)
        {
            var user = await _userManager.GetUserAsync(User);
            var portfolio = await _portfolioRepository.GetPortfolioById(portfolioId);
            if (portfolio == null)
            {
                return NotFound();
            }
            return View(portfolio);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(PortfolioCreateEditDTO portfolioCreateEditDTO)
        {
            if (ModelState.IsValid)
            {
                await _portfolioRepository.UpdatePortfolioAsync(portfolioCreateEditDTO, await _userManager.GetUserAsync(User));
                return RedirectToAction("Index");
            }
            return View(portfolioCreateEditDTO);
        }
    }
}
