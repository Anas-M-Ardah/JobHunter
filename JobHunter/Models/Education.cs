using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace JobHunter.Models
{
    public class Education
    {
        [Key]
        public Guid EducationId { get; set; } = Guid.NewGuid();

        [Required(ErrorMessage = "College/University name is required")]
        [StringLength(150, ErrorMessage = "College name cannot exceed 150 characters")]
        [DisplayName("College/University")]
        public string CollegeName { get; set; }

        [Required(ErrorMessage = "Degree type is required")]
        [StringLength(100, ErrorMessage = "Degree type cannot exceed 100 characters")]
        [DisplayName("Degree")]
        public string DegreeType { get; set; }

        [Required(ErrorMessage = "Start date is required")]
        [DataType(DataType.Date)]
        [DisplayName("Start Date")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateOnly StartDate { get; set; }

        [DataType(DataType.Date)]
        [DisplayName("End Date/Expected Graduation")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateOnly? EndDate { get; set; }

        [Required(ErrorMessage = "Major is required")]
        [StringLength(100, ErrorMessage = "Major cannot exceed 100 characters")]
        [DisplayName("Field of Study")]
        public string Major { get; set; }

        [Range(0, 4.0, ErrorMessage = "GPA must be between 0.0 and 4.0")]
        [DisplayFormat(DataFormatString = "{0:F2}", ApplyFormatInEditMode = true)]
        [DisplayName("GPA")]
        public double? GPA { get; set; }
    }
}