using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using JobHunter.Models;

namespace JobHunter.DTOs
{
    public class ResumeCreateEditDTO : PersonalInfo
    {
        [Key]
        public Guid ResumeId { get; set; } = Guid.NewGuid();
        [Required]
        public string Educations { get; set; }
        [Required]
        public string Experiences { get; set; }
        [Required]
        public string Skills { get; set; }
        [Required]
        public string Languages { get; set; }
        [Required]
        public string Certificates { get; set; }

        [Required(ErrorMessage = "User is required")]
        public Guid EndUserId { get; set; }
    }
}
