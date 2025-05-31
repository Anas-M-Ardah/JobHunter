using System.ComponentModel.DataAnnotations;

public class ServiceDTO
{
    public Guid ServiceId { get; set; } = Guid.NewGuid();

    [Required(ErrorMessage = "Service name is required")]
    [StringLength(100, MinimumLength = 3, ErrorMessage = "Service name must be between 3 and 100 characters")]
    public string ServiceName { get; set; }

    [Required(ErrorMessage = "Service image URL is required")]
    [Url(ErrorMessage = "Please enter a valid URL")]
    public string ServiceImage { get; set; }

    [Required(ErrorMessage = "Service description is required")]
    [StringLength(1000, MinimumLength = 10, ErrorMessage = "Service description must be between 10 and 1000 characters")]
    public string ServiceDescription { get; set; }
}