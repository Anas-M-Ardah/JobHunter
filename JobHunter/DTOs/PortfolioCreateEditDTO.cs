using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
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

        [Required(ErrorMessage = "At least one service is required")]
        [DisplayName("Services Offered")]
        public List<ServiceDTO> Services { get; set; } = new();

        [Required(ErrorMessage = "At least one project is required")]
        [DisplayName("Projects")]
        public List<ProjectDTO> Projects { get; set; } = new();

        [Required(ErrorMessage = "User ID is required")]
        public Guid EndUserId { get; set; }

        // Custom validation method to ensure all projects have valid date ranges
        public bool AreAllProjectDatesValid()
        {
            return Projects?.All(p => p.IsValidDateRange()) ?? true;
        }

        // Custom validation to ensure service IDs in projects are valid
        public bool AreAllProjectServiceIdsValid()
        {
            if (Projects == null || Services == null) return true;

            var serviceIds = Services.Select(s => s.ServiceId).ToHashSet();
            return Projects.All(p => serviceIds.Contains(p.ServiceId));
        }
    }
}