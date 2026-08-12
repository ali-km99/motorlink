namespace CarDealer.API.Entities
{
    // الصيانة
    public class Maintenance
    {
        public int Id { get; set; }
        public int CarId { get; set; }
        public string IssueDescription { get; set; } = string.Empty;
        public decimal RepairCost { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public Car Car { get; set; } = null!;
    }
}
