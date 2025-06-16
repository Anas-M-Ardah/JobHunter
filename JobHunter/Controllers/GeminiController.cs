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
                var prompt = "are you able to generate base64 images?";
                var result = await _geminiService.GenerateTextAsync(prompt);

                return Json(new { success = true, text = result });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, error = ex.Message });
            }
        }

        public async Task<IActionResult> TestSkills()
        {
            try
            {
                var prompt = "I can write code in nodejs react and expressjs and also I can develop frontend apps using " +
                    "html css js and bootstrap and I can do version control with git and github and also I learned " +
                    "typescript and developed apis with it";

                var skills = await _geminiService.GenerateSkillsAsync(prompt);

                return Json(new
                {
                    success = true,
                    skills = skills,
                    totalSkills = skills.Count,
                    message = $"Successfully generated {skills.Count} skills"
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    error = ex.Message,
                    skills = new List<object>() // Empty array for consistency
                });
            }
        }

        public async Task<IActionResult> TestEducation()
        {
            try
            {
                var prompt = "I graduated from University of California, Los Angeles with a Bachelor of Science in Computer Science in 2022. " +
                    "My GPA was 3.5. Before that I completed high school at Lincoln High School in 2018. " +
                    "Currently I'm pursuing a Master's degree in Artificial Intelligence at Stanford University, started in 2023.";

                var education = await _geminiService.GenerateEducationAsync(prompt);

                return Json(new
                {
                    success = true,
                    education = education,
                    totalEducation = education.Count,
                    message = $"Successfully generated {education.Count} education entries"
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    error = ex.Message,
                    education = new List<object>() // Empty array for consistency
                });
            }
        }
        public async Task<IActionResult> TestCertificates()
        {
            try
            {
                var prompt = "I got AWS Solutions Architect Associate certification from Amazon Web Services in March 2023 with a score of 85%. " +
                    "Also have Microsoft Azure Fundamentals AZ-900 from Microsoft in June 2023. " +
                    "I'm CompTIA Security+ certified which I got in December 2022. " +
                    "Recently completed Google Cloud Professional Cloud Architect certification in September 2023 with 88% score.";

                var certificates = await _geminiService.GenerateCertificatesAsync(prompt);

                return Json(new
                {
                    success = true,
                    certificates = certificates,
                    totalCertificates = certificates.Count,
                    message = $"Successfully generated {certificates.Count} certificate entries"
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    error = ex.Message,
                    certificates = new List<object>() // Empty array for consistency
                });
            }
        }

        public async Task<IActionResult> TestExperience()
        {
            try
            {
                var prompt = "I worked as a Senior Software Developer at Microsoft Corporation from March 2022 to present. " +
                    "My responsibilities include developing web applications using C# and ASP.NET Core, leading a team of 3 junior developers, " +
                    "and improving application performance by 40% through code optimization. " +
                    "Before that, I was a Junior Software Developer at TechStart Inc from June 2020 to March 2022. " +
                    "There I built responsive web interfaces using React and JavaScript, participated in code reviews and agile development processes, " +
                    "and contributed to bug fixes and feature implementations. " +
                    "I also worked as an Intern Software Developer at CodeCraft Solutions from January 2020 to May 2020, " +
                    "where I gained experience in version control with Git and assisted senior developers with various projects.";

                var experiences = await _geminiService.GenerateExperienceAsync(prompt);

                return Json(new
                {
                    success = true,
                    experiences = experiences,
                    totalExperiences = experiences.Count,
                    message = $"Successfully generated {experiences.Count} experience entries"
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    error = ex.Message,
                    experiences = new List<object>() // Empty array for consistency
                });
            }
        }

        public async Task<IActionResult> TestLanguages()
        {
            try
            {
                var prompt = "I speak English fluently as it's my native language. " +
                    "I'm also fluent in Spanish since I lived in Mexico for 3 years. " +
                    "I have intermediate level French from my university studies. " +
                    "I can speak basic German that I learned during my exchange program. " +
                    "I'm currently learning Mandarin Chinese and I'm at beginner level. " +
                    "I also know some Arabic from my family background - I'd say intermediate level.";

                var languages = await _geminiService.GenerateLanguagesAsync(prompt);

                return Json(new
                {
                    success = true,
                    languages = languages,
                    totalLanguages = languages.Count,
                    message = $"Successfully generated {languages.Count} language entries"
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    error = ex.Message,
                    languages = new List<object>() // Empty array for consistency
                });
            }
        }

        public async Task<IActionResult> TestAboutMe()
        {
            try
            {
                string result = await _geminiService.GenerateAboutMeAsync("I am a software developer with a passion for building scalable web applications. " +
                    "I have experience in full-stack development using technologies like React, Node.js, and MongoDB. " +
                    "In my free time, I enjoy contributing to open-source projects and learning new programming languages.");
                return Json(new
                {
                    success = true,
                    aboutMe = result
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    error = ex.Message,
                    aboutMe = string.Empty // Empty string for consistency
                });
            }
        }
        public async Task<IActionResult> TestRevisor()
        {
            try
            {
                // Sample data with intentional inconsistencies
                var aboutMe = "I'm a passionate software developer with 5 years of experience in C++ and Python development. " +
                             "I specialize in machine learning and have worked extensively with TensorFlow. " +
                             "I'm fluent in Spanish and have basic knowledge of French. " +
                             "I hold a Master's degree in Computer Science and am AWS certified.";

                var skills = "JavaScript, HTML, CSS, React, Node.js, SQL, Git";

                var education = "Bachelor of Science in Computer Engineering from XYZ University, 2019";

                var experience = "Software Engineer at Tech Corp (2020-2023) - Developed web applications using React and Node.js. " +
                                "Junior Developer at StartupXYZ (2019-2020) - Built machine learning models using Python and TensorFlow. " +
                                "Worked on C++ performance optimization projects.";

                var certificates = "Google Analytics Certified, 2022";

                var languages = "English - Native";

                // Call the revisor method
                var result = await _geminiService.ReviseContentAsync(
                    aboutMe: aboutMe,
                    skills: skills,
                    education: education,
                    experience: experience,
                    certificates: certificates,
                    languages: languages
                );

                return Json(new
                {
                    success = true,
                    original = new
                    {
                        aboutMe = aboutMe,
                        skills = skills,
                        education = education,
                        experience = experience,
                        certificates = certificates,
                        languages = languages
                    },
                    revised = new
                    {
                        aboutMe = result.RevisedAboutMe,
                        skills = result.RevisedSkills,
                        education = result.RevisedEducation,
                        experience = result.RevisedExperience,
                        certificates = result.RevisedCertificates,
                        languages = result.RevisedLanguages
                    },
                    message = "Revision completed successfully. Check for added missing information like C++, Python, TensorFlow in skills, Spanish/French in languages, AWS certification in certificates, and Master's degree in education."
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    error = ex.Message,
                    original = new object(),
                    revised = new object()
                });
            }
        }
    }//end of class GeminiController
} // End of namespace JobHunter.Controllers
