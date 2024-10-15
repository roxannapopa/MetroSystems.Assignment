using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SalesAPI.Application.DTOs;
using SalesAPI.Application.Services;
using SalesAPI.Data;
using SalesAPI.Domain.Entities;

namespace SalesAPI.Tests.Application.Services
{
    [TestFixture]
    public class PaymentServiceTests : IDisposable
    {
        private SalesApiDbContext _context;
        private PaymentService _paymentService;
        private IMapper _mapper;

        [SetUp]
        public void Setup()
        {
            // Set up in-memory database options
            var options = new DbContextOptionsBuilder<SalesApiDbContext>()
                .UseInMemoryDatabase("TestDatabase")
                .Options;

            _context = new SalesApiDbContext(options);
            var config = new MapperConfiguration(cfg =>
            {
                // Add your AutoMapper configuration here
                cfg.CreateMap<PaymentDTO, Payment>();
                cfg.CreateMap<Payment, PaymentDTO>();
            });
            _mapper = config.CreateMapper();
            _paymentService = new PaymentService(_context, _mapper);

            // Seed data for testing
            var payment = new Payment { PaymentId = 1, Amount = 100, PaymentMethod = "Credit Card" };
            _context.Payments.Add(payment);
            _context.SaveChanges();
        }

        [TearDown]
        public void TearDown()
        {
            // Clean up and dispose of the context
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        [Test]
        public async Task GetAllAsync_ReturnsAllPayments()
        {
            // Act
            var payments = await _paymentService.GetAllAsync();

            // Assert
            Assert.That(payments, Is.Not.Empty);
            Assert.That(payments.Count(), Is.EqualTo(1));
        }

        [Test]
        public async Task GetByIdAsync_ReturnsCorrectPayment()
        {
            // Act
            var payment = await _paymentService.GetByIdAsync(1);

            // Assert
            Assert.That(payment, Is.Not.Null);
            Assert.That(payment.PaymentId, Is.EqualTo(1));
        }

        [Test]
        public async Task CreateAsync_AddsPaymentSuccessfully()
        {
            // Arrange
            var newPaymentDto = new PaymentDTO { Amount = 150, PaymentMethod = "PayPal" };

            // Act
            var createdPayment = await _paymentService.CreateAsync(newPaymentDto);
            var paymentInDb = await _context.Payments.FindAsync(createdPayment.PaymentId);

            // Assert
            Assert.That(paymentInDb, Is.Not.Null);
            Assert.That(paymentInDb.Amount, Is.EqualTo(150));
            Assert.That(paymentInDb.PaymentMethod, Is.EqualTo("PayPal"));
        }

        [Test]
        public async Task UpdateAsync_UpdatesPaymentSuccessfully()
        {
            // Arrange
            var updatedPaymentDto = new PaymentDTO { PaymentId = 1, Amount = 200, PaymentMethod = "Debit Card" };

            // Act
            var updatedPayment = await _paymentService.UpdateAsync(updatedPaymentDto);
            var paymentInDb = await _context.Payments.FindAsync(updatedPayment.PaymentId);

            // Assert
            Assert.That(paymentInDb, Is.Not.Null);
            Assert.That(paymentInDb.Amount, Is.EqualTo(200));
            Assert.That(paymentInDb.PaymentMethod, Is.EqualTo("Debit Card"));
        }

        [Test]
        public async Task UpdateAsync_ReturnsNull_WhenPaymentDoesNotExist()
        {
            // Arrange
            var updatedPaymentDto = new PaymentDTO { PaymentId = 2, Amount = 200, PaymentMethod = "Debit Card" }; // PaymentId 2 does not exist

            // Act
            var result = await _paymentService.UpdateAsync(updatedPaymentDto);

            // Assert
            Assert.That(result, Is.Null);
        }

        [Test]
        public async Task DeleteAsync_RemovesPaymentSuccessfully()
        {
            // Act
            await _paymentService.DeleteAsync(1);
            var paymentInDb = await _context.Payments.FindAsync(1);

            // Assert
            Assert.That(paymentInDb, Is.Null); // Payment should be deleted
        }

        [Test]
        public async Task DeleteAsync_DoesNothing_WhenPaymentDoesNotExist()
        {
            // Act
            await _paymentService.DeleteAsync(2); // PaymentId 2 does not exist
            var paymentCount = await _context.Payments.CountAsync();

            // Assert
            Assert.That(paymentCount, Is.EqualTo(1)); // Should still be 1 payment in the database
        }

        public void Dispose()
        {
            TearDown();
        }
    }
}
