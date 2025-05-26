using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace JobHunter.Models
{
    public class Experience
    {
        [Key]
        public Guid ExperienceId { get; set; } = Guid.NewGuid();

        [Required(ErrorMessage = "Title is required")]
        [StringLength(100, ErrorMessage = "Title cannot exceed 100 characters")]
        [DisplayName("Job Title")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Company name is required")]
        [StringLength(100, ErrorMessage = "Company name cannot exceed 100 characters")]
        [DisplayName("Company")]
        public string Company { get; set; }

        [Required(ErrorMessage = "Start date is required")]
        [DataType(DataType.Date)]
        [DisplayName("Start Date")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateOnly StartDate { get; set; }

        [DataType(DataType.Date)]
        [DisplayName("End Date")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateOnly? EndDate { get; set; }

        [DisplayName("Current Position")]
        public bool IsCurrent { get; set; }

        [DataType(DataType.MultilineText)]
        [StringLength(2000, ErrorMessage = "Duties cannot exceed 2000 characters")]
        public string Duties { get; set; }
    }
}