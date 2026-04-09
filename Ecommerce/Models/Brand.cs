using System.ComponentModel.DataAnnotations;

namespace Ecommerce.Models
{
    public class Brand
    {
        public int Id { get; set; }
        [Required]
        [MaxLength(10)]
        [MinLength(3)]
        //[Length(3, 10)]
        public string Name { get; set; } = string.Empty;
        public string Logo { get; set; } = string.Empty;
        public bool Status { get; set; }
    }
}
