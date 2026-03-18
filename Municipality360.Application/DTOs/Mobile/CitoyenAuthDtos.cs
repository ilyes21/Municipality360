using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.ComponentModel.DataAnnotations;

namespace Municipality360.Application.DTOs.Mobile;

/// <summary>تسجيل مواطن جديد عبر التطبيق المحمول</summary>
public class CitoyenRegisterMobileDto
{
    [Required, MaxLength(200)]
    public string Nom { get; set; } = string.Empty;

    [Required, MaxLength(200)]
    public string Prenom { get; set; } = string.Empty;

    [Required, MaxLength(20)]
    public string CIN { get; set; } = string.Empty;

    [Required, MaxLength(50)]
    public string Telephone { get; set; } = string.Empty;

    [MaxLength(256), EmailAddress]
    public string? Email { get; set; }

    [Required, MinLength(6)]
    public string Password { get; set; } = string.Empty;

    /// <summary>FCM token للإشعارات Push من Firebase</summary>
    public string? FcmToken { get; set; }
}

/// <summary>تسجيل دخول المواطن (برقم الهاتف أو CIN)</summary>
public class CitoyenLoginMobileDto
{
    /// <summary>CIN أو رقم الهاتف أو البريد الإلكتروني</summary>
    [Required]
    public string Identifiant { get; set; } = string.Empty;

    [Required]
    public string Password { get; set; } = string.Empty;

    /// <summary>FCM token — يُحدَّث عند كل دخول</summary>
    public string? FcmToken { get; set; }
}

/// <summary>استجابة المصادقة للمواطن</summary>
public class CitoyenAuthResponseDto
{
    public string Token { get; set; } = string.Empty;
    public int CitoyenId { get; set; }
    public string NomComplet { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string Telephone { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }
}

/// <summary>تحديث FCM Token عند تجديده من Firebase</summary>
public class UpdateFcmTokenDto
{
    [Required]
    public string FcmToken { get; set; } = string.Empty;
}

/// <summary>ملف المواطن — بيانات كاملة للتطبيق</summary>
public class CitoyenProfileMobileDto
{
    public int Id { get; set; }
    public string NomComplet { get; set; } = string.Empty;
    public string CIN { get; set; } = string.Empty;
    public string Telephone { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? Adresse { get; set; }
    public string? Ville { get; set; }
    public int NombreReclamations { get; set; }
    public int NombrePermis { get; set; }
    /// <summary>إحصائيات سريعة للداشبورد</summary>
    public CitoyenDashboardStatsDto Stats { get; set; } = new();
}

/// <summary>إحصائيات الداشبورد الشخصي</summary>
public class CitoyenDashboardStatsDto
{
    public int ReclamationsEnCours { get; set; }
    public int ReclamationsTraitees { get; set; }
    public int PermisEnAttente { get; set; }
    public int PermisApprouves { get; set; }
    public int NotificationsNonLues { get; set; }
}