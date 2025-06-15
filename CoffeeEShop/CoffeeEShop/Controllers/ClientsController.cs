using CoffeeEShop.Core.Models;
using CoffeeEShop.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CoffeeEShop.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ClientsController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public ClientsController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Client>>> GetAllClientsAsync()
    {
        return Ok(await _context.Clients.ToListAsync());
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Client>> GetClientByIdAsync(int id)
    {
        var client = await _context.Clients.FindAsync(id);
        if (client == null) return NotFound($"Client with ID {id} not found.");
        return Ok(client);
    }
}