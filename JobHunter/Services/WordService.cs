using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using JobHunter.Models;

namespace JobHunter.Services
{
    public class WordService : IWordService
    {
        public byte[] GenerateWordDocument(Resume model)
        {
            using (var stream = new MemoryStream())
            {
                using (var document = WordprocessingDocument.Create(stream, WordprocessingDocumentType.Document))
                {
                    // Create main document part
                    var mainPart = document.AddMainDocumentPart();
                    mainPart.Document = new Document();
                    var body = mainPart.Document.AppendChild(new Body());

                    // Add styles first
                    AddStyles(document);

                    // Header Section
                    AddHeader(body, model);

                    // About Section
                    if (!string.IsNullOrEmpty(model.Bio))
                    {
                        AddSection(body, "ABOUT", model.Bio);
                    }

                    // Skills Section
                    if (model.Skills?.Any() == true)
                    {
                        AddSkillsSection(body, model.Skills);
                    }

                    // Experience Section
                    if (model.Experiences?.Any() == true)
                    {
                        AddExperienceSection(body, model.Experiences);
                    }

                    // Education Section
                    if (model.Educations?.Any() == true)
                    {
                        AddEducationSection(body, model.Educations);
                    }

                    // Languages Section
                    if (model.Languages?.Any() == true)
                    {
                        AddLanguagesSection(body, model.Languages);
                    }

                    // Certificates Section
                    if (model.Certificates?.Any() == true)
                    {
                        AddCertificatesSection(body, model.Certificates);
                    }

                    // Save the document
                    mainPart.Document.Save();
                }

                return stream.ToArray();
            }
        }

        public void AddStyles(WordprocessingDocument document)
        {
            var stylesPart = document.MainDocumentPart.AddNewPart<StyleDefinitionsPart>();
            stylesPart.Styles = new Styles();

            // Create default paragraph style
            var defaultStyle = new Style()
            {
                Type = StyleValues.Paragraph,
                StyleId = "Normal",
                Default = true
            };
            defaultStyle.Append(new Name() { Val = "Normal" });
            //defaultStyle.Append(new PrimaryStyle() { Val = true });

            var defaultParagraphProps = new StyleParagraphProperties();
            defaultParagraphProps.Append(new SpacingBetweenLines() { After = "120" });
            defaultStyle.Append(defaultParagraphProps);

            var defaultRunProps = new StyleRunProperties();
            defaultRunProps.Append(new RunFonts() { Ascii = "Calibri", HighAnsi = "Calibri" });
            defaultRunProps.Append(new FontSize() { Val = "22" }); // 11pt
            defaultStyle.Append(defaultRunProps);

            stylesPart.Styles.Append(defaultStyle);

            // Header style
            var headerStyle = new Style()
            {
                Type = StyleValues.Paragraph,
                StyleId = "Header"
            };
            headerStyle.Append(new Name() { Val = "Header" });

            var headerParagraphProps = new StyleParagraphProperties();
            headerParagraphProps.Append(new Justification() { Val = JustificationValues.Center });
            headerParagraphProps.Append(new SpacingBetweenLines() { After = "240" });
            headerStyle.Append(headerParagraphProps);

            var headerRunProps = new StyleRunProperties();
            headerRunProps.Append(new RunFonts() { Ascii = "Calibri", HighAnsi = "Calibri" });
            headerRunProps.Append(new FontSize() { Val = "56" }); // 28pt
            headerRunProps.Append(new Bold());
            headerRunProps.Append(new Color() { Val = "2C3E50" });
            headerStyle.Append(headerRunProps);

            stylesPart.Styles.Append(headerStyle);

            // Section title style
            var sectionStyle = new Style()
            {
                Type = StyleValues.Paragraph,
                StyleId = "SectionTitle"
            };
            sectionStyle.Append(new Name() { Val = "SectionTitle" });

            var sectionParagraphProps = new StyleParagraphProperties();
            sectionParagraphProps.Append(new SpacingBetweenLines() { Before = "240", After = "120" });
            sectionStyle.Append(sectionParagraphProps);

            var sectionRunProps = new StyleRunProperties();
            sectionRunProps.Append(new RunFonts() { Ascii = "Calibri", HighAnsi = "Calibri" });
            sectionRunProps.Append(new FontSize() { Val = "28" }); // 14pt
            sectionRunProps.Append(new Bold());
            sectionRunProps.Append(new Color() { Val = "2C3E50" });
            sectionStyle.Append(sectionRunProps);

            stylesPart.Styles.Append(sectionStyle);

            // Contact info style
            var contactStyle = new Style()
            {
                Type = StyleValues.Paragraph,
                StyleId = "ContactInfo"
            };
            contactStyle.Append(new Name() { Val = "ContactInfo" });

            var contactParagraphProps = new StyleParagraphProperties();
            contactParagraphProps.Append(new Justification() { Val = JustificationValues.Center });
            contactParagraphProps.Append(new SpacingBetweenLines() { After = "240" });
            contactStyle.Append(contactParagraphProps);

            var contactRunProps = new StyleRunProperties();
            contactRunProps.Append(new RunFonts() { Ascii = "Calibri", HighAnsi = "Calibri" });
            contactRunProps.Append(new FontSize() { Val = "20" }); // 10pt
            contactStyle.Append(contactRunProps);

            stylesPart.Styles.Append(contactStyle);
        }

