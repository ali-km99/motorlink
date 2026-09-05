using CarDealer.API.Features.Cars.DTOs;
namespace CarDealer.API.Features.Cars.Services.Interfaces
{
    public interface IFeatureService
    {
        Task<List<FeatureDto>> GetAllAsync();
        Task<FeatureDto?> GetByIdAsync(int id);
        Task<FeatureDto> CreateAsync(CreateFeatureDto dto);
        Task<bool> UpdateAsync(int id, UpdateFeatureDto dto);
        Task<bool> DeleteAsync(int id);
    }
}
