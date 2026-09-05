using CarDealer.API.Shared.Entities;

namespace CarDealer.API.Features.PublicSharing.Entities;

public class ShareView
{
    public int Id { get; set; }
    public int ShareId { get; set; }
    public PublicShare Share { get; set; } = default!;
    public string? IpAddress { get; set; }
    public string? UserAgent { get; set; }
    public DateTime ViewedAt { get; set; }

    // ─── Multi-Tenant (Phase 1: schema only) ─────────────────────────────────
    public int? TenantId { get; set; }
    public Tenant? Tenant { get; set; } = null!;
}