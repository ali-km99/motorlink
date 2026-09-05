using CarDealer.API.Entities;
using CarDealer.API.Features.Cars.Entities;

namespace CarDealer.API.Features.Users.Entities
{
    public class AppUser
    {
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public string Role { get; set; } = "Admin";          // Admin / Viewer
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? LastLoginAt { get; set; }

        // Refresh Token
        public string? RefreshToken { get; set; }
        public DateTime? RefreshTokenExpiry { get; set; }

        // ─── Multi-Tenant (Phase 1: schema only) ─────────────────────────────────
        // NULL for Platform Admins (cross-tenant access).
        public int? TenantId { get; set; }
        public Tenant? Tenant { get; set; } = null!;

        // When true, the user bypasses tenant scoping and can manage all tenants.
        public bool IsPlatformAdmin { get; set; } = false;

        public ICollection<UserPermission> Permissions { get; set; } = new List<UserPermission>();
        public ICollection<CarComment> Comments { get; set; } = new List<CarComment>();
    }
}
