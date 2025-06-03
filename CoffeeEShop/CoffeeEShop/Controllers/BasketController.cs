using CoffeeEShop.Models.DTOs;
using CoffeeEShop.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CoffeeEShop.Controllers;

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
    public ActionResult GetBasketByClientId(int clientId)
    {
        var client = _clientService.GetClientById(clientId);
        if (client == null)
            return NotFound($"Client with ID {clientId} not found.");

        var basketItems = _basketService.GetBasketByClientId(clientId);
        return Ok(basketItems);
    }

    [HttpGet("{id}")]
    public ActionResult GetBasketItemById(int id)
    {
        var basketItem = _basketService.GetBasketItemById(id);
        if (basketItem == null)
            return NotFound($"Basket item with ID {id} not found.");

        return Ok(basketItem);
    }

    [HttpPost]
    public ActionResult AddToBasket([FromBody] CreateBasketItemDTO dto)
    {
        if (dto.Quantity <= 0)
            return BadRequest("Quantity must be greater than 0.");

        var basketItem = _basketService.AddToBasket(dto);
        if (basketItem == null)
            return BadRequest("Unable to add item to basket. Check if client exists, product is available, and sufficient stock.");

        return CreatedAtAction(nameof(GetBasketItemById), new { id = basketItem.Id }, basketItem);
    }

    [HttpPut("{id}/quantity")]
    public ActionResult UpdateBasketItemQuantity(int id, [FromBody] int quantity)
    {
        if (quantity <= 0)
            return BadRequest("Quantity must be greater than 0.");

        var basketItem = _basketService.UpdateBasketItem(id, quantity);
        if (basketItem == null)
            return NotFound($"Basket item with ID {id} not found or insufficient stock.");

        return Ok(basketItem);
    }

    [HttpDelete("{id}")]
    public ActionResult RemoveFromBasket(int id)
    {
        var result = _basketService.RemoveFromBasket(id);
        if (!result)
            return NotFound($"Basket item with ID {id} not found.");

        return NoContent();
    }

    [HttpDelete("client/{clientId}/clear")]
    public ActionResult ClearBasket(int clientId)
    {
        var client = _clientService.GetClientById(clientId);
        if (client == null)
            return NotFound($"Client with ID {clientId} not found.");

        _basketService.ClearBasket(clientId);
        return NoContent();
    }
}