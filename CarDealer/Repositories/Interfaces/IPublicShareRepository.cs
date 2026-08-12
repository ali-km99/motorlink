using CarDealer.API.Entities;

public interface IPublicShareRepository
{
    Task<PublicShare?> GetByTokenAsync(string token);
    Task<PublicShare?> GetByIdAsync(int id);
    Task<List<PublicShare>> GetByIdsAsync(List<int> ids);
    Task<List<PublicShare>> GetAllAsync();
    
    Task AddAsync(PublicShare share);
    Task SaveChangesAsync();
}