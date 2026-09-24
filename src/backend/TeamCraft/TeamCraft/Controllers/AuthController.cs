using Microsoft.AspNetCore.Mvc;
using TeamCraft.Application.DTOs.Auth;
using TeamCraft.Application.Services.Interfaces;

namespace TeamCraft.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("login")]
    public async Task<ActionResult<AuthResponseDto>> Login(LoginDto dto)
    {
        var authResponse = await _authService.LoginAsync(dto);

        if (authResponse == null)
        {
            return Unauthorized();
        }

        return Ok(authResponse);
    }

    [HttpPost("register")]
    public async Task<ActionResult<AuthResponseDto>> Register(RegisterDto dto)
    {
        var authResponse = await _authService.RegisterAsync(dto);

        if (authResponse == null)
        {
            return BadRequest();
        }

        return Ok(authResponse);
    }
}

