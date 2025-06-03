using CoffeeEShop.Models.DTOs;
using CoffeeEShop.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CoffeeEShop.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ClientsController : ControllerBase
{
    private readonly IClientService _clientService;

    public ClientsController(IClientService clientService)
    {
        _clientService = clientService;
    }

    [HttpGet]
    public ActionResult GetAllClients()
    {
        var clients = _clientService.GetAllClients();
        return Ok(clients);
    }

    [HttpGet("{id}")]
    public ActionResult GetClientById(int id)
    {
        var client = _clientService.GetClientById(id);
        if (client == null)
            return NotFound($"Client with ID {id} not found.");

        return Ok(client);
    }
}