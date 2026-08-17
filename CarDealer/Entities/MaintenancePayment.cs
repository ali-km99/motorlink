namespace CarDealer.API.Entities
{
    public class MaintenancePayment
    {
        public int Id { get; set; }
        public int MaintenanceId { get; set; }
        public decimal Amount { get; set; }
        public DateTime PaymentDate { get; set; } = DateTime.UtcNow;
        public string? Notes { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public Maintenance Maintenance { get; set; } = null!;
    }
}
