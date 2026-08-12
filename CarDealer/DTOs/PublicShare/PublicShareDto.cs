namespace CarDealer.API.DTOs.PublicShare;

public record ContactEntryDto(string Label, string Value);

public record GenerateShareLinkRequestDto(
    string? ContactAddress,
    List<ContactEntryDto>? Contacts,
    DateTime? ExpiresAt
);
public record ShareLinkAnalyticsDto(
    int ShareId,
    string Token,
    int ViewsCount
);
public record GenerateShareLinkResponseDto(string Url, string Token, DateTime CreatedAt);

public record BatchToggleSharesDto(List<int> Ids, bool IsActive);

public record ShareAnalyticsDto(int TotalViews, List<ViewsOverTimeDto> ViewsOverTime, List<ShareLinkAnalyticsDto> Links);
public record ViewsOverTimeDto(string Date, int Count);

// ─── منتج العميل النهائي ─────────────────────────────
public record PublicCarViewDto(
    string Title,
    List<string> Images,
    decimal SellingPrice,
    string? ExteriorColor,
    string? InteriorColor,
    int? Mileage,
    string? MileageUnit,
    string? BodyType,
    int? NumberOfSeats,
    string? Transmission,
    string? Condition,
    string? FuelType,
    int? EngineSize,
     CarFeaturesGroupedDto Features,
    string? ContactAddress,
    List<ContactEntryDto> Contacts
);