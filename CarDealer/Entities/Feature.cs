namespace CarDealer.API.Entities
{
    public class Feature
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public ICollection<CarFeature> CarFeatures { get; set; } = new List<CarFeature>();
    }
}