        public void AddHeader(Body body, Resume model)
        {
            // Name with style
            var namePara = new Paragraph();
            var nameParaProps = new ParagraphProperties();
            nameParaProps.Append(new ParagraphStyleId() { Val = "Header" });
            namePara.Append(nameParaProps);

            var nameRun = new Run();
            nameRun.Append(new Text($"{model.FirstName} {model.LastName}"));
            namePara.Append(nameRun);
            body.Append(namePara);

            // Contact info
            var contactInfo = new List<string>();
            if (!string.IsNullOrEmpty(model.Title)) contactInfo.Add(model.Title);
            if (!string.IsNullOrEmpty(model.Address)) contactInfo.Add(model.Address);
            if (!string.IsNullOrEmpty(model.PhoneNumber)) contactInfo.Add(model.PhoneNumber);
            if (!string.IsNullOrEmpty(model.Email)) contactInfo.Add(model.Email);
            if (!string.IsNullOrEmpty(model.LinkedInLink)) contactInfo.Add(model.LinkedInLink);
            if (!string.IsNullOrEmpty(model.PortfolioLink)) contactInfo.Add(model.PortfolioLink);

            if (contactInfo.Any())
            {
                var contactPara = new Paragraph();
                var contactParaProps = new ParagraphProperties();
                contactParaProps.Append(new ParagraphStyleId() { Val = "ContactInfo" });
                contactPara.Append(contactParaProps);

                var contactRun = new Run();
                contactRun.Append(new Text(string.Join(" • ", contactInfo)));
                contactPara.Append(contactRun);
                body.Append(contactPara);
            }
        }

        public void AddSection(Body body, string title, string content)
        {
            // Section title with style
            var titlePara = new Paragraph();
            var titleParaProps = new ParagraphProperties();
            titleParaProps.Append(new ParagraphStyleId() { Val = "SectionTitle" });
            titlePara.Append(titleParaProps);

            var titleRun = new Run();
            titleRun.Append(new Text(title));
            titlePara.Append(titleRun);
            body.Append(titlePara);

            // Content
            var contentPara = new Paragraph();
            var contentRun = new Run();
            contentRun.Append(new Text(content));
            contentPara.Append(contentRun);
            body.Append(contentPara);
        }

        public void AddExperienceSection(Body body, IEnumerable<Experience> experiences)
        {
            AddSectionTitle(body, "EXPERIENCE");

            foreach (var exp in experiences.OrderByDescending(e => e.StartDate))
            {
                // Job title and duration
                var titlePara = new Paragraph();
                var titleParaProps = new ParagraphProperties();
                titleParaProps.Append(new SpacingBetweenLines() { Before = "120" });
                titlePara.Append(titleParaProps);

                var titleRun = new Run();
                titleRun.Append(new RunProperties(new Bold()));
                titleRun.Append(new Text(exp.Title));
                titlePara.Append(titleRun);

                var durationRun = new Run();
                durationRun.Append(new Text($" | {exp.StartDate:MMM yyyy} - {(exp.EndDate?.ToString("MMM yyyy") ?? "Present")}"));
                titlePara.Append(durationRun);
                body.Append(titlePara);

                // Company
                var companyPara = new Paragraph();
                var companyRun = new Run();
                companyRun.Append(new RunProperties(new Italic()));
                companyRun.Append(new Text(exp.Company));
                companyPara.Append(companyRun);
                body.Append(companyPara);

                // Description
                if (!string.IsNullOrEmpty(exp.Duties))
                {
                    var descPara = new Paragraph();
                    var descParaProps = new ParagraphProperties();
                    descParaProps.Append(new Indentation() { Left = "360" }); // Indent description
                    descPara.Append(descParaProps);

                    var descRun = new Run();
                    descRun.Append(new Text(exp.Duties));
                    descPara.Append(descRun);
                    body.Append(descPara);
                }
            }
        }

