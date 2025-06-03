using CoffeeEShop.Models;

namespace CoffeeEShop.Services.Interfaces
{
    public interface IClientService
    {
        Task<List<Client>> GetAllClientsAsync();
        Task<Client?> GetClientByIdAsync(int id);
    }
}