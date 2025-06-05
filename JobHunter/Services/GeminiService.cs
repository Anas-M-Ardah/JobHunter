// Services/GeminiService.cs
using System;
using System.Threading.Tasks;
using JobHunter.Models;
using Microsoft.Extensions.Configuration;
using Mscc.GenerativeAI;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Language = JobHunter.Models.Language;

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

        public async Task<List<Skill>> GenerateSkillsAsync(string userWrittenSkills)
        {
            try
            {
                // Validate input
                if (string.IsNullOrWhiteSpace(userWrittenSkills))
                {
                    return new List<Skill>();
                }

                // Step 1: Send to AI for reformatting
                string prompt = $@"
                    Convert the following user-written skills into professional resume/CV format.
                    Categorize each skill appropriately (e.g., Technical Skills, Soft Skills, Languages, Programming Languages, Tools & Software, Frameworks, Databases).
                    Make skill names professional and properly formatted.

                    User input: {userWrittenSkills}

                    Return ONLY a valid JSON array with this exact format (no additional text or explanations):
                    [
                        {{""skillName"": ""skill name"", ""skillType"": ""category""}}
                    ]

                    Example output:
                    [
                        {{""skillName"": ""C# Programming"", ""skillType"": ""Programming Languages""}},
                        {{""skillName"": ""Team Leadership"", ""skillType"": ""Soft Skills""}}
                    ]";

                // Get AI response
                var aiResponse = await _model.GenerateContent(prompt);
                string aiResponseText = aiResponse?.Text?.Trim();

                if (string.IsNullOrWhiteSpace(aiResponseText))
                {
                    return new List<Skill>();
                }

                // Clean the response - remove any markdown formatting or extra text
                aiResponseText = CleanJsonResponse(aiResponseText);

                // Step 2: Parse AI response and create Skill objects
                try
                {
                    var skillsArray = JsonConvert.DeserializeObject<JArray>(aiResponseText);
                    var skills = new List<Skill>();

                    foreach (var skillToken in skillsArray)
                    {
                        var skillObj = skillToken as JObject;
                        if (skillObj != null)
                        {
                            string skillName = skillObj["skillName"]?.ToString();
                            string skillType = skillObj["skillType"]?.ToString();

                            if (!string.IsNullOrWhiteSpace(skillName) && !string.IsNullOrWhiteSpace(skillType))
                            {
                                skills.Add(new Skill
                                {
                                    SkillId = Guid.NewGuid(),
                                    SkillName = skillName.Trim(),
                                    SkillType = skillType.Trim()
                                });
                            }
                        }
                    }

                    return skills;
                }
                catch (JsonException ex)
                {
                    // Log the error for debugging
                    Console.WriteLine($"JSON parsing error: {ex.Message}");
                    Console.WriteLine($"AI Response: {aiResponseText}");

                    // Fallback: try to extract skills manually
                    return ExtractSkillsManually(userWrittenSkills);
                }
            }
            catch (Exception ex)
            {
                // Log the error for debugging
                Console.WriteLine($"GenerateSkillsAsync error: {ex.Message}");

                // Return fallback skills instead of empty list
                return ExtractSkillsManually(userWrittenSkills);
            }
        }

        private string CleanJsonResponse(string response)
        {
            // Remove markdown code blocks
            response = response.Replace("```json", "").Replace("```", "");

            // Find the JSON array in the response
            int startIndex = response.IndexOf('[');
            int endIndex = response.LastIndexOf(']');

            if (startIndex >= 0 && endIndex > startIndex)
            {
                response = response.Substring(startIndex, endIndex - startIndex + 1);
            }

            return response.Trim();
        }

        private List<Skill> ExtractSkillsManually(string userSkills)
        {
            var skills = new List<Skill>();

            // Simple fallback: split by common delimiters and create basic skills
            var skillNames = userSkills.Split(new char[] { ',', ';', '\n', '|' }, StringSplitOptions.RemoveEmptyEntries);

            foreach (var skillName in skillNames)
            {
                var trimmedSkill = skillName.Trim();
                if (!string.IsNullOrWhiteSpace(trimmedSkill))
                {
                    skills.Add(new Skill
                    {
                        SkillId = Guid.NewGuid(),
                        SkillName = trimmedSkill,
                        SkillType = "General Skills" // Default category
                    });
                }
            }

            return skills;
        }

        public async Task<List<Education>> GenerateEducationAsync(string userWrittenEducation)
        {
            try
            {
                // Validate input
                if (string.IsNullOrWhiteSpace(userWrittenEducation))
                {
                    return new List<Education>();
                }

                // Step 1: Send to AI for reformatting
                string prompt = $@"
                    Convert the following user-written education information into professional resume/CV format.
                    Extract all educational entries and format them properly.

                    User input: {userWrittenEducation}

                    Return ONLY a valid JSON array with this exact format (no additional text or explanations):
                    [
                        {{
                            ""collegeName"": ""institution name"",
                            ""degreeType"": ""degree type (e.g., Bachelor of Science, Master of Arts, High School Diploma)"",
                            ""major"": ""field of study/major"",
                            ""startDate"": ""YYYY-MM-DD"",
                            ""endDate"": ""YYYY-MM-DD or null if ongoing"",
                            ""gpa"": ""GPA as number or null if not mentioned""
                        }}
                    ]

                    Guidelines:
                    - Use proper institution names (full names, not abbreviations when possible)
                    - Standardize degree types (Bachelor of Science, Master of Arts, etc.)
                    - Format dates as YYYY-MM-DD or use reasonable estimates if exact dates not provided
                    - Set endDate to null for ongoing education
                    - Only include GPA if explicitly mentioned
                    - If graduation year only is mentioned, use June as the month (YYYY-06-01)

                    Example output:
                    [
                        {{
                            ""collegeName"": ""University of California, Los Angeles"",
                            ""degreeType"": ""Bachelor of Science"",
                            ""major"": ""Computer Science"",
                            ""startDate"": ""2018-09-01"",
                            ""endDate"": ""2022-06-01"",
                            ""gpa"": 3.5
                        }}
                    ]";

                // Get AI response
                var aiResponse = await _model.GenerateContent(prompt);
                string aiResponseText = aiResponse?.Text?.Trim();

                if (string.IsNullOrWhiteSpace(aiResponseText))
                {
                    return new List<Education>();
                }

                // Clean the response - remove any markdown formatting or extra text
                aiResponseText = CleanJsonResponse(aiResponseText);

                // Step 2: Parse AI response and create Education objects
                try
                {
                    var educationArray = JsonConvert.DeserializeObject<JArray>(aiResponseText);
                    var educationList = new List<Education>();

                    foreach (var eduToken in educationArray)
                    {
                        var eduObj = eduToken as JObject;
                        if (eduObj != null)
                        {
                            var education = CreateEducationFromJson(eduObj);
                            if (education != null)
                            {
                                educationList.Add(education);
                            }
                        }
                    }

                    return educationList;
                }
                catch (JsonException ex)
                {
                    // Log the error for debugging
                    Console.WriteLine($"JSON parsing error: {ex.Message}");
                    Console.WriteLine($"AI Response: {aiResponseText}");

                    // Fallback: try to extract education manually
                    return ExtractEducationManually(userWrittenEducation);
                }
            }
            catch (Exception ex)
            {
                // Log the error for debugging
                Console.WriteLine($"GenerateEducationAsync error: {ex.Message}");

                // Return fallback education instead of empty list
                return ExtractEducationManually(userWrittenEducation);
            }
        }

        private Education CreateEducationFromJson(JObject eduObj)
        {
            try
            {
                string collegeName = eduObj["collegeName"]?.ToString();
                string degreeType = eduObj["degreeType"]?.ToString();
                string major = eduObj["major"]?.ToString();
                string startDateStr = eduObj["startDate"]?.ToString();
                string endDateStr = eduObj["endDate"]?.ToString();
                string gpaStr = eduObj["gpa"]?.ToString();

                // Validate required fields
                if (string.IsNullOrWhiteSpace(collegeName) ||
                    string.IsNullOrWhiteSpace(degreeType) ||
                    string.IsNullOrWhiteSpace(major) ||
                    string.IsNullOrWhiteSpace(startDateStr))
                {
                    return null;
                }

                var education = new Education
                {
                    EducationId = Guid.NewGuid(),
                    CollegeName = collegeName.Trim(),
                    DegreeType = degreeType.Trim(),
                    Major = major.Trim()
                };

                // Parse start date
                if (DateOnly.TryParse(startDateStr, out DateOnly startDate))
                {
                    education.StartDate = startDate;
                }
                else
                {
                    // Default to January 1st if parsing fails
                    education.StartDate = DateOnly.FromDateTime(DateTime.Now.AddYears(-4));
                }

                // Parse end date (nullable)
                if (!string.IsNullOrWhiteSpace(endDateStr) &&
                    !endDateStr.Equals("null", StringComparison.OrdinalIgnoreCase))
                {
                    if (DateOnly.TryParse(endDateStr, out DateOnly endDate))
                    {
                        education.EndDate = endDate;
                    }
                }

                // Parse GPA (nullable)
                if (!string.IsNullOrWhiteSpace(gpaStr) &&
                    !gpaStr.Equals("null", StringComparison.OrdinalIgnoreCase))
                {
                    if (double.TryParse(gpaStr, out double gpa) && gpa >= 0 && gpa <= 4.0)
                    {
                        education.GPA = gpa;
                    }
                }

                return education;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating education from JSON: {ex.Message}");
                return null;
            }
        }

        private List<Education> ExtractEducationManually(string userEducation)
        {
            var educationList = new List<Education>();

            try
            {
                // Simple fallback: create a basic education entry
                // This is a very basic implementation - you might want to enhance it
                var lines = userEducation.Split(new char[] { '\n', ';' }, StringSplitOptions.RemoveEmptyEntries);

                foreach (var line in lines)
                {
                    var trimmedLine = line.Trim();
                    if (!string.IsNullOrWhiteSpace(trimmedLine) && trimmedLine.Length > 10)
                    {
                        educationList.Add(new Education
                        {
                            EducationId = Guid.NewGuid(),
                            CollegeName = "Institution Name", // Default
                            DegreeType = "Degree", // Default
                            Major = trimmedLine, // Use the input as major
                            StartDate = DateOnly.FromDateTime(DateTime.Now.AddYears(-4)),
                            EndDate = DateOnly.FromDateTime(DateTime.Now)
                        });
                    }
                }

                // If no valid entries found, create one default entry
                if (!educationList.Any())
                {
                    educationList.Add(new Education
                    {
                        EducationId = Guid.NewGuid(),
                        CollegeName = "Educational Institution",
                        DegreeType = "Degree",
                        Major = userEducation.Length > 100 ? userEducation.Substring(0, 100) : userEducation,
                        StartDate = DateOnly.FromDateTime(DateTime.Now.AddYears(-4)),
                        EndDate = DateOnly.FromDateTime(DateTime.Now)
                    });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in manual education extraction: {ex.Message}");
            }

            return educationList;
        }

        public async Task<List<Certificate>> GenerateCertificatesAsync(string userWrittenCertificates)
        {
            try
            {
                // Validate input
                if (string.IsNullOrWhiteSpace(userWrittenCertificates))
                {
                    return new List<Certificate>();
                }

                // Step 1: Send to AI for reformatting
                string prompt = $@"
                Convert the following user-written certificate information into professional resume/CV format.
                Extract all certificate entries and format them properly.

                User input: {userWrittenCertificates}

                Return ONLY a valid JSON array with this exact format (no additional text or explanations):
                [
                    {{
                        ""providerName"": ""certification provider/organization"",
                        ""topicName"": ""certificate name/title"",
                        ""startDate"": ""YYYY-MM-DD (issue/completion date)"",
                        ""endDate"": ""YYYY-MM-DD or null if no expiration"",
                        ""gpa"": ""score/grade as number or null if not mentioned""
                    }}
                ]

                Guidelines:
                - Use proper organization names (Microsoft, AWS, Google, Cisco, etc.)
                - Standardize certificate names (full official titles when possible)
                - Format dates as YYYY-MM-DD or use reasonable estimates if exact dates not provided
                - Set endDate to null for certificates that don't expire
                - Only include score/grade if explicitly mentioned (convert percentages to 0-100 scale)
                - If only year is mentioned, use January as the month (YYYY-01-01)
                - Common providers: Microsoft, Amazon Web Services, Google Cloud, Cisco, CompTIA, Oracle, etc.

                Example output:
                [
                    {{
                        ""providerName"": ""Microsoft"",
                        ""topicName"": ""Azure Fundamentals (AZ-900)"",
                        ""startDate"": ""2023-06-15"",
                        ""endDate"": null,
                        ""gpa"": 85.5
                    }},
                    {{
                        ""providerName"": ""Amazon Web Services"",
                        ""topicName"": ""AWS Certified Solutions Architect - Associate"",
                        ""startDate"": ""2023-03-10"",
                        ""endDate"": ""2026-03-10"",
                        ""gpa"": null
                    }}
                ]";

                // Get AI response
                var aiResponse = await _model.GenerateContent(prompt);
                string aiResponseText = aiResponse?.Text?.Trim();

                if (string.IsNullOrWhiteSpace(aiResponseText))
                {
                    return new List<Certificate>();
                }

                // Clean the response - remove any markdown formatting or extra text
                aiResponseText = CleanJsonResponse(aiResponseText);

                // Step 2: Parse AI response and create Certificate objects
                try
                {
                    var certificatesArray = JsonConvert.DeserializeObject<JArray>(aiResponseText);
                    var certificatesList = new List<Certificate>();

                    foreach (var certToken in certificatesArray)
                    {
                        var certObj = certToken as JObject;
                        if (certObj != null)
                        {
                            var certificate = CreateCertificateFromJson(certObj);
                            if (certificate != null)
                            {
                                certificatesList.Add(certificate);
                            }
                        }
                    }

                    return certificatesList;
                }
                catch (JsonException ex)
                {
                    // Log the error for debugging
                    Console.WriteLine($"JSON parsing error: {ex.Message}");
                    Console.WriteLine($"AI Response: {aiResponseText}");

                    // Fallback: try to extract certificates manually
                    return ExtractCertificatesManually(userWrittenCertificates);
                }
            }
            catch (Exception ex)
            {
                // Log the error for debugging
                Console.WriteLine($"GenerateCertificatesAsync error: {ex.Message}");

                // Return fallback certificates instead of empty list
                return ExtractCertificatesManually(userWrittenCertificates);
            }
        }

        private Certificate CreateCertificateFromJson(JObject certObj)
        {
            try
            {
                string providerName = certObj["providerName"]?.ToString();
                string topicName = certObj["topicName"]?.ToString();
                string startDateStr = certObj["startDate"]?.ToString();
                string endDateStr = certObj["endDate"]?.ToString();
                string gpaStr = certObj["gpa"]?.ToString();

                // Validate required fields
                if (string.IsNullOrWhiteSpace(providerName) ||
                    string.IsNullOrWhiteSpace(topicName) ||
                    string.IsNullOrWhiteSpace(startDateStr))
                {
                    return null;
                }

                var certificate = new Certificate
                {
                    CertificateId = Guid.NewGuid(),
                    ProviderName = providerName.Trim(),
                    TopicName = topicName.Trim()
                };

                // Parse start date (issue date)
                if (DateOnly.TryParse(startDateStr, out DateOnly startDate))
                {
                    certificate.StartDate = startDate;
                }
                else
                {
                    // Default to current date if parsing fails
                    certificate.StartDate = DateOnly.FromDateTime(DateTime.Now);
                }

                // Parse end date (expiration date - nullable)
                if (!string.IsNullOrWhiteSpace(endDateStr) &&
                    !endDateStr.Equals("null", StringComparison.OrdinalIgnoreCase))
                {
                    if (DateOnly.TryParse(endDateStr, out DateOnly endDate))
                    {
                        certificate.EndDate = endDate;
                    }
                }

                // Parse score/grade (nullable)
                if (!string.IsNullOrWhiteSpace(gpaStr) &&
                    !gpaStr.Equals("null", StringComparison.OrdinalIgnoreCase))
                {
                    if (double.TryParse(gpaStr, out double score) && score >= 0 && score <= 100)
                    {
                        certificate.GPA = score;
                    }
                }

                return certificate;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating certificate from JSON: {ex.Message}");
                return null;
            }
        }

        private List<Certificate> ExtractCertificatesManually(string userCertificates)
        {
            var certificatesList = new List<Certificate>();

            try
            {
                // Simple fallback: split by common delimiters and create basic certificates
                var lines = userCertificates.Split(new char[] { '\n', ';', ',' }, StringSplitOptions.RemoveEmptyEntries);

                foreach (var line in lines)
                {
                    var trimmedLine = line.Trim();
                    if (!string.IsNullOrWhiteSpace(trimmedLine) && trimmedLine.Length > 5)
                    {
                        // Try to extract provider and topic from the line
                        var parts = trimmedLine.Split(new char[] { '-', ':', '|' }, 2);

                        string providerName = "Certification Provider";
                        string topicName = trimmedLine;

                        if (parts.Length == 2)
                        {
                            providerName = parts[0].Trim();
                            topicName = parts[1].Trim();
                        }

                        certificatesList.Add(new Certificate
                        {
                            CertificateId = Guid.NewGuid(),
                            ProviderName = providerName,
                            TopicName = topicName,
                            StartDate = DateOnly.FromDateTime(DateTime.Now.AddMonths(-6)) // Default to 6 months ago
                        });
                    }
                }

                // If no valid entries found, create one default entry
                if (!certificatesList.Any())
                {
                    certificatesList.Add(new Certificate
                    {
                        CertificateId = Guid.NewGuid(),
                        ProviderName = "Certification Provider",
                        TopicName = userCertificates.Length > 200 ? userCertificates.Substring(0, 200) : userCertificates,
                        StartDate = DateOnly.FromDateTime(DateTime.Now)
                    });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in manual certificate extraction: {ex.Message}");
            }

            return certificatesList;
        }

        public async Task<List<Experience>> GenerateExperienceAsync(string userWrittenExperience)
        {
            try
            {
                // Validate input
                if (string.IsNullOrWhiteSpace(userWrittenExperience))
                {
                    return new List<Experience>();
                }

                // Step 1: Send to AI for reformatting
                string prompt = $@"
                    Convert the following user-written work experience information into professional resume/CV format.
                    Extract all work experience entries and format them properly.

                    User input: {userWrittenExperience}

                    Return ONLY a valid JSON array with this exact format (no additional text or explanations):
                    [
                        {{
                            ""title"": ""job title/position"",
                            ""company"": ""company name"",
                            ""startDate"": ""YYYY-MM-DD"",
                            ""endDate"": ""YYYY-MM-DD or null if current position"",
                            ""isCurrent"": true/false,
                            ""duties"": ""detailed job responsibilities and achievements""
                        }}
                    ]

                    Guidelines:
                    - Use proper job titles (Software Developer, Marketing Manager, Sales Representative, etc.)
                    - Use full company names when possible
                    - Format dates as YYYY-MM-DD or use reasonable estimates if exact dates not provided
                    - Set endDate to null and isCurrent to true for current positions
                    - Duties should be detailed, professional, and highlight achievements/responsibilities
                    - If only year/month is mentioned, use the 1st day of the month (YYYY-MM-01)
                    - Keep duties under 2000 characters
                    - Use action verbs and quantify achievements where possible

                    Example output:
                    [
                        {{
                            ""title"": ""Senior Software Developer"",
                            ""company"": ""Microsoft Corporation"",
                            ""startDate"": ""2022-03-15"",
                            ""endDate"": null,
                            ""isCurrent"": true,
                            ""duties"": ""Developed and maintained web applications using C# and ASP.NET Core. Led a team of 3 junior developers. Improved application performance by 40% through code optimization. Collaborated with cross-functional teams to deliver high-quality software solutions.""
                        }},
                        {{
                            ""title"": ""Junior Software Developer"",
                            ""company"": ""TechStart Inc."",
                            ""startDate"": ""2020-06-01"",
                            ""endDate"": ""2022-03-10"",
                            ""isCurrent"": false,
                            ""duties"": ""Built responsive web interfaces using React and JavaScript. Participated in code reviews and agile development processes. Contributed to bug fixes and feature implementations. Gained experience in version control with Git.""
                        }}
                    ]";

                // Get AI response
                var aiResponse = await _model.GenerateContent(prompt);
                string aiResponseText = aiResponse?.Text?.Trim();

                if (string.IsNullOrWhiteSpace(aiResponseText))
                {
                    return new List<Experience>();
                }

                // Clean the response - remove any markdown formatting or extra text
                aiResponseText = CleanJsonResponse(aiResponseText);

                // Step 2: Parse AI response and create Experience objects
                try
                {
                    var experiencesArray = JsonConvert.DeserializeObject<JArray>(aiResponseText);
                    var experiencesList = new List<Experience>();

                    foreach (var expToken in experiencesArray)
                    {
                        var expObj = expToken as JObject;
                        if (expObj != null)
                        {
                            var experience = CreateExperienceFromJson(expObj);
                            if (experience != null)
                            {
                                experiencesList.Add(experience);
                            }
                        }
                    }

                    return experiencesList;
                }
                catch (JsonException ex)
                {
                    // Log the error for debugging
                    Console.WriteLine($"JSON parsing error: {ex.Message}");
                    Console.WriteLine($"AI Response: {aiResponseText}");

                    // Return empty list if JSON parsing fails
                    return new List<Experience>();
                }
            }
            catch (Exception ex)
            {
                // Log the error for debugging
                Console.WriteLine($"GenerateExperienceAsync error: {ex.Message}");

                // Return empty list instead of fallback experiences
                return new List<Experience>();
            }
        }

        private Experience CreateExperienceFromJson(JObject expObj)
        {
            try
            {
                string title = expObj["title"]?.ToString();
                string company = expObj["company"]?.ToString();
                string startDateStr = expObj["startDate"]?.ToString();
                string endDateStr = expObj["endDate"]?.ToString();
                string isCurrentStr = expObj["isCurrent"]?.ToString();
                string duties = expObj["duties"]?.ToString();

                // Validate required fields
                if (string.IsNullOrWhiteSpace(title) ||
                    string.IsNullOrWhiteSpace(company) ||
                    string.IsNullOrWhiteSpace(startDateStr))
                {
                    return null;
                }

                var experience = new Experience
                {
                    ExperienceId = Guid.NewGuid(),
                    Title = title.Trim().Length > 100 ? title.Trim().Substring(0, 100) : title.Trim(),
                    Company = company.Trim().Length > 100 ? company.Trim().Substring(0, 100) : company.Trim(),
                    Duties = !string.IsNullOrWhiteSpace(duties) ?
                        (duties.Trim().Length > 2000 ? duties.Trim().Substring(0, 2000) : duties.Trim()) :
                        string.Empty
                };

                // Parse start date (required)
                if (DateOnly.TryParse(startDateStr, out DateOnly startDate))
                {
                    experience.StartDate = startDate;
                }
                else
                {
                    // Default to current date if parsing fails
                    experience.StartDate = DateOnly.FromDateTime(DateTime.Now);
                }

                // Parse isCurrent flag
                if (bool.TryParse(isCurrentStr, out bool isCurrent))
                {
                    experience.IsCurrent = isCurrent;
                }

                // Parse end date (nullable)
                if (!string.IsNullOrWhiteSpace(endDateStr) &&
                    !endDateStr.Equals("null", StringComparison.OrdinalIgnoreCase))
                {
                    if (DateOnly.TryParse(endDateStr, out DateOnly endDate))
                    {
                        experience.EndDate = endDate;
                        experience.IsCurrent = false; // If end date exists, it's not current
                    }
                }
                else if (experience.IsCurrent)
                {
                    experience.EndDate = null; // Current position has no end date
                }

                return experience;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating experience from JSON: {ex.Message}");
                return null;
            }
        }

        public async Task<List<Language>> GenerateLanguagesAsync(string userWrittenLanguages)
        {
            try
            {
                // Validate input
                if (string.IsNullOrWhiteSpace(userWrittenLanguages))
                {
                    return new List<Language>();
                }

                // Step 1: Send to AI for reformatting
                string prompt = $@"
                    Convert the following user-written language information into professional resume/CV format.
                    Extract all language entries and format them properly.

                    User input: {userWrittenLanguages}

                    Return ONLY a valid JSON array with this exact format (no additional text or explanations):
                    [
                        {{
                            ""languageName"": ""language name"",
                            ""level"": ""proficiency level""
                        }}
                    ]

                    Guidelines:
                    - Use proper language names (English, Spanish, French, Arabic, Mandarin, etc.)
                    - Standardize proficiency levels to one of these: ""Native"", ""Fluent"", ""Advanced"", ""Intermediate"", ""Basic"", ""Beginner""
                    - Language name must be between 2-50 characters
                    - Level description must not exceed 30 characters
                    - If no level is specified, default to ""Intermediate""
                    - Common language names: English, Spanish, French, German, Italian, Portuguese, Arabic, Mandarin, Japanese, Korean, Russian, Dutch, etc.

                    Example output:
                    [
                        {{
                            ""languageName"": ""English"",
                            ""level"": ""Native""
                        }},
                        {{
                            ""languageName"": ""Spanish"",
                            ""level"": ""Fluent""
                        }},
                        {{
                            ""languageName"": ""French"",
                            ""level"": ""Intermediate""
                        }}
                    ]";

                // Get AI response
                var aiResponse = await _model.GenerateContent(prompt);
                string aiResponseText = aiResponse?.Text?.Trim();

                if (string.IsNullOrWhiteSpace(aiResponseText))
                {
                    return new List<Language>();
                }

                // Clean the response - remove any markdown formatting or extra text
                aiResponseText = CleanJsonResponse(aiResponseText);

                // Step 2: Parse AI response and create Language objects
                try
                {
                    var languagesArray = JsonConvert.DeserializeObject<JArray>(aiResponseText);
                    var languagesList = new List<Language>();

                    foreach (var langToken in languagesArray)
                    {
                        var langObj = langToken as JObject;
                        if (langObj != null)
                        {
                            var language = CreateLanguageFromJson(langObj);
                            if (language != null)
                            {
                                languagesList.Add(language);
                            }
                        }
                    }

                    return languagesList;
                }
                catch (JsonException ex)
                {
                    // Log the error for debugging
                    Console.WriteLine($"JSON parsing error: {ex.Message}");
                    Console.WriteLine($"AI Response: {aiResponseText}");

                    // Return empty list if JSON parsing fails
                    return new List<Language>();
                }
            }
            catch (Exception ex)
            {
                // Log the error for debugging
                Console.WriteLine($"GenerateLanguagesAsync error: {ex.Message}");

                // Return empty list instead of fallback languages
                return new List<Language>();
            }
        }

        private Language CreateLanguageFromJson(JObject langObj)
        {
            try
            {
                string languageName = langObj["languageName"]?.ToString();
                string level = langObj["level"]?.ToString();

                // Validate required fields
                if (string.IsNullOrWhiteSpace(languageName) ||
                    string.IsNullOrWhiteSpace(level))
                {
                    return null;
                }

                // Trim and validate length constraints
                languageName = languageName.Trim();
                level = level.Trim();

                // Enforce length limits
                if (languageName.Length < 2 || languageName.Length > 50)
                {
                    return null;
                }

                if (level.Length > 30)
                {
                    level = level.Substring(0, 30);
                }

                var language = new Language
                {
                    LanguageId = Guid.NewGuid(),
                    LanguageName = languageName,
                    Level = level
                };

                return language;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating language from JSON: {ex.Message}");
                return null;
            }
        }

        public async Task<string> GenerateAboutMeAsync(string userWrittenAboutMe)
        {
            try
            {
                string prompt = $@"
                    Generate a professional 'About Me' section for a resume/CV based on the following user input:
                    {userWrittenAboutMe}
                    The output should be concise, engaging, and highlight key skills and experiences.
                    Focus on the candidate's strengths, career goals, and unique value proposition.
                    Keep it under 300 characters.
                    Example output:
                    ""Dynamic software developer with 5+ years of experience in full-stack development. Passionate about building scalable web applications and improving user experience. Seeking to leverage expertise in JavaScript and Python to contribute to innovative projects at a forward-thinking company.""
                    Return ONLY the text without any additional formatting or explanations.";

                // Get AI response
                var aiResponse = await _model.GenerateContent(prompt);
                string aiResponseText = aiResponse?.Text?.Trim();
                return aiResponseText ?? "ERROR";
            }
            catch (Exception ex)
            {
                // Log the error for debugging
                Console.WriteLine($"GenerateAboutMeAsync error: {ex.Message}");
                return string.Empty;
            }
        }

        public async Task<RevisorResult> ReviseContentAsync(string aboutMe, string skills, string education, string experience, string certificates, string languages)
        {
            try
            {
                // Concatenate all sections for comprehensive analysis
                string combinedContent = $@"
                    ABOUT ME: {aboutMe}
                    SKILLS: {skills}
                    EDUCATION: {education}
                    EXPERIENCE: {experience}
                    CERTIFICATES: {certificates}
                    LANGUAGES: {languages}
                ";

                string prompt = $@"
                    Analyze the following resume sections and ensure consistency across all sections.
                    Look for missing information that should be added to maintain consistency.
                    For example:
                    - If C++ is mentioned in About Me but not in Skills, add it to Skills
                    - If a certification is mentioned in Experience but not in Certificates, add it to Certificates
                    - If a language is mentioned in About Me but not in Languages, add it to Languages
                    - If education details are in Experience but missing from Education, add them

                    Current resume content:
                    {combinedContent}

                    Return ONLY a valid JSON object with the revised sections:
                    {{
                        ""aboutMe"": ""revised about me text"",
                        ""skills"": ""revised skills text"",
                        ""education"": ""revised education text"",
                        ""experience"": ""revised experience text"",
                        ""certificates"": ""revised certificates text"",
                        ""languages"": ""revised languages text""
                    }}

                    Make sure each section is complete and consistent with information from other sections.
                    Keep the original tone and style but add any missing relevant information.
                ";

                var aiResponse = await _model.GenerateContent(prompt);
                string aiResponseText = aiResponse?.Text?.Trim();

                if (string.IsNullOrWhiteSpace(aiResponseText))
                {
                    // Return original content if AI fails
                    return new RevisorResult
                    {
                        RevisedAboutMe = aboutMe,
                        RevisedSkills = skills,
                        RevisedEducation = education,
                        RevisedExperience = experience,
                        RevisedCertificates = certificates,
                        RevisedLanguages = languages
                    };
                }

                aiResponseText = CleanJsonResponse(aiResponseText);

                try
                {
                    var revisedContent = JsonConvert.DeserializeObject<JObject>(aiResponseText);

                    return new RevisorResult
                    {
                        RevisedAboutMe = revisedContent["aboutMe"]?.ToString() ?? aboutMe,
                        RevisedSkills = revisedContent["skills"]?.ToString() ?? skills,
                        RevisedEducation = revisedContent["education"]?.ToString() ?? education,
                        RevisedExperience = revisedContent["experience"]?.ToString() ?? experience,
                        RevisedCertificates = revisedContent["certificates"]?.ToString() ?? certificates,
                        RevisedLanguages = revisedContent["languages"]?.ToString() ?? languages
                    };
                }
                catch (JsonException)
                {
                    // Return original content if JSON parsing fails
                    return new RevisorResult
                    {
                        RevisedAboutMe = aboutMe,
                        RevisedSkills = skills,
                        RevisedEducation = education,
                        RevisedExperience = experience,
                        RevisedCertificates = certificates,
                        RevisedLanguages = languages
                    };
                }
            }
            catch (Exception)
            {
                // Return original content if any error occurs
                return new RevisorResult
                {
                    RevisedAboutMe = aboutMe,
                    RevisedSkills = skills,
                    RevisedEducation = education,
                    RevisedExperience = experience,
                    RevisedCertificates = certificates,
                    RevisedLanguages = languages
                };
            }
        }

    }
}