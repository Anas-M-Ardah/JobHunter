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
        private readonly IGeminiService _geminiService;

        public ResumeController(IResumeRepository resumeRepository, UserManager<User> userManager,
            IWordService wordService, IGeminiService geminiService)
        {
            _resumeRepository = resumeRepository;
            _userManager = userManager;
            _wordService = wordService;
            _geminiService = geminiService;
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

        public async Task<IActionResult> UnguidedResumeCreate()
        {
            try
            {
                // Get the current user
                var user = await _userManager.GetUserAsync(User);

                // Create the DTO model with user information pre-filled
                var model = new UnguidedResumeCreateEditDTO
                {
                    EndUserId = Guid.Parse(user.Id),
                    UserInformation = string.Empty,
                    JobDescription = string.Empty
                };

                return View(model);
            }
            catch (Exception ex)
            {
                // Redirect to error page or show error message
                TempData["ErrorMessage"] = "An error occurred while loading the page. Please try again.";
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpPost]
        public async Task<IActionResult> UnguidedCreate(string userInformation, string jobDescription)
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);

                if (user == null)
                {
                    return Json(new
                    {
                        success = false,
                        message = "User not authenticated. Please log in again.",
                        requiresLogin = true
                    });
                }

                if (string.IsNullOrWhiteSpace(userInformation) || string.IsNullOrWhiteSpace(jobDescription))
                {
                    return Json(new
                    {
                        success = false,
                        message = "Both user information and job description are required.",
                        isValidationError = true
                    });
                }

                //_logger.LogInformation("Generating resume for user {UserId}", user.Id);

                var result = await _geminiService.GenerateBaseResumeAsync(jobDescription, userInformation);

                if (!result.IsValid)
                {
                    return Json(new
                    {
                        success = false,
                        message = result.ValidationMessage,
                        isValidationError = true
                    });
                }

                if (result.ResumeData == null)
                {
                    return Json(new
                    {
                        success = false,
                        message = "Failed to generate resume data",
                        isGenerationError = true
                    });
                }

                // Map to Resume entity for potential saving
                var resume = _resumeRepository.MapToResumeEntity(
                    result.ResumeData,
                    Guid.Parse(user.Id),
                    jobDescription
                );

                // You can save the resume to database here if needed
                // await _resumeService.SaveResumeAsync(resume);

                //_logger.LogInformation("Resume generated successfully for user {UserId}", user.Id);

                return Json(new
                {
                    success = true,
                    resume = result.ResumeData.FormattedResume,
                    resumeData = result.ResumeData // Optional: return structured data too
                });
            }
            catch (Exception ex)
            {
                //_logger.LogError(ex, "Error generating resume for user {UserId}", User.Identity?.Name ?? "Unknown");

                return Json(new
                {
                    success = false,
                    message = "An error occurred while generating the resume. Please try again."
                });
            }
        }
        [HttpPost]
        public async Task<IActionResult> SaveResume([FromBody] Resume resume)
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);

                if (user == null)
                {
                    return Json(new
                    {
                        success = false,
                        message = "User not authenticated. Please log in again."
                    });
                }

                if (resume == null)
                {
                    return Json(new
                    {
                        success = false,
                        message = "Resume data is required."
                    });
                }

                // Generate new ResumeId if it's empty
                if (resume.ResumeId == Guid.Empty)
                {
                    resume.ResumeId = Guid.NewGuid();
                }

                // Set metadata
                resume.CreatedDate = DateTime.Now;
                resume.ModifiedDate = DateTime.Now;
                resume.IsDeleted = false;
                

                // Save to database
                if(await _resumeRepository.SaveResumeAsync(resume, user))
                {
                    return Json(new
                    {
                        success = true,
                        message = "Resume saved successfully!",
                        resumeId = resume.ResumeId
                    });
                }
                else
                {
                    return Json(new
                    {
                        success = false,
                        message = "Failed to save resume. Please try again."
                    });
                }

               
            }
            catch (Exception ex)
            {
                //_logger.LogError(ex, "Error saving resume for user {UserId}", User.Identity?.Name ?? "Unknown");

                return Json(new
                {
                    success = false,
                    message = "An error occurred while saving the resume. Please try again."
                });
            }
        }
    }
}
