using JobHunter.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using JobHunter.Models;
using JobHunter.DTOs;
using JobHunter.Services;
namespace JobHunter.Controllers
{
    public class PortfolioController : BaseController
    {
        private readonly IPortfolioRepository _portfolioRepository;
        private readonly UserManager<User> _userManager;
        private readonly IGeminiService _geminiService;

        public PortfolioController(IPortfolioRepository portfolioRepository, UserManager<User> userManager, IGeminiService geminiService)
        {
            _portfolioRepository = portfolioRepository;
            _userManager = userManager;
            _geminiService = geminiService;
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

        public async Task<IActionResult> Edit(Guid id)
        {
            var user = await _userManager.GetUserAsync(User);
            var portfolio = await _portfolioRepository.GetPortfolioByIdForEdit(id);
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

            // Method 1: Print to Debug Console
            foreach (var modelError in ModelState)
            {
                var key = modelError.Key;
                var errors = modelError.Value.Errors;
                if (errors.Count > 0)
                {
                    System.Diagnostics.Debug.WriteLine($"Property: {key}");
                    Console.WriteLine($"Property: {key}");
                    foreach (var error in errors)
                    {
                        System.Diagnostics.Debug.WriteLine($"  Error: {error.ErrorMessage}");
                        Console.WriteLine($"  Error: {error.ErrorMessage}");
                    }
                }
            }

            return View(portfolioCreateEditDTO);
        }

        [HttpPost]
        public async Task<IActionResult> ImproveBio(string bio)
        {
            string improvedBio = await _geminiService.ImproveBio(bio);
            return Json(new { improvedBio });
        }

        [HttpPost]
        public async Task<IActionResult> ImproveServiceDescription(string serviceDescription)
        {
            string improvedServiceDescription = await _geminiService.ImproveServiceDescription(serviceDescription);
            return Json(new { improvedServiceDescription });
        }

        [HttpPost]
        public async Task<IActionResult> ImproveProjectDescription(string projectDescription)
        {
            string improvedProjectDescription = await _geminiService.ImproveProjectDescription(projectDescription);
            return Json(new { improvedProjectDescription });
        }


        public async Task<IActionResult> View(Guid id)
        {
            var portfolio = await _portfolioRepository.GetPortfolioById(id);
            if (portfolio == null)
            {
                return NotFound();
            }
            return View(portfolio);
        }


        [HttpPost]
        public async Task<IActionResult> Delete(Guid id)
        {
         
            if (await _portfolioRepository.DeletePortfolioAsync(id))
            {
                TempData["SuccessMessage"] = "Portfolio deleted successfully.";
                return RedirectToAction("Index");
            }
            else
            {
                TempData["ErrorMessage"] = "Failed to delete portfolio.";
                return RedirectToAction("Index");
            }
        }

        public async Task<IActionResult> ViewFile(Guid projectId)
        {
            try
            {
                var fileResult = await _portfolioRepository.GetFileFromDatabaseAsync(projectId);

                if (fileResult == null)
                    return NotFound("File not found");

                return fileResult;
            }
            catch (ApplicationException ex)
            {
                // Log the exception
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                // Log the exception
                return StatusCode(500, "An error occurred while retrieving the file");
            }
        }

    }
}
