namespace JobHunter.Models.JSONResponse
{
    public class ResumeData
    {
        // Personal Information
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string DateOfBirth { get; set; } = string.Empty;
        public string Major { get; set; } = string.Empty;
        public string? LinkedInLink { get; set; }
        public string? GitHubLink { get; set; }
        public string? PortfolioLink { get; set; }
        public string? Bio { get; set; }
        public string? Title { get; set; }

        // Structured Lists
        public List<EducationData> Education { get; set; } = new List<EducationData>();
        public List<ExperienceData> Experience { get; set; } = new List<ExperienceData>();
        public List<SkillData> Skills { get; set; } = new List<SkillData>();
        public List<LanguageData> Languages { get; set; } = new List<LanguageData>();
        public List<CertificateData> Certificates { get; set; } = new List<CertificateData>();

        // Formatted resume content
        public string FormattedResume { get; set; } = string.Empty;
    }


    // Supporting data classes
    public class EducationData
    {
        public string CollegeName { get; set; } = string.Empty;
        public string DegreeType { get; set; } = string.Empty;
        public string StartDate { get; set; } = string.Empty;
        public string? EndDate { get; set; }
        public string Major { get; set; } = string.Empty;
        public double? GPA { get; set; }
    }

    public class ExperienceData
    {
        public string Title { get; set; } = string.Empty;
        public string Company { get; set; } = string.Empty;
        public string StartDate { get; set; } = string.Empty;
        public string? EndDate { get; set; }
        public bool IsCurrent { get; set; }
        public string Duties { get; set; } = string.Empty;
    }

    public class SkillData
    {
        public string SkillName { get; set; } = string.Empty;
        public string SkillType { get; set; } = string.Empty;
    }

    public class LanguageData
    {
        public string LanguageName { get; set; } = string.Empty;
        public string Level { get; set; } = string.Empty;
    }

    public class CertificateData
    {
        public string ProviderName { get; set; } = string.Empty;
        public string StartDate { get; set; } = string.Empty;
        public string? EndDate { get; set; }
        public string TopicName { get; set; } = string.Empty;
        public double? GPA { get; set; }
    }
}
