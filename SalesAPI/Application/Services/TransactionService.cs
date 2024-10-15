using AutoMapper;
using SalesAPI.Application.DTOs;
using SalesAPI.Application.Services.Interfaces;
using SalesAPI.Data;
using SalesAPI.Domain.Entities;

namespace SalesAPI.Application.Services
{
    public class TransactionService : ITransactionService
    {
        private readonly SalesApiDbContext _context;
        private readonly IInventoryService _inventoryService;
        private readonly IPaymentProcessingService _paymentProcessingService;
        private readonly IMapper _mapper;

        public TransactionService(SalesApiDbContext context,
                                  IInventoryService inventoryService,
                                  IPaymentProcessingService paymentProcessingService,
                                  IMapper mapper)
        {
            _context = context;
            _inventoryService = inventoryService;
            _paymentProcessingService = paymentProcessingService;
            _mapper = mapper;
        }

        public async Task<TransactionDTO> CreateTransactionAsync(TransactionDTO transactionDto)
        {
            // Map TransactionDTO to Transaction entity
            var transaction = _mapper.Map<Transaction>(transactionDto);

            using (var dbTransaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    // Update Inventory
                    foreach (var articleId in transactionDto.ArticleIds)
                    {
                        var inventoryDto = new InventoryDTO { ArticleId = articleId, Quantity = 1 };

                        if (await _inventoryService.CheckAvailabilityAsync(inventoryDto))
                        {
                            var articleTransaction = new ArticleTransaction { ArticleId = articleId }; // Ensure this matches your model
                            transaction.TransactionArticles.Add(articleTransaction);
                            await _inventoryService.UpdateInventoryAsync(inventoryDto);
                        }
                        else
                        {
                            throw new InvalidOperationException($"Article with ID {articleId} is not available in inventory.");
                        }
                    }

                    // Process Payments
                    await _paymentProcessingService.ProcessPaymentsAsync(transactionDto.Payments, transaction);

                    // Add Transaction to the context
                    _context.Transactions.Add(transaction);
                    await _context.SaveChangesAsync();
                    await dbTransaction.CommitAsync();
                }
                catch
                {
                    await dbTransaction.RollbackAsync();
                    throw;
                }
            }
            
            var createdTransactionDto = _mapper.Map<TransactionDTO>(transaction);
            return createdTransactionDto;
        }

    }
}

