using System.ComponentModel.DataAnnotations;

namespace Ecommerce.ViewModels
{
    public class ChangePasswordVM
    {
        public int Id { get; set; }

        [Required, DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        [Required, DataType(DataType.Password), Compare(nameof(Password))]
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}
