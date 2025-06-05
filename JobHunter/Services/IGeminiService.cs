using JobHunter.Models;
using Mscc.GenerativeAI;
using Language = JobHunter.Models.Language;

namespace JobHunter.Services
{
    public interface IGeminiService
    {
        Task<string> GenerateTextAsync(string prompt);
        Task<string> GenerateTextAsync(string prompt, GenerationConfig config);
        Task<List<Skill>> GenerateSkillsAsync(string userWrittenSkills);
        Task<List<Education>> GenerateEducationAsync(string userWrittenEducation);
        Task<List<Certificate>> GenerateCertificatesAsync(string userWrittenCertificates);
        Task<List<Experience>> GenerateExperienceAsync(string userWrittenExperience);
        Task<List<Language>> GenerateLanguagesAsync(string userWrittenLanguages);
        Task<string> GenerateAboutMeAsync(string userWrittenAboutMe);
        Task<RevisorResult> ReviseContentAsync(string aboutMe, string skills, string education, string experience, string certificates, string languages);
    }
}
