using SalesAPI.Application.DTOs;
using SalesAPI.Application.Services;
using SalesAPI.Domain.Entities;

namespace SalesAPI.Tests.Application.Services
{
    [TestFixture]
    public class PaymentProcessingServiceTests
    {
        private PaymentProcessingService _paymentProcessingService;

        [SetUp]
        public void Setup()
        {
            _paymentProcessingService = new PaymentProcessingService();
        }

        [Test]
        public async Task ProcessPaymentsAsync_AddsPaymentsToTransaction()
        {
            // Arrange
            var transaction = new Transaction { TransactionId = 1, Payments = new List<Payment>() };
            var payments = new List<PaymentDTO>
            {
                new PaymentDTO { Amount = 100, PaymentDate = DateTime.Now, PaymentMethod = "Credit Card" },
                new PaymentDTO { Amount = 150, PaymentDate = DateTime.Now, PaymentMethod = "PayPal" }
            };

            // Act
            await _paymentProcessingService.ProcessPaymentsAsync(payments, transaction);

            // Assert
            Assert.That(transaction.Payments, Is.Not.Empty);
            Assert.That(transaction.Payments.Count, Is.EqualTo(2));
            Assert.That(transaction.Payments.First().Amount, Is.EqualTo(100));
            Assert.That(transaction.Payments.ElementAt(1).Amount, Is.EqualTo(150));
        }

        [Test]
        public async Task ProcessPaymentsAsync_DoesNothing_WhenPaymentsIsEmpty()
        {
            // Arrange
            var transaction = new Transaction { TransactionId = 1, Payments = new List<Payment>() };
            var payments = new List<PaymentDTO>(); // Empty list

            // Act
            await _paymentProcessingService.ProcessPaymentsAsync(payments, transaction);

            // Assert
            Assert.That(transaction.Payments, Is.Empty);
        }
    }
}
