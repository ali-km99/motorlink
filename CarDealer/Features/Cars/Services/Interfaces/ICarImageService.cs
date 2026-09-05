using CarDealer.API.Features.Cars.DTOs;

namespace CarDealer.API.Features.Cars.Services.Interfaces
{
    public interface ICarImageService
    {
        Task<List<CarImageDto>> UploadImagesAsync(int carId, List<IFormFile> files);
        Task<bool> DeleteImageAsync(int imageId);
        Task<bool> SetPrimaryAsync(int imageId);
    }
}
