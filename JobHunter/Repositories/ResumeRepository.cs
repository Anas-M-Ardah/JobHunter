using JobHunter.Data;
using JobHunter.DTOs;
using JobHunter.Models;
using Microsoft.AspNetCore.Identity;

namespace JobHunter.Repositories
{
    public class ResumeRepository : IResumeRepository
    {
        private readonly ApplicationDbContext _context;
        public ResumeRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public bool CreateResume(ResumeCreateEditDTO resumeCreateDTO, User user)
        {
            try
            {
                Resume resume = MapResumeDTOToResumeModel(resumeCreateDTO, user);
                if (resume == null)
                {
                    return false; // Mapping failed
                }

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

        public bool UpdateResume(ResumeCreateEditDTO resumeCreateDTO)
        {
            try
            {
                var existingResume = _context.Resumes.FirstOrDefault(r => r.ResumeId == resumeCreateDTO.ResumeId);
                if (existingResume == null)
                {
                    return false; // Resume not found
                }
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
                existingResume.Bio = resumeCreateDTO.Bio;
                existingResume.Title = resumeCreateDTO.Title;
                // Update collections as needed (not implemented here)
                existingResume.ModifiedDate = DateTime.Now;
                //for the list of skills, educations, experiences, languages, and certificates
                //the implemenation will be done using methods later
                _context.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                // Log the exception (not implemented here)
                return false;
            }
        }

        public List<ResumeIndexDTO> GetAllResumesByUserId(string userId)
        {
            try
            {
                return _context.Resumes
                    .Where(r => r.EndUser.Id == userId)
                    .Select(r => new ResumeIndexDTO
                    {
                        ResumeId = r.ResumeId,
                        FirstName = r.FirstName,
                        LastName = r.LastName,
                        Title = r.Title ?? "No Title", // Handle null titles
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


        public Resume MapResumeDTOToResumeModel(ResumeCreateEditDTO resumeCreateDTO, User user)
        {
            try
            {
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
                    Bio = resumeCreateDTO.Bio,
                    Title = resumeCreateDTO.Title,

                    //TODO: replace this with a method (after we do the openAI Integeration)
                    Skills = new List<Skill>
                    {
                        // Map each skill from DTO to Model
                        // Example: new Skill { Name = skill.Name, Proficiency = skill.Proficiency }
                    },

                    //TODO: replace this with a method (after we do the openAI Integeration)
                    Educations = new List<Education>
                    {
                        // Map each education from DTO to Model
                        // Example: new Education { Degree = education.Degree, Institution = education.Institution, Year = education.Year }
                    },

                    //TODO: replace this with a method (after we do the openAI Integeration)
                    Certificates = new List<Certificate> { },

                    //TODO: replace this with a method (after we do the openAI Integeration)
                    Languages = new List<Language> { },

                    //TODO: replace this with a method (after we do the openAI Integeration)
                    Experiences = new List<Experience> { },

                    CreatedDate = DateTime.Now,
                    ModifiedDate = DateTime.Now,

                    EndUser = (EndUser)user,
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
                    Bio = resume.Bio,
                    Title = resume.Title,

                    // Convert collections to string representations
                    Educations = ConvertEducationsToString(resume.Educations),
                    Experiences = ConvertExperiencesToString(resume.Experiences),
                    Skills = ConvertSkillsToString(resume.Skills),
                    Languages = ConvertLanguagesToString(resume.Languages),
                    Certificates = ConvertCertificatesToString(resume.Certificates),
                };
            }
            catch (Exception ex)
            {
                // Log the exception
                return null;
            }
        }

        // Helper methods to convert collections to strings
        private string ConvertEducationsToString(ICollection<Education> educations)
        {
            if (educations == null || !educations.Any())
                return string.Empty;

            // Format based on your needs - this is just an example
            return string.Join("\n\n", educations.Select(e =>
                $"{e.DegreeType} in {e.Major}\n{e.CollegeName}\n{e.StartDate.Year} - {(e.EndDate.HasValue ? e.EndDate.Value.Year.ToString() : "Present")}\n"
            ));
        }

        private string ConvertExperiencesToString(ICollection<Experience> experiences)
        {
            if (experiences == null || !experiences.Any())
                return string.Empty;

            return string.Join("\n\n", experiences.Select(e =>
                $"{e.Title}\n{e.Company}\n{e.StartDate.Year} - {(e.EndDate.HasValue ? e.EndDate.Value.Year.ToString() : "Present")}\n"
            ));
        }

        private string ConvertSkillsToString(ICollection<Skill> skills)
        {
            if (skills == null || !skills.Any())
                return string.Empty;

            return string.Join(", ", skills.Select(s => s.SkillName));
        }

        private string ConvertLanguagesToString(ICollection<Language> languages)
        {
            if (languages == null || !languages.Any())
                return string.Empty;

            return string.Join(", ", languages.Select(l => $"{l.LanguageName} ({l.Level})"));
        }

        private string ConvertCertificatesToString(ICollection<Certificate> certificates)
        {
            if (certificates == null || !certificates.Any())
                return string.Empty;

            return string.Join("\n", certificates.Select(c =>
                $"{c.TopicName} - {c.ProviderName})"
            ));
        }
    }
    }
