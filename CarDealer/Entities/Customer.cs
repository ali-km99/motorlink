namespace CarDealer.API.Entities
{
    public class Customer
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string? Notes { get; set; }
        public bool IsDeleted { get; set; } = false;

        public ICollection<Sale> Sales { get; set; } = new List<Sale>();
    }
}
