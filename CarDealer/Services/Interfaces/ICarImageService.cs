using CarDealer.API.DTOs.Car;

namespace CarDealer.API.Services.Interfaces
{
    public interface ICarImageService
    {
        Task<List<CarImageDto>> UploadImagesAsync(int carId, List<IFormFile> files);
        Task<bool> DeleteImageAsync(int imageId);
        Task<bool> SetPrimaryAsync(int imageId);
    }
}