        public void AddSkillsSection(Body body, IEnumerable<Skill> skills)
        {
            AddSectionTitle(body, "SKILLS");

            var skillGroups = skills.GroupBy(s => s.SkillType ?? "Technical Skills");
            foreach (var group in skillGroups)
            {
                var skillPara = new Paragraph();
                var skillParaProps = new ParagraphProperties();
                skillParaProps.Append(new SpacingBetweenLines() { After = "60" });
                skillPara.Append(skillParaProps);

                // Category name
                var categoryRun = new Run();
                categoryRun.Append(new RunProperties(new Bold()));
                categoryRun.Append(new Text($"{group.Key}: "));
                skillPara.Append(categoryRun);

                // Skills
                var skillsRun = new Run();
                skillsRun.Append(new Text(string.Join(" • ", group.Select(s => s.SkillName))));
                skillPara.Append(skillsRun);

                body.Append(skillPara);
            }
        }

        public void AddEducationSection(Body body, IEnumerable<Education> educations)
        {
            AddSectionTitle(body, "EDUCATION");

            foreach (var edu in educations.OrderByDescending(e => e.StartDate))
            {
                var eduPara = new Paragraph();
                var eduParaProps = new ParagraphProperties();
                eduParaProps.Append(new SpacingBetweenLines() { Before = "120" });
                eduPara.Append(eduParaProps);

                var institutionRun = new Run();
                institutionRun.Append(new RunProperties(new Bold()));
                institutionRun.Append(new Text(edu.CollegeName));
                eduPara.Append(institutionRun);

                var durationRun = new Run();
                durationRun.Append(new Text($" | {edu.StartDate:MMM yyyy} - {(edu.EndDate?.ToString("MMM yyyy") ?? "Present")}"));
                eduPara.Append(durationRun);

                body.Append(eduPara);

                var majorPara = new Paragraph();
                var majorRun = new Run();
                majorRun.Append(new RunProperties(new Italic()));
                majorRun.Append(new Text(edu.Major));
                majorPara.Append(majorRun);
                body.Append(majorPara);
            }
        }

        public void AddLanguagesSection(Body body, IEnumerable<Language> languages)
        {
            AddSectionTitle(body, "LANGUAGES");

            var langPara = new Paragraph();
            var languageTexts = languages.Select(l => $"{l.LanguageName} ({l.Level})");
            var langRun = new Run();
            langRun.Append(new Text(string.Join(" • ", languageTexts)));
            langPara.Append(langRun);
            body.Append(langPara);
        }

        public void AddCertificatesSection(Body body, IEnumerable<Certificate> certificates)
        {
            AddSectionTitle(body, "CERTIFICATIONS");

            foreach (var cert in certificates.OrderByDescending(c => c.StartDate))
            {
                var certPara = new Paragraph();
                var certParaProps = new ParagraphProperties();
                certParaProps.Append(new SpacingBetweenLines() { Before = "120" });
                certPara.Append(certParaProps);

                var nameRun = new Run();
                nameRun.Append(new RunProperties(new Bold()));
                nameRun.Append(new Text(cert.TopicName));
                certPara.Append(nameRun);

                if (cert.StartDate != default(DateOnly))
                {
                    var dateRun = new Run();
                    dateRun.Append(new Text($" | {cert.StartDate:MMM yyyy}"));
                    certPara.Append(dateRun);
                }

                body.Append(certPara);

                if (!string.IsNullOrEmpty(cert.ProviderName))
                {
                    var providerPara = new Paragraph();
                    var providerRun = new Run();
                    providerRun.Append(new RunProperties(new Italic()));
                    providerRun.Append(new Text(cert.ProviderName));
                    providerPara.Append(providerRun);
                    body.Append(providerPara);
                }
            }
        }

        public void AddSectionTitle(Body body, string title)
        {
            var titlePara = new Paragraph();
            var titleParaProps = new ParagraphProperties();
            titleParaProps.Append(new ParagraphStyleId() { Val = "SectionTitle" });
            titlePara.Append(titleParaProps);

            var titleRun = new Run();
            titleRun.Append(new Text(title));
            titlePara.Append(titleRun);
            body.Append(titlePara);
        }

    }
}
