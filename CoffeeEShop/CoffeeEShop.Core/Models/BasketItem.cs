namespace CoffeeEShop.Core.Models;

public class BasketItem
{
    public int Id { get; set; }
    public int ClientId { get; set; }
    public int ProductId { get; set; }
    public Product? Product { get; set; }
    public int Quantity { get; set; }
    public DateTime AddedAt { get; set; } = DateTime.Now;
}
