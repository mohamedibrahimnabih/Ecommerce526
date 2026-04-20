namespace Ecommerce.Models
{
    public class UserReview
    {
        public int Id { get; set; }

        public int ProductId { get; set; }
        public Product Product { get; set; } = null!;
        public string ApplicationUserId { get; set; } = string.Empty;
        public ApplicationUser ApplicationUser { get; set; } = null!;

        public string Comment { get; set; } = string.Empty;
        public int Rate { get; set; }
        public string Img { get; set; } = string.Empty;
    }
}
