namespace CarDealer.API.Entities;

public class Permission
{
    public int Id { get; set; }
    public string Code { get; set; } = default!;       // "Cars.Delete"
    public string Name { get; set; } = default!;        // "حذف السيارات"
    public string Category { get; set; } = default!;    // "Cars"

    public ICollection<UserPermission> UserPermissions { get; set; } = new List<UserPermission>();
}