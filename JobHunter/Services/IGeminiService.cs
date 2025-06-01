using Mscc.GenerativeAI;

namespace JobHunter.Services
{
    public interface IGeminiService
    {
        Task<string> GenerateTextAsync(string prompt);
        Task<string> GenerateTextAsync(string prompt, GenerationConfig config);
    }
}
