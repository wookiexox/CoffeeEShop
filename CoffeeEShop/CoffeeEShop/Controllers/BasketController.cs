using CoffeeEShop.Application.Services.Interfaces;
using CoffeeEShop.Core.Models.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace CoffeeEShop.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BasketController : ControllerBase
{
    private readonly IBasketService _basketService;
    private readonly IClientService _clientService;

    public BasketController(IBasketService basketService, IClientService clientService)
    {
        _basketService = basketService;
        _clientService = clientService;
    }

    [HttpGet("client/{clientId}")]
    public async Task<ActionResult> GetBasketByClientIdAsync(int clientId)
    {
        var client = await _clientService.GetClientByIdAsync(clientId);
        if (client == null)
            return NotFound($"Client with ID {clientId} not found.");

        var basketItems = await _basketService.GetBasketByClientIdAsync(clientId);
        return Ok(basketItems);
    }

    [HttpGet("{id}", Name = "GetBasketItemById")]
    public async Task<ActionResult<CreateBasketItemDTO>> GetBasketItemByIdAsync(int id)
    {
        var basketItem = await _basketService.GetBasketItemByIdAsync(id);
        if (basketItem == null)
            return NotFound($"Basket item with ID {id} not found.");

        return Ok(basketItem);
    }

    [HttpPost]
    public async Task<ActionResult> AddToBasketAsync([FromBody] CreateBasketItemDTO dto)
    {
        if (dto.Quantity <= 0)
            return BadRequest("Quantity must be greater than 0.");

        var basketItem = await _basketService.AddToBasketAsync(dto);
        if (basketItem == null)
            return BadRequest("Unable to add item to basket. Check if client exists, product is available, and sufficient stock.");

        return CreatedAtRoute("GetBasketItemById", new { id = basketItem.Id }, basketItem);
    }

    [HttpPut("{id}/quantity")]
    public async Task<ActionResult> UpdateBasketItemQuantityAsync(int id, [FromBody] int quantity)
    {
        if (quantity <= 0)
            return BadRequest("Quantity must be greater than 0.");

        var basketItem = await _basketService.UpdateBasketItemAsync(id, quantity);
        if (basketItem == null)
            return NotFound($"Basket item with ID {id} not found or insufficient stock.");

        return Ok(basketItem);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> RemoveFromBasketAsync(int id)
    {
        var result = await _basketService.RemoveFromBasketAsync(id);
        if (!result)
            return NotFound($"Basket item with ID {id} not found.");

        return NoContent();
    }

    [HttpDelete("client/{clientId}/clear")]
    public async Task<ActionResult> ClearBasketAsync(int clientId)
    {
        var client = await _clientService.GetClientByIdAsync(clientId);
        if (client == null)
            return NotFound($"Client with ID {clientId} not found.");

        await _basketService.ClearBasketAsync(clientId);
        return NoContent();
    }
}