namespace CarDealer.API.Entities
{
    public class Sale
    {
        public int Id { get; set; }
        public int CarId { get; set; }
        public int CustomerId { get; set; }
        public decimal SoldPrice { get; set; }
        public DateTime SoldDate { get; set; } = DateTime.UtcNow;
        public string? Notes { get; set; }

        public Car Car { get; set; } = null!;
        public Customer Customer { get; set; } = null!;
    }
}
