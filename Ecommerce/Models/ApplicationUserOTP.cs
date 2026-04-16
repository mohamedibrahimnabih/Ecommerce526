namespace Ecommerce.Models
{
    public class ApplicationUserOTP
    {
        public int Id { get; set; }
        public string OTP { get; set; } = string.Empty;
        public DateTime CreateAt { get; set; } = DateTime.UtcNow;
        public DateTime ExpireIn { get; set; } = DateTime.UtcNow.AddMinutes(30);
        public bool IsUsed { get; set; } = false;

        public string ApplicationUserId { get; set; }
        public ApplicationUser ApplicationUser { get; set; }

        public bool IsValid => (ExpireIn - CreateAt).TotalMinutes > 0 && !IsUsed;
    }
}
