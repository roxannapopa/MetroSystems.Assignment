using Microsoft.AspNetCore.Mvc;
using SalesAPI.Application.DTOs;
using SalesAPI.Application.Services.Interfaces;

namespace SalesAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TransactionsController : ControllerBase
    {
        private readonly ITransactionService _transactionService;
        private readonly ILogger<TransactionsController> _logger;

        public TransactionsController(ITransactionService transactionService, ILogger<TransactionsController> logger)
        {
            _transactionService = transactionService;
            _logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> CreateTransaction([FromBody] TransactionDTO transactionDto)
        {
            if (transactionDto == null || !transactionDto.ArticleIds.Any() || !transactionDto.Payments.Any())
            {
                return BadRequest("Transaction must include at least one article and one payment.");
            }

            try
            {
                var transaction = await _transactionService.CreateTransactionAsync(transactionDto);
                return CreatedAtAction(nameof(CreateTransaction), new { id = transaction.TransactionId }, transaction);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating transaction.");
                return StatusCode(500, "Internal server error while creating the transaction.");
            }
        }
    }
}
