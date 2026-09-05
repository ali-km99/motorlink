
using CarDealer.API.Features.Cars.DTOs;

namespace CarDealer.API.Features.Cars.Services.Interfaces
{
    public interface ICarStatusService
    {
        Task<List<CarStatusDto>> GetAllAsync();
    }
}
