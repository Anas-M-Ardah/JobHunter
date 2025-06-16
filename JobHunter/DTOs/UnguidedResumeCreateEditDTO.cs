using System.ComponentModel.DataAnnotations;

namespace JobHunter.DTOs
{
    public class UnguidedResumeCreateEditDTO
    {
        [Key]
        public Guid ResumeId { get; set; } = Guid.NewGuid();
        [Required]
        public string UserInformation { get; set; }
        [Required]
        public string JobDescription { get; set; }

        [Required(ErrorMessage = "User is required")]
        public Guid EndUserId { get; set; }
    }
}
