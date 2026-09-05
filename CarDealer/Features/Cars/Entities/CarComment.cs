using CarDealer.API.Features.Users.Entities;
using CarDealer.API.Shared.Entities;

namespace CarDealer.API.Features.Cars.Entities
{
    /// <summary>
    /// Comment on a car listing.
    /// Can be internal (staff-only) or public (visible via PublicShare).
    /// </summary>
    public class CarComment
    {
        public int Id { get; set; }
        public int CarId { get; set; }
        public Car Car { get; set; } = null!;

        // ─── Tenant scoping (Phase 1: schema only, no query filter yet) ────
        public int? TenantId { get; set; }
        public Tenant? Tenant { get; set; } = null!;

        public int? AuthorUserId { get; set; }                 // nullable to allow anonymous comments later
        public AppUser? AuthorUser { get; set; } = null!;

        public string Body { get; set; } = string.Empty;
        public bool IsInternal { get; set; } = false;          // true = staff-only, false = public
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public bool IsDeleted { get; set; } = false;
    }
}
