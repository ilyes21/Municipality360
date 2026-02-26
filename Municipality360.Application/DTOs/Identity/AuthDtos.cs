using System.ComponentModel.DataAnnotations;

namespace Municipality360.Application.DTOs.Identity;

// ════════════════════════════════════════════════════
//  AUTH
// ════════════════════════════════════════════════════

public class RegisterDto
{
    [Required, MaxLength(50)]
    public string FirstName { get; set; } = string.Empty;

    [Required, MaxLength(50)]
    public string LastName { get; set; } = string.Empty;

    [Required, EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required, MinLength(8)]
    public string Password { get; set; } = string.Empty;

    [Required]
    public string Role { get; set; } = "Employee";
}

public class LoginDto
{
    [Required, EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    public string Password { get; set; } = string.Empty;
}

public class AuthResponseDto
{
    public string Token { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public IList<string> Roles { get; set; } = new List<string>();
    public DateTime ExpiresAt { get; set; }
}

// ════════════════════════════════════════════════════
//  USERS
// ════════════════════════════════════════════════════

/// <summary>DTO أساسي – قائمة مبسّطة</summary>
public class UserDto
{
    public string Id { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public IList<string> Roles { get; set; } = new List<string>();
    public string FullName => $"{FirstName} {LastName}";
}

/// <summary>DTO تفصيلي – يشمل حالة التفعيل وتأكيد البريد</summary>
public class UserDetailDto
{
    public string Id { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
    public bool EmailConfirmed { get; set; }
    public IList<string> Roles { get; set; } = new List<string>();
    public string FullName => $"{FirstName} {LastName}";
}

// ════════════════════════════════════════════════════
//  ROLE MANAGEMENT
// ════════════════════════════════════════════════════

/// <summary>تعيين أو إلغاء دور لمستخدم</summary>
public class AssignRoleDto
{
    [Required]
    public string UserId { get; set; } = string.Empty;

    [Required]
    public string Role { get; set; } = string.Empty;
}

// ════════════════════════════════════════════════════
//  USER MANAGEMENT
// ════════════════════════════════════════════════════

/// <summary>تفعيل أو إيقاف حساب مستخدم</summary>
public class ToggleUserDto
{
    [Required]
    public string UserId { get; set; } = string.Empty;

    public bool IsActive { get; set; }
}

/// <summary>إعادة تعيين كلمة مرور مستخدم</summary>
public class ResetPasswordDto
{
    [Required]
    public string UserId { get; set; } = string.Empty;

    [Required, MinLength(8)]
    public string NewPassword { get; set; } = string.Empty;
}

// ════════════════════════════════════════════════════
//  SELF-UPDATE (كل مستخدم يُحدّث بياناته الشخصية)
// ════════════════════════════════════════════════════

/// <summary>تغيير الاسم واللقب للمستخدم الحالي</summary>
public class UpdateProfileDto
{
    [Required, MaxLength(50)]
    public string FirstName { get; set; } = string.Empty;

    [Required, MaxLength(50)]
    public string LastName { get; set; } = string.Empty;
}

/// <summary>تغيير كلمة المرور للمستخدم الحالي</summary>
public class ChangePasswordDto
{
    [Required]
    public string CurrentPassword { get; set; } = string.Empty;

    [Required, MinLength(8)]
    public string NewPassword { get; set; } = string.Empty;
}
