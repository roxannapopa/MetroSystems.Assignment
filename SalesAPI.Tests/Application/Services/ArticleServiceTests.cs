using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SalesAPI.Application.DTOs;
using SalesAPI.Application.Services;
using SalesAPI.Data;
using SalesAPI.Domain.Entities;

namespace SalesAPI.Tests.Application.Services
{
    [TestFixture]
    public class ArticleServiceTests : IDisposable
    {
        private SalesApiDbContext _context;
        private ArticleService _articleService;
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
                cfg.CreateMap<ArticleDTO, Article>();
                cfg.CreateMap<Article, ArticleDTO>();
            });
            _mapper = config.CreateMapper();
            _articleService = new ArticleService(_context, _mapper);
        }

        [TearDown]
        public void TearDown()
        {
            // Clean up and dispose of the context
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        [Test]
        public async Task GetAllAsync_ReturnsAllArticles()
        {
            // Arrange
            var article = new ArticleDTO { ArticleId = 1, Name = "Test Article" };
            await _articleService.CreateAsync(article);

            // Act
            var articles = await _articleService.GetAllAsync();

            // Assert
            Assert.That(articles, Is.Not.Empty);
            Assert.That(articles.Count(), Is.EqualTo(1));
        }

        [Test]
        public async Task GetByIdAsync_ReturnsArticle_WhenArticleExists()
        {
            // Arrange
            var articleDto = new ArticleDTO { ArticleId = 1, Name = "Test Article" };
            await _articleService.CreateAsync(articleDto);

            // Act
            var article = await _articleService.GetByIdAsync(1);

            // Assert
            Assert.That(article, Is.Not.Null);
            Assert.That(article.Name, Is.EqualTo("Test Article"));
        }

        [Test]
        public async Task GetByIdAsync_ReturnsNull_WhenArticleDoesNotExist()
        {
            // Act
            var article = await _articleService.GetByIdAsync(1);

            // Assert
            Assert.That(article, Is.Null);
        }

        [Test]
        public async Task CreateAsync_ReturnsCreatedArticle()
        {
            // Arrange
            var articleDto = new ArticleDTO { ArticleId = 1, Name = "New Article" };

            // Act
            var createdArticle = await _articleService.CreateAsync(articleDto);

            // Assert
            Assert.That(createdArticle, Is.Not.Null);
            Assert.That(createdArticle.Name, Is.EqualTo("New Article"));
            Assert.That(createdArticle.ArticleId, Is.EqualTo(1)); // Assuming it gets ID 1
        }

        [Test]
        public async Task UpdateAsync_ReturnsUpdatedArticle_WhenArticleExists()
        {
            // Arrange
            var articleDto = new ArticleDTO { ArticleId = 1, Name = "Old Article" };
            await _articleService.CreateAsync(articleDto);
            var updatedArticleDto = new ArticleDTO { ArticleId = 1, Name = "Updated Article" };

            // Act
            var updatedArticle = await _articleService.UpdateAsync(updatedArticleDto);

            // Assert
            Assert.That(updatedArticle, Is.Not.Null);
            Assert.That(updatedArticle.Name, Is.EqualTo("Updated Article"));
        }

        [Test]
        public async Task UpdateAsync_ReturnsNull_WhenArticleDoesNotExist()
        {
            // Arrange
            var updatedArticleDto = new ArticleDTO { ArticleId = 1, Name = "Updated Article" };

            // Act
            var updatedArticle = await _articleService.UpdateAsync(updatedArticleDto);

            // Assert
            Assert.That(updatedArticle, Is.Null);
        }

        [Test]
        public async Task DeleteAsync_DeletesArticle_WhenArticleExists()
        {
            // Arrange
            var articleDto = new ArticleDTO { ArticleId = 1, Name = "Test Article" };
            await _articleService.CreateAsync(articleDto);

            // Act
            await _articleService.DeleteAsync(1);
            var deletedArticle = await _articleService.GetByIdAsync(1);

            // Assert
            Assert.That(deletedArticle, Is.Null);
        }

        [Test]
        public async Task DeleteAsync_DoesNothing_WhenArticleDoesNotExist()
        {
            // Arrange
            await _articleService.DeleteAsync(1); // Attempt to delete a non-existent article

            // Act
            var result = await _articleService.GetAllAsync();

            // Assert
            Assert.That(result, Is.Empty); // No articles should exist
        }

        public void Dispose()
        {
            TearDown();
        }
    }
}
