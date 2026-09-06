namespace CarDealer.API.Features.Marketplace.Entities
{
    /// <summary>
    /// سجل زيارة لمنصة Marketplace — حالياً يُسجَّل عند كل تسجيل دخول لـ MarketplaceUser.
    /// لاحقاً (Phase 7) يمكن تمديده ليشمل تصفح مجهول بدون حساب.
    /// </summary>
    public class MarketplaceVisit
    {
        public int Id { get; set; }
        public int? MarketplaceUserId { get; set; }
        public MarketplaceUser? MarketplaceUser { get; set; }
        public DateTime VisitedAt { get; set; } = DateTime.UtcNow;
        public string? IpAddress { get; set; }
    }
}