using SalesAPI.Application.DTOs;

namespace SalesAPI.Application.Services.Interfaces
{
    public interface IPaymentService
    {
        Task<IEnumerable<PaymentDTO>> GetAllAsync();
        Task<PaymentDTO> GetByIdAsync(int paymentId);
        Task<PaymentDTO> CreateAsync(PaymentDTO paymentDto);
        Task<PaymentDTO> UpdateAsync(PaymentDTO paymentDto);
        Task DeleteAsync(int paymentId);
    }
}
