using System.ComponentModel.DataAnnotations;

namespace Ecommerce.Models
{
    public class Product
    {
        public int Id { get; set; }
        public string MainImg { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }

        public double Price { get; set; }
        public double Discount { get; set; }

        public double Rate { get; set; }
        public bool Status { get; set; }

        public int Traffic { get; set; }

        public int CategoryId { get; set; }
        public Category Category { get; set; } = null!;

        public int BrandId { get; set; }
        public Brand Brand { get; set; } = null!;
    }
}
