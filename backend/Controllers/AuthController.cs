using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers;

[ApiController]
[Route("[controller]")]
public class AuthController : ControllerBase
{
    [HttpPost("login")]
    public IActionResult Login([FromBody] LoginRequest request)
    {
        if (request.Username?.Trim().ToLowerInvariant() == "himesh" && request.Password == "123")
        {
            return Ok(new LoginResponse
            {
                Success = true,
                Message = "Login successful",
                Name = "Himeshwar"
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
}
