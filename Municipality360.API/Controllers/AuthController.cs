using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Municipality360.Application.DTOs.Identity;
using Municipality360.Application.Interfaces.Services;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Municipality360.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
        => _authService = authService;

    // ══════════════════════════════════════════════════
    //  AUTHENTIFICATION
    // ══════════════════════════════════════════════════

    /// <summary>تسجيل الدخول – متاح للجميع</summary>
    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login([FromBody] LoginDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        var result = await _authService.LoginAsync(dto);
        return result.Succeeded ? Ok(result) : Unauthorized(result);
    }

    // ══════════════════════════════════════════════════
    //  إدارة المستخدمين
    // ══════════════════════════════════════════════════

    /// <summary>إنشاء حساب مستخدم جديد</summary>
    [HttpPost("users")]
    [Authorize(Roles = "SuperAdmin,Admin")]
    public async Task<IActionResult> Register([FromBody] RegisterDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        var result = await _authService.RegisterAsync(dto);
        return result.Succeeded ? Ok(result) : BadRequest(result);
    }

    /// <summary>قائمة مبسّطة (UserDto)</summary>
    [HttpGet("users/simple")]
    [Authorize(Roles = "SuperAdmin,Admin")]
    public async Task<IActionResult> GetUsersSimple()
    {
        var result = await _authService.GetUsersAsync();
        return Ok(result);
    }

    /// <summary>قائمة تفصيلية مع IsActive و EmailConfirmed و Roles</summary>
    [HttpGet("users")]
    [Authorize(Roles = "SuperAdmin,Admin")]
    public async Task<IActionResult> GetUsers()
    {
        var result = await _authService.GetUsersDetailAsync();
        return Ok(result);
    }

    /// <summary>تفاصيل مستخدم واحد بالـ Id</summary>
    [HttpGet("users/{id}")]
    [Authorize(Roles = "SuperAdmin,Admin")]
    public async Task<IActionResult> GetUser(string id)
    {
        var result = await _authService.GetUserByIdAsync(id);
        return result.Succeeded ? Ok(result) : NotFound(result);
    }

    /// <summary>حذف حساب مستخدم – SuperAdmin فقط</summary>
    [HttpDelete("users/{id}")]
    [Authorize(Roles = "SuperAdmin")]
    public async Task<IActionResult> DeleteUser(string id)
    {
        var result = await _authService.DeleteUserAsync(id);
        return result.Succeeded ? Ok(result) : BadRequest(result);
    }

    /// <summary>تفعيل أو إيقاف حساب – SuperAdmin أو Admin</summary>
    [HttpPatch("users/toggle")]
    [Authorize(Roles = "SuperAdmin,Admin")]
    public async Task<IActionResult> ToggleActive([FromBody] ToggleUserDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        var result = await _authService.ToggleUserActiveAsync(dto);
        return result.Succeeded ? Ok(result) : BadRequest(result);
    }

    /// <summary>إعادة تعيين كلمة المرور – SuperAdmin فقط</summary>
    [HttpPost("users/reset-password")]
    [Authorize(Roles = "SuperAdmin")]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        var result = await _authService.ResetPasswordAsync(dto);
        return result.Succeeded ? Ok(result) : BadRequest(result);
    }
    // ══════════════════════════════════════════════════
    //  SELF-UPDATE  — كل مستخدم مسجّل الدخول
    // ══════════════════════════════════════════════════

    /// <summary>تحديث الاسم واللقب للمستخدم الحالي</summary>
    [HttpPut("me/profile")]
    [Authorize]
    public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)
                  ?? User.FindFirstValue(JwtRegisteredClaimNames.Sub);
        if (string.IsNullOrEmpty(userId)) return Unauthorized();
        var result = await _authService.UpdateProfileAsync(userId, dto);
        return result.Succeeded ? Ok(result) : BadRequest(result);
    }

    /// <summary>تغيير كلمة المرور للمستخدم الحالي</summary>
    [HttpPut("me/password")]
    [Authorize]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)
                  ?? User.FindFirstValue(JwtRegisteredClaimNames.Sub);
        if (string.IsNullOrEmpty(userId)) return Unauthorized();
        var result = await _authService.ChangePasswordAsync(userId, dto);
        return result.Succeeded ? Ok(result) : BadRequest(result);
    }

    // ══════════════════════════════════════════════════
    //  إدارة الأدوار
    // ══════════════════════════════════════════════════

    /// <summary>قائمة الأدوار المتاحة في النظام</summary>
    [HttpGet("roles")]
    [Authorize(Roles = "SuperAdmin,Admin")]
    public async Task<IActionResult> GetRoles()
    {
        var result = await _authService.GetAllRolesAsync();
        return Ok(result);
    }

    /// <summary>تعيين دور لمستخدم – SuperAdmin فقط</summary>
    [HttpPost("roles/assign")]
    [Authorize(Roles = "SuperAdmin")]
    public async Task<IActionResult> AssignRole([FromBody] AssignRoleDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        var result = await _authService.AssignRoleAsync(dto);
        return result.Succeeded ? Ok(result) : BadRequest(result);
    }

    /// <summary>إلغاء دور من مستخدم – SuperAdmin فقط</summary>
    [HttpPost("roles/remove")]
    [Authorize(Roles = "SuperAdmin")]
    public async Task<IActionResult> RemoveRole([FromBody] AssignRoleDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        var result = await _authService.RemoveRoleAsync(dto);
        return result.Succeeded ? Ok(result) : BadRequest(result);
    }
}