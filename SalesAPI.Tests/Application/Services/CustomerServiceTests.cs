using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SalesAPI.Application.DTOs;
using SalesAPI.Application.Services;
using SalesAPI.Data;
using SalesAPI.Domain.Entities;

namespace SalesAPI.Tests.Application.Services
{
    [TestFixture]
    public class CustomerServiceTests : IDisposable
    {
        private SalesApiDbContext _context;
        private CustomerService _customerService;
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
                cfg.CreateMap<CustomerDTO, Customer>();
                cfg.CreateMap<Customer, CustomerDTO>();
            });
            _mapper = config.CreateMapper();
            _customerService = new CustomerService(_context, _mapper);
        }

        [TearDown]
        public void TearDown()
        {
            // Clean up and dispose of the context
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        [Test]
        public async Task GetAllAsync_ReturnsAllCustomers()
        {
            // Arrange
            var customerDto = new CustomerDTO { CustomerId = 1, Name = "Test Customer" };
            await _customerService.CreateAsync(customerDto);

            // Act
            var customers = await _customerService.GetAllAsync();

            // Assert
            Assert.That(customers, Is.Not.Empty);
            Assert.That(customers.Count(), Is.EqualTo(1));
        }

        [Test]
        public async Task GetByIdAsync_ReturnsCustomer_WhenCustomerExists()
        {
            // Arrange
            var customerDto = new CustomerDTO { CustomerId = 1, Name = "Test Customer" };
            await _customerService.CreateAsync(customerDto);

            // Act
            var customer = await _customerService.GetByIdAsync(1);

            // Assert
            Assert.That(customer, Is.Not.Null);
            Assert.That(customer.Name, Is.EqualTo("Test Customer"));
        }

        [Test]
        public async Task GetByIdAsync_ReturnsNull_WhenCustomerDoesNotExist()
        {
            // Act
            var customer = await _customerService.GetByIdAsync(1);

            // Assert
            Assert.That(customer, Is.Null);
        }

        [Test]
        public async Task CreateAsync_ReturnsCreatedCustomer()
        {
            // Arrange
            var customerDto = new CustomerDTO { CustomerId = 1, Name = "New Customer" };

            // Act
            var createdCustomer = await _customerService.CreateAsync(customerDto);

            // Assert
            Assert.That(createdCustomer, Is.Not.Null);
            Assert.That(createdCustomer.Name, Is.EqualTo("New Customer"));
            Assert.That(createdCustomer.CustomerId, Is.EqualTo(1)); // Assuming it gets ID 1
        }

        [Test]
        public async Task UpdateAsync_ReturnsUpdatedCustomer_WhenCustomerExists()
        {
            // Arrange
            var customerDto = new CustomerDTO { CustomerId = 1, Name = "Old Customer" };
            await _customerService.CreateAsync(customerDto);
            var updatedCustomerDto = new CustomerDTO { CustomerId = 1, Name = "Updated Customer" };

            // Act
            var updatedCustomer = await _customerService.UpdateAsync(updatedCustomerDto);

            // Assert
            Assert.That(updatedCustomer, Is.Not.Null);
            Assert.That(updatedCustomer.Name, Is.EqualTo("Updated Customer"));
        }

        [Test]
        public async Task UpdateAsync_ReturnsNull_WhenCustomerDoesNotExist()
        {
            // Arrange
            var updatedCustomerDto = new CustomerDTO { CustomerId = 1, Name = "Updated Customer" };

            // Act
            var updatedCustomer = await _customerService.UpdateAsync(updatedCustomerDto);

            // Assert
            Assert.That(updatedCustomer, Is.Null);
        }

        [Test]
        public async Task DeleteAsync_DeletesCustomer_WhenCustomerExists()
        {
            // Arrange
            var customerDto = new CustomerDTO { CustomerId = 1, Name = "Test Customer" };
            await _customerService.CreateAsync(customerDto);

            // Act
            await _customerService.DeleteAsync(1);
            var deletedCustomer = await _customerService.GetByIdAsync(1);

            // Assert
            Assert.That(deletedCustomer, Is.Null);
        }

        [Test]
        public async Task DeleteAsync_DoesNothing_WhenCustomerDoesNotExist()
        {
            // Arrange
            await _customerService.DeleteAsync(1); // Attempt to delete a non-existent customer

            // Act
            var result = await _customerService.GetAllAsync();

            // Assert
            Assert.That(result, Is.Empty); // No customers should exist
        }

        public void Dispose()
        {
            TearDown();
        }
    }
}
