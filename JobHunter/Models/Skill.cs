using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace JobHunter.Models
{
    public class Skill
    {
        [Key]
        public Guid SkillId { get; set; } = Guid.NewGuid();

        [Required(ErrorMessage = "Skill name is required")]
        [StringLength(100, MinimumLength = 1, ErrorMessage = "Skill name must be between 1 and 100 characters")]
        [DisplayName("Skill")]
        public string SkillName { get; set; }

        [Required(ErrorMessage = "Skill type is required")]
        [StringLength(50, ErrorMessage = "Skill type cannot exceed 50 characters")]
        [DisplayName("Category")]
        public string SkillType { get; set; }
        public Guid ResumeId { get; internal set; }
    }
}