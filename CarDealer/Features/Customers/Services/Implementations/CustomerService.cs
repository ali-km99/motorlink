using CarDealer.API.Features.Customers.DTOs;
using CarDealer.API.Features.Customers.Entities;
using CarDealer.API.Features.Customers.Repositories.Interfaces;
using CarDealer.API.Features.Customers.Services.Interfaces;
using CarDealer.API.Shared.Services.Interfaces;

namespace CarDealer.API.Features.Customers.Services.Implementations
{
    // ─── Customer Service ──────────────────────────────────────────────────────────

    public class CustomerService : ICustomerService
    {
        private readonly ICustomerRepository _repo;
        private readonly ICurrentTenantService _currentTenant;   // ← جديد
        public CustomerService(ICustomerRepository repo, ICurrentTenantService currentTenant)
        {
            _repo = repo;
            _currentTenant = currentTenant;
        }

        public async Task<List<CustomerDto>> GetAllAsync() => await _repo.GetAllWithStatsAsync();

        public async Task<CustomerDto?> GetByIdAsync(int id)
        {
            var c = await _repo.GetByIdAsync(id);
            if (c is null) return null;
            return new CustomerDto(c.Id, c.Name, c.Phone, c.Notes, 0);
        }

        public async Task<CustomerDto> CreateAsync(CreateCustomerDto dto)
        {
            var c = new Customer
            {
                Name = dto.Name,
                Phone = dto.Phone,
                Notes = dto.Notes,
                TenantId = _currentTenant.TenantId
            };   // ← جديد
            await _repo.AddAsync(c);
            await _repo.SaveChangesAsync();
            return new CustomerDto(c.Id, c.Name, c.Phone, c.Notes, 0);
        }

        public async Task<bool> UpdateAsync(int id, UpdateCustomerDto dto)
        {
            var c = await _repo.GetByIdAsync(id);
            if (c is null) return false;

            c.Name = dto.Name;
            c.Phone = dto.Phone;
            c.Notes = dto.Notes;

            await _repo.UpdateAsync(c);
            await _repo.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var c = await _repo.GetByIdAsync(id);
            if (c is null) return false;

            c.IsDeleted = true;
            await _repo.UpdateAsync(c);
            await _repo.SaveChangesAsync();
            return true;
        }
    }

}
