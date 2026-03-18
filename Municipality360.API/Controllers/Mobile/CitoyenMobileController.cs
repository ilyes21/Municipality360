using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Municipality360.Application.DTOs.Mobile;
using Municipality360.Application.DTOs.Reclamations;
using Municipality360.Application.Interfaces.Services;

namespace Municipality360.API.Controllers.Mobile;

[ApiController]
[Route("api/mobile")]
public class CitoyenMobileController : ControllerBase
{
    private readonly ICitoyenAuthService _authService;
    private readonly IReclamationService _reclamationService;
    private readonly INotificationService _notificationService;
    private readonly IDemandePermisBatirService _permisService;

    public CitoyenMobileController(
        ICitoyenAuthService authService,
        IReclamationService reclamationService,
        INotificationService notificationService,
        IDemandePermisBatirService permisService)
    {
        _authService = authService;
        _reclamationService = reclamationService;
        _notificationService = notificationService;
        _permisService = permisService;
    }

    // ══════════════════════════════════════════
    //  AUTH
    // ══════════════════════════════════════════

    /// <summary>تسجيل مواطن جديد عبر التطبيق</summary>
    [HttpPost("auth/register")]
    [AllowAnonymous]
    public async Task<IActionResult> Register([FromBody] CitoyenRegisterMobileDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        var result = await _authService.RegisterAsync(dto);
        return result.Succeeded ? Ok(result) : BadRequest(result);
    }

    /// <summary>تسجيل دخول المواطن</summary>
    [HttpPost("auth/login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login([FromBody] CitoyenLoginMobileDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        var result = await _authService.LoginAsync(dto);
        return result.Succeeded ? Ok(result) : Unauthorized(result);
    }

    // ══════════════════════════════════════════
    //  DASHBOARD & PROFILE
    // ══════════════════════════════════════════

    /// <summary>ملف وإحصائيات المواطن (للداشبورد)</summary>
    [HttpGet("profile")]
    [Authorize(Roles = "Citoyen")]
    public async Task<IActionResult> GetProfile()
    {
        var id = GetCitoyenId();
        if (id == 0) return Unauthorized();
        var result = await _authService.GetProfileAsync(id);
        return result.Succeeded ? Ok(result) : NotFound(result);
    }

    /// <summary>إحصائيات سريعة للداشبورد</summary>
    [HttpGet("dashboard/stats")]
    [Authorize(Roles = "Citoyen")]
    public async Task<IActionResult> GetDashboardStats()
    {
        var id = GetCitoyenId();
        if (id == 0) return Unauthorized();
        var result = await _authService.GetDashboardStatsAsync(id);
        return Ok(result);
    }

    /// <summary>تحديث FCM Token لإشعارات Firebase</summary>
    [HttpPut("fcm-token")]
    [Authorize(Roles = "Citoyen")]
    public async Task<IActionResult> UpdateFcmToken([FromBody] UpdateFcmTokenDto dto)
    {
        var id = GetCitoyenId();
        if (id == 0) return Unauthorized();
        var result = await _authService.UpdateFcmTokenAsync(id, dto.FcmToken);
        return result.Succeeded ? Ok(result) : BadRequest(result);
    }

    // ══════════════════════════════════════════
    //  RÉCLAMATIONS
    // ══════════════════════════════════════════

    /// <summary>قائمة شكاوى المواطن</summary>
    [HttpGet("reclamations")]
    [Authorize(Roles = "Citoyen")]
    public async Task<IActionResult> GetMyReclamations()
    {
        var id = GetCitoyenId();
        if (id == 0) return Unauthorized();
        var items = await _reclamationService.GetByCitoyenAsync(id);
        return Ok(items);
    }

    /// <summary>تقديم شكوى جديدة — يُعيّن CitoyenId تلقائياً من JWT</summary>
    [HttpPost("reclamations")]
    [Authorize(Roles = "Citoyen")]
    public async Task<IActionResult> DeposerReclamation([FromBody] CreateReclamationDto dto)
    {
        var id = GetCitoyenId();
        if (id == 0) return Unauthorized();

        // تأكيد أن الشكوى تخص المواطن الحالي
        dto.CitoyenId = id;

        var result = await _reclamationService.DeposerAsync(dto, agentId: null);
        return Ok(result);
    }

    /// <summary>متابعة حالة شكوى برقمها العام (بدون JWT — للمتابعة الخارجية)</summary>
    [HttpGet("reclamations/suivi/{numero}")]
    [AllowAnonymous]
    public async Task<IActionResult> SuiviReclamation(string numero)
    {
        var result = await _reclamationService.GetByNumeroPublicAsync(numero);
        return Ok(result);
    }

    // ══════════════════════════════════════════
    //  PERMIS DE BÂTIR
    // ══════════════════════════════════════════

    /// <summary>متابعة حالة رخصة البناء برقمها</summary>
    [HttpGet("permis/suivi/{numero}")]
    [AllowAnonymous]
    public async Task<IActionResult> SuiviPermis(string numero)
    {
        var result = await _permisService.GetByNumeroPublicAsync(numero);
        return Ok(result);
    }

    // ══════════════════════════════════════════
    //  NOTIFICATIONS
    // ══════════════════════════════════════════

    /// <summary>إشعارات المواطن</summary>
    [HttpGet("notifications")]
    [Authorize(Roles = "Citoyen")]
    public async Task<IActionResult> GetNotifications([FromQuery] bool nonLuesOnly = false)
    {
        var id = GetCitoyenId();
        if (id == 0) return Unauthorized();
        var items = await _notificationService.GetByCitoyenAsync(id.ToString(), nonLuesOnly);
        return Ok(items);
    }

    /// <summary>عدد الإشعارات غير المقروءة</summary>
    [HttpGet("notifications/count")]
    [Authorize(Roles = "Citoyen")]
    public async Task<IActionResult> GetNotificationsCount()
    {
        var id = GetCitoyenId();
        if (id == 0) return Unauthorized();
        var count = await _notificationService.GetNombreNonLuesAsync(id.ToString(), estAgent: false);
        return Ok(new { NonLues = count });
    }

    /// <summary>تحديد إشعار كمقروء</summary>
    [HttpPatch("notifications/{id}/lue")]
    [Authorize(Roles = "Citoyen")]
    public async Task<IActionResult> MarquerLue(int id)
    {
        var citId = GetCitoyenId();
        if (citId == 0) return Unauthorized();
        await _notificationService.MarquerLueAsync(id, citId.ToString());
        return Ok();
    }

    /// <summary>تحديد كل الإشعارات كمقروءة</summary>
    [HttpPatch("notifications/lire-tout")]
    [Authorize(Roles = "Citoyen")]
    public async Task<IActionResult> MarquerToutesLues()
    {
        var id = GetCitoyenId();
        if (id == 0) return Unauthorized();
        await _notificationService.MarquerToutesLuesAsync(id.ToString(), estAgent: false);
        return Ok();
    }

    // ── Helper ──────────────────────────────────────────────────────
    private int GetCitoyenId()
    {
        var claim = User.FindFirstValue("citoyenId");
        return int.TryParse(claim, out var id) ? id : 0;
    }
}
