namespace Ecommerce.ViewModels
{
    public class PaymentSuccessViewModel
    {
        public int OrderId { get; set; } 
        public DateTime OrderDate { get; set; }
        public double TotalPrice { get; set; }
        public string PaymentMethod { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
    }
}
