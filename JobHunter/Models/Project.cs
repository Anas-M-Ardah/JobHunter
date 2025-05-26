using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace JobHunter.Models
{
    public class Project
    {
        [Key]
        public Guid ProjectId { get; set; } = Guid.NewGuid();

        [Required(ErrorMessage = "Project name is required")]
        [StringLength(150, MinimumLength = 3, ErrorMessage = "Project name must be between 3 and 150 characters")]
        [DisplayName("Project Name")]
        public string ProjectName { get; set; }

        [Required(ErrorMessage = "Project description is required")]
        [StringLength(2000, ErrorMessage = "Description cannot exceed 2000 characters")]
        [DataType(DataType.MultilineText)]
        [DisplayName("Description")]
        public string ProjectDescription { get; set; }

        [Required(ErrorMessage = "Start date is required")]
        [DataType(DataType.Date)]
        [DisplayName("Start Date")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateOnly StartDate { get; set; }

        [DataType(DataType.Date)]
        [DisplayName("End Date")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateOnly? EndDate { get; set; }

        [Required(ErrorMessage = "Service ID is required")]
        public Guid ServiceId { get; set; }

        [Url(ErrorMessage = "Please enter a valid project URL")]
        [StringLength(500, ErrorMessage = "Project link cannot exceed 500 characters")]
        [DisplayName("Project Link")]
        public string ProjectLink { get; set; }

        //TODO: Add Project Attachments
    }
}