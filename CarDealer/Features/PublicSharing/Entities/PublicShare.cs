using CarDealer.API.Entities;
using CarDealer.API.Features.Cars.Entities;

namespace CarDealer.API.Features.PublicSharing.Entities;

public class PublicShare
{
    public int Id { get; set; }
    public int CarId { get; set; }
    public Car Car { get; set; } = default!;
    public string Token { get; set; } = default!;
    public bool IsActive { get; set; } = true;
    public int ViewsCount { get; set; } = 0;
    public DateTime CreatedAt { get; set; }
    public DateTime? ExpiresAt { get; set; }

    public string? ContactAddress { get; set; }

    // ─── Multi-Tenant (Phase 1: schema only) ─────────────────────────────────
    public int? TenantId { get; set; }
    public Tenant? Tenant { get; set; } = null!;

    public ICollection<ShareView> Views { get; set; } = new List<ShareView>();
    public ICollection<ShareContact> Contacts { get; set; } = new List<ShareContact>();
}