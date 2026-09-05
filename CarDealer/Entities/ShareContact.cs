using CarDealer.API.Features.PublicSharing.Entities;

namespace CarDealer.API.Entities;

public class ShareContact
{
    public int Id { get; set; }
    public int ShareId { get; set; }
    public PublicShare Share { get; set; } = default!;

    public string Label { get; set; } = default!;   // "هاتف", "واتساب", "فرع طرابلس"...
    public string Value { get; set; } = default!;    // الرقم نفسه
    public int DisplayOrder { get; set; } = 0;        // ترتيب العرض بالواجهة

    // ─── Multi-Tenant (Phase 1: schema only) ─────────────────────────────────
    public int? TenantId { get; set; }
    public Tenant? Tenant { get; set; } = null!;
}