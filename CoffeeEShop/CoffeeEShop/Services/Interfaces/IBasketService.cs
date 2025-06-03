using CoffeeEShop.Models;
using CoffeeEShop.Models.DTOs;

namespace CoffeeEShop.Services.Interfaces
{
    public interface IBasketService
    {
        Task<List<BasketItem>> GetBasketByClientIdAsync(int clientId);
        Task<BasketItem?> GetBasketItemByIdAsync(int id);
        Task<BasketItem?> AddToBasketAsync(CreateBasketItemDTO dto);
        Task<BasketItem?> UpdateBasketItemAsync(int id, int quantity);
        Task<bool> RemoveFromBasketAsync(int id);
        Task<bool> ClearBasketAsync(int clientId);
    }
}