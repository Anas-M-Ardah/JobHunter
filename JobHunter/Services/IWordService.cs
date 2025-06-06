using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using JobHunter.Models;

namespace JobHunter.Services
{
    public interface IWordService
    {
        public byte[] GenerateWordDocument(Resume model);
        public void AddStyles(WordprocessingDocument document);
        public void AddHeader(Body body, Resume model);
        public void AddSection(Body body, string title, string content);
        public void AddExperienceSection(Body body, IEnumerable<Experience> experiences);
        public void AddSkillsSection(Body body, IEnumerable<Skill> skills);
        public void AddEducationSection(Body body, IEnumerable<Education> educations);
        public void AddLanguagesSection(Body body, IEnumerable<Language> languages);
        public void AddCertificatesSection(Body body, IEnumerable<Certificate> certificates);
        public void AddSectionTitle(Body body, string title);
    }
}
