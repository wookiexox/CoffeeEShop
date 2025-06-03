using CoffeeEShop.Models;
using CoffeeEShop.Models.DTOs;

namespace CoffeeEShop.Services.Interfaces
{
    public interface IBasketService
    {
        public interface IBasketService
        {
            List<BasketItem> GetBasketByClientId(int clientId);
            BasketItem? GetBasketItemById(int id);
            BasketItem? AddToBasket(CreateBasketItemDTO dto);
            BasketItem? UpdateBasketItem(int id, int quantity);
            bool RemoveFromBasket(int id);
            bool ClearBasket(int clientId);
        }
    }
}
