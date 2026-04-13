using System.ComponentModel.DataAnnotations;

namespace Ecommerce.ViewModels
{
    public class LoginVM
    {
        public int Id { get; set; }
        [Required]
        public string UserNameOrEmail { get; set; } = string.Empty;
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;
        public bool RememberMe { get; set; } // by default : 14 day
    }
}
