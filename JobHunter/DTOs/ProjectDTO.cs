using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Http;

namespace JobHunter.DTOs
{
    public class ProjectDTO
    {
        [Key]
        public Guid ProjectId { get; set; } = Guid.NewGuid();

        [Required(ErrorMessage = "Project name is required")]
        [StringLength(100, ErrorMessage = "Project name cannot exceed 100 characters")]
        [DisplayName("Project Name")]
        public string ProjectName { get; set; }

        [Url(ErrorMessage = "Please enter a valid URL for the project link")]
        [StringLength(500, ErrorMessage = "Project link cannot exceed 500 characters")]
        [DisplayName("Project Link")]
        public string ProjectLink { get; set; }

        [Required(ErrorMessage = "Project description is required")]
        [StringLength(1000, ErrorMessage = "Project description cannot exceed 1000 characters")]
        [DisplayName("Project Description")]
        public string ProjectDescription { get; set; }

        [Required(ErrorMessage = "Start date is required")]
        [DisplayName("Start Date")]
        public DateTime StartDate { get; set; }

        [DisplayName("End Date")]
        public DateTime? EndDate { get; set; }

        [Required(ErrorMessage = "Service association is required")]
        [DisplayName("Service Association")]
        public Guid ServiceId { get; set; }

        [AllowNull]
        [DisplayName("Project Attachments")]
        public IFormFile ProjectAttachments { get; set; }

        // Custom validation to ensure end date is after start date
        public bool IsValidDateRange()
        {
            return !EndDate.HasValue || EndDate.Value >= StartDate;
        }
    }
}