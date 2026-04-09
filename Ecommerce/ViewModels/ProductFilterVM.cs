namespace Ecommerce.ViewModels
{
    public record ProductFilterVM(string Name, double? MinPrice, double? MaxPrice, int? CategoryId, int? BrandId);
}
