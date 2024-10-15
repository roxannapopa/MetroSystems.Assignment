using SalesAPI.Application.DTOs;

namespace SalesAPI.Application.Services.Interfaces
{
    public interface IInventoryService
    {
        Task<bool> CheckAvailabilityAsync(InventoryDTO inventoryDto);
        Task UpdateInventoryAsync(InventoryDTO inventoryDto);
    }
}
