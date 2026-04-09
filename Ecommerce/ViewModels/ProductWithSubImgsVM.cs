namespace Ecommerce.ViewModels
{
    public class ProductWithSubImgsVM
    {
        public Product Product { get; set; } = null!;
        public IEnumerable<ProductSubImg> ProductSubImgs { get; set; } = [];

        public IEnumerable<Category> Categories { get; set; } = [];
        public IEnumerable<Brand> Brands { get; set; } = [];
    }
}
