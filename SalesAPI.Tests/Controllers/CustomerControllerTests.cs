using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using SalesAPI.Application.DTOs;
using SalesAPI.Application.Services.Interfaces;
using SalesAPI.Controllers;

namespace SalesAPI.Tests.Controllers
{
    [TestFixture]
    public class CustomersControllerTests
    {
        private CustomersController _controller;
        private Mock<ICustomerService> _mockCustomerService;
        private Mock<ILogger<CustomersController>> _mockLogger;

        [SetUp]
        public void Setup()
        {
            _mockCustomerService = new Mock<ICustomerService>();
            _mockLogger = new Mock<ILogger<CustomersController>>();
            _controller = new CustomersController(_mockCustomerService.Object, _mockLogger.Object);
        }

        [Test]
        public async Task GetAll_ReturnsOk_WhenCustomersExist()
        {
            // Arrange
            var customers = new List<CustomerDTO>
            {
                new CustomerDTO { CustomerId = 1, Name = "Customer 1" },
                new CustomerDTO { CustomerId = 2, Name = "Customer 2" }
            };
            _mockCustomerService.Setup(service => service.GetAllAsync()).ReturnsAsync(customers);

            // Act
            var result = await _controller.GetAll();

            // Assert
            Assert.That(result.Result, Is.InstanceOf<OkObjectResult>(), "Expected OkObjectResult.");
            var okResult = result.Result as OkObjectResult;
            Assert.That(okResult.StatusCode, Is.EqualTo(200));
            Assert.That(okResult.Value, Is.EqualTo(customers));
        }

        [Test]
        public async Task GetAll_ReturnsInternalServerError_WhenExceptionThrown()
        {
            // Arrange
            _mockCustomerService.Setup(service => service.GetAllAsync()).ThrowsAsync(new Exception("Service error"));

            // Act
            var result = await _controller.GetAll();

            // Assert
            Assert.That(result.Result, Is.InstanceOf<ObjectResult>(), "Expected ObjectResult.");
            var objectResult = result.Result as ObjectResult;
            Assert.That(objectResult.StatusCode, Is.EqualTo(500), "Expected status code 500 (Internal Server Error).");
        }

        [Test]
        public async Task GetById_ReturnsBadRequest_WhenIdIsInvalid()
        {
            // Arrange
            int invalidId = -1;

            // Act
            var result = await _controller.GetById(invalidId);

            // Assert
            Assert.That(result.Result, Is.InstanceOf<BadRequestObjectResult>(), "Expected BadRequestObjectResult.");
        }

        [Test]
        public async Task GetById_ReturnsNotFound_WhenCustomerDoesNotExist()
        {
            // Arrange
            int nonExistentId = 99;
            _mockCustomerService.Setup(service => service.GetByIdAsync(nonExistentId)).ReturnsAsync((CustomerDTO)null);

            // Act
            var result = await _controller.GetById(nonExistentId);

            // Assert
            Assert.That(result.Result, Is.InstanceOf<NotFoundResult>(), "Expected NotFoundResult when customer does not exist.");
        }

        [Test]
        public async Task GetById_ReturnsInternalServerError_WhenExceptionThrown()
        {
            // Arrange
            int id = 1;
            _mockCustomerService.Setup(service => service.GetByIdAsync(id)).ThrowsAsync(new Exception("Service error"));

            // Act
            var result = await _controller.GetById(id);

            // Assert
            Assert.That(result.Result, Is.InstanceOf<ObjectResult>(), "Expected ObjectResult.");
            var objectResult = result.Result as ObjectResult;
            Assert.That(objectResult.StatusCode, Is.EqualTo(500), "Expected status code 500 (Internal Server Error).");
        }

        [Test]
        public async Task Create_ReturnsCreatedAtAction_WhenCustomerIsCreated()
        {
            // Arrange
            var newCustomerDto = new CustomerDTO { Name = "New Customer" };
            var createdCustomer = new CustomerDTO { CustomerId = 1, Name = "New Customer" };
            _mockCustomerService.Setup(service => service.CreateAsync(newCustomerDto)).ReturnsAsync(createdCustomer);

            // Act
            var result = await _controller.Create(newCustomerDto);

            // Assert
            Assert.That(result.Result, Is.InstanceOf<CreatedAtActionResult>(), "Expected CreatedAtActionResult.");
            var createdResult = result.Result as CreatedAtActionResult;
            Assert.That(createdResult.StatusCode, Is.EqualTo(201));
            Assert.That(createdResult.Value, Is.EqualTo(createdCustomer));
        }

