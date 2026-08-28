namespace CarDealer.API.Common;

public static class Roles
{
    public const string SuperAdmin = "SuperAdmin";
    public const string Owner = "Owner";
    public const string Staff = "Staff";
    public const string Viewer = "Viewer";

    public static readonly string[] All = { SuperAdmin, Owner, Staff, Viewer };

    public static bool IsValid(string? role) => role is not null && All.Contains(role);
}