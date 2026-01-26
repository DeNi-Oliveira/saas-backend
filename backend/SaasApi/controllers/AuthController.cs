using Microsoft.AspNetCore.Mvc;
using SaasApi.DTOs;
using SaasApi.Services;

namespace SaasApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] CreateUsuarioRequest request)
    {
        try
        {
            var usuario = await _authService.RegistrarAsync(request);
            return Ok(new { message = "Usuário criado com sucesso!", id = usuario.Id });
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var token = await _authService.LoginAsync(request);

        if (token == null)
            return Unauthorized(new { error = "Email ou senha inválidos." });

        return Ok(new { token = token });
    }
}