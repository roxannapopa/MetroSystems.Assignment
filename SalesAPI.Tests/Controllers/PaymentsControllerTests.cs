using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using SalesAPI.Application.DTOs;
using SalesAPI.Application.Services.Interfaces;
using SalesAPI.Controllers;

namespace SalesAPI.Tests.Controllers
{
    [TestFixture]
    public class PaymentsControllerTests
    {
        private PaymentsController _controller;
        private Mock<IPaymentService> _mockPaymentService;
        private Mock<ILogger<PaymentsController>> _mockLogger;

        [SetUp]
        public void Setup()
        {
            _mockPaymentService = new Mock<IPaymentService>();
            _mockLogger = new Mock<ILogger<PaymentsController>>();
            _controller = new PaymentsController(_mockPaymentService.Object, _mockLogger.Object);
        }

        [Test]
        public async Task GetAll_ReturnsOk_WhenPaymentsExist()
        {
            // Arrange
            var payments = new List<PaymentDTO>
            {
                new PaymentDTO { PaymentId = 1, Amount = 100 },
                new PaymentDTO { PaymentId = 2, Amount = 200 }
            };
            _mockPaymentService.Setup(service => service.GetAllAsync()).ReturnsAsync(payments);

            // Act
            var result = await _controller.GetAll();

            // Assert
            Assert.That(result.Result, Is.InstanceOf<OkObjectResult>(), "Expected OkObjectResult.");
            var okResult = result.Result as OkObjectResult;
            Assert.That(okResult.StatusCode, Is.EqualTo(200));
            Assert.That(okResult.Value, Is.EqualTo(payments));
        }

        [Test]
        public async Task GetAll_ReturnsInternalServerError_WhenExceptionThrown()
        {
            // Arrange
            _mockPaymentService.Setup(service => service.GetAllAsync()).ThrowsAsync(new Exception("Service error"));

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
            Assert.That(result.Result, Is.InstanceOf<BadRequestObjectResult>(), "Expected BadRequestObjectResult for invalid ID.");
        }

        [Test]
        public async Task GetById_ReturnsNotFound_WhenPaymentDoesNotExist()
        {
            // Arrange
            int nonExistentId = 99;
            _mockPaymentService.Setup(service => service.GetByIdAsync(nonExistentId)).ReturnsAsync((PaymentDTO)null);

            // Act
            var result = await _controller.GetById(nonExistentId);

            // Assert
            Assert.That(result.Result, Is.InstanceOf<NotFoundResult>(), "Expected NotFoundResult when payment does not exist.");
        }

        [Test]
        public async Task GetById_ReturnsInternalServerError_WhenExceptionThrown()
        {
            // Arrange
            int id = 1;
            _mockPaymentService.Setup(service => service.GetByIdAsync(id)).ThrowsAsync(new Exception("Service error"));

            // Act
            var result = await _controller.GetById(id);

            // Assert
            Assert.That(result.Result, Is.InstanceOf<ObjectResult>(), "Expected ObjectResult.");
            var objectResult = result.Result as ObjectResult;
            Assert.That(objectResult.StatusCode, Is.EqualTo(500), "Expected status code 500 (Internal Server Error).");
        }

        [Test]
        public async Task Create_ReturnsCreatedAtAction_WhenPaymentIsCreated()
        {
            // Arrange
            var newPaymentDto = new PaymentDTO { Amount = 150 };
            var createdPayment = new PaymentDTO { PaymentId = 1, Amount = 150 };
            _mockPaymentService.Setup(service => service.CreateAsync(newPaymentDto)).ReturnsAsync(createdPayment);

            // Act
            var result = await _controller.Create(newPaymentDto);

            // Assert
            Assert.That(result.Result, Is.InstanceOf<CreatedAtActionResult>(), "Expected CreatedAtActionResult.");
            var createdResult = result.Result as CreatedAtActionResult;
            Assert.That(createdResult.StatusCode, Is.EqualTo(201));
            Assert.That(createdResult.Value, Is.EqualTo(createdPayment));
        }

