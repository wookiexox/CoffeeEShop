using CoffeeEShop.Core.Models;
using CoffeeEShop.Core.Models.DTOs;

namespace CoffeeEShop.Application.Services.Interfaces
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