using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace JobHunter.Models
{
    public class Certificate
    {
        [Key]
        public Guid CertificateId { get; set; } = Guid.NewGuid();

        [Required(ErrorMessage = "Provider name is required")]
        [StringLength(150, ErrorMessage = "Provider name cannot exceed 150 characters")]
        [DisplayName("Certification Provider")]
        public string ProviderName { get; set; }

        [Required(ErrorMessage = "Start date is required")]
        [DataType(DataType.Date)]
        [DisplayName("Issue Date")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateOnly StartDate { get; set; }

        [DataType(DataType.Date)]
        [DisplayName("Expiration Date")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateOnly? EndDate { get; set; }

        [Required(ErrorMessage = "Certificate name/topic is required")]
        [StringLength(200, ErrorMessage = "Certificate name cannot exceed 200 characters")]
        [DisplayName("Certificate Name")]
        public string TopicName { get; set; }

        [Range(0, 100, ErrorMessage = "Score must be between 0 and 100")]
        [DisplayFormat(DataFormatString = "{0:F1}", ApplyFormatInEditMode = true)]
        [DisplayName("Score/Grade")]
        public double? GPA { get; set; }
        public Guid ResumeId { get; internal set; }
    }
}