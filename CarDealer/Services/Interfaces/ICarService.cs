using CarDealer.API.DTOs;
using CarDealer.API.DTOs.Car;


namespace CarDealer.API.Services.Interfaces;

public interface ICarService
{
    Task<PagedResult<CarListDto>> GetCarsAsync(CarFilterDto filter);
    Task<CarDetailDto?> GetCarByIdAsync(int id);
    Task<CarListDto> CreateCarAsync(CreateCarDto dto);
    Task<bool> UpdateCarAsync(int id, UpdateCarDto dto);
    Task<bool> DeleteCarAsync(int id);
    Task<List<string>> GetBrandsAsync();
}
