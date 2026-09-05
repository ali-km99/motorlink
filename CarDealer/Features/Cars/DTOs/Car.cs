using CarDealer.API.Features.Maintenance.DTOs;
using CarDealer.API.Features.Sales.DTOs;

namespace CarDealer.API.Features.Cars.DTOs
{
    // ─── Car DTOs ──────────────────────────────────────────────────────────────────

    public record CarListDto(
        int Id,
        string Brand,
        string Model,
        int Year,
        string ExteriorColor,
        decimal SellingPrice,
        string StatusName,
        string? PrimaryImageUrl,
        string? Condition,
        string? BodyType,
        int? Mileage,
        string? MileageUnit,
        DateTime CreatedAt
    );

    public record CarDetailDto(
        int Id,
        string Brand,
        string Model,
        int Year,

        // الألوان
        string ExteriorColor,
        string? InteriorColor,

        // الأسعار
        decimal CostPrice,
        decimal ShippingCost,
        decimal SellingPrice,
        decimal Profit,
        decimal TotalRepairCost,

        // المواصفات التقنية
        string? VinNumber,
        int? Mileage,
        string? MileageUnit,
        string? BodyType,
        int? NumberOfSeats,
        string? Transmission,
        string? Condition,
        string? FuelType,
        string? Specs,
        int? EngineSize,

        // الحالة والوثائق
        string? BodyCondition,
        bool HasLicense,
        bool HasInsurance,
        bool HasCustomsClearance,
        string? PaymentMethod,

        // الحالة والتصنيف
        int StatusId,
        string StatusName,
        string? Notes,
        DateTime CreatedAt,

        // البيانات المرتبطة
        List<CarImageDto> Images,
        CarFeaturesGroupedDto Features,   // مجمّعة حسب الـ Category
        List<MaintenanceDto> Maintenances,
        SaleInfoDto? Sale
    );

    public record CreateCarDto(
     string Brand,
     string Model,
     int Year,
     string ExteriorColor,
     string? InteriorColor,
     decimal CostPrice,
     decimal ShippingCost,
     decimal SellingPrice,
     int StatusId,
     string? Notes,

     // المواصفات التقنية
     string? VinNumber,
     int? Mileage,
     string? MileageUnit,
     string? BodyType,
     int? NumberOfSeats,
     string? Transmission,
     string? Condition,
     string? FuelType,
     string? Specs,
     int? EngineSize,

     // الحالة والوثائق
     string? BodyCondition,
     bool HasLicense,
     bool HasInsurance,
     bool HasCustomsClearance,
     string? PaymentMethod,

     List<int>? FeatureIds
 );

    public record UpdateCarDto(
     string Brand,
     string Model,
     int Year,
     string ExteriorColor,
     string? InteriorColor,
     decimal CostPrice,
     decimal ShippingCost,
     decimal SellingPrice,
     int StatusId,
     string? Notes,

     string? VinNumber,
     int? Mileage,
     string? MileageUnit,
     string? BodyType,
     int? NumberOfSeats,
     string? Transmission,
     string? Condition,
     string? FuelType,
     string? Specs,
     int? EngineSize,

     string? BodyCondition,
     bool HasLicense,
     bool HasInsurance,
     bool HasCustomsClearance,
     string? PaymentMethod,

     List<int>? FeatureIds
 );

    public record CarImageDto(
        int Id,
        string ImageUrl,
        bool IsPrimary
    );
  

 


    public record CarFilterDto(
    string? Brand,
    int? YearFrom,
    int? YearTo,
    decimal? PriceFrom,
    decimal? PriceTo,
    int? StatusId,
    string? BodyType,
    string? Transmission,
    string? Condition,
    string? FuelType,
    string? Specs,
    string? SearchTerm,
    int Page = 1,
    int PageSize = 12
);
}
