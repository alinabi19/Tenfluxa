using Microsoft.AspNetCore.Mvc;
using Tenfluxa.Application.Services;

namespace Tenfluxa.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly JwtService _jwtService;

    public AuthController(JwtService jwtService)
    {
        _jwtService = jwtService;
    }

    [HttpPost("login")]
    public IActionResult Login()
    {
        // Dummy login (for now)
        var userId = Guid.NewGuid();
        var tenantId = Guid.Parse("11111111-1111-1111-1111-111111111111");

        var token = _jwtService.GenerateToken(userId, tenantId);

        return Ok(new { token });
    }
}