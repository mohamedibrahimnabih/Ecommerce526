namespace Ecommerce.Models
{
    public class Promotion
    {
        public int Id { get; set; }
        public double Discount { get; set; }
        public string Code { get; set; } = string.Empty;
        public int Usage { get; set; } = 100;
        public DateTime ValidTo { get; set; } = DateTime.Now.AddDays(7);

        public int? ProductId { get; set; }
        public Product? Product { get; set; }
    }
}
