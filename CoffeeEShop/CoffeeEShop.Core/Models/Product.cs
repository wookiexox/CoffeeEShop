namespace CoffeeEShop.Core.Models;

public class Product
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int CategoryId { get; set; }
    public ProductCategory? Category { get; set; }
    public bool IsAvailable { get; set; } = true;
    public int StockQuantity { get; set; }
}
