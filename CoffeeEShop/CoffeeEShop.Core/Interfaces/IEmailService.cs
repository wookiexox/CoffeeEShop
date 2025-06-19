using CoffeeEShop.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoffeeEShop.Core.Interfaces;

public interface IEmailService
{
    Task SendOrderConfirmationEmailAsync(Order order);
}
