using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoffeeEShop.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace CoffeeEShop.Infrastructure;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Client> Clients { get; set; }
    public DbSet<ProductCategory> Categories { get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet<BasketItem> BasketItems { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Seed initial data
        var categories = new[]
        {
            new ProductCategory { Id = 1, Name = "Brazil", Description = "Chocolate and hazelnut notes" },
            new ProductCategory { Id = 2, Name = "Columbia", Description = "Chocolate and carmel notes" },
            new ProductCategory { Id = 3, Name = "Ethiopia", Description = "Tea and herbal notes" },
            new ProductCategory { Id = 4, Name = "Kenya", Description = "Red fruits notes" }
        };
        modelBuilder.Entity<ProductCategory>().HasData(categories);

        var products = new[]
        {
            new Product { Id = 1, Name = "CoffeeLab", Description = "250g", Price = 40.00m, CategoryId = 1, IsAvailable = true, StockQuantity = 100 },
            new Product { Id = 2, Name = "Braziliana", Description = "500g", Price = 70.99m, CategoryId = 1, IsAvailable = true, StockQuantity = 80 },
            new Product { Id = 3, Name = "Qubana", Description = "1000g", Price = 120.75m, CategoryId = 1, IsAvailable = true, StockQuantity = 90 },
            new Product { Id = 4, Name = "ColCoffee", Description = "500g", Price = 52.25m, CategoryId = 2, IsAvailable = true, StockQuantity = 50 },
            new Product { Id = 5, Name = "EthCoffee", Description = "250g", Price = 21.37m, CategoryId = 3, IsAvailable = true, StockQuantity = 30 },
            new Product { Id = 6, Name = "CoffeeKenya", Description = "500g", Price = 111.33m, CategoryId = 4, IsAvailable = true, StockQuantity = 0 }
        };
        modelBuilder.Entity<Product>().HasData(products);

        var clients = new[]
        {
            new Client { Id = 1, FirstName = "John", LastName = "Doe", Email = "john.doe@email.com", PhoneNumber = "123-456-7890" },
            new Client { Id = 2, FirstName = "Jane", LastName = "Smith", Email = "jane.smith@email.com", PhoneNumber = "098-765-4321" },
            new Client { Id = 3, FirstName = "Mike", LastName = "Johnson", Email = "mike.johnson@email.com", PhoneNumber = "555-123-4567" }
        };
        modelBuilder.Entity<Client>().HasData(clients);
    }
}