using CoffeeEShop.Core.Interfaces;
using CoffeeEShop.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoffeeEShop.Infrastructure.Services;

public class EmailService : IEmailService
{
    public Task SendOrderConfirmationEmailAsync(Order order)
    {
        // just simulation of sending email
        Console.WriteLine("---- SIMULATING EMAIL ----");
        Console.WriteLine($"To: User ID {order.ClientId}");
        Console.WriteLine("Subject: Your CoffeeEShop Order Confirmation");
        Console.WriteLine($"Thank you for your order! Order ID: {order.Id}");
        Console.WriteLine($"Total Price: ${order.TotalPrice}");
        Console.WriteLine("--------------------------");

        return Task.CompletedTask;
    }
}
