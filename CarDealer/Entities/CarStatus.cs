namespace CarDealer.API.Entities
{
    public class CarStatus
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty; // Ready / Maintenance / Shipping / Sold

        public ICollection<Car> Cars { get; set; } = new List<Car>();
    }
}