        [Test]
        public async Task Create_ReturnsBadRequest_WhenModelStateIsInvalid()
        {
            // Arrange
            var newCustomerDto = new CustomerDTO { Name = "" }; // Invalid state
            _controller.ModelState.AddModelError("Name", "The Name field is required.");

            // Act
            var result = await _controller.Create(newCustomerDto);

            // Assert
            Assert.That(result.Result, Is.InstanceOf<BadRequestObjectResult>(), "Expected BadRequestObjectResult.");
        }

        [Test]
        public async Task Create_ReturnsInternalServerError_WhenExceptionThrown()
        {
            // Arrange
            var newCustomerDto = new CustomerDTO { Name = "New Customer" };
            _mockCustomerService.Setup(service => service.CreateAsync(newCustomerDto)).ThrowsAsync(new Exception("Service error"));

            // Act
            var result = await _controller.Create(newCustomerDto);

            // Assert
            Assert.That(result.Result, Is.InstanceOf<ObjectResult>(), "Expected ObjectResult.");
            var objectResult = result.Result as ObjectResult;
            Assert.That(objectResult.StatusCode, Is.EqualTo(500), "Expected status code 500 (Internal Server Error).");
        }

        [Test]
        public async Task Update_ReturnsBadRequest_WhenIdDoesNotMatch()
        {
            // Arrange
            int id = 1;
            var customerDto = new CustomerDTO { CustomerId = 2, Name = "Updated Customer" };

            // Act
            var result = await _controller.Update(id, customerDto);

            // Assert
            Assert.That(result, Is.InstanceOf<BadRequestResult>(), "Expected BadRequestResult.");
        }

        [Test]
        public async Task Update_ReturnsNotFound_WhenCustomerDoesNotExist()
        {
            // Arrange
            int id = 1;
            var customerDto = new CustomerDTO { CustomerId = id, Name = "Updated Customer" };
            _mockCustomerService.Setup(service => service.UpdateAsync(customerDto)).ReturnsAsync((CustomerDTO)null);

            // Act
            var result = await _controller.Update(id, customerDto);

            // Assert
            Assert.That(result, Is.InstanceOf<NotFoundResult>(), "Expected NotFoundResult.");
        }

        [Test]
        public async Task Update_ReturnsInternalServerError_WhenExceptionThrown()
        {
            // Arrange
            int id = 1;
            var customerDto = new CustomerDTO { CustomerId = id, Name = "Updated Customer" };
            _mockCustomerService.Setup(service => service.UpdateAsync(customerDto)).ThrowsAsync(new Exception("Service error"));

            // Act
            var result = await _controller.Update(id, customerDto);

            // Assert
            Assert.That(result, Is.InstanceOf<ObjectResult>(), "Expected ObjectResult.");
            var objectResult = result as ObjectResult;
            Assert.That(objectResult.StatusCode, Is.EqualTo(500), "Expected status code 500 (Internal Server Error).");
        }

        [Test]
        public async Task Delete_ReturnsNotFound_WhenCustomerDoesNotExist()
        {
            // Arrange
            int id = 1;
            _mockCustomerService.Setup(service => service.GetByIdAsync(id)).ReturnsAsync((CustomerDTO)null);

            // Act
            var result = await _controller.Delete(id);

            // Assert
            Assert.That(result, Is.InstanceOf<NotFoundObjectResult>(), "Expected NotFoundObjectResult when customer does not exist.");
        }

        [Test]
        public async Task Delete_ReturnsInternalServerError_WhenExceptionThrown()
        {
            // Arrange
            int id = 1;
            _mockCustomerService.Setup(service => service.GetByIdAsync(id)).ThrowsAsync(new Exception("Service error"));

            // Act
            var result = await _controller.Delete(id);

            // Assert
            Assert.That(result, Is.InstanceOf<ObjectResult>(), "Expected ObjectResult.");
            var objectResult = result as ObjectResult;
            Assert.That(objectResult.StatusCode, Is.EqualTo(500), "Expected status code 500 (Internal Server Error).");
        }

        [Test]
        public async Task Delete_ReturnsNoContent_WhenCustomerIsDeleted()
        {
            // Arrange
            int id = 1;
            var customer = new CustomerDTO { CustomerId = id, Name = "Customer" };
            _mockCustomerService.Setup(service => service.GetByIdAsync(id)).ReturnsAsync(customer);
            _mockCustomerService.Setup(service => service.DeleteAsync(id)).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.Delete(id);

            // Assert
            Assert.That(result, Is.InstanceOf<NoContentResult>(), "Expected NoContentResult.");
        }
    }
}
