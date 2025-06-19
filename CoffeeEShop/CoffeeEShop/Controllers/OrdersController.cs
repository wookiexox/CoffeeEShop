using CoffeeEShop.Core;
using CoffeeEShop.Core.Interfaces;
using CoffeeEShop.Core.Models;
using CoffeeEShop.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace CoffeeEShop.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class OrdersController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly IEmailService _emailService;

    public OrdersController(ApplicationDbContext context, IEmailService emailService)
    {
        _context = context;
        _emailService = emailService;
    }

    [HttpPost("checkout")]
    public async Task<ActionResult<Order>> Checkout()
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var basketItems = await _context.BasketItems
            .Include(bi => bi.Product)
            .Where(bi => bi.ClientId == userId)
            .ToListAsync();

        if (!basketItems.Any())
        {
            return BadRequest("Your basket is empty.");
        }

        await using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            var newOrder = new Order { ClientId = userId, TotalPrice = 0 };

            foreach (var item in basketItems)
            {
                if (item.Product == null || item.Product.StockQuantity < item.Quantity)
                {
                    await transaction.RollbackAsync();
                    return BadRequest($"Insufficient stock for product: {item.Product?.Name}");
                }

                item.Product.StockQuantity -= item.Quantity;

                var orderItem = new OrderItem
                {
                    ProductId = item.ProductId,
                    ProductName = item.Product.Name,
                    Price = item.Product.Price,
                    Quantity = item.Quantity
                };
                newOrder.OrderItems.Add(orderItem);
                newOrder.TotalPrice += item.Product.Price * item.Quantity;
            }

            _context.Orders.Add(newOrder);

            _context.BasketItems.RemoveRange(basketItems);

            await _context.SaveChangesAsync();

            await transaction.CommitAsync();

            await _emailService.SendOrderConfirmationEmailAsync(newOrder);

            return Ok(newOrder);
        }
        catch
        {
            await transaction.RollbackAsync();
            return StatusCode(500, "An error occurred while processing your order.");
        }
    }
}
