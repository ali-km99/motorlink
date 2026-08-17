using CarDealer.API.Data;
using CarDealer.API.DTOs;
using CarDealer.API.Entities;
using CarDealer.API.Repositories.Interfaces;
using CarDealer.API.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CarDealer.API.Services.Implementations
{
    // ─── Sale Service ──────────────────────────────────────────────────────────────

    public class SaleService : ISaleService
    {
        private readonly ISaleRepository _saleRepo;
        private readonly ICarRepository _carRepo;
        private readonly IMaintenanceRepository _maintenanceRepo;
        private readonly AppDbContext _context;

        public SaleService(ISaleRepository saleRepo, ICarRepository carRepo,
            IMaintenanceRepository maintenanceRepo, AppDbContext context)
        {
            _saleRepo = saleRepo;
            _carRepo = carRepo;
            _maintenanceRepo = maintenanceRepo;
            _context = context;
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
                SoldDate = DateTime.UtcNow
            };

            await _saleRepo.AddAsync(sale);

            // Update car status to Sold
            car.StatusId = 4; // Sold
            await _carRepo.UpdateAsync(car);

            //  حساب تكلفة الصيانة
            var repairCost = await _maintenanceRepo.GetTotalCostByCarIdAsync(dto.CarId);

            //  إجمالي التكلفة (مع معالجة null)
            var totalCost = car.CostPrice + car.ShippingCost + repairCost;

            //  الربح أو الخسارة
            var profit = dto.SoldPrice - totalCost;

            //  تحديد نوع العملية
            var transactionType = profit >= 0 ? "Income" : "Expense";

            //  نخزن القيمة بدون سالب
            var transactionAmount = Math.Abs(profit);

            //  تسجيل العملية
            _context.Transactions.Add(new Transaction
            {
                Type = transactionType,
                Amount = transactionAmount,
                RelatedEntity = "Sale",
                RelatedId = sale.Id,
                Description = profit >= 0
                    ? $"الربح من بيع سيارة : {car.Brand} {car.Model} {car.Year}"
                    : $"خسارة من بيع سيارة : {car.Brand} {car.Model} {car.Year}",
                Date = DateTime.UtcNow
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
