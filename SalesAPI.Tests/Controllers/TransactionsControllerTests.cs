using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using SalesAPI.Application.DTOs;
using SalesAPI.Application.Services.Interfaces;
using SalesAPI.Controllers;

namespace SalesAPI.Tests.Controllers
{
    [TestFixture]
    public class TransactionsControllerTests
    {
        private TransactionsController _controller;
        private Mock<ITransactionService> _mockTransactionService;

        [SetUp]
        public void Setup()
        {
            _mockTransactionService = new Mock<ITransactionService>();
            _controller = new TransactionsController(_mockTransactionService.Object, Mock.Of<ILogger<TransactionsController>>());
        }

        [Test]
        public async Task CreateTransaction_ReturnsCreated_WhenTransactionIsValid()
        {
            // Arrange
            var validTransactionDto = new TransactionDTO
            {
                CustomerId = 1,
                ArticleIds = new List<int> { 1, 2 },
                Payments = new List<PaymentDTO>
                {
                    new PaymentDTO
                    {
                        PaymentDate = DateTime.Now,
                        PaymentId = 1,
                        PaymentMethod ="Card",
                        Amount = 20.00M,
                        TransactionId = 1,
                    }
                }
            };

            var createdTransaction = new TransactionDTO
            {
                CustomerId = 1,
                ArticleIds = new List<int> { 1, 2 },
                Payments = new List<PaymentDTO>
                {
                    new PaymentDTO 
                    { 
                        PaymentDate = DateTime.Now,
                        PaymentId = 1,
                        PaymentMethod ="Card",
                        Amount = 20.00M,
                        TransactionId = 1,}
                }
            };

            _mockTransactionService
                .Setup(service => service.CreateTransactionAsync(validTransactionDto))
                .ReturnsAsync(createdTransaction);

            // Act
            var result = await _controller.CreateTransaction(validTransactionDto);

            // Assert
            Assert.That(result, Is.InstanceOf<CreatedAtActionResult>());
            var createdResult = result as CreatedAtActionResult;
            Assert.That(createdResult?.ActionName, Is.EqualTo(nameof(TransactionsController.CreateTransaction)));
            Assert.That(createdResult?.Value, Is.EqualTo(createdTransaction));
        }


        [Test]
        public async Task CreateTransaction_ReturnsBadRequest_WhenTransactionIsInvalid()
        {
            // Arrange
            var invalidTransactionDto = new TransactionDTO
            {
                CustomerId = 1,
                ArticleIds = new List<int>(), // No articles
                Payments = new List<PaymentDTO>() // No payments
            };

            // Act
            var result = await _controller.CreateTransaction(invalidTransactionDto);

            // Assert
            Assert.That(result, Is.InstanceOf<BadRequestObjectResult>(), "Expected BadRequestObjectResult.");
            var badRequestResult = result as BadRequestObjectResult;
            Assert.That(badRequestResult?.Value, Is.EqualTo("Transaction must include at least one article and one payment."),
                "Expected error message about required articles and payments.");
        }

        [Test]
        public async Task CreateTransaction_ReturnsInternalServerError_WhenServiceThrowsException()
        {
            // Arrange
            var validTransactionDto = new TransactionDTO
            {
                CustomerId = 1,
                ArticleIds = new List<int> { 1, 2 },
                Payments = new List<PaymentDTO>
                {
                    new PaymentDTO { /* Initialize payment properties */ }
                }
            };

            _mockTransactionService.Setup(service => service.CreateTransactionAsync(validTransactionDto))
                .ThrowsAsync(new Exception("Database error."));

            // Act
            var result = await _controller.CreateTransaction(validTransactionDto);

            // Assert
            Assert.That(result, Is.InstanceOf<ObjectResult>());
            var objectResult = result as ObjectResult;
            Assert.That(objectResult?.StatusCode, Is.EqualTo(500));
            Assert.That(objectResult?.Value, Is.EqualTo("Internal server error while creating the transaction."));
        }
    }
}
