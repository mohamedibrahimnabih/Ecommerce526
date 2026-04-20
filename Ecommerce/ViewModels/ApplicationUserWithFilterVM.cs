namespace Ecommerce.ViewModels
{
    public class ApplicationUserWithFilterVM
    {
        public Dictionary<ApplicationUser, string> UserRoles { get; set; } = null!;

        public double TotalPages { get; set; }
        public int CurrentPage { get; set; }
    }
}
