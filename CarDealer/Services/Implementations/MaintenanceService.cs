using CarDealer.API.Common;
using CarDealer.API.Data;
using CarDealer.API.DTOs;
using CarDealer.API.Entities;
using CarDealer.API.Repositories.Interfaces;
using CarDealer.API.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace CarDealer.API.Services.Implementations;

public class MaintenanceService : IMaintenanceService
{
    private readonly IMaintenanceRepository _repo;
    private readonly IMaintenanceCenterRepository _centerRepo;
    private readonly IMaintenancePaymentRepository _paymentRepo;
    private readonly ICarRepository _carRepo;
    private readonly AppDbContext _context;

    public MaintenanceService(
        IMaintenanceRepository repo,
        IMaintenanceCenterRepository centerRepo,
        IMaintenancePaymentRepository paymentRepo,
        ICarRepository carRepo,
        AppDbContext context)
    {
        _repo = repo;
        _centerRepo = centerRepo;
        _paymentRepo = paymentRepo;
        _carRepo = carRepo;
        _context = context;
    }

    public async Task<List<MaintenanceDto>> GetByCarIdAsync(int carId)
    {
        var items = await _repo.GetByCarIdAsync(carId);
        return items.Select(MaintenanceMapping.ToDto).ToList();
    }

    public async Task<MaintenanceDto?> GetByIdAsync(int id)
    {
        var maintenance = await _repo.GetByIdWithDetailsAsync(id);
        return maintenance is null ? null : MaintenanceMapping.ToDto(maintenance);
    }

    public async Task<MaintenanceDto> CreateAsync(CreateMaintenanceDto dto)
    {
        var car = await _carRepo.GetByIdAsync(dto.CarId)
            ?? throw new InvalidOperationException("السيارة غير موجودة");

        var center = await _centerRepo.GetByIdAsync(dto.MaintenanceCenterId)
            ?? throw new InvalidOperationException("مركز الصيانة غير موجود");

        if (dto.RepairCost <= 0)
            throw new InvalidOperationException("قيمة الصيانة يجب أن تكون أكبر من صفر");

        var initialPaid = dto.InitialPaidAmount ?? 0;
        if (initialPaid < 0)
            throw new InvalidOperationException("المبلغ المدفوع لا يمكن أن يكون سالباً");
        if (initialPaid > dto.RepairCost)
            throw new InvalidOperationException("المبلغ المدفوع لا يمكن أن يتجاوز قيمة الصيانة");

        var maintenance = new Maintenance
        {
            CarId = car.Id,
            MaintenanceCenterId = center.Id,
            IssueDescription = dto.IssueDescription,
            RepairCost = dto.RepairCost
        };

        var strategy = _context.Database.CreateExecutionStrategy();
        await strategy.ExecuteAsync(async () =>
        {
            await using var dbTransaction = await _context.Database.BeginTransactionAsync();

            await _repo.AddAsync(maintenance);
            await _repo.SaveChangesAsync();

            if (initialPaid > 0)
            {
                var payment = new MaintenancePayment
                {
                    MaintenanceId = maintenance.Id,
                    Amount = initialPaid,
                    PaymentDate = DateTime.UtcNow,
                    Notes = dto.PaymentNotes,
                    CreatedAt = DateTime.UtcNow
                };

                await _paymentRepo.AddAsync(payment);
                _context.Transactions.Add(MaintenanceMapping.ToPaymentTransaction(maintenance, payment));
                await _context.SaveChangesAsync();

                maintenance.Payments.Add(payment);
            }

            await dbTransaction.CommitAsync();
        });

        maintenance.MaintenanceCenter = center;
        return MaintenanceMapping.ToDto(maintenance);
    }

    public async Task<MaintenanceDto?> UpdateAsync(int id, UpdateMaintenanceDto dto)
    {
        var maintenance = await _repo.GetTrackedWithPaymentsAsync(id);
        if (maintenance is null)
            return null;

        if (dto.RepairCost <= 0)
            throw new InvalidOperationException("قيمة الصيانة يجب أن تكون أكبر من صفر");

        var totalPaid = MaintenanceMapping.TotalPaid(maintenance.Payments);
        if (dto.RepairCost < totalPaid)
            throw new InvalidOperationException(
                $"لا يمكن تعديل قيمة الصيانة إلى أقل من مجموع الدفعات السابقة ({totalPaid})");

        if (dto.MaintenanceCenterId.HasValue
            && dto.MaintenanceCenterId.Value != maintenance.MaintenanceCenterId)
        {
            var center = await _centerRepo.GetByIdAsync(dto.MaintenanceCenterId.Value)
                ?? throw new InvalidOperationException("مركز الصيانة غير موجود");

            maintenance.MaintenanceCenterId = center.Id;
            maintenance.MaintenanceCenter = center;
        }

        maintenance.IssueDescription = dto.IssueDescription;
        maintenance.RepairCost = dto.RepairCost;

        await _repo.SaveChangesAsync();
        return MaintenanceMapping.ToDto(maintenance);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var maintenance = await _repo.GetTrackedWithPaymentsAsync(id);
        if (maintenance is null)
            return false;

        if (maintenance.Payments.Count > 0)
            throw new InvalidOperationException(
                "لا يمكن حذف عملية صيانة تحتوي على دفعات مسجلة");

        var strategy = _context.Database.CreateExecutionStrategy();
        await strategy.ExecuteAsync(async () =>
        {
            await using var dbTransaction = await _context.Database.BeginTransactionAsync();

            await _context.Transactions
                .Where(t => t.RelatedEntity == "Maintenance" && t.RelatedId == id)
                .ExecuteUpdateAsync(t => t.SetProperty(x => x.IsDeleted, true));

            await _repo.DeleteAsync(maintenance);
            await _repo.SaveChangesAsync();
            await dbTransaction.CommitAsync();
        });

        return true;
    }

