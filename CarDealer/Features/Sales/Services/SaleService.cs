using Microsoft.EntityFrameworkCore;
using CarDealer.API.Features.Cars.Repositories.Interfaces;
using CarDealer.API.Features.Maintenance.Repositories.Interfaces;
using CarDealer.API.Features.Sales.DTOs;
using CarDealer.API.Features.Sales.Entities;
using CarDealer.API.Features.Transactions.Entities;
using CarDealer.API.Shared.Services.Interfaces;
using CarDealer.API.Shared.Data;
using CarDealer.API.Features.Sales.Repositories.Interfaces;

namespace CarDealer.API.Features.Sales.Services
{
    // ─── Sale Service ──────────────────────────────────────────────────────────────

    public class SaleService : ISaleService
    {
        private readonly ISaleRepository _saleRepo;
        private readonly ICarRepository _carRepo;
        private readonly IMaintenanceRepository _maintenanceRepo;
        private readonly AppDbContext _context;
        private readonly ICurrentTenantService _currentTenant;

        public SaleService(ISaleRepository saleRepo, ICarRepository carRepo,
            IMaintenanceRepository maintenanceRepo, AppDbContext context,
            ICurrentTenantService currentTenant)
        {
            _saleRepo = saleRepo;
            _carRepo = carRepo;
            _maintenanceRepo = maintenanceRepo;
            _context = context;
            _currentTenant = currentTenant;
        }

        public async Task<List<SaleListDto>> GetAllAsync() =>
            await _saleRepo.GetAllWithDetailsAsync();

        public async Task<SaleListDto?> GetByIdAsync(int id)
        {
            var sale = await _context.Sales
                .Include(s => s.Car)
                .Include(s => s.Customer)
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.Id == id);

            if (sale is null) return null;

            var repairCost = await _maintenanceRepo.GetTotalCostByCarIdAsync(sale.CarId);
            var profit = sale.SoldPrice - sale.Car.CostPrice - sale.Car.ShippingCost - repairCost;

            return new SaleListDto(
                sale.Id,
                $"{sale.Car.Brand} {sale.Car.Model} {sale.Car.Year}",
                sale.Customer.Name,
                sale.SoldPrice,
                profit,
                sale.SoldDate);
        }

        public async Task<SaleListDto> CreateSaleAsync(CreateSaleDto dto)
        {
            var car = await _carRepo.GetByIdAsync(dto.CarId)
                ?? throw new InvalidOperationException("Car not found");

            var sale = new Sale
            {
                CarId = dto.CarId,
                CustomerId = dto.CustomerId,
                SoldPrice = dto.SoldPrice,
                Notes = dto.Notes,
                SoldDate = DateTime.UtcNow,
                TenantId = _currentTenant.TenantId
            };

            await _saleRepo.AddAsync(sale);

            // Update car status to Sold
            car.StatusId = 4; // Sold
            await _carRepo.UpdateAsync(car);

            //  حساب تكلفة الصيانة
            var repairCost = await _maintenanceRepo.GetTotalCostByCarIdAsync(dto.CarId);

            //  إجمالي التكلفة (مع معالجة null)
            var totalCost = car.CostPrice + car.ShippingCost + repairCost;

            //  الربح أو الخسارة (لأغراض العرض/الـ DTO فقط — مش لتحديد نوع الـ Transaction)
            var profit = dto.SoldPrice - totalCost;

            //  تسجيل العملية: سعر البيع الكامل كـ Income
            //  (التكلفة أصلاً مسجّلة كـ Expense منفصلة عند شراء السيارة وعند دفعات الصيانة،
            // netProfit = Income - Expense يطلع صحيح،
            
            _context.Transactions.Add(new Transaction
            {
                Type = "Income",
                Amount = dto.SoldPrice,
                RelatedEntity = "Sale",
                RelatedId = sale.Id,
                Description = profit >= 0
                    ? $"بيع سيارة: {car.Brand} {car.Model} {car.Year} (ربح: {profit:N0})"
                    : $"بيع سيارة: {car.Brand} {car.Model} {car.Year} (خسارة: {Math.Abs(profit):N0})",
                Date = DateTime.UtcNow,
                TenantId = _currentTenant.TenantId
            });

            await _context.SaveChangesAsync();

            return new SaleListDto(
                sale.Id,
                $"{car.Brand} {car.Model} {car.Year}",
                string.Empty,
                dto.SoldPrice,
                profit,
                sale.SoldDate
            );
        }

    }
}
