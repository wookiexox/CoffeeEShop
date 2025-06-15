using CoffeeEShop.Core.Models;

namespace CoffeeEShop.Application.Services.Interfaces
{
    public interface IClientService
    {
        Task<List<Client>> GetAllClientsAsync();
        Task<Client?> GetClientByIdAsync(int id);
    }
}