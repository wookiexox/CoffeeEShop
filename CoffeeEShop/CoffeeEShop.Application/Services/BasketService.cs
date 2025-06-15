using CoffeeEShop.Application.Services.Interfaces;
using CoffeeEShop.Core.Models;
using CoffeeEShop.Core.Models.DTOs;
using CoffeeEShop.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace CoffeeEShop.Application.Services;

public class BasketService : IBasketService
{
    private readonly ApplicationDbContext _context;

    public BasketService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<BasketItem>> GetBasketByClientIdAsync(int clientId)
    {
        return await _context.BasketItems
            .Where(bi => bi.ClientId == clientId)
            .Include(bi => bi.Product)
            .ThenInclude(p => p.Category)
            .ToListAsync();
    }

    public async Task<BasketItem?> GetBasketItemByIdAsync(int id)
    {
        return await _context.BasketItems
            .Include(bi => bi.Product)
            .ThenInclude(p => p.Category)
            .FirstOrDefaultAsync(bi => bi.Id == id);
    }

    public async Task<BasketItem?> AddToBasketAsync(CreateBasketItemDTO dto)
    {
        var product = await _context.Products.FindAsync(dto.ProductId);
        if (product == null || !product.IsAvailable || product.StockQuantity < dto.Quantity)
            return null;

        var client = await _context.Clients.FindAsync(dto.ClientId);
        if (client == null) return null;

        var existingItem = await _context.BasketItems
            .FirstOrDefaultAsync(bi => bi.ClientId == dto.ClientId && bi.ProductId == dto.ProductId);

        if (existingItem != null)
        {
            if (product.StockQuantity < existingItem.Quantity + dto.Quantity)
                return null;
            existingItem.Quantity += dto.Quantity;
        }
        else
        {
            existingItem = new BasketItem
            {
                ClientId = dto.ClientId,
                ProductId = dto.ProductId,
                Quantity = dto.Quantity,
            };
            _context.BasketItems.Add(existingItem);
        }

        await _context.SaveChangesAsync();
        return await GetBasketItemByIdAsync(existingItem.Id);
    }

    public async Task<BasketItem?> UpdateBasketItemAsync(int id, int quantity)
    {
        var basketItem = await _context.BasketItems.FindAsync(id);
        if (basketItem == null) return null;

        var product = await _context.Products.FindAsync(basketItem.ProductId);
        if (product == null || product.StockQuantity < quantity) return null;

        basketItem.Quantity = quantity;
        await _context.SaveChangesAsync();
        return await GetBasketItemByIdAsync(id);
    }

    public async Task<bool> RemoveFromBasketAsync(int id)
    {
        var basketItem = await _context.BasketItems.FindAsync(id);
        if (basketItem == null) return false;

        _context.BasketItems.Remove(basketItem);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ClearBasketAsync(int clientId)
    {
        var basketItems = _context.BasketItems.Where(bi => bi.ClientId == clientId);
        _context.BasketItems.RemoveRange(basketItems);
        await _context.SaveChangesAsync();
        return true;
    }
}