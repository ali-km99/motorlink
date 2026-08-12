namespace CarDealer.API.Entities
{
    public class Car
    {
        public int Id { get; set; }
        public string Brand { get; set; } = string.Empty;
        public string Model { get; set; } = string.Empty;
        public int Year { get; set; }

        // ─── الألوان ───────────────────────────────────────────────────────────────
        public string ExteriorColor { get; set; } = string.Empty;
        public string? InteriorColor { get; set; }

        // ─── الأسعار ───────────────────────────────────────────────────────────────
        public decimal CostPrice { get; set; }
        public decimal ShippingCost { get; set; }
        public decimal SellingPrice { get; set; }

        // ─── المواصفات التقنية ─────────────────────────────────────────────────────
        public string? VinNumber { get; set; }                  // رقم الهيكل
        public int? Mileage { get; set; }                       // عداد الكيلومترات
        public string? MileageUnit { get; set; }                // KM / MI
        public string? BodyType { get; set; }                   // SUV / Sedan / Hatchback ...
        public int? NumberOfSeats { get; set; }
        public string? Transmission { get; set; }               // Automatic / Manual / CVT
        public string? Condition { get; set; }                  // New / Used / Like New
        public string? FuelType { get; set; }                   // Petrol / Diesel / Hybrid / Electric
        public string? Specs { get; set; }                      // Korean / USA / Gulf / European
        public int? EngineSize { get; set; }                    // cc — مثلاً 2000

        // ─── حالة السيارة والوثائق ─────────────────────────────────────────────────
        public string? BodyCondition { get; set; }              // Excellent / Good / Fair / Poor
        public bool HasLicense { get; set; } = false;
        public bool HasInsurance { get; set; } = false;
        public bool HasCustomsClearance { get; set; } = false;

        // ─── طريقة الدفع ──────────────────────────────────────────────────────────
        public string? PaymentMethod { get; set; }              // Cash / Installment / Both

        // ─── متفرقات ──────────────────────────────────────────────────────────────
        public int StatusId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public string? Notes { get; set; }
        public bool IsDeleted { get; set; } = false;

        // ─── Navigation Properties ─────────────────────────────────────────────────
        public CarStatus Status { get; set; } = null!;
        public ICollection<CarImage> Images { get; set; } = new List<CarImage>();
        public ICollection<CarFeature> CarFeatures { get; set; } = new List<CarFeature>();
        public ICollection<Maintenance> Maintenances { get; set; } = new List<Maintenance>();
        public Sale? Sale { get; set; }
    }

}
