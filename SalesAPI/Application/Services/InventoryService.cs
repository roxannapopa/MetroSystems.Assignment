using Microsoft.EntityFrameworkCore;
using SalesAPI.Application.DTOs;
using SalesAPI.Application.Services.Interfaces;
using SalesAPI.Data;

namespace SalesAPI.Application.Services
{
    public class InventoryService : IInventoryService
    {
        private readonly SalesApiDbContext _context;

        public InventoryService(SalesApiDbContext context)
        {
            _context = context;
        }

        public async Task<bool> CheckAvailabilityAsync(InventoryDTO inventoryDto)
        {
            var inventory = await _context.ArticleInventories.FirstOrDefaultAsync(i => i.ArticleId == inventoryDto.ArticleId);
            return inventory != null && inventory.Quantity >= inventoryDto.Quantity;
        }        

        public async Task UpdateInventoryAsync(InventoryDTO inventoryDto)
        {
            var inventory = await _context.ArticleInventories.FirstOrDefaultAsync(i => i.ArticleId == inventoryDto.ArticleId);
            if (inventory != null)
            {
                inventory.Quantity -= inventoryDto.Quantity;
                await _context.SaveChangesAsync();
            }
        }
      
    }

}
