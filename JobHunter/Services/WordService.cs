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

                    // Add horizontal line after header
                    AddHorizontalLine(body);

                    // About Section
                    if (!string.IsNullOrEmpty(model.Bio))
                    {
                        AddSection(body, "PROFESSIONAL SUMMARY", model.Bio);
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

                    // Languages and Certificates in two columns if both exist
                    var hasLanguages = model.Languages?.Any() == true;
                    var hasCertificates = model.Certificates?.Any() == true;

                    if (hasLanguages && hasCertificates)
                    {
                        AddTwoColumnSection(body, model.Languages, model.Certificates);
                    }
                    else if (hasLanguages)
                    {
                        AddLanguagesSection(body, model.Languages);
                    }
                    else if (hasCertificates)
                    {
                        AddCertificatesSection(body, model.Certificates);
                    }

                    // Set document margins AFTER adding content
                    SetDocumentMargins(body);

                    // Save the document
                    mainPart.Document.Save();
                }

                return stream.ToArray();
            }
        }

        private void SetDocumentMargins(Body body)
        {
            var sectionProps = new SectionProperties();
            var pageMargin = new PageMargin()
            {
                Top = 720,    // 0.5 inch
                Right = 720,  // 0.5 inch
                Bottom = 720, // 0.5 inch
                Left = 720,   // 0.5 inch
                Header = 720,
                Footer = 720,
                Gutter = 0
            };
            sectionProps.Append(pageMargin);

            // Add page size for consistency
            var pageSize = new PageSize()
            {
                Width = 12240,  // 8.5 inches
                Height = 15840  // 11 inches
            };
            sectionProps.Append(pageSize);

            body.Append(sectionProps);
        }

        public void AddStyles(WordprocessingDocument document)
        {
            var stylesPart = document.MainDocumentPart.AddNewPart<StyleDefinitionsPart>();
            stylesPart.Styles = new Styles();

            // Default paragraph style - fixed spacing
            var defaultStyle = new Style()
            {
                Type = StyleValues.Paragraph,
                StyleId = "Normal",
                Default = true
            };
            defaultStyle.Append(new Name() { Val = "Normal" });

            var defaultParagraphProps = new StyleParagraphProperties();
            defaultParagraphProps.Append(new SpacingBetweenLines()
            {
                After = "120",  // 6pt after
                Line = "240",   // 12pt line spacing
                LineRule = LineSpacingRuleValues.Auto
            });
            defaultStyle.Append(defaultParagraphProps);

            var defaultRunProps = new StyleRunProperties();
            defaultRunProps.Append(new RunFonts() { Ascii = "Calibri", HighAnsi = "Calibri" });
            defaultRunProps.Append(new FontSize() { Val = "22" }); // 11pt
            defaultRunProps.Append(new Color() { Val = "000000" }); // Black
            defaultStyle.Append(defaultRunProps);

            stylesPart.Styles.Append(defaultStyle);

            // Header name style
            var headerStyle = new Style()
            {
                Type = StyleValues.Paragraph,
                StyleId = "Header"
            };
            headerStyle.Append(new Name() { Val = "Header" });

            var headerParagraphProps = new StyleParagraphProperties();
            headerParagraphProps.Append(new Justification() { Val = JustificationValues.Left });
            headerParagraphProps.Append(new SpacingBetweenLines() { After = "120" });
            headerStyle.Append(headerParagraphProps);

            var headerRunProps = new StyleRunProperties();
            headerRunProps.Append(new RunFonts() { Ascii = "Calibri", HighAnsi = "Calibri" });
            headerRunProps.Append(new FontSize() { Val = "48" }); // 24pt
            headerRunProps.Append(new Bold());
            headerRunProps.Append(new Color() { Val = "2F5496" }); // Professional blue
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
            var borders = new ParagraphBorders();
            borders.Append(new BottomBorder()
            {
                Val = BorderValues.Single,
                Size = 6,
                Color = "2F5496"
            });
            sectionParagraphProps.Append(borders);
            sectionStyle.Append(sectionParagraphProps);

            var sectionRunProps = new StyleRunProperties();
            sectionRunProps.Append(new RunFonts() { Ascii = "Calibri", HighAnsi = "Calibri" });
            sectionRunProps.Append(new FontSize() { Val = "28" }); // 14pt
            sectionRunProps.Append(new Bold());
            sectionRunProps.Append(new Color() { Val = "2F5496" });
            sectionRunProps.Append(new SmallCaps());
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
            contactParagraphProps.Append(new Justification() { Val = JustificationValues.Left });
            contactParagraphProps.Append(new SpacingBetweenLines() { After = "60" });
            contactStyle.Append(contactParagraphProps);

            var contactRunProps = new StyleRunProperties();
            contactRunProps.Append(new RunFonts() { Ascii = "Calibri", HighAnsi = "Calibri" });
            contactRunProps.Append(new FontSize() { Val = "20" }); // 10pt
            contactRunProps.Append(new Color() { Val = "595959" });
            contactStyle.Append(contactRunProps);

            stylesPart.Styles.Append(contactStyle);

            // Job title style
            var jobTitleStyle = new Style()
            {
                Type = StyleValues.Paragraph,
                StyleId = "JobTitle"
            };
            jobTitleStyle.Append(new Name() { Val = "JobTitle" });

            var jobTitleParagraphProps = new StyleParagraphProperties();
            jobTitleParagraphProps.Append(new SpacingBetweenLines() { Before = "120", After = "60" });
            jobTitleStyle.Append(jobTitleParagraphProps);

            var jobTitleRunProps = new StyleRunProperties();
            jobTitleRunProps.Append(new RunFonts() { Ascii = "Calibri", HighAnsi = "Calibri" });
            jobTitleRunProps.Append(new FontSize() { Val = "24" }); // 12pt
            jobTitleRunProps.Append(new Bold());
            jobTitleRunProps.Append(new Color() { Val = "000000" });
            jobTitleStyle.Append(jobTitleRunProps);

            stylesPart.Styles.Append(jobTitleStyle);

            // Company style
            var companyStyle = new Style()
            {
                Type = StyleValues.Paragraph,
                StyleId = "Company"
            };
            companyStyle.Append(new Name() { Val = "Company" });

            var companyParagraphProps = new StyleParagraphProperties();
            companyParagraphProps.Append(new SpacingBetweenLines() { After = "60" });
            companyStyle.Append(companyParagraphProps);

            var companyRunProps = new StyleRunProperties();
            companyRunProps.Append(new RunFonts() { Ascii = "Calibri", HighAnsi = "Calibri" });
            companyRunProps.Append(new FontSize() { Val = "22" }); // 11pt
            companyRunProps.Append(new Italic());
            companyRunProps.Append(new Color() { Val = "2F5496" });
            companyStyle.Append(companyRunProps);

            stylesPart.Styles.Append(companyStyle);

            // Compact text style
            var compactStyle = new Style()
            {
                Type = StyleValues.Paragraph,
                StyleId = "Compact"
            };
            compactStyle.Append(new Name() { Val = "Compact" });

            var compactParagraphProps = new StyleParagraphProperties();
            compactParagraphProps.Append(new SpacingBetweenLines() { After = "60" });
            compactStyle.Append(compactParagraphProps);

            var compactRunProps = new StyleRunProperties();
            compactRunProps.Append(new RunFonts() { Ascii = "Calibri", HighAnsi = "Calibri" });
            compactRunProps.Append(new FontSize() { Val = "20" }); // 10pt
            compactStyle.Append(compactRunProps);

            stylesPart.Styles.Append(compactStyle);
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

            // Title on separate line
            if (!string.IsNullOrEmpty(model.Title))
            {
                var titlePara = new Paragraph();
                var titleParaProps = new ParagraphProperties();
                titleParaProps.Append(new SpacingBetweenLines() { After = "120" });
                titlePara.Append(titleParaProps);

                var titleRun = new Run();
                titleRun.Append(new RunProperties(
                    new RunFonts() { Ascii = "Calibri", HighAnsi = "Calibri" },
                    new FontSize() { Val = "26" },
                    new Color() { Val = "595959" },
                    new Italic()
                ));
                titleRun.Append(new Text(model.Title));
                titlePara.Append(titleRun);
                body.Append(titlePara);
            }

            // Contact info
            var contactInfo = new List<string>();
            if (!string.IsNullOrEmpty(model.Address)) contactInfo.Add(model.Address);
            if (!string.IsNullOrEmpty(model.PhoneNumber)) contactInfo.Add(model.PhoneNumber);
            if (!string.IsNullOrEmpty(model.Email)) contactInfo.Add(model.Email);

            if (contactInfo.Any())
            {
                var contactPara = new Paragraph();
                var contactParaProps = new ParagraphProperties();
                contactParaProps.Append(new ParagraphStyleId() { Val = "ContactInfo" });
                contactPara.Append(contactParaProps);

                var contactRun = new Run();
                contactRun.Append(new Text(string.Join(" | ", contactInfo)));
                contactPara.Append(contactRun);
                body.Append(contactPara);
            }

            // Links on separate line
            var links = new List<string>();
            if (!string.IsNullOrEmpty(model.LinkedInLink)) links.Add($"LinkedIn: {model.LinkedInLink}");
            if (!string.IsNullOrEmpty(model.PortfolioLink)) links.Add($"Portfolio: {model.PortfolioLink}");

            if (links.Any())
            {
                var linksPara = new Paragraph();
                var linksParaProps = new ParagraphProperties();
                linksParaProps.Append(new ParagraphStyleId() { Val = "ContactInfo" });
                linksPara.Append(linksParaProps);

                var linksRun = new Run();
                linksRun.Append(new Text(string.Join(" | ", links)));
                linksPara.Append(linksRun);
                body.Append(linksPara);
            }
        }

        private void AddHorizontalLine(Body body)
        {
            var para = new Paragraph();
            var paraProps = new ParagraphProperties();
            var borders = new ParagraphBorders();
            borders.Append(new TopBorder()
            {
                Val = BorderValues.Single,
                Size = 8,
                Color = "2F5496"
            });
            paraProps.Append(borders);
            paraProps.Append(new SpacingBetweenLines() { After = "120" });
            para.Append(paraProps);
            body.Append(para);
        }

        public void AddSection(Body body, string title, string content)
        {
            AddSectionTitle(body, title);

            var contentPara = new Paragraph();
            var contentParaProps = new ParagraphProperties();
            contentParaProps.Append(new Justification() { Val = JustificationValues.Both });
            contentParaProps.Append(new SpacingBetweenLines() { After = "120" });
            contentPara.Append(contentParaProps);

            var contentRun = new Run();
            contentRun.Append(new Text(content));
            contentPara.Append(contentRun);
            body.Append(contentPara);
        }

        public void AddExperienceSection(Body body, IEnumerable<Experience> experiences)
        {
            AddSectionTitle(body, "PROFESSIONAL EXPERIENCE");

            foreach (var exp in experiences.OrderByDescending(e => e.StartDate))
            {
                // Job title and duration in one line
                var titlePara = new Paragraph();
                var titleParaProps = new ParagraphProperties();
                titleParaProps.Append(new ParagraphStyleId() { Val = "JobTitle" });

                // Create tab stops for right alignment
                var tabs = new Tabs();
                tabs.Append(new TabStop() { Val = TabStopValues.Right, Position = 9360 });
                titleParaProps.Append(tabs);
                titlePara.Append(titleParaProps);

                var titleRun = new Run();
                titleRun.Append(new Text(exp.Title));
                titlePara.Append(titleRun);

                titlePara.Append(new Run(new TabChar()));

                var durationRun = new Run();
                durationRun.Append(new RunProperties(
                    new RunFonts() { Ascii = "Calibri", HighAnsi = "Calibri" },
                    new FontSize() { Val = "20" },
                    new Color() { Val = "595959" }
                ));
                durationRun.Append(new Text($"{exp.StartDate:MMM yyyy} - {(exp.EndDate?.ToString("MMM yyyy") ?? "Present")}"));
                titlePara.Append(durationRun);
                body.Append(titlePara);

                // Company
                var companyPara = new Paragraph();
                var companyParaProps = new ParagraphProperties();
                companyParaProps.Append(new ParagraphStyleId() { Val = "Company" });
                companyPara.Append(companyParaProps);

                var companyRun = new Run();
                companyRun.Append(new Text(exp.Company));
                companyPara.Append(companyRun);
                body.Append(companyPara);

                // Description
                if (!string.IsNullOrEmpty(exp.Duties))
                {
                    var duties = exp.Duties.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);

                    if (duties.Length > 1)
                    {
                        foreach (var duty in duties)
                        {
                            if (!string.IsNullOrWhiteSpace(duty))
                            {
                                AddBulletPoint(body, duty.Trim());
                            }
                        }
                    }
                    else
                    {
                        var descPara = new Paragraph();
                        var descParaProps = new ParagraphProperties();
                        descParaProps.Append(new Indentation() { Left = "360" });
                        descParaProps.Append(new Justification() { Val = JustificationValues.Both });
                        descParaProps.Append(new SpacingBetweenLines() { After = "120" });
                        descPara.Append(descParaProps);

                        var descRun = new Run();
                        descRun.Append(new Text(exp.Duties));
                        descPara.Append(descRun);
                        body.Append(descPara);
                    }
                }
            }
        }

        private void AddBulletPoint(Body body, string text)
        {
            var bulletPara = new Paragraph();
            var bulletParaProps = new ParagraphProperties();
            bulletParaProps.Append(new Indentation() { Left = "720", Hanging = "360" });
            bulletParaProps.Append(new SpacingBetweenLines() { After = "60" });
            bulletPara.Append(bulletParaProps);

            var bulletRun = new Run();
            bulletRun.Append(new Text("• "));
            bulletPara.Append(bulletRun);

            var textRun = new Run();
            textRun.Append(new Text(text));
            bulletPara.Append(textRun);

            body.Append(bulletPara);
        }

        public void AddSkillsSection(Body body, IEnumerable<Skill> skills)
        {
            AddSectionTitle(body, "CORE COMPETENCIES");

            var skillGroups = skills.GroupBy(s => s.SkillType ?? "Technical Skills");

            foreach (var group in skillGroups)
            {
                var skillPara = new Paragraph();
                var skillParaProps = new ParagraphProperties();
                skillParaProps.Append(new SpacingBetweenLines() { After = "120" });
                skillPara.Append(skillParaProps);

                var categoryRun = new Run();
                categoryRun.Append(new RunProperties(
                    new Bold(),
                    new Color() { Val = "2F5496" }
                ));
                categoryRun.Append(new Text($"{group.Key}: "));
                skillPara.Append(categoryRun);

                var skillsText = string.Join(" • ", group.Select(s => s.SkillName));
                var skillsRun = new Run();
                skillsRun.Append(new Text(skillsText));
                skillPara.Append(skillsRun);

                body.Append(skillPara);
            }
        }

        public void AddEducationSection(Body body, IEnumerable<Education> educations)
        {
            AddSectionTitle(body, "EDUCATION");

            foreach (var edu in educations.OrderByDescending(e => e.StartDate))
            {
                // Institution and Duration line
                var eduPara = new Paragraph();
                var eduParaProps = new ParagraphProperties();
                eduParaProps.Append(new ParagraphStyleId() { Val = "JobTitle" });

                var tabs = new Tabs();
                tabs.Append(new TabStop() { Val = TabStopValues.Right, Position = 9360 });
                eduParaProps.Append(tabs);
                eduPara.Append(eduParaProps);

                var institutionRun = new Run();
                institutionRun.Append(new Text(edu.CollegeName));
                eduPara.Append(institutionRun);

                eduPara.Append(new Run(new TabChar()));

                var durationRun = new Run();
                durationRun.Append(new RunProperties(
                    new RunFonts() { Ascii = "Calibri", HighAnsi = "Calibri" },
                    new FontSize() { Val = "20" },
                    new Color() { Val = "595959" }
                ));
                durationRun.Append(new Text($"{edu.StartDate:MMM yyyy} - {(edu.EndDate?.ToString("MMM yyyy") ?? "Present")}"));
                eduPara.Append(durationRun);
                body.Append(eduPara);

                // Major/Field of Study
                var majorPara = new Paragraph();
                var majorParaProps = new ParagraphProperties();
                majorParaProps.Append(new ParagraphStyleId() { Val = "Company" });
                majorPara.Append(majorParaProps);

                var majorRun = new Run();
                majorRun.Append(new Text(edu.Major));
                majorPara.Append(majorRun);
                body.Append(majorPara);

                // Degree Type and GPA line (only if either exists)
                if (!string.IsNullOrEmpty(edu.DegreeType) || edu.GPA.HasValue)
                {
                    var degreeGpaPara = new Paragraph();
                    var degreeGpaParaProps = new ParagraphProperties();

                    // Use a slightly smaller font size and different color for degree/GPA
                    var degreeGpaStyle = new ParagraphStyleId() { Val = "Normal" };
                    degreeGpaParaProps.Append(degreeGpaStyle);

                    // Add tab stop for GPA alignment
                    var degreeGpaTabs = new Tabs();
                    degreeGpaTabs.Append(new TabStop() { Val = TabStopValues.Right, Position = 9360 });
                    degreeGpaParaProps.Append(degreeGpaTabs);

                    degreeGpaPara.Append(degreeGpaParaProps);

                    // Degree Type (left side)
                    if (!string.IsNullOrEmpty(edu.DegreeType))
                    {
                        var degreeRun = new Run();
                        degreeRun.Append(new RunProperties(
                            new RunFonts() { Ascii = "Calibri", HighAnsi = "Calibri" },
                            new FontSize() { Val = "20" }, // 10pt
                            new Color() { Val = "595959" },
                            new Italic()
                        ));
                        degreeRun.Append(new Text(edu.DegreeType));
                        degreeGpaPara.Append(degreeRun);
                    }

                    // GPA (right side)
                    if (edu.GPA.HasValue)
                    {
                        // Add tab character to align GPA to the right
                        degreeGpaPara.Append(new Run(new TabChar()));

                        var gpaRun = new Run();
                        gpaRun.Append(new RunProperties(
                            new RunFonts() { Ascii = "Calibri", HighAnsi = "Calibri" },
                            new FontSize() { Val = "20" }, // 10pt
                            new Color() { Val = "595959" }
                        ));
                        gpaRun.Append(new Text($"GPA: {edu.GPA.Value:F2}"));
                        degreeGpaPara.Append(gpaRun);
                    }

                    body.Append(degreeGpaPara);
                }

                // Add spacing between education items
                var spacingPara = new Paragraph();
                var spacingParaProps = new ParagraphProperties();
                spacingParaProps.Append(new SpacingBetweenLines() { After = "120" }); // 6pt spacing
                spacingPara.Append(spacingParaProps);
                body.Append(spacingPara);
            }
        }

        public void AddLanguagesSection(Body body, IEnumerable<Language> languages)
        {
            AddSectionTitle(body, "LANGUAGES");

            var langPara = new Paragraph();
            var langParaProps = new ParagraphProperties();
            langParaProps.Append(new SpacingBetweenLines() { After = "120" });
            langPara.Append(langParaProps);

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
                certParaProps.Append(new SpacingBetweenLines() { Before = "60", After = "60" });
                certPara.Append(certParaProps);

                var nameRun = new Run();
                nameRun.Append(new RunProperties(new Bold()));
                nameRun.Append(new Text(cert.TopicName));
                certPara.Append(nameRun);

                // Provider and date on same line
                var details = new List<string>();
                if (!string.IsNullOrEmpty(cert.ProviderName))
                    details.Add(cert.ProviderName);
                if (cert.StartDate != default(DateOnly))
                    details.Add(cert.StartDate.ToString("MMM yyyy"));

                if (details.Any())
                {
                    var detailsRun = new Run();
                    detailsRun.Append(new RunProperties(
                        new Color() { Val = "595959" },
                        new FontSize() { Val = "20" }
                    ));
                    detailsRun.Append(new Text($" | {string.Join(" | ", details)}"));
                    certPara.Append(detailsRun);
                }

                body.Append(certPara);
            }
        }

        private void AddTwoColumnSection(Body body, IEnumerable<Language> languages, IEnumerable<Certificate> certificates)
        {
            // Create a table for two columns
            var table = new Table();

            var tableProps = new TableProperties();
            tableProps.Append(new TableWidth() { Type = TableWidthUnitValues.Pct, Width = "5000" });
            tableProps.Append(new TableLayout() { Type = TableLayoutValues.Fixed });

            var tableBorders = new TableBorders(
                new TopBorder() { Val = BorderValues.None },
                new BottomBorder() { Val = BorderValues.None },
                new LeftBorder() { Val = BorderValues.None },
                new RightBorder() { Val = BorderValues.None },
                new InsideHorizontalBorder() { Val = BorderValues.None },
                new InsideVerticalBorder() { Val = BorderValues.None }
            );
            tableProps.Append(tableBorders);
            table.Append(tableProps);

            // Create table grid
            var tableGrid = new TableGrid();
            tableGrid.Append(new GridColumn() { Width = "4680" }); // Half width
            tableGrid.Append(new GridColumn() { Width = "4680" }); // Half width
            table.Append(tableGrid);

            var row = new TableRow();

            // Languages column
            var languageCell = new TableCell();
            var langCellProps = new TableCellProperties();
            langCellProps.Append(new TableCellWidth() { Type = TableWidthUnitValues.Dxa, Width = "4680" });
            langCellProps.Append(new TableCellVerticalAlignment() { Val = TableVerticalAlignmentValues.Top });
            langCellProps.Append(new TableCellMargin(
                new LeftMargin() { Width = "0", Type = TableWidthUnitValues.Dxa },
                new RightMargin() { Width = "144", Type = TableWidthUnitValues.Dxa }
            ));
            languageCell.Append(langCellProps);

            // Add languages content to cell
            var langTitlePara = new Paragraph();
            var langTitleParaProps = new ParagraphProperties();
            langTitleParaProps.Append(new ParagraphStyleId() { Val = "SectionTitle" });
            langTitlePara.Append(langTitleParaProps);
            langTitlePara.Append(new Run(new Text("LANGUAGES")));
            languageCell.Append(langTitlePara);

            var langContentPara = new Paragraph();
            var langContentProps = new ParagraphProperties();
            langContentProps.Append(new SpacingBetweenLines() { After = "120" });
            langContentPara.Append(langContentProps);

            var languageTexts = languages.Select(l => $"{l.LanguageName} ({l.Level})");
            langContentPara.Append(new Run(new Text(string.Join(" • ", languageTexts))));
            languageCell.Append(langContentPara);

            // Certificates column
            var certCell = new TableCell();
            var certCellProps = new TableCellProperties();
            certCellProps.Append(new TableCellWidth() { Type = TableWidthUnitValues.Dxa, Width = "4680" });
            certCellProps.Append(new TableCellVerticalAlignment() { Val = TableVerticalAlignmentValues.Top });
            certCellProps.Append(new TableCellMargin(
                new LeftMargin() { Width = "144", Type = TableWidthUnitValues.Dxa },
                new RightMargin() { Width = "0", Type = TableWidthUnitValues.Dxa }
            ));
            certCell.Append(certCellProps);

            // Add certificates content to cell
            var certTitlePara = new Paragraph();
            var certTitleParaProps = new ParagraphProperties();
            certTitleParaProps.Append(new ParagraphStyleId() { Val = "SectionTitle" });
            certTitlePara.Append(certTitleParaProps);
            certTitlePara.Append(new Run(new Text("CERTIFICATIONS")));
            certCell.Append(certTitlePara);

            foreach (var cert in certificates.OrderByDescending(c => c.StartDate).Take(5)) // Limit for space
            {
                var certPara = new Paragraph();
                var certParaProps = new ParagraphProperties();
                certParaProps.Append(new ParagraphStyleId() { Val = "Compact" });
                certPara.Append(certParaProps);

                var nameRun = new Run();
                nameRun.Append(new RunProperties(new Bold()));
                nameRun.Append(new Text(cert.TopicName));
                certPara.Append(nameRun);

                if (!string.IsNullOrEmpty(cert.ProviderName))
                {
                    var providerRun = new Run();
                    providerRun.Append(new RunProperties(new Color() { Val = "595959" }));
                    providerRun.Append(new Text($" | {cert.ProviderName}"));
                    certPara.Append(providerRun);
                }

                certCell.Append(certPara);
            }

            row.Append(languageCell);
            row.Append(certCell);
            table.Append(row);
            body.Append(table);
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