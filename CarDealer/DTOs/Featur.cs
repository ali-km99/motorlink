namespace CarDealer.API.DTOs
{
    // ─── Feature DTOs ──────────────────────────────────────────────────────────────

    public record FeatureDto(
        int Id,
        string Name,
        string Category,    // Technology / Interior / Exterior
        int UsageCount
    );

    // Features مجمّعة حسب الـ Category — للعرض المنسّق في الـ Frontend
    public record CarFeaturesGroupedDto(
        List<string> Technology,
        List<string> Interior,
        List<string> Exterior
    );

    public record CreateFeatureDto(
        string Name,
        string Category     // Technology / Interior / Exterior
    );

    public record UpdateFeatureDto(
        string Name,
        string Category
    );
}
