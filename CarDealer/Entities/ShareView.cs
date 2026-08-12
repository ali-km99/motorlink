namespace CarDealer.API.Entities;

public class ShareView
{
    public int Id { get; set; }
    public int ShareId { get; set; }
    public PublicShare Share { get; set; } = default!;
    public string? IpAddress { get; set; }
    public string? UserAgent { get; set; }
    public DateTime ViewedAt { get; set; }
}