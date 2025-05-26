using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using JobHunter.Models;

namespace JobHunter.DTOs
{
    public class PortfolioCreateEditDTO : PersonalInfo
    {
        [Key]
        public Guid PortfolioId { get; set; } = Guid.NewGuid();

        [Url(ErrorMessage = "Please enter a valid URL for the personal image")]
        [StringLength(500, ErrorMessage = "Image URL cannot exceed 500 characters")]
        [DisplayName("Profile Image")]
        public string PersonalImage { get; set; }

        [Required(ErrorMessage = "Services information is required")]
        [DisplayName("Services Offered")]
        [DataType(DataType.MultilineText)]
        public string Services { get; set; }

        [Required(ErrorMessage = "Projects information is required")]
        [DisplayName("Projects")]
        [DataType(DataType.MultilineText)]
        public string Projects { get; set; }

        [Required(ErrorMessage = "User ID is required")]
        public Guid EndUserId { get; set; }
    }
}