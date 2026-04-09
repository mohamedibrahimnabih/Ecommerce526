namespace Ecommerce.ViewModels
{
    public class CategoriesVM
    {
        public IEnumerable<Category> Categories { get; set; } = null!;

        public double TotalPages { get; set; }
        public int CurrentPage { get; set; }
    }
}
