using Microsoft.AspNetCore.Identity;

namespace Ecommerce.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public DateOnly DOB { get; set; }
        public string? Address { get; set; }
    }
}
