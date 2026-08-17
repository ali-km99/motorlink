using CarDealer.API.Data;
using CarDealer.API.DTOs;
using CarDealer.API.DTOs.Car;
using CarDealer.API.Entities;
using CarDealer.API.Repositories.Interfaces;
using CarDealer.API.Services.Interfaces;

using Microsoft.EntityFrameworkCore;

namespace CarDealer.API.Services;

// ─── CarImage Service ──────────────────────────────────────────────────────────

public class CarImageService : ICarImageService
{
    private readonly AppDbContext _context;
    private readonly IWebHostEnvironment _env;
    private readonly ILogger<CarImageService> _logger;

    public CarImageService(AppDbContext context, IWebHostEnvironment env, ILogger<CarImageService> logger)
    {
        _context = context;
        _env = env;
        _logger = logger;
    }
    public async Task<List<CarImageDto>> UploadImagesAsync(int carId, List<IFormFile> files)
    {
        var uploadPath = Path.Combine(_env.WebRootPath, "images", "cars");
        Directory.CreateDirectory(uploadPath);

        var hasExistingImages = await _context.CarImages.AnyAsync(i => i.CarId == carId);
        var result = new List<CarImageDto>();

        for (int i = 0; i < files.Count; i++)
        {
            var file = files[i];
            var extension = Path.GetExtension(file.FileName).ToLower();
            var allowed = new[] { ".jpg", ".jpeg", ".png", ".webp" };

            if (!allowed.Contains(extension)) continue;

            var fileName = $"{Guid.NewGuid()}{extension}";
            var filePath = Path.Combine(uploadPath, fileName);

            using var stream = new FileStream(filePath, FileMode.Create);
            await file.CopyToAsync(stream);

            var imageUrl = $"/images/cars/{fileName}";
            var isPrimary = !hasExistingImages && i == 0;

            var image = new CarImage { CarId = carId, ImageUrl = imageUrl, IsPrimary = isPrimary };
            _context.CarImages.Add(image);
            

            result.Add(new CarImageDto(image.Id, imageUrl, isPrimary));
        }
        await _context.SaveChangesAsync();
        return result;
    }

    public async Task<bool> DeleteImageAsync(int imageId)
    {
        var image = await _context.CarImages.FindAsync(imageId);
        if (image is null) return false;

        var wasPrimary = image.IsPrimary;
        var carId = image.CarId;
        var filePath = Path.Combine(
            _env.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot"),
            image.ImageUrl.TrimStart('/').Replace('/', Path.DirectorySeparatorChar)
        );

        // 1. احذف من DB أولاً
        _context.CarImages.Remove(image);
        await _context.SaveChangesAsync();

        // 2. احذف الملف الفعلي من wwwroot — في try/catch منفصل
        // لو فشل حذف الملف، الـ DB تم تنظيفه بالفعل ونرجع true
        // ونسجّل الخطأ بدل ما نكسر العملية كلها
        try
        {
            if (File.Exists(filePath))
                File.Delete(filePath);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to delete image file: {FilePath}", filePath);
        }

        // 3. لو كانت primary، عيّن التالية
        if (wasPrimary)
        {
            var next = await _context.CarImages
                .FirstOrDefaultAsync(i => i.CarId == carId);

            if (next is not null)
            {
                next.IsPrimary = true;
                await _context.SaveChangesAsync();
            }
        }

        return true;
    }

    public async Task<bool> SetPrimaryAsync(int imageId)
    {
        var image = await _context.CarImages.FindAsync(imageId);
        if (image is null) return false;

        var allImages = await _context.CarImages.Where(i => i.CarId == image.CarId).ToListAsync();
        allImages.ForEach(i => i.IsPrimary = false);
        image.IsPrimary = true;

        await _context.SaveChangesAsync();
        return true;
    }
}

// ─── Customer Service ──────────────────────────────────────────────────────────

public class CustomerService : ICustomerService
{
    private readonly ICustomerRepository _repo;

    public CustomerService(ICustomerRepository repo) => _repo = repo;

    public async Task<List<CustomerDto>> GetAllAsync() => await _repo.GetAllWithStatsAsync();

    public async Task<CustomerDto?> GetByIdAsync(int id)
    {
        var c = await _repo.GetByIdAsync(id);
        if (c is null) return null;
        return new CustomerDto(c.Id, c.Name, c.Phone, c.Notes, 0);
    }

    public async Task<CustomerDto> CreateAsync(CreateCustomerDto dto)
    {
        var c = new Customer { Name = dto.Name, Phone = dto.Phone, Notes = dto.Notes };
        await _repo.AddAsync(c);
        await _repo.SaveChangesAsync();
        return new CustomerDto(c.Id, c.Name, c.Phone, c.Notes, 0);
    }

    public async Task<bool> UpdateAsync(int id, UpdateCustomerDto dto)
    {
        var c = await _repo.GetByIdAsync(id);
        if (c is null) return false;

        c.Name = dto.Name;
        c.Phone = dto.Phone;
        c.Notes = dto.Notes;

        await _repo.UpdateAsync(c);
        await _repo.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var c = await _repo.GetByIdAsync(id);
        if (c is null) return false;

        c.IsDeleted = true;
        await _repo.UpdateAsync(c);
        await _repo.SaveChangesAsync();
        return true;
    }
}


public class CarStatusService : ICarStatusService
{
    private readonly AppDbContext _context;

    public CarStatusService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<CarStatusDto>> GetAllAsync()
    {
        return await _context.CarStatuses
            .AsNoTracking()
            .OrderBy(s => s.Id)
            .Select(s => new CarStatusDto(s.Id, s.Name))
            .ToListAsync();
    }
}