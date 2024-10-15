using SalesAPI.Application.DTOs;
using SalesAPI.Application.Services.Interfaces;
using SalesAPI.Domain.Entities;

namespace SalesAPI.Application.Services
{
    public class PaymentProcessingService : IPaymentProcessingService
    {
        public async Task ProcessPaymentsAsync(IEnumerable<PaymentDTO> payments, Transaction transaction)
        {
            foreach (var paymentDto in payments)
            {
                var payment = new Payment
                {
                    Amount = paymentDto.Amount,                    
                    PaymentDate = paymentDto.PaymentDate,
                    PaymentMethod = paymentDto.PaymentMethod,
                    Transaction = transaction
                };

                transaction.Payments.Add(payment);
            }
        }
    }

}
