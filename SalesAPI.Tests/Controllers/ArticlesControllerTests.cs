using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using SalesAPI.Application.DTOs;
using SalesAPI.Application.Services.Interfaces;
using SalesAPI.Controllers;

namespace SalesAPI.Tests.Controllers
{
    [TestFixture]
    public class ArticlesControllerTests
    {
        private ArticlesController _controller;
        private Mock<IArticleService> _mockArticleService;
        private Mock<ILogger<ArticlesController>> _mockLogger;

        [SetUp]
        public void Setup()
        {
            _mockArticleService = new Mock<IArticleService>();
            _mockLogger = new Mock<ILogger<ArticlesController>>();
            _controller = new ArticlesController(_mockArticleService.Object, _mockLogger.Object);
        }

        [Test]
        public async Task GetAll_ReturnsOk_WhenArticlesExist()
        {
            // Arrange
            var articles = new List<ArticleDTO>
            {
                new ArticleDTO { ArticleId = 1, Name = "Article 1" },
                new ArticleDTO { ArticleId = 2, Name = "Article 2" }
            };
            _mockArticleService.Setup(service => service.GetAllAsync()).ReturnsAsync(articles);

            // Act
            var result = await _controller.GetAll();

            // Assert
            Assert.That(result.Result, Is.InstanceOf<OkObjectResult>(), "Expected OkObjectResult.");
            var okResult = result.Result as OkObjectResult;
            Assert.That(okResult.StatusCode, Is.EqualTo(200));
            Assert.That(okResult.Value, Is.EqualTo(articles));
        }

        [Test]
        public async Task GetAll_ReturnsInternalServerError_WhenExceptionThrown()
        {
            // Arrange
            _mockArticleService.Setup(service => service.GetAllAsync()).ThrowsAsync(new Exception("Service error"));

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
            Assert.That(result.Result, Is.InstanceOf<BadRequestObjectResult>());
        }

        [Test]
        public async Task GetById_ReturnsNotFound_WhenArticleDoesNotExist()
        {
            // Arrange
            int nonExistentId = 99;
            _mockArticleService.Setup(service => service.GetByIdAsync(nonExistentId)).ReturnsAsync((ArticleDTO)null);

            // Act
            var result = await _controller.GetById(nonExistentId);

            // Assert
            Assert.That(result.Result, Is.InstanceOf<NotFoundResult>(), "Expected NotFoundResult when article does not exist.");
        }


        [Test]
        public async Task GetById_ReturnsInternalServerError_WhenExceptionThrown()
        {
            // Arrange
            int id = 1;
            _mockArticleService.Setup(service => service.GetByIdAsync(id)).ThrowsAsync(new Exception("Service error"));

            // Act
            var result = await _controller.GetById(id);

            // Assert
            Assert.That(result.Result, Is.InstanceOf<ObjectResult>(), "Expected ObjectResult.");
            var objectResult = result.Result as ObjectResult;
            Assert.That(objectResult.StatusCode, Is.EqualTo(500), "Expected status code 500 (Internal Server Error).");
        }

        [Test]
        public async Task Create_ReturnsCreatedAtAction_WhenArticleIsCreated()
        {
            // Arrange
            var newArticleDto = new ArticleDTO { Name = "New Article" };
            var createdArticle = new ArticleDTO { ArticleId = 1, Name = "New Article" };
            _mockArticleService.Setup(service => service.CreateAsync(newArticleDto)).ReturnsAsync(createdArticle);

            // Act
            var result = await _controller.Create(newArticleDto);

            // Assert
            Assert.That(result.Result, Is.InstanceOf<CreatedAtActionResult>());
            var createdResult = result.Result as CreatedAtActionResult;
            Assert.That(createdResult.StatusCode, Is.EqualTo(201));
            Assert.That(createdResult.Value, Is.EqualTo(createdArticle));
        }

        [Test]
        public async Task Create_ReturnsInternalServerError_WhenExceptionThrown()
        {
            // Arrange
            var newArticleDto = new ArticleDTO { Name = "New Article" };
            _mockArticleService.Setup(service => service.CreateAsync(newArticleDto)).ThrowsAsync(new Exception("Service error"));

            // Act
            var result = await _controller.Create(newArticleDto);

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
            var articleDto = new ArticleDTO { ArticleId = 2, Name = "Updated Article" };

            // Act
            var result = await _controller.Update(id, articleDto);

            // Assert
            Assert.That(result, Is.InstanceOf<BadRequestResult>());
        }

        [Test]
        public async Task Update_ReturnsNotFound_WhenArticleDoesNotExist()
        {
            // Arrange
            int id = 1;
            var articleDto = new ArticleDTO { ArticleId = id, Name = "Updated Article" };
            _mockArticleService.Setup(service => service.UpdateAsync(articleDto)).ReturnsAsync((ArticleDTO)null);

            // Act
            var result = await _controller.Update(id, articleDto);

            // Assert
            Assert.That(result, Is.InstanceOf<NotFoundResult>());
        }

        [Test]
        public async Task Update_ReturnsInternalServerError_WhenExceptionThrown()
        {
            // Arrange
            int id = 1;
            var articleDto = new ArticleDTO { ArticleId = id, Name = "Updated Article" };
            _mockArticleService.Setup(service => service.UpdateAsync(articleDto)).ThrowsAsync(new Exception("Service error"));

            // Act
            var result = await _controller.Update(id, articleDto);

            // Assert
            Assert.That(result, Is.InstanceOf<ObjectResult>(), "Expected ObjectResult.");
            var objectResult = result as ObjectResult;
            Assert.That(objectResult.StatusCode, Is.EqualTo(500), "Expected status code 500 (Internal Server Error).");
        }

        [Test]
        public async Task Delete_ReturnsNotFound_WhenArticleDoesNotExist()
        {
            // Arrange
            int id = 1;
            _mockArticleService.Setup(service => service.GetByIdAsync(id)).ReturnsAsync((ArticleDTO)null);

            // Act
            var result = await _controller.Delete(id);

            // Assert
            Assert.That(result, Is.InstanceOf<NotFoundObjectResult>(), "Expected NotFoundObjectResult when article does not exist.");
        }

        [Test]
        public async Task Delete_ReturnsInternalServerError_WhenExceptionThrown()
        {
            // Arrange
            int id = 1;
            _mockArticleService.Setup(service => service.GetByIdAsync(id)).ThrowsAsync(new Exception("Service error"));

            // Act
            var result = await _controller.Delete(id);

            // Assert
            Assert.That(result, Is.InstanceOf<ObjectResult>(), "Expected ObjectResult.");
            var objectResult = result as ObjectResult;
            Assert.That(objectResult.StatusCode, Is.EqualTo(500), "Expected status code 500 (Internal Server Error).");
        }

        [Test]
        public async Task Delete_ReturnsNoContent_WhenArticleIsDeleted()
        {
            // Arrange
            int id = 1;
            var article = new ArticleDTO { ArticleId = id, Name = "Article" };
            _mockArticleService.Setup(service => service.GetByIdAsync(id)).ReturnsAsync(article);
            _mockArticleService.Setup(service => service.DeleteAsync(id)).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.Delete(id);

            // Assert
            Assert.That(result, Is.InstanceOf<NoContentResult>());
        }
    }
}
