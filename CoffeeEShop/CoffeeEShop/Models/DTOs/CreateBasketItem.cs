namespace CoffeeEShop.Models.DTOs;

public class CreateBasketItem
{
    public int ClientId { get; set; }
    public int ProductId { get; set; }
    public int Quantity { get; set; }
}
