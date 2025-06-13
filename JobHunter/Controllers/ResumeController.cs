using System.Text.Json;
using JobHunter.DTOs;
using JobHunter.Models;
using JobHunter.Repositories;
using JobHunter.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace JobHunter.Controllers
{
    public class ResumeController : BaseController
    {
        private readonly IResumeRepository _resumeRepository;
        private readonly UserManager<User> _userManager;
        private readonly IWordService _wordService;

        public ResumeController(IResumeRepository resumeRepository, UserManager<User> userManager, IWordService wordService)
        {
            _resumeRepository = resumeRepository;
            _userManager = userManager;
            _wordService = wordService;
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

                bool isCreated = await _resumeRepository.CreateResume(resumeCreateDTO, user);
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
                bool isUpdated = await _resumeRepository.UpdateResume(resumeCreateDTO);
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
        public async Task<IActionResult> View(Guid resumeId)
        {
            var resume = await _resumeRepository.GetResumeByIdAsync(resumeId);
            return View(resume);
        }
        //DELETE POST
        [HttpPost]
        public async Task<IActionResult> Delete(Guid id)
        {
            bool isDeleted = await _resumeRepository.DeleteResumeAsync(id);
            if (isDeleted)
            {
                TempData["SuccessMessage"] = "Resume deleted successfully.";
                Console.WriteLine("Resume deleted successfully.");
            }
            else
            {
                TempData["ErrorMessage"] = "Failed to delete resume. Please try again.";
                Console.WriteLine("Failed to delete resume. Please try again.");
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> DownloadWordDocument(Guid resumeId)
        {
            Resume model = await _resumeRepository.GetResumeByIdAsync(resumeId);
            var wordDocument = _wordService.GenerateWordDocument(model);
            var fileName = $"{model.FirstName}_{model.LastName}_Resume.docx";

            return File(wordDocument,
                "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
                fileName);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateResume([FromBody] JsonElement resumeJson)
        {
            try
            {
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                    Converters = { new DateOnlyJsonConverter(), new NullableDateOnlyJsonConverter() }
                };

                var resume = JsonSerializer.Deserialize<Resume>(resumeJson.GetRawText(), options);

                if (resume == null || resume.ResumeId == Guid.Empty)
                {
                    return Json(new { success = false, message = "Invalid resume data" });
                }

                bool isUpdated = await _resumeRepository.UpdateResume(resume);
                if (!isUpdated)
                {
                    return Json(new { success = false, message = "Failed to update resume" });
                }

                return Json(new { success = true, message = "Resume updated successfully" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Deserialization error: {ex.Message}" });
            }
        }


    }
}
