namespace CarDealer.API.Entities;

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

    public ICollection<ShareView> Views { get; set; } = new List<ShareView>();
    public ICollection<ShareContact> Contacts { get; set; } = new List<ShareContact>();
}