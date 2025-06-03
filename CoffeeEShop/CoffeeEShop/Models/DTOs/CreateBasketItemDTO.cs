namespace CoffeeEShop.Models.DTOs;

public class CreateBasketItemDTO
{
    public int ClientId { get; set; }
    public int ProductId { get; set; }
    public int Quantity { get; set; }
}
