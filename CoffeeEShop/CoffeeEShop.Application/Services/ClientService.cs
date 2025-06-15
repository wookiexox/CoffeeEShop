using CoffeeEShop.Application.Services.Interfaces;
using CoffeeEShop.Core.Models;
using CoffeeEShop.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace CoffeeEShop.Application.Services;

public class ClientService : IClientService
{
    private readonly ApplicationDbContext _context;

    public ClientService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<Client>> GetAllClientsAsync()
    {
        return await _context.Clients.ToListAsync();
    }

    public async Task<Client?> GetClientByIdAsync(int id)
    {
        return await _context.Clients.FindAsync(id);
    }
}