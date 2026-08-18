namespace CarDealer.API.Entities
{
    public class MaintenanceCenter
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Notes { get; set; }

        public ICollection<Maintenance> Maintenances { get; set; } = new List<Maintenance>();
        public ICollection<MaintenanceCenterPhone> Phones { get; set; } = new List<MaintenanceCenterPhone>();
    }
}