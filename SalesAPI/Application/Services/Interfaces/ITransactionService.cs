using SalesAPI.Application.DTOs;

namespace SalesAPI.Application.Services.Interfaces
{
    public interface ITransactionService
    {
        Task<TransactionDTO> CreateTransactionAsync(TransactionDTO transactionDto);
    }
}
