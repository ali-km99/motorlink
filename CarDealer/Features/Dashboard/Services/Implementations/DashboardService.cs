using CarDealer.API.Features.Dashboard.DTOs;
using CarDealer.API.Features.Dashboard.Services.Interfaces;
using CarDealer.API.Features.Maintenance.Repositories.Interfaces;
using CarDealer.API.Shared.Data;
using CarDealer.API.Shared.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CarDealer.API.Features.Dashboard.Services.Implementations
{
    public class DashboardService : IDashboardService
    {
        private readonly AppDbContext _context;
        private readonly IMaintenanceRepository _maintenanceRepo;
        private readonly ICurrentTenantService _currentTenant;   // ← جديد

        public DashboardService(AppDbContext context, IMaintenanceRepository maintenanceRepo,
            ICurrentTenantService currentTenant)
        {
            _context = context;
            _maintenanceRepo = maintenanceRepo;
            _currentTenant = currentTenant;
        }

        public async Task<DashboardStatsDto> GetStatsAsync()
        {
            var totalCars = await _context.Cars.CountAsync(c => c.StatusId != 4);
            var availableCars = await _context.Cars.CountAsync(c => c.StatusId == 1);
            var soldCars = await _context.Cars.CountAsync(c => c.StatusId == 4);
            var inMaintenance = await _context.Cars.CountAsync(c => c.StatusId == 2);
            var inShipping = await _context.Cars.CountAsync(c => c.StatusId == 3);

            // ─── Financial Stats ───────────────────────────────────────
            // IgnoreQueryFilters هنا مقصود لتجاوز فلتر IsDeleted على Car (نريد
            // بيانات تاريخية حتى لو السيارة اتحذفت)، لكن لازم نعيد فلتر TenantId يدوياً
            var salesWithCarCosts = await _context.Sales
                .IgnoreQueryFilters()
                .Where(s => s.Car != null
                    && (s.TenantId == _currentTenant.TenantId || _currentTenant.IsPlatformAdmin))
                .Select(s => new
                {
                    s.CarId,
                    s.SoldPrice,
                    s.Car!.CostPrice,
                    s.Car!.ShippingCost
                })
                .ToListAsync();

            var totalRevenue = salesWithCarCosts.Sum(s => s.SoldPrice);
            var totalCostPrice = salesWithCarCosts.Sum(s => s.CostPrice);
            var totalShipping = salesWithCarCosts.Sum(s => s.ShippingCost);

            var soldCarIds = salesWithCarCosts.Select(s => s.CarId).Distinct().ToList();

            var maintenanceCostByCarId = soldCarIds.Count > 0
                ? await _context.Maintenances
                    .Where(m => soldCarIds.Contains(m.CarId))
                    .GroupBy(m => m.CarId)
                    .Select(g => new { CarId = g.Key, Total = g.Sum(m => m.RepairCost) })
                    .ToDictionaryAsync(x => x.CarId, x => x.Total)
                : new Dictionary<int, decimal>();

            var totalRepairsFromMaintenances = maintenanceCostByCarId.Values.Sum();
            var totalProfit = totalRevenue - totalCostPrice - totalShipping - totalRepairsFromMaintenances;

            var totalMaintenanceCost = await _context.Transactions
                .Where(t => t.RelatedEntity == "Maintenance")
                .SumAsync(t => (decimal?)t.Amount) ?? 0;

            // ─── Monthly Sales ───────────────────────────────────────
            var sixMonthsAgo = DateTime.UtcNow.AddMonths(-6);
            var monthlySalesRaw = await _context.Sales
                .Where(s => s.SoldDate >= sixMonthsAgo)
                .GroupBy(s => new { s.SoldDate.Year, s.SoldDate.Month })
                .Select(g => new { g.Key.Year, g.Key.Month, Count = g.Count(), Total = g.Sum(s => s.SoldPrice) })
                .OrderBy(x => x.Year).ThenBy(x => x.Month)
                .ToListAsync();

            var monthlySales = monthlySalesRaw
                .Select(x => new MonthlySalesDto($"{x.Year}-{x.Month:D2}", x.Count, x.Total))
                .ToList();

            // ─── Recent Sales ────────────────────────────────────────
            var recentSalesRaw = await _context.Sales
                .IgnoreQueryFilters()
                .Where(s => s.TenantId == _currentTenant.TenantId || _currentTenant.IsPlatformAdmin)   // ← إعادة الفلتر يدوياً
                .Include(s => s.Car)
                .Include(s => s.Customer)
                .OrderByDescending(s => s.SoldDate)
                .Take(5)
                .Select(s => new
                {
                    s.CarId,
                    CarLabel = s.Car != null ? $"{s.Car.Brand} {s.Car.Model} {s.Car.Year}" : "سيارة محذوفة",
                    CustomerName = s.Customer != null ? s.Customer.Name : "عميل محذوف",
                    s.SoldPrice,
                    CostPrice = s.Car != null ? s.Car.CostPrice : 0,
                    ShippingCost = s.Car != null ? s.Car.ShippingCost : 0,
                    s.SoldDate
                })
                .ToListAsync();

            var recentSales = recentSalesRaw
                .Select(s =>
                {
                    var maintenanceCost = maintenanceCostByCarId.TryGetValue(s.CarId, out var cost) ? cost : 0;
                    var profit = s.SoldPrice - maintenanceCost - s.ShippingCost - s.CostPrice;
                    return new RecentSaleDto(s.CarLabel, s.CustomerName, s.SoldPrice, profit, s.SoldDate);
                })
                .ToList();

            // ─── Maintenance Debts ───────────────────────────────────
            var allMaintenances = await _maintenanceRepo.GetForDebtReportAsync(
                centerId: null, carId: null, status: null, dateFrom: null, dateTo: null);

            var maintenanceDebtItems = allMaintenances
                .Select(m =>
                {
                    var paid = m.Payments?.Sum(p => p.Amount) ?? 0;
                    var remaining = m.RepairCost - paid;
                    return new
                    {
                        m.Id,
                        CarLabel = m.Car is null ? "سيارة محذوفة" : $"{m.Car.Brand} {m.Car.Model} {m.Car.Year}",
                        CenterName = m.MaintenanceCenter?.Name ?? string.Empty,
                        m.RepairCost,
                        Remaining = remaining,
                        m.CreatedAt
                    };
                })
                .ToList();

            var totalMaintenanceDebt = maintenanceDebtItems.Sum(m => m.Remaining);

            var topMaintenanceDebts = maintenanceDebtItems
                .Where(m => m.Remaining > 0)
                .OrderByDescending(m => m.Remaining)
                .Take(5)
                .Select(m => new TopMaintenanceDebtDto(m.Id, m.CarLabel, m.CenterName, m.RepairCost, m.Remaining, m.CreatedAt))
                .ToList();

            return new DashboardStatsDto(
                totalCars, availableCars, soldCars, inMaintenance, inShipping,
                totalRevenue, totalProfit, totalMaintenanceCost,
                monthlySales, recentSales, totalMaintenanceDebt, topMaintenanceDebts);
        }
    }
}