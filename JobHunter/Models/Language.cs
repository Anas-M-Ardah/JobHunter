using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace JobHunter.Models
{
    public class Language
    {
        [Key]
        public Guid LanguageId { get; set; } = Guid.NewGuid();

        [Required(ErrorMessage = "Language name is required")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Language name must be between 2 and 50 characters")]
        [DisplayName("Language")]
        public string LanguageName { get; set; }

        [Required(ErrorMessage = "Proficiency level is required")]
        [StringLength(30, ErrorMessage = "Level description cannot exceed 30 characters")]
        [DisplayName("Proficiency Level")]
        public string Level { get; set; }
    }
}