using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Municipality360.Application.DTOs.Identity;
using Municipality360.Application.Interfaces.Services;

namespace Municipality360.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService) => _authService = authService;

    /// <summary>Connexion utilisateur</summary>
    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login([FromBody] LoginDto dto)
    {
        var result = await _authService.LoginAsync(dto);
        return result.Succeeded ? Ok(result) : Unauthorized(result);
    }

    /// <summary>Inscription d'un nouvel utilisateur</summary>
    [HttpPost("register")]
    [Authorize(Roles = "SuperAdmin,Admin")]
    public async Task<IActionResult> Register([FromBody] RegisterDto dto)
    {
        var result = await _authService.RegisterAsync(dto);
        return result.Succeeded ? Ok(result) : BadRequest(result);
    }

    /// <summary>Liste des utilisateurs</summary>
    [HttpGet("users")]
    [Authorize(Roles = "SuperAdmin,Admin")]
    public async Task<IActionResult> GetUsers()
    {
        var result = await _authService.GetUsersAsync();
        return Ok(result);
    }

    /// <summary>Supprimer un utilisateur</summary>
    [HttpDelete("users/{id}")]
    [Authorize(Roles = "SuperAdmin")]
    public async Task<IActionResult> DeleteUser(string id)
    {
        var result = await _authService.DeleteUserAsync(id);
        return result.Succeeded ? Ok(result) : BadRequest(result);
    }
}
