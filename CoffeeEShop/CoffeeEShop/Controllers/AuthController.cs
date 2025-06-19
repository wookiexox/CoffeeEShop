using CoffeeEShop.Core.DTOs.Auth;
using CoffeeEShop.Core.Models;
using CoffeeEShop.Core.Models.DTOs;
using CoffeeEShop.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace CoffeeEShop.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly IConfiguration _configuration;

    public AuthController(ApplicationDbContext context, IConfiguration configuration)
    {
        _context = context;
        _configuration = configuration;
    }

    [HttpPost("register")]
    public async Task<ActionResult<Client>> Register(UserRegisterDto request)
    {
        if (await _context.Clients.AnyAsync(c => c.Email == request.Email))
        {
            return BadRequest("User with this email already exists.");
        }

        CreatePasswordHash(request.Password, out byte[] passwordHash, out byte[] passwordSalt);

        var client = new Client
        {
            FirstName = request.FirstName,
            LastName = request.LastName,
            Email = request.Email,
            PhoneNumber = request.PhoneNumber,
            PasswordHash = passwordHash,
            PasswordSalt = passwordSalt,
            Role = "User" // Default role
        };

        _context.Clients.Add(client);
        await _context.SaveChangesAsync();

        return Ok(client);
    }

    [HttpPost("login")]
    public async Task<ActionResult<string>> Login(UserLoginDto request)
    {
        var client = await _context.Clients.FirstOrDefaultAsync(c => c.Email == request.Email);
        if (client == null || !VerifyPasswordHash(request.Password, client.PasswordHash, client.PasswordSalt))
        {
            return BadRequest("Invalid credentials.");
        }

        string token = CreateToken(client);
        return Ok(token);
    }

    private string CreateToken(Client client)
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, client.Id.ToString()),
            new Claim(ClaimTypes.Name, client.Email),
            new Claim(ClaimTypes.Role, client.Role)
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration.GetSection("Jwt:Key").Value!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.Now.AddDays(1),
            SigningCredentials = creds,
            Issuer = _configuration.GetSection("Jwt:Issuer").Value,
            Audience = _configuration.GetSection("Jwt:Audience").Value
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);

        return tokenHandler.WriteToken(token);
    }

    private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
    {
        using var hmac = new HMACSHA512();
        passwordSalt = hmac.Key;
        passwordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
    }

    private bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
    {
        using var hmac = new HMACSHA512(passwordSalt);
        var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
        return computedHash.SequenceEqual(passwordHash);
    }

    [HttpPost("forgot-password")]
    public async Task<IActionResult> ForgotPassword(ForgotPasswordDto request)
    {
        var client = await _context.Clients.FirstOrDefaultAsync(c => c.Email == request.Email);
        if (client == null)
        {
            return Ok("If an account with this email exists, a password reset link has been sent.");
        }

        var resetToken = Convert.ToHexString(RandomNumberGenerator.GetBytes(64));

        client.PasswordResetToken = resetToken;
        client.ResetTokenExpires = DateTime.Now.AddMinutes(15);

        await _context.SaveChangesAsync();

        // --- In a real application, you would email the token to the user here ---
        // Example: await _emailService.SendPasswordResetEmailAsync(client.Email, resetToken);

        // For testing, we return the token directly.
        return Ok($"Password reset token (for testing): {resetToken}");
    }

    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPassword(ResetPasswordDto request)
    {
        var client = await _context.Clients.FirstOrDefaultAsync(c => c.PasswordResetToken == request.Token);

        if (client == null || client.ResetTokenExpires < DateTime.Now)
        {
            return BadRequest("Invalid or expired token.");
        }

        CreatePasswordHash(request.NewPassword, out byte[] passwordHash, out byte[] passwordSalt);

        client.PasswordHash = passwordHash;
        client.PasswordSalt = passwordSalt;

        client.PasswordResetToken = null;
        client.ResetTokenExpires = null;

        await _context.SaveChangesAsync();

        return Ok("Password has been reset successfully.");
    }
}
