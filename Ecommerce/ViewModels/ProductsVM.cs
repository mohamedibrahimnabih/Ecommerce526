namespace Ecommerce.ViewModels
{
    public class ProductsVM
    {
        public IEnumerable<Product> Products { get; set; } = null!;

        public IEnumerable<Category> Categories { get; set; } = null!;
        public IEnumerable<Brand> Brands { get; set; } = null!;

        public double TotalPages { get; set; }
        public int CurrentPage { get; set; }
    }
}
