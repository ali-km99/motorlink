namespace CarDealer.API.Common;

public static class MaintenancePaymentStatuses
{
    public const string Unpaid = "Unpaid";
    public const string PartiallyPaid = "PartiallyPaid";
    public const string Paid = "Paid";

    public static bool IsValid(string? status) =>
        status is Unpaid or PartiallyPaid or Paid
        || (status is not null && (
            status.Equals(Unpaid, StringComparison.OrdinalIgnoreCase)
            || status.Equals(PartiallyPaid, StringComparison.OrdinalIgnoreCase)
            || status.Equals(Paid, StringComparison.OrdinalIgnoreCase)));

    public static string Normalize(string status)
    {
        if (status.Equals(Unpaid, StringComparison.OrdinalIgnoreCase))
            return Unpaid;
        if (status.Equals(PartiallyPaid, StringComparison.OrdinalIgnoreCase))
            return PartiallyPaid;
        if (status.Equals(Paid, StringComparison.OrdinalIgnoreCase))
            return Paid;

        throw new InvalidOperationException(
            "حالة الدفع غير صالحة. القيم المسموحة: Unpaid, PartiallyPaid, Paid");
    }
}
