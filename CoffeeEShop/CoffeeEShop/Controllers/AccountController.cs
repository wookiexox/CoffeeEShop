using CoffeeEShop.Core.DTOs.Account;
using CoffeeEShop.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CoffeeEShop.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class AccountController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public AccountController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpPut("me")]
    public async Task<IActionResult> UpdateMyAccount([FromBody] AccountEditDTO request)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null)
        {
            return Unauthorized();
        }

        var client = await _context.Clients.FindAsync(int.Parse(userId));
        if (client == null)
        {
            return NotFound("User not found.");
        }

        client.FirstName = request.FirstName;
        client.LastName = request.LastName;
        client.PhoneNumber = request.PhoneNumber;

        await _context.SaveChangesAsync();

        return Ok("Account updated successfully.");
    }
}
