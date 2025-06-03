using CoffeeEShop.Models;
using CoffeeEShop.Services.DataStore;
using CoffeeEShop.Services.Interfaces;

namespace CoffeeEShop.Services
{
    public class ClientService : IClientService
    {
        private readonly IDataStore _dataStore;

        public ClientService(IDataStore dataStore)
        {
            _dataStore = dataStore;
        }

        public async Task<List<Client>> GetAllClientsAsync()
        {
            await Task.Delay(1); // Simulate async operation
            return _dataStore.Clients.ToList();
        }

        public async Task<Client?> GetClientByIdAsync(int id)
        {
            await Task.Delay(1); // Simulate async operation
            return _dataStore.Clients.FirstOrDefault(c => c.Id == id);
        }
    }
}