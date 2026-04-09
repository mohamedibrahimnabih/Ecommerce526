namespace Ecommerce.ViewModels
{
    public class BrandsVM
    {
        public IEnumerable<Brand> Brands { get; set; } = null!;

        public double TotalPages { get; set; }
        public int CurrentPage { get; set; }
    }
}
