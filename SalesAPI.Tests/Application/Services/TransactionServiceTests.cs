using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Moq;
using SalesAPI.Application.DTOs;
using SalesAPI.Application.Services;
using SalesAPI.Application.Services.Interfaces;
using SalesAPI.Data;
using SalesAPI.Domain.Entities;
using SalesAPI.Domain.Mappers;

namespace SalesAPI.Tests.Application.Services
{
    [TestFixture]
    public class TransactionServiceTests
    {
        private SalesApiDbContext _context;
        private TransactionService _transactionService;
        private IMapper _mapper;
        private Mock<IInventoryService> _inventoryServiceMock;
        private Mock<IPaymentProcessingService> _paymentProcessingServiceMock;

        [SetUp]
        public void SetUp()
        {
            // Create your in-memory DbContext
            var options = new DbContextOptionsBuilder<SalesApiDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            _context = new SalesApiDbContext(options);

            // Set up AutoMapper
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<MappingProfile>(); // Add your mapping profile here
            });
            _mapper = config.CreateMapper();

            // Initialize mocks
            _inventoryServiceMock = new Mock<IInventoryService>();
            _paymentProcessingServiceMock = new Mock<IPaymentProcessingService>();

            // Initialize the service with dependencies
            _transactionService = new TransactionService(_context, _inventoryServiceMock.Object, _paymentProcessingServiceMock.Object, _mapper);
        }
        [TearDown]
        public void TearDown()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        [Test]
        public async Task CreateTransactionAsync_CreatesTransactionSuccessfully()
        {
            // Arrange
            var transactionDto = new TransactionDTO
            {
                ArticleIds = new List<int> { 1, 2 },
                Payments = new List<PaymentDTO>
        {
            new PaymentDTO { Amount = 100, PaymentDate = DateTime.Now, PaymentMethod = "Credit Card" }
        }
            };

            _inventoryServiceMock.Setup(s => s.CheckAvailabilityAsync(It.IsAny<InventoryDTO>())).ReturnsAsync(true);
            _paymentProcessingServiceMock.Setup(s => s.ProcessPaymentsAsync(It.IsAny<IEnumerable<PaymentDTO>>(), It.IsAny<Transaction>())).Returns(Task.CompletedTask);

            // Act
            var result = await _transactionService.CreateTransactionAsync(transactionDto);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.ArticleIds.Count, Is.EqualTo(2));
            Assert.That(result.Payments.Count(), Is.EqualTo(1));
            Assert.That(result.Payments.First().Amount, Is.EqualTo(100));
        }


        [Test]
        public async Task CreateTransactionAsync_ThrowsException_WhenArticleNotAvailable()
        {
            // Arrange
            var transactionDto = new TransactionDTO
            {
                ArticleIds = new List<int> { 1 },
                Payments = new List<PaymentDTO>
                {
                    new PaymentDTO { Amount = 100, PaymentDate = DateTime.Now, PaymentMethod = "Credit Card" }
                }
            };

            _inventoryServiceMock.Setup(s => s.CheckAvailabilityAsync(It.IsAny<InventoryDTO>())).ReturnsAsync(false);

            // Act & Assert
            Assert.ThrowsAsync<InvalidOperationException>(() => _transactionService.CreateTransactionAsync(transactionDto));
        }

        [Test]
        public async Task CreateTransactionAsync_RollbacksTransaction_OnFailure()
        {
            // Arrange
            var transactionDto = new TransactionDTO
            {
                ArticleIds = new List<int> { 1 },
                Payments = new List<PaymentDTO>
                {
                    new PaymentDTO { Amount = 100, PaymentDate = DateTime.Now, PaymentMethod = "Credit Card" }
                }
            };

            _inventoryServiceMock.Setup(s => s.CheckAvailabilityAsync(It.IsAny<InventoryDTO>())).ReturnsAsync(true);
            _paymentProcessingServiceMock.Setup(s => s.ProcessPaymentsAsync(It.IsAny<IEnumerable<PaymentDTO>>(), It.IsAny<Transaction>()))
                .Throws(new Exception("Payment processing failed"));

            // Act & Assert
            Assert.ThrowsAsync<Exception>(() => _transactionService.CreateTransactionAsync(transactionDto));

            // Ensure that no transactions were created
            var transactionsCount = await _context.Transactions.CountAsync();
            Assert.That(transactionsCount, Is.EqualTo(0));
        }
    }
}
