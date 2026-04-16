using System.ComponentModel.DataAnnotations;

namespace Ecommerce.ViewModels
{
    public class ValidateOTPVM
    {
        public int Id { get; set; }
        [Required]
        public string OTP { get; set; } = string.Empty;
    }
}
