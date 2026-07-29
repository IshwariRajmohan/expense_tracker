using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using backend.Data;
using backend.Models;

namespace backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public AuthController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        if (request == null || string.IsNullOrEmpty(request.Username) || string.IsNullOrEmpty(request.Password))
        {
            return BadRequest(new LoginResponse
            {
                Success = false,
                Message = "Username and password are required."
            });
        }

        var username = request.Username.Trim().ToLower();
        var user = await _context.UserCredentials
            .FirstOrDefaultAsync(u => u.Username.ToLower() == username && u.Password == request.Password);

        if (user != null)
        {
            return Ok(new LoginResponse
            {
                Success = true,
                Message = "Login successful",
                Name = user.DisplayName,
                Role = user.Role
            });
        }

        return Unauthorized(new LoginResponse
        {
            Success = false,
            Message = "Invalid username or password"
        });
    }
}

public class LoginRequest
{
    [JsonPropertyName("username")]
    public string? Username { get; set; }

    [JsonPropertyName("password")]
    public string? Password { get; set; }
}

public class LoginResponse
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public string? Name { get; set; }
    public string? Role { get; set; }
}
