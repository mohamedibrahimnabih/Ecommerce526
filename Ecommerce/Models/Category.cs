using System.ComponentModel.DataAnnotations;

namespace Ecommerce.Models
{
    public class Category
    {
        public int Id { get; set; }
        [Required]
        //[MaxLength(10)]
        //[MinLength(3)]
        [CustomLength(3, 20)]
        public string Name { get; set; } = string.Empty;
        public bool Status { get; set; }
    }
}
