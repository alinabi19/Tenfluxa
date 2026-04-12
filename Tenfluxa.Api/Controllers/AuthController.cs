using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Tenfluxa.Application.Services;

namespace Tenfluxa.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly JwtService _jwtService;
    private readonly IConfiguration _configuration;

    public AuthController(JwtService jwtService, IConfiguration configuration)
    {
        _jwtService = jwtService;
        _configuration = configuration;
    }

    [AllowAnonymous]
    [HttpPost("login")]
    public IActionResult Login()
    {
        var userId = Guid.NewGuid();
        var tenantId = Guid.Parse(
            _configuration["AuthSettings:DefaultTenantId"]
        );

        var token = _jwtService.GenerateToken(userId, tenantId);

        return Ok(new { token });
    }
}