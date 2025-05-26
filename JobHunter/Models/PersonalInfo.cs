using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace JobHunter.Models
{
    public class PersonalInfo
    {

        [Required(ErrorMessage = "First name is required")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "First name must be between 2 and 50 characters")]
        [DisplayName("First Name")]
        public string FirstName { get; set; }

        [StringLength(50, ErrorMessage = "Second name cannot exceed 50 characters")]
        [DisplayName("Second Name")]
        public string? SecondName { get; set; }

        [StringLength(50, ErrorMessage = "Third name cannot exceed 50 characters")]
        [DisplayName("Third Name")]
        public string? ThirdName { get; set; }

        [Required(ErrorMessage = "Last name is required")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Last name must be between 2 and 50 characters")]
        [DisplayName("Last Name")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Email address is required")]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        [StringLength(100, ErrorMessage = "Email cannot exceed 100 characters")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Date of birth is required")]
        [DataType(DataType.Date)]
        [DisplayName("Date of Birth")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateOnly DateOfBirth { get; set; }

        [Required(ErrorMessage = "Phone number is required")]
        [Phone(ErrorMessage = "Invalid phone number")]
        [StringLength(20, ErrorMessage = "Phone number cannot exceed 20 characters")]
        [DisplayName("Phone Number")]
        public string PhoneNumber { get; set; }

        [Required(ErrorMessage = "Address is required")]
        [StringLength(200, ErrorMessage = "Address cannot exceed 200 characters")]
        public string Address { get; set; }

        [Required(ErrorMessage = "Major is required")]
        [StringLength(100, ErrorMessage = "Major cannot exceed 100 characters")]
        public string Major { get; set; }

        [Url(ErrorMessage = "Please enter a valid GitHub URL")]
        [StringLength(255, ErrorMessage = "GitHub link cannot exceed 255 characters")]
        [DisplayName("GitHub Profile")]
        public string? GitHubLink { get; set; }

        [Url(ErrorMessage = "Please enter a valid LinkedIn URL")]
        [StringLength(255, ErrorMessage = "LinkedIn link cannot exceed 255 characters")]
        [DisplayName("LinkedIn Profile")]
        public string? LinkedInLink { get; set; }

        [Url(ErrorMessage = "Please enter a valid portfolio URL")]
        [StringLength(255, ErrorMessage = "Portfolio link cannot exceed 255 characters")]
        [DisplayName("Portfolio")]
        public string? PortfolioLink { get; set; }

        [StringLength(1000, ErrorMessage = "Bio cannot exceed 1000 characters")]
        [DataType(DataType.MultilineText)]
        [DisplayName("Professional Summary")]
        public string? Bio { get; set; }

        [StringLength(100, ErrorMessage = "Title cannot exceed 100 characters")]
        [DisplayName("Professional Title")]
        public string? Title { get; set; }
    }
}