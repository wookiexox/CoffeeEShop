namespace CoffeeEShop.Core.Models.DTOs;

public class CreateBasketItemDto
{
    public int ClientId { get; set; }
    public int ProductId { get; set; }
    public int Quantity { get; set; }
}
