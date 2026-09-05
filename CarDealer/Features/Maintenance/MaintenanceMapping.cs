using CarDealer.API.Common;
using CarDealer.API.Features.Cars.Entities;
using CarDealer.API.Features.Maintenance.DTOs;
using CarDealer.API.Features.Maintenance.Entities;
using CarDealer.API.Features.Transactions.Entities;

namespace CarDealer.API.Features.Maintenance;

internal static class MaintenanceMapping
{
    public static decimal TotalPaid(IEnumerable<MaintenancePayment> payments) =>
        payments.Sum(p => p.Amount);

    public static decimal Remaining(decimal repairCost, decimal totalPaid) =>
        repairCost - totalPaid;

    public static string GetStatus(decimal repairCost, decimal totalPaid)
    {
        if (totalPaid <= 0m)
            return MaintenancePaymentStatuses.Unpaid;
        if (totalPaid >= repairCost)
            return MaintenancePaymentStatuses.Paid;
        return MaintenancePaymentStatuses.PartiallyPaid;
    }

    public static string CarLabel(Car? car) =>
        car is null ? "سيارة محذوفة" : $"{car.Brand} {car.Model} {car.Year}";

    public static MaintenancePaymentDto ToPaymentDto(MaintenancePayment payment) =>
        new(
            payment.Id,
            payment.MaintenanceId,
            payment.Amount,
            payment.PaymentDate,
            payment.Notes,
            payment.CreatedAt);

    public static MaintenanceDto ToDto(MaintenanceEntity maintenance)
    {
        var payments = maintenance.Payments ?? new List<MaintenancePayment>();
        var totalPaid = TotalPaid(payments);
        var remaining = Remaining(maintenance.RepairCost, totalPaid);

        return new MaintenanceDto(
            maintenance.Id,
            maintenance.CarId,
            maintenance.MaintenanceCenterId,
            maintenance.MaintenanceCenter?.Name ?? string.Empty,
            maintenance.IssueDescription,
            maintenance.RepairCost,
            totalPaid,
            remaining,
            GetStatus(maintenance.RepairCost, totalPaid),
            maintenance.CreatedAt,
            payments
                .OrderBy(p => p.PaymentDate)
                .ThenBy(p => p.Id)
                .Select(ToPaymentDto)
                .ToList());
    }

    public static MaintenanceDebtItemDto ToDebtItem(MaintenanceEntity maintenance)
    {
        var totalPaid = TotalPaid(maintenance.Payments ?? Array.Empty<MaintenancePayment>());
        var remaining = Remaining(maintenance.RepairCost, totalPaid);

        return new MaintenanceDebtItemDto(
            maintenance.Id,
            maintenance.CarId,
            CarLabel(maintenance.Car),
            maintenance.MaintenanceCenterId,
            maintenance.MaintenanceCenter?.Name ?? string.Empty,
            maintenance.IssueDescription,
            maintenance.RepairCost,
            totalPaid,
            remaining,
            GetStatus(maintenance.RepairCost, totalPaid),
            maintenance.CreatedAt);
    }

    public static Transaction ToPaymentTransaction(MaintenanceEntity maintenance, MaintenancePayment payment) =>
        new()
        {
            Type = "Expense",
            Amount = payment.Amount,
            RelatedEntity = "Maintenance",
            RelatedId = maintenance.Id,
            Description = string.IsNullOrWhiteSpace(payment.Notes)
                ? $"Maintenance payment: {maintenance.IssueDescription}"
                : $"Maintenance payment: {payment.Notes}",
            Date = payment.PaymentDate
        };
}