    public async Task<List<MaintenancePaymentDto>> GetPaymentsAsync(int maintenanceId)
    {
        var exists = await _repo.GetByIdAsync(maintenanceId);
        if (exists is null)
            throw new KeyNotFoundException("الصيانة غير موجودة");

        var payments = await _paymentRepo.GetByMaintenanceIdAsync(maintenanceId);
        return payments.Select(MaintenanceMapping.ToPaymentDto).ToList();
    }

    public async Task<MaintenancePaymentDto> AddPaymentAsync(int maintenanceId, CreateMaintenancePaymentDto dto)
    {
        if (dto.Amount <= 0)
            throw new InvalidOperationException("مبلغ الدفعة يجب أن يكون أكبر من صفر");

        MaintenancePayment? created = null;

        var strategy = _context.Database.CreateExecutionStrategy();
        await strategy.ExecuteAsync(async () =>
        {
            await using var dbTransaction = await _context.Database.BeginTransactionAsync(IsolationLevel.Serializable);

            var maintenance = await _repo.GetTrackedWithPaymentsAsync(maintenanceId)
                ?? throw new KeyNotFoundException("الصيانة غير موجودة");

            var totalPaid = MaintenanceMapping.TotalPaid(maintenance.Payments);
            var remaining = MaintenanceMapping.Remaining(maintenance.RepairCost, totalPaid);

            if (dto.Amount > remaining)
                throw new InvalidOperationException(
                    $"مبلغ الدفعة يتجاوز المبلغ المتبقي ({remaining})");

            var payment = new MaintenancePayment
            {
                MaintenanceId = maintenance.Id,
                Amount = dto.Amount,
                PaymentDate = dto.PaymentDate ?? DateTime.UtcNow,
                Notes = dto.Notes,
                CreatedAt = DateTime.UtcNow
            };

            await _paymentRepo.AddAsync(payment);
            _context.Transactions.Add(MaintenanceMapping.ToPaymentTransaction(maintenance, payment));
            await _context.SaveChangesAsync();
            await dbTransaction.CommitAsync();

            created = payment;
        });

        return MaintenanceMapping.ToPaymentDto(created!);
    }

    public async Task<MaintenanceCenterDebtDto?> GetCenterDebtsAsync(int centerId)
    {
        var center = await _centerRepo.GetByIdAsync(centerId);
        if (center is null)
            return null;

        var maintenances = await _repo.GetByCenterIdWithDetailsAsync(centerId);
        return BuildCenterDebt(center.Id, center.Name, maintenances);
    }

    public async Task<MaintenanceDebtReportDto> GetDebtsAsync(MaintenanceDebtFilterDto filter)
    {
        if (filter.Status is not null && !MaintenancePaymentStatuses.IsValid(filter.Status))
            MaintenancePaymentStatuses.Normalize(filter.Status);

        var items = await _repo.GetForDebtReportAsync(
            filter.CenterId, filter.CarId, filter.Status, filter.DateFrom, filter.DateTo);

        var debtItems = items.Select(MaintenanceMapping.ToDebtItem).ToList();

        return new MaintenanceDebtReportDto(
            debtItems.Sum(i => i.RepairCost),
            debtItems.Sum(i => i.TotalPaid),
            debtItems.Sum(i => i.RemainingAmount),
            debtItems.Count(i => i.PaymentStatus == MaintenancePaymentStatuses.Unpaid),
            debtItems.Count(i => i.PaymentStatus == MaintenancePaymentStatuses.PartiallyPaid),
            debtItems.Count(i => i.PaymentStatus == MaintenancePaymentStatuses.Paid),
            debtItems);
    }

    private static MaintenanceCenterDebtDto BuildCenterDebt(
        int centerId, string centerName, List<Maintenance> maintenances)
    {
        var items = maintenances.Select(MaintenanceMapping.ToDebtItem).ToList();

        var cars = items
            .Where(i => i.RemainingAmount > 0)
            .GroupBy(i => new { i.CarId, i.CarLabel })
            .Select(g => new MaintenanceDebtCarDto(
                g.Key.CarId,
                g.Key.CarLabel,
                g.Sum(x => x.RemainingAmount)))
            .OrderByDescending(c => c.Debt)
            .ToList();

        return new MaintenanceCenterDebtDto(
            centerId,
            centerName,
            items.Sum(i => i.RepairCost),
            items.Sum(i => i.TotalPaid),
            items.Sum(i => i.RemainingAmount),
            items.Count(i => i.PaymentStatus == MaintenancePaymentStatuses.Unpaid),
            items.Count(i => i.PaymentStatus == MaintenancePaymentStatuses.PartiallyPaid),
            items.Count(i => i.PaymentStatus == MaintenancePaymentStatuses.Paid),
            cars);
    }
}
