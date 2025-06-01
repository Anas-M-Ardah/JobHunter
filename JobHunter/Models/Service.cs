using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace JobHunter.Models
{
    public class Service
    {
        [Key]
        public Guid ServiceId { get; set; } = Guid.NewGuid();

        [Required(ErrorMessage = "Service name is required")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "Service name must be between 3 and 100 characters")]
        [DisplayName("Service Name")]
        public string ServiceName { get; set; }

        [Required(ErrorMessage = "Service description is required")]
        [StringLength(1000, ErrorMessage = "Description cannot exceed 1000 characters")]
        [DataType(DataType.MultilineText)]
        [DisplayName("Description")]
        public string ServiceDescription { get; set; }

        [Url(ErrorMessage = "Please enter a valid URL for the service image")]
        [StringLength(500, ErrorMessage = "Image URL cannot exceed 500 characters")]
        [DisplayName("Image URL")]
        public string ServiceImage { get; set; }

        [AllowNull]
        [DisplayName("Project ID")]
        [ForeignKey("Project")]
        public Guid ProjectId { get; set; }

        [Required(ErrorMessage = "Portfolio ID is required")]
        [DisplayName("Portfolio ID")]
        [ForeignKey("Portfolio")]
        public Guid PortfolioId { get; set; }
    }
}