namespace Ecommerce.Models
{
    public enum PaymentMethod 
    { 
        Visa,
        Cash
    }

    public enum OrderStatus
    {
        Pending,
        InProcessing,
        Shipped,
        OnTheWay,
        Completed,
        Canceled
    }

    public enum PaymentStatus
    {
        Pending,
        Successed,
        COD,
        Refunded
    }

    public class Order
    {
        public int Id { get; set; }
        public DateTime CreateAt { get; set; } = DateTime.Now;

        public PaymentMethod PaymentMethod { get; set; } = PaymentMethod.Visa;
        public PaymentStatus PaymentStatus { get; set; } = PaymentStatus.Pending;


        public string SessionId { get; set; } = string.Empty;
        public string? TransactionId { get; set; }

        public OrderStatus OrderStatus { get; set; } = OrderStatus.Pending;

        public string? Tracker { get; set; }
        public string? TrackingNumber { get; set; }
        public DateTime? ShippedDate { get; set; }

        public double TotalPrice { get; set; }

        public string ApplicationUserId { get; set; }
        public ApplicationUser ApplicationUser { get; set; }
    }
}
