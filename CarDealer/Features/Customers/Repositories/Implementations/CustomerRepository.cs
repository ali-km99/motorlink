using CarDealer.API.Data;
using CarDealer.API.Features.Customers.DTOs;
using CarDealer.API.Features.Customers.Entities;
using CarDealer.API.Features.Customers.Repositories.Interfaces;
using CarDealer.API.Repositories;
using Microsoft.EntityFrameworkCore;

namespace CarDealer.API.Features.Customers.Repositories.Implementations
{
    // ─── Customer Repository ───────────────────────────────────────────────────────

    public class CustomerRepository : Repository<Customer>, ICustomerRepository
    {
        public CustomerRepository(AppDbContext context) : base(context) { }

        public async Task<List<CustomerDto>> GetAllWithStatsAsync()
        {
            var customers = await _context.Customers
                .AsNoTracking()
                .OrderBy(c => c.Name)
                .Select(c => new { c.Id, c.Name, c.Phone, c.Notes })
                .ToListAsync();

            var salesCounts = await _context.Sales
                .AsNoTracking()
                .GroupBy(s => s.CustomerId)
                .Select(g => new { CustomerId = g.Key, Count = g.Count() })
                .ToDictionaryAsync(x => x.CustomerId, x => x.Count);

            return customers.Select(c => new CustomerDto(
                c.Id,
                c.Name,
                c.Phone,
                c.Notes,
                salesCounts.TryGetValue(c.Id, out var count) ? count : 0
            )).ToList();
        }

        public async Task<Customer?> GetByPhoneAsync(string phone) =>
            await _context.Customers
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Phone == phone);
    }
}
