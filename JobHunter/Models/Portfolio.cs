using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace JobHunter.Models
{
    public class Portfolio : PersonalInfo
    {
        [Key]
        public Guid PortfolioId { get; set; } = Guid.NewGuid();

        [Url(ErrorMessage = "Please enter a valid URL for the personal image")]
        [StringLength(500, ErrorMessage = "Image URL cannot exceed 500 characters")]
        [DisplayName("Profile Image")]
        public string PersonalImage { get; set; }

        public List<Service> Services { get; set; }

        public List<Project> Projects { get; set; }

        [Required(ErrorMessage = "User is required")]
        public EndUser EndUser { get; set; }

        [DataType(DataType.DateTime)]
        [DisplayName("Created Date")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd HH:mm}", ApplyFormatInEditMode = true)]
        public DateTime CreatedDate { get; set; } = DateTime.Now;

        [DataType(DataType.DateTime)]
        [DisplayName("Last Modified")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd HH:mm}", ApplyFormatInEditMode = true)]
        public DateTime ModifiedDate { get; set; } = DateTime.Now;

        
    }
}