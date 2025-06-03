using CoffeeEShop.Models;

namespace CoffeeEShop.Services.Interfaces
{
    public interface IClientService
    {
        List<Client> GetAllClients();
        Client? GetClientById(int id);
    }
}
