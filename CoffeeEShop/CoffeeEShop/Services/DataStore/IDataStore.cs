using CoffeeEShop.Models;

namespace CoffeeEShop.Services.DataStore
{
    public interface IDataStore
    {
        List<Client> Clients { get; }
        int GetNextClientId();

        List<ProductCategory> Categories { get; }
        int GetNextCategoryId();

        List<Product> Products { get; }
        int GetNextProductId();

        List<BasketItem> BasketItems { get; }
        int GetNextBasketItemId();
    }
}
