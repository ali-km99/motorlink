using CarDealer.API.DTOs;
using CarDealer.API.Entities;

namespace CarDealer.API.Repositories.Interfaces
{
    // ─── Customer Repository ───────────────────────────────────────────────────────

    public interface ICustomerRepository : IRepository<Customer>
    {
        Task<List<CustomerDto>> GetAllWithStatsAsync();
        Task<Customer?> GetByPhoneAsync(string phone);
    }
}
