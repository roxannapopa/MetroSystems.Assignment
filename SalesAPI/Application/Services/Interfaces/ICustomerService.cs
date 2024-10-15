using SalesAPI.Application.DTOs;

namespace SalesAPI.Application.Services.Interfaces
{
    public interface ICustomerService
    {
        Task<IEnumerable<CustomerDTO>> GetAllAsync();
        Task<CustomerDTO> GetByIdAsync(int customerId);
        Task<CustomerDTO> CreateAsync(CustomerDTO customerDto);
        Task<CustomerDTO> UpdateAsync(CustomerDTO customerDto);
        Task DeleteAsync(int customerId);
    }
}
