namespace Ecommerce.Models
{
    public class Cart
    {
        public int Id { get; set; }

        public int ProductId { get; set; }
        public Product Product { get; set; } = null!;
        public string ApplicationUserId { get; set; } = string.Empty;
        public ApplicationUser ApplicationUser { get; set; } = null!;

        public int Count { get; set; }
        public double PricePerProduct { get; set; }
        public double TotalPrice { get; set; } 
    }
}
