using JobHunter.Services;
using Microsoft.AspNetCore.Mvc;

namespace JobHunter.Controllers
{
    public class GeminiController : BaseController
    {
        private readonly IGeminiService _geminiService;
        public GeminiController(IGeminiService geminiService)
        {
            _geminiService = geminiService;
        }
        // Simple text generation
        [HttpGet]
        public async Task<IActionResult> TestSimpleGeneration()
        {
            try
            {
                var prompt = "Write a brief professional summary for a software developer with 5 years of experience";
                var result = await _geminiService.GenerateTextAsync(prompt);

                return Json(new { success = true, text = result });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, error = ex.Message });
            }
        }
    }
}
