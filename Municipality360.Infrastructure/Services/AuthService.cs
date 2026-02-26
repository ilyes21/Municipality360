using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Municipality360.Application.Common;
using Municipality360.Application.DTOs.Identity;
using Municipality360.Application.Interfaces.Services;
using Municipality360.Infrastructure.Identity;

namespace Municipality360.Infrastructure.Services;

public class AuthService : IAuthService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IConfiguration _configuration;

    public AuthService(
        UserManager<ApplicationUser> userManager,
        IConfiguration configuration)
    {
        _userManager = userManager;
        _configuration = configuration;
    }

    // ══════════════════════════════════════════════════
    //  AUTHENTIFICATION
    // ══════════════════════════════════════════════════

    public async Task<Result<AuthResponseDto>> LoginAsync(LoginDto dto)
    {
        var user = await _userManager.FindByEmailAsync(dto.Email);

        // التحقق من وجود المستخدم وكلمة المرور
        if (user is null || !await _userManager.CheckPasswordAsync(user, dto.Password))
            return Result<AuthResponseDto>.Failure("البريد الإلكتروني أو كلمة المرور غير صحيحة.");

        // التحقق من أن الحساب غير موقوف
        if (user.LockoutEnabled && user.LockoutEnd.HasValue && user.LockoutEnd > DateTimeOffset.UtcNow)
            return Result<AuthResponseDto>.Failure("الحساب موقوف. يرجى التواصل مع المسؤول.");

        var token = await GenerateTokenAsync(user);
        var roles = await _userManager.GetRolesAsync(user);

        return Result<AuthResponseDto>.Success(new AuthResponseDto
        {
            Token = token,
            Email = user.Email!,
            FullName = user.FullName,
            Roles = roles,
            ExpiresAt = DateTime.UtcNow.AddHours(24)
        });
    }

    public async Task<Result<AuthResponseDto>> RegisterAsync(RegisterDto dto)
    {
        if (await _userManager.FindByEmailAsync(dto.Email) is not null)
            return Result<AuthResponseDto>.Failure("يوجد مستخدم بهذا البريد الإلكتروني مسبقاً.");

        var user = new ApplicationUser
        {
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            Email = dto.Email,
            UserName = dto.Email,
            EmailConfirmed = true  // auto-confirm في بيئة الإدارة الداخلية
        };

        var createResult = await _userManager.CreateAsync(user, dto.Password);
        if (!createResult.Succeeded)
            return Result<AuthResponseDto>.Failure(
                createResult.Errors.Select(e => e.Description));

        await _userManager.AddToRoleAsync(user, dto.Role);

        var token = await GenerateTokenAsync(user);
        var roles = await _userManager.GetRolesAsync(user);

        return Result<AuthResponseDto>.Success(new AuthResponseDto
        {
            Token = token,
            Email = user.Email!,
            FullName = user.FullName,
            Roles = roles,
            ExpiresAt = DateTime.UtcNow.AddHours(24)
        }, "تم إنشاء الحساب بنجاح.");
    }

    // ══════════════════════════════════════════════════
    //  GESTION DES UTILISATEURS
    // ══════════════════════════════════════════════════

    public async Task<Result<IEnumerable<UserDto>>> GetUsersAsync()
    {
        var users = _userManager.Users.ToList();
        var dtos = new List<UserDto>();

        foreach (var u in users)
        {
            var roles = await _userManager.GetRolesAsync(u);
            dtos.Add(new UserDto
            {
                Id = u.Id,
                FirstName = u.FirstName,
                LastName = u.LastName,
                Email = u.Email!,
                Roles = roles
            });
        }
        return Result<IEnumerable<UserDto>>.Success(dtos);
    }

    public async Task<Result<IEnumerable<UserDetailDto>>> GetUsersDetailAsync()
    {
        var users = _userManager.Users.ToList();
        var dtos = new List<UserDetailDto>();

        foreach (var u in users)
        {
            var roles = await _userManager.GetRolesAsync(u);
            dtos.Add(new UserDetailDto
            {
                Id = u.Id,
                FirstName = u.FirstName,
                LastName = u.LastName,
                Email = u.Email!,
                IsActive = IsUserActive(u),
                EmailConfirmed = u.EmailConfirmed,
                Roles = roles
            });
        }
        return Result<IEnumerable<UserDetailDto>>.Success(dtos);
    }

    public async Task<Result<UserDetailDto>> GetUserByIdAsync(string userId)
    {
        var u = await _userManager.FindByIdAsync(userId);
        if (u is null)
            return Result<UserDetailDto>.Failure("المستخدم غير موجود.");

        var roles = await _userManager.GetRolesAsync(u);
        return Result<UserDetailDto>.Success(new UserDetailDto
        {
            Id = u.Id,
            FirstName = u.FirstName,
            LastName = u.LastName,
            Email = u.Email!,
            IsActive = IsUserActive(u),
            EmailConfirmed = u.EmailConfirmed,
            Roles = roles
        });
    }

    public async Task<Result> DeleteUserAsync(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user is null)
            return Result.Failure("المستخدم غير موجود.");

        var result = await _userManager.DeleteAsync(user);
        if (!result.Succeeded)
            return Result.Failure(result.Errors.Select(e => e.Description));

        return Result.Success("تم حذف الحساب بنجاح.");
    }

    public async Task<Result> ToggleUserActiveAsync(ToggleUserDto dto)
    {
        var user = await _userManager.FindByIdAsync(dto.UserId);
        if (user is null)
            return Result.Failure("المستخدم غير موجود.");

        if (dto.IsActive)
        {
            // ── تفعيل: رفع الـ lockout ──────────────────
            await _userManager.SetLockoutEnabledAsync(user, false);
            await _userManager.SetLockoutEndDateAsync(user, null);
        }
        else
        {
            // ── إيقاف: lockout دائم ──────────────────────
            await _userManager.SetLockoutEnabledAsync(user, true);
            await _userManager.SetLockoutEndDateAsync(user, DateTimeOffset.MaxValue);
        }

        return Result.Success(dto.IsActive ? "تم تفعيل الحساب بنجاح." : "تم إيقاف الحساب بنجاح.");
    }

    public async Task<Result> ResetPasswordAsync(ResetPasswordDto dto)
    {
        var user = await _userManager.FindByIdAsync(dto.UserId);
        if (user is null)
            return Result.Failure("المستخدم غير موجود.");

        // إعادة تعيين بدون طلب التوكن القديم (Admin override)
        var token = await _userManager.GeneratePasswordResetTokenAsync(user);
        var result = await _userManager.ResetPasswordAsync(user, token, dto.NewPassword);

        if (!result.Succeeded)
            return Result.Failure(result.Errors.Select(e => e.Description));

        return Result.Success("تم إعادة تعيين كلمة المرور بنجاح.");
    }

    // ══════════════════════════════════════════════════
    //  GESTION DES RÔLES
    // ══════════════════════════════════════════════════

    public Task<Result<IEnumerable<string>>> GetAllRolesAsync()
    {
        // الأدوار المعرَّفة في النظام (مطابقة لـ DbSeeder)
        var roles = new[] { "SuperAdmin", "Admin", "Manager", "Employee" };
        return Task.FromResult(Result<IEnumerable<string>>.Success(roles));
    }

    public async Task<Result> AssignRoleAsync(AssignRoleDto dto)
    {
        var user = await _userManager.FindByIdAsync(dto.UserId);
        if (user is null)
            return Result.Failure("المستخدم غير موجود.");

        if (await _userManager.IsInRoleAsync(user, dto.Role))
            return Result.Failure($"المستخدم يمتلك دور '{dto.Role}' مسبقاً.");

        var result = await _userManager.AddToRoleAsync(user, dto.Role);
        if (!result.Succeeded)
            return Result.Failure(result.Errors.Select(e => e.Description));

        return Result.Success($"تم تعيين دور '{dto.Role}' بنجاح.");
    }

    public async Task<Result> RemoveRoleAsync(AssignRoleDto dto)
    {
        var user = await _userManager.FindByIdAsync(dto.UserId);
        if (user is null)
            return Result.Failure("المستخدم غير موجود.");

        if (!await _userManager.IsInRoleAsync(user, dto.Role))
            return Result.Failure($"المستخدم لا يمتلك دور '{dto.Role}'.");

        var result = await _userManager.RemoveFromRoleAsync(user, dto.Role);
        if (!result.Succeeded)
            return Result.Failure(result.Errors.Select(e => e.Description));

        return Result.Success($"تم إلغاء دور '{dto.Role}' بنجاح.");
    }

    // ══════════════════════════════════════════════════
    //  HELPERS
    // ══════════════════════════════════════════════════

    /// <summary>يُحدد ما إذا كان الحساب نشطاً (غير موقوف)</summary>
    private static bool IsUserActive(ApplicationUser u)
        => !u.LockoutEnabled
        || u.LockoutEnd is null
        || u.LockoutEnd < DateTimeOffset.UtcNow;

    /// <summary>يُنشئ JWT token للمستخدم</summary>
    private async Task<string> GenerateTokenAsync(ApplicationUser user)
    {
        var roles = await _userManager.GetRolesAsync(user);
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub,   user.Id),
            new(JwtRegisteredClaimNames.Email, user.Email!),
            new(JwtRegisteredClaimNames.Jti,   Guid.NewGuid().ToString()),
            new("firstName", user.FirstName),
            new("lastName",  user.LastName),
        };
        claims.AddRange(roles.Select(r => new Claim(ClaimTypes.Role, r)));

        var jwtKey = _configuration["Jwt:Key"]
            ?? throw new InvalidOperationException("Jwt:Key manquant dans appsettings.");

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddHours(24),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}