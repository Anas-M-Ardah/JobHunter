using Microsoft.AspNetCore.Identity;

namespace JobHunter.Models
{
    public class User : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.Now;

        // Add the missing Portfolios property to fix the error
        public ICollection<Portfolio> Portfolios { get; set; }
    }
}
