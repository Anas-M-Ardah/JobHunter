// Services/GeminiService.cs
using Mscc.GenerativeAI;
using Microsoft.Extensions.Configuration;
using System;
using System.Threading.Tasks;

namespace JobHunter.Services
{
    public class GeminiService : IGeminiService
    {
        private readonly IConfiguration _configuration;
        private readonly GenerativeModel _model;
        private readonly GoogleAI _googleAI;

        public GeminiService(IConfiguration configuration)
        {
            _configuration = configuration;

            // Get API key from configuration
            var apiKey = _configuration["Gemini:ApiKey"];

            if (string.IsNullOrEmpty(apiKey))
            {
                throw new InvalidOperationException(
                    "Gemini API key is not configured. Please add it to your configuration."
                );
            }

            // Initialize Google AI
            _googleAI = new GoogleAI(apiKey);

            // Get model name from config or use default
            var modelName = _configuration["Gemini:Model"] ?? "gemini-pro";

            // Create the generative model
            _model = _googleAI.GenerativeModel(model: modelName);
        }

        /// <summary>
        /// Generates text based on the provided prompt using default settings
        /// </summary>
        /// <param name="prompt">The input prompt for text generation</param>
        /// <returns>Generated text response</returns>
        public async Task<string> GenerateTextAsync(string prompt)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(prompt))
                {
                    throw new ArgumentException("Prompt cannot be empty", nameof(prompt));
                }

                // Generate content
                var response = await _model.GenerateContent(prompt);

                // Return the generated text
                return response.Text ?? string.Empty;
            }
            catch (Exception ex)
            {
                // Log the error (you should inject ILogger in production)
                throw new Exception($"Error generating text: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Generates text with custom generation configuration
        /// </summary>
        /// <param name="prompt">The input prompt for text generation</param>
        /// <param name="config">Custom generation configuration</param>
        /// <returns>Generated text response</returns>
        public async Task<string> GenerateTextAsync(string prompt, GenerationConfig config)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(prompt))
                {
                    throw new ArgumentException("Prompt cannot be empty", nameof(prompt));
                }

                // Create model with custom config
                var customModel = _googleAI.GenerativeModel(
                    model: _configuration["Gemini:Model"] ?? "gemini-pro",
                    generationConfig: config
                );

                // Updated the method call to use the correct method name based on the provided type signatures.
                var response = await _model.GenerateContent(prompt);

                return response.Text ?? string.Empty;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error generating text with custom config: {ex.Message}", ex);
            }
        }
    }
}