using CarDealer.API.DTOs;
using CarDealer.API.DTOs.Car;

namespace CarDealer.API.Services.Interfaces
{
    public interface ICarStatusService
    {
        Task<List<CarStatusDto>> GetAllAsync();
    }
}
