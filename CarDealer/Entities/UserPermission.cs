namespace CarDealer.API.Entities;

public class UserPermission
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public AppUser User { get; set; } = default!;
    public int PermissionId { get; set; }
    public Permission Permission { get; set; } = default!;

    // ─── Multi-Tenant (Phase 1: schema only) ─────────────────────────────────
    public int? TenantId { get; set; }
    public Tenant? Tenant { get; set; } = null!;
}