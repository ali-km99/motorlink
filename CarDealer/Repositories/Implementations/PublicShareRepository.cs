using CarDealer.API.Data;
using CarDealer.API.Entities;
using Microsoft.EntityFrameworkCore;

public class PublicShareRepository : IPublicShareRepository
{
    private readonly AppDbContext _context;
    public PublicShareRepository(AppDbContext context) => _context = context;

    public async Task<PublicShare?> GetByTokenAsync(string token) =>
    await _context.PublicShares
        .Include(s => s.Car).ThenInclude(c => c.Images)
        .Include(s => s.Car).ThenInclude(c => c.CarFeatures).ThenInclude(cf => cf.Feature)
        .Include(s => s.Contacts)
        .IgnoreQueryFilters()
        .FirstOrDefaultAsync(s => s.Token == token);

    public async Task<List<PublicShare>> GetAllAsync()
    {
        return await _context.PublicShares
            .AsNoTracking()
            .ToListAsync();
    }


    public async Task<PublicShare?> GetByIdAsync(int id) =>
        await _context.PublicShares.FirstOrDefaultAsync(s => s.Id == id);

    public async Task<List<PublicShare>> GetByIdsAsync(List<int> ids) =>
        await _context.PublicShares.Where(s => ids.Contains(s.Id)).ToListAsync();

    public async Task AddAsync(PublicShare share) => await _context.PublicShares.AddAsync(share);

    public async Task SaveChangesAsync() => await _context.SaveChangesAsync();
}