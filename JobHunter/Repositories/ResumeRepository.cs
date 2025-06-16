using JobHunter.Data;
using JobHunter.DTOs;
using JobHunter.Models;
using JobHunter.Models.JSONResponse;
using JobHunter.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace JobHunter.Repositories
{
    public class ResumeRepository : IResumeRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly IGeminiService _geminiService;
        public ResumeRepository(ApplicationDbContext context, IGeminiService geminiService)
        {
            _context = context;
            _geminiService = geminiService;
        }

        public async Task<bool> CreateResume(ResumeCreateEditDTO resumeCreateDTO, User user)
        {
            try
            {
                Resume resume = await MapResumeDTOToResumeModel(resumeCreateDTO, user);
                if (resume == null)
                {
                    return false; // Mapping failed
                }
                resume.JobDescription = "none";
                _context.Resumes.Add(resume);
                _context.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                // Log the exception (not implemented here)
                return false;
            }
        }

        public async Task<bool> UpdateResume(ResumeCreateEditDTO resumeCreateDTO)
        {
            try
            {
                var existingResume = await _context.Resumes
                    .FirstOrDefaultAsync(r => r.ResumeId == resumeCreateDTO.ResumeId);

                if (existingResume == null)
                {
                    return false; // Resume not found
                }

                //revise the user input and connect the points together
                RevisorResult revise = await _geminiService.ReviseContentAsync(aboutMe: resumeCreateDTO.Bio,
                                        skills: resumeCreateDTO.Skills, education: resumeCreateDTO.Educations,
                                        certificates: resumeCreateDTO.Certificates, experience: resumeCreateDTO.Experiences,
                                        languages: resumeCreateDTO.Languages);

                // Map the updated properties
                existingResume.FirstName = resumeCreateDTO.FirstName;
                existingResume.SecondName = resumeCreateDTO.SecondName;
                existingResume.ThirdName = resumeCreateDTO.ThirdName;
                existingResume.LastName = resumeCreateDTO.LastName;
                existingResume.Email = resumeCreateDTO.Email;
                existingResume.DateOfBirth = resumeCreateDTO.DateOfBirth;
                existingResume.PhoneNumber = resumeCreateDTO.PhoneNumber;
                existingResume.Address = resumeCreateDTO.Address;
                existingResume.Major = resumeCreateDTO.Major;
                existingResume.GitHubLink = resumeCreateDTO.GitHubLink;
                existingResume.LinkedInLink = resumeCreateDTO.LinkedInLink;
                existingResume.PortfolioLink = resumeCreateDTO.PortfolioLink;
                existingResume.Title = resumeCreateDTO.Title;
                existingResume.ModifiedDate = DateTime.Now;

                existingResume.Bio = await _geminiService.GenerateAboutMeAsync(revise.RevisedAboutMe);
                await UpdateResumeSkillsAsync(resumeCreateDTO.ResumeId, revise.RevisedSkills);
                await UpdateResumeEducationAsync(resumeCreateDTO.ResumeId, revise.RevisedEducation);
                await UpdateResumeCertificatesAsync(resumeCreateDTO.ResumeId, revise.RevisedCertificates);
                await UpdateResumeExperienceAsync(resumeCreateDTO.ResumeId, revise.RevisedExperience);
                await UpdateResumeLanguagesAsync(resumeCreateDTO.ResumeId, revise.RevisedLanguages);

                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                // Log the exception (consider using proper logging like ILogger)
                Console.WriteLine($"Error updating resume: {ex.Message}");
                return false;
            }
        }

        private async Task UpdateResumeSkillsAsync(Guid resumeId, string skillsInput)
        {
            try
            {
                // Step 1: Remove all existing skills for this resume
                var existingSkills = await _context.Skills
                    .Where(s => s.ResumeId == resumeId)
                    .ToListAsync();

                if (existingSkills.Any())
                {
                    _context.Skills.RemoveRange(existingSkills);
                }

                // Step 2: Generate and add new skills if input is provided
                if (!string.IsNullOrWhiteSpace(skillsInput))
                {
                    var newSkills = await _geminiService.GenerateSkillsAsync(skillsInput);

                    // Set the ResumeId for all new skills
                    foreach (var skill in newSkills)
                    {
                        skill.ResumeId = resumeId;
                    }

                    // Add new skills to context
                    await _context.Skills.AddRangeAsync(newSkills);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating resume skills: {ex.Message}");
                throw; // Re-throw to be handled by the calling method
            }
        }

        private async Task UpdateResumeEducationAsync(Guid resumeId, string eduInput)
        {
            try
            {
                // Step 1: Remove all existing skills for this resume
                var existingEdu = await _context.Educations
                    .Where(s => s.ResumeId == resumeId)
                    .ToListAsync();

                if (existingEdu.Any())
                {
                    _context.Educations.RemoveRange(existingEdu);
                }

                // Step 2: Generate and add new skills if input is provided
                if (!string.IsNullOrWhiteSpace(eduInput))
                {
                    var newEdu = await _geminiService.GenerateEducationAsync(eduInput);

                    // Set the ResumeId for all new skills
                    foreach (var education in newEdu)
                    {
                        education.ResumeId = resumeId;
                    }

                    // Add new skills to context
                    await _context.Educations.AddRangeAsync(newEdu);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating resume skills: {ex.Message}");
                throw; // Re-throw to be handled by the calling method
            }
        }

        private async Task UpdateResumeCertificatesAsync(Guid resumeId, string cerInput)
        {
            try
            {
                // Step 1: Remove all existing certificates for this resume
                var existingCer = await _context.Certificates
                    .Where(s => s.ResumeId == resumeId)
                    .ToListAsync();
                if (existingCer.Any())
                {
                    _context.Certificates.RemoveRange(existingCer);
                }
                // Step 2: Generate and add new certificates if input is provided
                if (!string.IsNullOrWhiteSpace(cerInput))
                {
                    var newCer = await _geminiService.GenerateCertificatesAsync(cerInput);
                    // Set the ResumeId for all new certificates
                    foreach (var certificate in newCer)
                    {
                        certificate.ResumeId = resumeId;
                    }
                    // Add new certificates to context
                    await _context.Certificates.AddRangeAsync(newCer);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating resume certificates: {ex.Message}");
                throw; // Re-throw to be handled by the calling method
            }
        }

        private async Task UpdateResumeExperienceAsync(Guid resumeId, string expInput)
        {
            try
            {
                // Step 1: Remove all existing experience for this resume
                var existingExp = await _context.Experiences
                    .Where(s => s.ResumeId == resumeId)
                    .ToListAsync();
                if (existingExp.Any())
                {
                    _context.Experiences.RemoveRange(existingExp);
                }
                // Step 2: Generate and add new experience if input is provided
                if (!string.IsNullOrWhiteSpace(expInput))
                {
                    var newExp = await _geminiService.GenerateExperienceAsync(expInput);
                    // Set the ResumeId for all new experience
                    foreach (var experience in newExp)
                    {
                        experience.ResumeId = resumeId;
                    }
                    // Add new experience to context
                    await _context.Experiences.AddRangeAsync(newExp);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating resume experience: {ex.Message}");
                throw; // Re-throw to be handled by the calling method
            }
        }

        private async Task UpdateResumeLanguagesAsync(Guid resumeId, string langInput)
        {
            try
            {
                // Step 1: Remove all existing languages for this resume
                var existingLang = await _context.Languages
                    .Where(s => s.ResumeId == resumeId)
                    .ToListAsync();
                if (existingLang.Any())
                {
                    _context.Languages.RemoveRange(existingLang);
                }
                // Step 2: Generate and add new languages if input is provided
                if (!string.IsNullOrWhiteSpace(langInput))
                {
                    var newLang = await _geminiService.GenerateLanguagesAsync(langInput);
                    // Set the ResumeId for all new languages
                    foreach (var language in newLang)
                    {
                        language.ResumeId = resumeId;
                    }
                    // Add new languages to context
                    await _context.Languages.AddRangeAsync(newLang);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating resume languages: {ex.Message}");
                throw; // Re-throw to be handled by the calling method
            }
        }

        public List<ResumeIndexDTO> GetAllResumesByUserId(string userId)
        {
            try
            {
                return _context.Resumes
                    .Where(r => r.EndUser.Id == userId && r.IsDeleted == false)
                    .Select(r => new ResumeIndexDTO
                    {
                        ResumeId = r.ResumeId,
                        FirstName = r.FirstName,
                        LastName = r.LastName,
                        Title = r.Title ?? "No Title",
                        UpdatedAt = r.ModifiedDate
                    })
                    .ToList();
            }
            catch (Exception ex)
            {
                // Log the exception
                return new List<ResumeIndexDTO>(); // Return empty list instead of null
            }
        }

        public ResumeCreateEditDTO GetResumeById(Guid resumeId)
        {
            try
            {
                var resume = _context.Resumes.FirstOrDefault(r => r.ResumeId == resumeId);
                return MapResumeModelToResumeCreateEditDTO(resume);
            }
            catch (Exception ex)
            {
                // Log the exception (not implemented here)
                return null; // Return null if not found or error occurs
            }
        }


        public async Task<Resume> MapResumeDTOToResumeModel(ResumeCreateEditDTO resumeCreateDTO, User user)
        {
            try
            {

                //revise the user input and connect the points together
                RevisorResult revise = await _geminiService.ReviseContentAsync(aboutMe: resumeCreateDTO.Bio,
                                        skills: resumeCreateDTO.Skills, education: resumeCreateDTO.Educations,
                                        certificates: resumeCreateDTO.Certificates, experience: resumeCreateDTO.Experiences,
                                        languages: resumeCreateDTO.Languages);

                return new Resume
                {
                    //Personal Information
                    FirstName = resumeCreateDTO.FirstName,
                    SecondName = resumeCreateDTO.SecondName,
                    ThirdName = resumeCreateDTO.ThirdName,
                    LastName = resumeCreateDTO.LastName,
                    Email = resumeCreateDTO.Email,
                    DateOfBirth = resumeCreateDTO.DateOfBirth,
                    PhoneNumber = resumeCreateDTO.PhoneNumber,
                    Address = resumeCreateDTO.Address,
                    Major = resumeCreateDTO.Major,
                    GitHubLink = resumeCreateDTO.GitHubLink,
                    LinkedInLink = resumeCreateDTO.LinkedInLink,
                    PortfolioLink = resumeCreateDTO.PortfolioLink,
                    Title = resumeCreateDTO.Title,

                    Bio = await _geminiService.GenerateAboutMeAsync(revise.RevisedAboutMe),
                    Skills = await _geminiService.GenerateSkillsAsync(revise.RevisedSkills),
                    Educations = await _geminiService.GenerateEducationAsync(revise.RevisedEducation),
                    Certificates = await _geminiService.GenerateCertificatesAsync(revise.RevisedCertificates),
                    Experiences = await _geminiService.GenerateExperienceAsync(revise.RevisedExperience),
                    Languages = await _geminiService.GenerateLanguagesAsync(revise.RevisedLanguages),

                    UserInputSkills = resumeCreateDTO.Skills,
                    UserInputEducation = resumeCreateDTO.Educations,
                    UserInputCertificates = resumeCreateDTO.Certificates,
                    UserInputExperiences = resumeCreateDTO.Experiences,
                    UserInputLanguages = resumeCreateDTO.Languages,
                    UserInputBio = resumeCreateDTO.Bio,

                    CreatedDate = DateTime.Now,
                    ModifiedDate = DateTime.Now,
                    EndUser = (EndUser)user
                    
                };
            }
            catch (Exception ex)
            {
                // Log the exception (not implemented here)
                return null;
            }
        }

        public ResumeCreateEditDTO MapResumeModelToResumeCreateEditDTO(Resume resume)
        {
            try
            {
                return new ResumeCreateEditDTO
                {
                    // Map basic PersonalInfo properties
                    ResumeId = resume.ResumeId,
                    FirstName = resume.FirstName,
                    SecondName = resume.SecondName,
                    ThirdName = resume.ThirdName,
                    LastName = resume.LastName,
                    Email = resume.Email,
                    DateOfBirth = resume.DateOfBirth,
                    PhoneNumber = resume.PhoneNumber,
                    Address = resume.Address,
                    Major = resume.Major,
                    GitHubLink = resume.GitHubLink,
                    LinkedInLink = resume.LinkedInLink,
                    PortfolioLink = resume.PortfolioLink,
                    Bio = resume.UserInputBio,
                    Title = resume.Title,

                    // Convert collections to string representations
                    Educations = resume.UserInputEducation,
                    Experiences = resume.UserInputExperiences,
                    Skills = resume.UserInputSkills,
                    Languages = resume.UserInputLanguages,
                    Certificates = resume.UserInputCertificates,
                };
            }
            catch (Exception ex)
            {
                // Log the exception
                return null;
            }
        }

        public async Task<Resume> GetResumeByIdAsync(Guid resumeId)
        {
            return await _context.Resumes
                .Include(r => r.Skills)
                .Include(r => r.Educations)
                .Include(r => r.Experiences)
                .Include(r => r.Languages)
                .Include(r => r.Certificates)
                .FirstOrDefaultAsync(r => r.ResumeId == resumeId && !r.IsDeleted);
        }

        public async Task<bool> DeleteResumeAsync(Guid resumeId)
        {

            try
            {
                await _context.Resumes
                    .Where(r => r.ResumeId == resumeId)
                    .ExecuteUpdateAsync(r => r.SetProperty(r => r.IsDeleted, true));
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<bool> UpdateResume(Resume resume)
        {
            try
            {
                var existingResume = await GetResumeByIdAsync(resume.ResumeId);

                if (existingResume == null)
                {
                    return false;
                }

                // Update the entity state
                existingResume.FirstName = resume.FirstName;
                existingResume.LastName = resume.LastName;
                existingResume.Title = resume.Title;
                existingResume.LinkedInLink = resume.LinkedInLink;
                existingResume.GitHubLink = resume.GitHubLink;
                existingResume.PortfolioLink = resume.PortfolioLink;
                existingResume.Email = resume.Email;
                existingResume.DateOfBirth = resume.DateOfBirth;
                existingResume.PhoneNumber = resume.PhoneNumber;
                existingResume.Address = resume.Address;
                existingResume.Bio = resume.Bio;

                // Update navigation properties
                existingResume.Skills = resume.Skills;
                existingResume.Educations = resume.Educations;
                existingResume.Experiences = resume.Experiences;
                existingResume.Languages = resume.Languages;
                existingResume.Certificates = resume.Certificates;
                existingResume.ModifiedDate = DateTime.Now;

                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating resume: {ex.Message}");
                return false;
            }
        }

        public Resume MapToResumeEntity(ResumeData resumeData, Guid endUserId, string jobDescription)
        {
            var resume = new Resume
            {
                // Personal Information
                FirstName = resumeData.FirstName,
                LastName = resumeData.LastName,
                Email = resumeData.Email,
                PhoneNumber = resumeData.PhoneNumber,
                Address = resumeData.Address,
                Major = resumeData.Major,
                LinkedInLink = resumeData.LinkedInLink,
                GitHubLink = resumeData.GitHubLink,
                PortfolioLink = resumeData.PortfolioLink,
                Bio = resumeData.Bio,
                Title = resumeData.Title,
                JobDescription = jobDescription,

                // Initialize lists
                Educations = new List<Education>(),
                Experiences = new List<Experience>(),
                Skills = new List<Skill>(),
                Languages = new List<Language>(),
                Certificates = new List<Certificate>(),

                // Metadata
                //EndUserId = endUserId,
                CreatedDate = DateTime.Now,
                ModifiedDate = DateTime.Now
            };

            // Parse DateOfBirth
            if (DateOnly.TryParse(resumeData.DateOfBirth, out var dateOfBirth))
            {
                resume.DateOfBirth = dateOfBirth;
            }

            // Map Education
            foreach (var edu in resumeData.Education)
            {
                var education = new Education
                {
                    CollegeName = edu.CollegeName,
                    DegreeType = edu.DegreeType,
                    Major = edu.Major,
                    GPA = edu.GPA,
                    ResumeId = resume.ResumeId
                };

                if (DateOnly.TryParse(edu.StartDate, out var startDate))
                    education.StartDate = startDate;

                if (!string.IsNullOrEmpty(edu.EndDate) && DateOnly.TryParse(edu.EndDate, out var endDate))
                    education.EndDate = endDate;

                resume.Educations.Add(education);
            }

            // Map Experience
            foreach (var exp in resumeData.Experience)
            {
                var experience = new Experience
                {
                    Title = exp.Title,
                    Company = exp.Company,
                    Duties = exp.Duties,
                    IsCurrent = exp.IsCurrent,
                    ResumeId = resume.ResumeId
                };

                if (DateOnly.TryParse(exp.StartDate, out var startDate))
                    experience.StartDate = startDate;

                if (!string.IsNullOrEmpty(exp.EndDate) && DateOnly.TryParse(exp.EndDate, out var endDate))
                    experience.EndDate = endDate;

                resume.Experiences.Add(experience);
            }

            // Map Skills
            foreach (var skill in resumeData.Skills)
            {
                resume.Skills.Add(new Skill
                {
                    SkillName = skill.SkillName,
                    SkillType = skill.SkillType,
                    ResumeId = resume.ResumeId
                });
            }

            // Map Languages
            foreach (var lang in resumeData.Languages)
            {
                resume.Languages.Add(new Language
                {
                    LanguageName = lang.LanguageName,
                    Level = lang.Level,
                    ResumeId = resume.ResumeId
                });
            }

            // Map Certificates
            foreach (var cert in resumeData.Certificates)
            {
                var certificate = new Certificate
                {
                    ProviderName = cert.ProviderName,
                    TopicName = cert.TopicName,
                    GPA = cert.GPA,
                    ResumeId = resume.ResumeId
                };

                if (DateOnly.TryParse(cert.StartDate, out var startDate))
                    certificate.StartDate = startDate;

                if (!string.IsNullOrEmpty(cert.EndDate) && DateOnly.TryParse(cert.EndDate, out var endDate))
                    certificate.EndDate = endDate;

                resume.Certificates.Add(certificate);
            }

            // Also populate UserInput fields for backup/reference
            resume.UserInputSkills = string.Join("; ", resumeData.Skills.Select(s => $"{s.SkillName} ({s.SkillType})"));
            resume.UserInputLanguages = string.Join("; ", resumeData.Languages.Select(l => $"{l.LanguageName} - {l.Level}"));
            resume.UserInputEducation = string.Join("; ", resumeData.Education.Select(e => $"{e.DegreeType} in {e.Major} from {e.CollegeName}"));
            resume.UserInputExperiences = string.Join("; ", resumeData.Experience.Select(e => $"{e.Title} at {e.Company}"));
            resume.UserInputCertificates = string.Join("; ", resumeData.Certificates.Select(c => $"{c.TopicName} from {c.ProviderName}"));

            return resume;
        }

        public async Task<bool> SaveResumeAsync(Resume resume, User user)
        {
            try
            {
                resume.EndUser = (EndUser)user;
                resume.JobDescription = "none";
                await _context.Resumes.AddAsync(resume);
                await _context.SaveChangesAsync();
                return true; // Return true if save is successful
            }
            catch(Exception ex)
            {
                Console.WriteLine($"Error saving resume: {ex.Message}");
                return false; // Return false if an error occurs
            }
        }
    }
    }