        [Test]
        public async Task Create_ReturnsBadRequest_WhenModelStateIsInvalid()
        {
            // Arrange
            var newPaymentDto = new PaymentDTO { Amount = -10 }; // Assuming negative amount is invalid
            _controller.ModelState.AddModelError("Amount", "Invalid amount");

            // Act
            var result = await _controller.Create(newPaymentDto);

            // Assert
            Assert.That(result.Result, Is.InstanceOf<BadRequestObjectResult>(), "Expected BadRequestObjectResult for invalid model state.");
        }

        [Test]
        public async Task Create_ReturnsInternalServerError_WhenExceptionThrown()
        {
            // Arrange
            var newPaymentDto = new PaymentDTO { Amount = 150 };
            _mockPaymentService.Setup(service => service.CreateAsync(newPaymentDto)).ThrowsAsync(new Exception("Service error"));

            // Act
            var result = await _controller.Create(newPaymentDto);

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
            var paymentDto = new PaymentDTO { PaymentId = 2, Amount = 200 };

            // Act
            var result = await _controller.Update(id, paymentDto);

            // Assert
            Assert.That(result, Is.InstanceOf<BadRequestResult>(), "Expected BadRequestResult when ID does not match.");
        }

        [Test]
        public async Task Update_ReturnsNotFound_WhenPaymentDoesNotExist()
        {
            // Arrange
            int id = 1;
            var paymentDto = new PaymentDTO { PaymentId = id, Amount = 200 };
            _mockPaymentService.Setup(service => service.UpdateAsync(paymentDto)).ReturnsAsync((PaymentDTO)null);

            // Act
            var result = await _controller.Update(id, paymentDto);

            // Assert
            Assert.That(result, Is.InstanceOf<NotFoundResult>(), "Expected NotFoundResult when payment does not exist.");
        }

        [Test]
        public async Task Update_ReturnsInternalServerError_WhenExceptionThrown()
        {
            // Arrange
            int id = 1;
            var paymentDto = new PaymentDTO { PaymentId = id, Amount = 200 };
            _mockPaymentService.Setup(service => service.UpdateAsync(paymentDto)).ThrowsAsync(new Exception("Service error"));

            // Act
            var result = await _controller.Update(id, paymentDto);

            // Assert
            Assert.That(result, Is.InstanceOf<ObjectResult>(), "Expected ObjectResult.");
            var objectResult = result as ObjectResult;
            Assert.That(objectResult.StatusCode, Is.EqualTo(500), "Expected status code 500 (Internal Server Error).");
        }

        [Test]
        public async Task Delete_ReturnsNotFound_WhenPaymentDoesNotExist()
        {
            // Arrange
            int id = 1;
            _mockPaymentService.Setup(service => service.GetByIdAsync(id)).ReturnsAsync((PaymentDTO)null);

            // Act
            var result = await _controller.Delete(id);

            // Assert
            Assert.That(result, Is.InstanceOf<NotFoundObjectResult>(), "Expected NotFoundObjectResult when payment does not exist.");
        }

        [Test]
        public async Task Delete_ReturnsInternalServerError_WhenExceptionThrown()
        {
            // Arrange
            int id = 1;
            _mockPaymentService.Setup(service => service.GetByIdAsync(id)).ThrowsAsync(new Exception("Service error"));

            // Act
            var result = await _controller.Delete(id);

            // Assert
            Assert.That(result, Is.InstanceOf<ObjectResult>(), "Expected ObjectResult.");
            var objectResult = result as ObjectResult;
            Assert.That(objectResult.StatusCode, Is.EqualTo(500), "Expected status code 500 (Internal Server Error).");
        }

        [Test]
        public async Task Delete_ReturnsNoContent_WhenPaymentIsDeleted()
        {
            // Arrange
            int id = 1;
            var payment = new PaymentDTO { PaymentId = id, Amount = 200 };
            _mockPaymentService.Setup(service => service.GetByIdAsync(id)).ReturnsAsync(payment);
            _mockPaymentService.Setup(service => service.DeleteAsync(id)).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.Delete(id);

            // Assert
            Assert.That(result, Is.InstanceOf<NoContentResult>(), "Expected NoContentResult when payment is deleted.");
        }
    }
}
