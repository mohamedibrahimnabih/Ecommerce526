namespace Ecommerce.Models
{
    public class FavoriteItem
    {
        public int Id { get; set; }

        public int ProductId { get; set; }
        public Product Product { get; set; } = null!;
        public string ApplicationUserId { get; set; } = string.Empty;
        public ApplicationUser ApplicationUser { get; set; } = null!;

        public double PricePerProduct { get; set; }
    }
}
