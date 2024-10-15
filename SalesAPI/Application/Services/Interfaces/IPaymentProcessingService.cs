using SalesAPI.Application.DTOs;
using SalesAPI.Domain.Entities;

namespace SalesAPI.Application.Services.Interfaces
{
    public interface IPaymentProcessingService
    {
        Task ProcessPaymentsAsync(IEnumerable<PaymentDTO> payments, Transaction transaction);
    }
}
