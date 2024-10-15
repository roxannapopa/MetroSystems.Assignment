using Microsoft.EntityFrameworkCore;
using SalesAPI.Application.DTOs;
using SalesAPI.Application.Services;
using SalesAPI.Data;
using SalesAPI.Domain.Entities;

namespace SalesAPI.Tests.Application.Services
{
    [TestFixture]
    public class InventoryServiceTests : IDisposable
    {
        private SalesApiDbContext _context;
        private InventoryService _inventoryService;

        [SetUp]
        public void Setup()
        {
            // Set up in-memory database options
            var options = new DbContextOptionsBuilder<SalesApiDbContext>()
                .UseInMemoryDatabase("TestDatabase")
                .Options;

            _context = new SalesApiDbContext(options);
            _inventoryService = new InventoryService(_context);

            // Seed data for testing
            var inventoryItem = new Inventory { ArticleId = 1, Quantity = 10 };
            _context.ArticleInventories.Add(inventoryItem);
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
        public async Task CheckAvailabilityAsync_ReturnsTrue_WhenInventoryIsSufficient()
        {
            // Arrange
            var inventoryDto = new InventoryDTO { ArticleId = 1, Quantity = 5 };

            // Act
            var isAvailable = await _inventoryService.CheckAvailabilityAsync(inventoryDto);

            // Assert
            Assert.That(isAvailable, Is.True);
        }

        [Test]
        public async Task CheckAvailabilityAsync_ReturnsFalse_WhenInventoryIsInsufficient()
        {
            // Arrange
            var inventoryDto = new InventoryDTO { ArticleId = 1, Quantity = 15 };

            // Act
            var isAvailable = await _inventoryService.CheckAvailabilityAsync(inventoryDto);

            // Assert
            Assert.That(isAvailable, Is.False);
        }

        [Test]
        public async Task CheckAvailabilityAsync_ReturnsFalse_WhenInventoryDoesNotExist()
        {
            // Arrange
            var inventoryDto = new InventoryDTO { ArticleId = 2, Quantity = 1 }; // ArticleId 2 does not exist

            // Act
            var isAvailable = await _inventoryService.CheckAvailabilityAsync(inventoryDto);

            // Assert
            Assert.That(isAvailable, Is.False);
        }

        [Test]
        public async Task UpdateInventoryAsync_DecreasesQuantity_WhenInventoryExists()
        {
            // Arrange
            var inventoryDto = new InventoryDTO { ArticleId = 1, Quantity = 5 };

            // Act
            await _inventoryService.UpdateInventoryAsync(inventoryDto);
            var inventory = await _context.ArticleInventories.FirstOrDefaultAsync(i => i.ArticleId == 1);

            // Assert
            Assert.That(inventory, Is.Not.Null);
            Assert.That(inventory.Quantity, Is.EqualTo(5)); // Quantity should be decreased from 10 to 5
        }

        [Test]
        public async Task UpdateInventoryAsync_DoesNothing_WhenInventoryDoesNotExist()
        {
            // Arrange
            var inventoryDto = new InventoryDTO { ArticleId = 2, Quantity = 5 }; // ArticleId 2 does not exist

            // Act
            await _inventoryService.UpdateInventoryAsync(inventoryDto);
            var inventory = await _context.ArticleInventories.FirstOrDefaultAsync(i => i.ArticleId == 2);

            // Assert
            Assert.That(inventory, Is.Null); // No inventory should exist for ArticleId 2
        }

        public void Dispose()
        {
            TearDown();
        }
    }
}
