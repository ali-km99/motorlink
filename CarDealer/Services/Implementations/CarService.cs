using CarDealer.API.Data;
using CarDealer.API.DTOs;
using CarDealer.API.DTOs.Car;
using CarDealer.API.Entities;
using CarDealer.API.Repositories.Interfaces;
using CarDealer.API.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CarDealer.API.Services.Implementations
{
    public class CarService : ICarService
    {
        private readonly ICarRepository _carRepo;
        private readonly IMaintenanceRepository _maintenanceRepo;
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;
        private readonly ILogger<CarService> _logger;

        public CarService(
            ICarRepository carRepo,
            IMaintenanceRepository maintenanceRepo,
            AppDbContext context,
            IWebHostEnvironment env,
            ILogger<CarService> logger)
        {
            _carRepo = carRepo;
            _maintenanceRepo = maintenanceRepo;
            _context = context;
            _env = env;
            _logger = logger;
        }

        public async Task<PagedResult<CarListDto>> GetCarsAsync(CarFilterDto filter) =>
            await _carRepo.GetPagedAsync(filter);

        public async Task<CarDetailDto?> GetCarByIdAsync(int id)
        {
            var car = await _carRepo.GetWithDetailsAsync(id);
            if (car is null) return null;

            var totalRepairCost = car.Maintenances.Sum(m => m.RepairCost);
            var profit = car.SellingPrice - car.CostPrice - car.ShippingCost - totalRepairCost;

            // تجميع الـ Features حسب الـ Category
            var featuresGrouped = new CarFeaturesGroupedDto(
                car.CarFeatures
                   .Where(cf => cf.Feature.Category == "Technology")
                   .Select(cf => cf.Feature.Name).ToList(),
                car.CarFeatures
                   .Where(cf => cf.Feature.Category == "Interior")
                   .Select(cf => cf.Feature.Name).ToList(),
                car.CarFeatures
                   .Where(cf => cf.Feature.Category == "Exterior")
                   .Select(cf => cf.Feature.Name).ToList()
            );

            return new CarDetailDto(
                car.Id, car.Brand, car.Model, car.Year,
                car.ExteriorColor, car.InteriorColor,
                car.CostPrice, car.ShippingCost, car.SellingPrice, profit, totalRepairCost,
                car.VinNumber, car.Mileage, car.MileageUnit, car.BodyType,
                car.NumberOfSeats, car.Transmission, car.Condition, car.FuelType,
                car.Specs, car.EngineSize,
                car.BodyCondition, car.HasLicense, car.HasInsurance,
                car.HasCustomsClearance, car.PaymentMethod,
                car.StatusId, car.Status.Name, car.Notes, car.CreatedAt,
                car.Images.Select(i => new CarImageDto(i.Id, i.ImageUrl, i.IsPrimary)).ToList(),
                featuresGrouped,
                car.Maintenances.Select(m => new MaintenanceDto(
                    m.Id, m.CarId, m.IssueDescription, m.RepairCost, m.CreatedAt)).ToList(),
                car.Sale is null ? null : new SaleInfoDto(
                    car.Sale.Id, car.Sale.Customer.Name,
                    car.Sale.Customer.Phone, car.Sale.SoldPrice, car.Sale.SoldDate)
            );
        }

        public async Task<CarListDto> CreateCarAsync(CreateCarDto dto)
        {
            var car = new Car
            {
                Brand = dto.Brand,
                Model = dto.Model,
                Year = dto.Year,
                ExteriorColor = dto.ExteriorColor,
                InteriorColor = dto.InteriorColor,
                CostPrice = dto.CostPrice,
                ShippingCost = dto.ShippingCost,
                SellingPrice = dto.SellingPrice,
                StatusId = dto.StatusId,
                Notes = dto.Notes,
                VinNumber = dto.VinNumber,
                Mileage = dto.Mileage,
                MileageUnit = dto.MileageUnit,
                BodyType = dto.BodyType,
                NumberOfSeats = dto.NumberOfSeats,
                Transmission = dto.Transmission,
                Condition = dto.Condition,
                FuelType = dto.FuelType,
                Specs = dto.Specs,
                EngineSize = dto.EngineSize,
                BodyCondition = dto.BodyCondition,
                HasLicense = dto.HasLicense,
                HasInsurance = dto.HasInsurance,
                HasCustomsClearance = dto.HasCustomsClearance,
                PaymentMethod = dto.PaymentMethod,
                CreatedAt = DateTime.UtcNow
            };

            if (dto.FeatureIds?.Any() == true)
            {
                car.CarFeatures = dto.FeatureIds
                    .Distinct()
                    .Select(fId => new CarFeature { FeatureId = fId })
                    .ToList();
            }

            var strategy = _context.Database.CreateExecutionStrategy();

            await strategy.ExecuteAsync(async () =>
            {
                using var transaction = await _context.Database.BeginTransactionAsync();

                await _carRepo.AddAsync(car);
                await _carRepo.SaveChangesAsync();   // ← لازم نحفظ هنا حتى يتولد car.Id فعلياً

                // ─── تسجيل تكلفة السيارة كمصروف تلقائي ─────────────────────
                var totalCost = car.CostPrice + car.ShippingCost;

                _context.Transactions.Add(new Entities.Transaction
                {
                    Type = "Expense",
                    Amount = totalCost,
                    RelatedEntity = "Car",
                    RelatedId = car.Id,
                    Description = $"Purchase cost of {car.Brand} {car.Model} {car.Year}",
                    Date = DateTime.UtcNow
                });

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
            });

            return new CarListDto(
                car.Id, car.Brand, car.Model, car.Year,
                car.ExteriorColor, car.SellingPrice, "Ready",
                null, car.Condition, car.BodyType,
                car.Mileage, car.MileageUnit, car.CreatedAt);
        }

        public async Task<bool> UpdateCarAsync(int id, UpdateCarDto dto)
        {
            var car = await _context.Cars
                .Include(c => c.CarFeatures)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (car is null) return false;

            // ─── تحديث الحقول ─────────────────────────────────────────────────
            car.Brand = dto.Brand;
            car.Model = dto.Model;
            car.Year = dto.Year;
            car.ExteriorColor = dto.ExteriorColor;
            car.InteriorColor = dto.InteriorColor;
            car.CostPrice = dto.CostPrice;
            car.ShippingCost = dto.ShippingCost;
            car.SellingPrice = dto.SellingPrice;
            car.StatusId = dto.StatusId;
            car.Notes = dto.Notes;
            car.VinNumber = dto.VinNumber;
            car.Mileage = dto.Mileage;
            car.MileageUnit = dto.MileageUnit;
            car.BodyType = dto.BodyType;
            car.NumberOfSeats = dto.NumberOfSeats;
            car.Transmission = dto.Transmission;
            car.Condition = dto.Condition;
            car.FuelType = dto.FuelType;
            car.Specs = dto.Specs;
            car.EngineSize = dto.EngineSize;
            car.BodyCondition = dto.BodyCondition;
            car.HasLicense = dto.HasLicense;
            car.HasInsurance = dto.HasInsurance;
            car.HasCustomsClearance = dto.HasCustomsClearance;
            car.PaymentMethod = dto.PaymentMethod;

            // ─── تحديث Features ────────────────────────────────────────────────
            var incomingIds = dto.FeatureIds?.Distinct().ToList() ?? new List<int>();
            var existingIds = car.CarFeatures.Select(cf => cf.FeatureId).ToList();

            var toRemove = car.CarFeatures.Where(cf => !incomingIds.Contains(cf.FeatureId)).ToList();
            var toAdd = incomingIds.Where(fId => !existingIds.Contains(fId))
                                      .Select(fId => new CarFeature { CarId = id, FeatureId = fId })
                                      .ToList();

            _context.CarFeatures.RemoveRange(toRemove);
            await _context.CarFeatures.AddRangeAsync(toAdd);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteCarAsync(int id)
        {
            var strategy = _context.Database.CreateExecutionStrategy();

            return await strategy.ExecuteAsync(async () =>
            {
                using var transaction = await _context.Database.BeginTransactionAsync();

                var car = await _context.Cars
                    .FirstOrDefaultAsync(c => c.Id == id && !c.IsDeleted);

                if (car == null)
                    return false;

                // جلب Ids الصيانات المرتبطة بالسيارة (قبل حذفها)
                var maintenanceIds = await _context.Maintenances
                    .Where(m => m.CarId == id)
                    .Select(m => m.Id)
                    .ToListAsync();

                // حذف نهائي للمعاملات المالية المرتبطة (بالسيارة أو بصياناتها)
                await _context.Transactions
                    .Where(t =>
                        (t.RelatedEntity == "Car" && t.RelatedId == id) ||
                        (t.RelatedEntity == "Maintenance" && maintenanceIds.Contains(t.RelatedId)))
                    .ExecuteDeleteAsync();

                // حذف نهائي لسجلات الصيانة
                if (maintenanceIds.Count > 0)
                {
                    await _context.Maintenances
                        .Where(m => maintenanceIds.Contains(m.Id))
                        .ExecuteDeleteAsync();
                }

                // السيارة نفسها تبقى Soft Delete
                car.IsDeleted = true;
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();
                return true;
            });
        }

        public async Task<List<string>> GetBrandsAsync() =>
            await _carRepo.GetAllBrandsAsync();
    }
}
