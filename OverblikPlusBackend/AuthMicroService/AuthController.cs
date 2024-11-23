using Microsoft.AspNetCore.Mvc;

namespace AuthMicroService;

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
    public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
    {
        var token = await _authService.LoginAsync(loginDto);
        return Ok(new { Token = token });
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
    {
        var result = await _authService.RegisterAsync(registerDto);
        if (!result.Success) return BadRequest(result.Errors);

        return Ok("User registered successfully.");
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> RefreshToken([FromBody] string token)
    {
        var newToken = await _authService.RefreshTokenAsync(token);
        return Ok(new { Token = newToken });
    }
}