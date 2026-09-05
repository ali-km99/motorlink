using CarDealer.API.Features.Customers.DTOs;
using CarDealer.API.Features.Customers.Entities;
using CarDealer.API.Repositories.Interfaces;

namespace CarDealer.API.Features.Customers.Repositories.Interfaces
{
    // ─── Customer Repository ───────────────────────────────────────────────────────

    public interface ICustomerRepository : IRepository<Customer>
    {
        Task<List<CustomerDto>> GetAllWithStatsAsync();
        Task<Customer?> GetByPhoneAsync(string phone);
    }
}
