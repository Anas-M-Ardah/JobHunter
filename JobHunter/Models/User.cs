using Microsoft.AspNetCore.Identity;

namespace JobHunter.Models
{
    public class User : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }

        // Add the missing Portfolios property to fix the error
        public ICollection<Portfolio> Portfolios { get; set; }
    }
}
