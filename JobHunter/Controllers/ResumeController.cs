using JobHunter.DTOs;
using JobHunter.Models;
using JobHunter.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace JobHunter.Controllers
{
    public class ResumeController : BaseController
    {
        private readonly IResumeRepository _resumeRepository;
        private readonly UserManager<User> _userManager;


        public ResumeController(IResumeRepository resumeRepository, UserManager<User> userManager)
        {
            _resumeRepository = resumeRepository;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User); ;
            return View(_resumeRepository.GetAllResumesByUserId(user.Id));
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(ResumeCreateEditDTO resumeCreateDTO)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByIdAsync(resumeCreateDTO.EndUserId.ToString());
                bool isCreated = _resumeRepository.CreateResume(resumeCreateDTO, user);
                if (isCreated)
                {
                    TempData["SuccessMessage"] = "Resume created successfully.";
                    Console.WriteLine("Resume created successfully.");
                }
                else
                {
                    TempData["ErrorMessage"] = "Failed to create resume. Please try again.";
                    Console.WriteLine("Failed to create resume. Please try again.");
                }
                return RedirectToAction("Index");
            }
            return View(resumeCreateDTO);
        }

        public IActionResult Edit(Guid resumeId)
        {
            return View(_resumeRepository.GetResumeById(resumeId));
        }

        [HttpPost]
        public async Task<IActionResult> Edit(ResumeCreateEditDTO resumeCreateDTO)
        {
            if (ModelState.IsValid)
            {
                bool isUpdated = _resumeRepository.UpdateResume(resumeCreateDTO);
                if (isUpdated)
                {
                    TempData["SuccessMessage"] = "Resume updated successfully.";
                    Console.WriteLine("Resume updated successfully.");
                }
                else
                {
                    TempData["ErrorMessage"] = "Failed to update resume. Please try again.";
                    Console.WriteLine("Failed to update resume. Please try again.");
                }
                return RedirectToAction("Index");
            }
            return View(resumeCreateDTO);

        }

        //VIEW GET
        //DELETE POST
    }
}
