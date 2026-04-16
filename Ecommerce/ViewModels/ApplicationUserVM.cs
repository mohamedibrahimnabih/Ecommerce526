using System.ComponentModel.DataAnnotations;
using System.Reflection.PortableExecutable;

namespace Ecommerce.ViewModels
{
    public class ApplicationUserVM
    {
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        [Required]
        public string Email { get; set; }
        public string? Address { get; set; }
        public string? PhoneNumber { get; set; }
    }
}
