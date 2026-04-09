namespace Ecommerce.ViewModels
{
    public class ProductsWithCategoriesVM
    {
        public IEnumerable<Product> Products { get; set; } = [];

        public Dictionary<string, int> CategoryWithTotal { get; set; } = [];
    }
}
