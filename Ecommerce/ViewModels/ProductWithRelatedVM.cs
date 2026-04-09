namespace Ecommerce.ViewModels
{
    public class ProductWithRelatedVM
    {
        public Product Product { get; set; } = null!;

        public IEnumerable<Product> SameCategory { get; set; } = [];
        public IEnumerable<Product> SameName { get; set; } = [];
        public IEnumerable<Product> TopProducts { get; set; } = [];
    }
}
