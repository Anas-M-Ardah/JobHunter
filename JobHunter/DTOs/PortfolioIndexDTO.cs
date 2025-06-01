using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace JobHunter.DTOs
{
    public class PortfolioIndexDTO
    {
        [Required]
        public Guid PortfolioId { get; set; }

        [Required(ErrorMessage = "First name is required")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "First name must be between 2 and 50 characters")]
        [DisplayName("First Name")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Last name is required")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Last name must be between 2 and 50 characters")]
        [DisplayName("Last Name")]
        public string LastName { get; set; }

        [StringLength(100, ErrorMessage = "Title cannot exceed 100 characters")]
        [DisplayName("Professional Title")]
        public string? Title { get; set; }

        [DataType(DataType.DateTime)]
        [DisplayName("Last Modified")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd HH:mm}", ApplyFormatInEditMode = true)]
        public DateTime ModifiedDate { get; set; } = DateTime.Now;

    }
}
