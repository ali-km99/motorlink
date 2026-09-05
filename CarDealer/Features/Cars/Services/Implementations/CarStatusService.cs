using CarDealer.API.Features.Cars.DTOs;
using CarDealer.API.Features.Cars.Services.Interfaces;
using CarDealer.API.Shared.Data;
using Microsoft.EntityFrameworkCore;

namespace CarDealer.API.Features.Cars.Services.Implementations
{
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
}
