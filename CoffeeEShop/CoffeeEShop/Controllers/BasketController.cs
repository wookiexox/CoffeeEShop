using CoffeeEShop.Core;
using CoffeeEShop.Core.Models;
using CoffeeEShop.Core.Models.DTOs;
using CoffeeEShop.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CoffeeEShop.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BasketController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public BasketController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet("client/{clientId}")]
    public async Task<ActionResult<IEnumerable<BasketItem>>> GetBasketByClientIdAsync(int clientId)
    {
        var client = await _context.Clients.FindAsync(clientId);
        if (client == null)
            return NotFound($"Client with ID {clientId} not found.");

        var basketItems = await _context.BasketItems
            .Where(bi => bi.ClientId == clientId)
            .Include(bi => bi.Product)
            .ThenInclude(p => p.Category)
            .ToListAsync();

        return Ok(basketItems);
    }

    [HttpGet("{id}", Name = "GetBasketItemById")]
    public async Task<ActionResult<BasketItem>> GetBasketItemByIdAsync(int id)
    {
        var basketItem = await _context.BasketItems
            .Include(bi => bi.Product)
            .ThenInclude(p => p.Category)
            .FirstOrDefaultAsync(bi => bi.Id == id);

        if (basketItem == null)
            return NotFound($"Basket item with ID {id} not found.");

        return Ok(basketItem);
    }

    [HttpPost]
    public async Task<ActionResult<BasketItem>> AddToBasketAsync([FromBody] CreateBasketItemDTO dto)
    {
        if (dto.Quantity <= 0)
            return BadRequest("Quantity must be greater than 0.");

        var client = await _context.Clients.FindAsync(dto.ClientId);
        if (client == null)
            return BadRequest("Client not found.");

        var product = await _context.Products.FindAsync(dto.ProductId);
        if (product == null || !product.IsAvailable || product.StockQuantity < dto.Quantity)
            return BadRequest("Unable to add item. Product is unavailable or has insufficient stock.");

        // Check if item already exists in basket to update quantity instead of adding a new line item.
        var existingItem = await _context.BasketItems
            .FirstOrDefaultAsync(bi => bi.ClientId == dto.ClientId && bi.ProductId == dto.ProductId);

        if (existingItem != null)
        {
            if (product.StockQuantity < existingItem.Quantity + dto.Quantity)
                return BadRequest("Insufficient stock to add more of this item.");

            existingItem.Quantity += dto.Quantity;
            await _context.SaveChangesAsync();
            return Ok(existingItem);
        }

        var basketItem = new BasketItem
        {
            ClientId = dto.ClientId,
            ProductId = dto.ProductId,
            Quantity = dto.Quantity,
            AddedAt = DateTime.Now
        };

        _context.BasketItems.Add(basketItem);
        await _context.SaveChangesAsync();

        return CreatedAtRoute("GetBasketItemById", new { id = basketItem.Id }, basketItem);
    }

    [HttpPut("{id}/quantity")]
    public async Task<ActionResult<BasketItem>> UpdateBasketItemQuantityAsync(int id, [FromBody] int quantity)
    {
        if (quantity <= 0)
            return BadRequest("Quantity must be greater than 0.");

        var basketItem = await _context.BasketItems.FindAsync(id);
        if (basketItem == null)
            return NotFound($"Basket item with ID {id} not found.");

        var product = await _context.Products.FindAsync(basketItem.ProductId);
        if (product == null || product.StockQuantity < quantity)
            return BadRequest("Insufficient stock.");

        basketItem.Quantity = quantity;
        await _context.SaveChangesAsync();

        return Ok(basketItem);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> RemoveFromBasketAsync(int id)
    {
        var basketItem = await _context.BasketItems.FindAsync(id);
        if (basketItem == null)
            return NotFound($"Basket item with ID {id} not found.");

        _context.BasketItems.Remove(basketItem);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    [HttpDelete("client/{clientId}/clear")]
    public async Task<ActionResult> ClearBasketAsync(int clientId)
    {
        var client = await _context.Clients.FindAsync(clientId);
        if (client == null)
            return NotFound($"Client with ID {clientId} not found.");

        var basketItems = _context.BasketItems.Where(bi => bi.ClientId == clientId);
        _context.BasketItems.RemoveRange(basketItems);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}