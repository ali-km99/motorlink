namespace CarDealer.API.Entities
{
    public class CarImage
    {
        public int Id { get; set; }
        public int CarId { get; set; }
        public string ImageUrl { get; set; } = string.Empty;
        public bool IsPrimary { get; set; } = false;

        public Car Car { get; set; } = null!;
    }
}
