namespace CarDealer.API.Entities
{
    public class MaintenanceCenterPhone
    {
        public int Id { get; set; }
        public int MaintenanceCenterId { get; set; }
        public MaintenanceCenter MaintenanceCenter { get; set; } = null!;

        public string Label { get; set; } = string.Empty;        // مثال: "المدير", "الاستقبال", "واتساب"
        public string PhoneNumber { get; set; } = string.Empty;
        public int DisplayOrder { get; set; } = 0;
    }
}