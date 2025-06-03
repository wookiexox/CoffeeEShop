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

        public List<Client> GetAllClients()
        {
            return _dataStore.Clients.ToList();
        }

        public Client? GetClientById(int id)
        {
            return _dataStore.Clients.FirstOrDefault(c => c.Id == id);
        }
    }

}
