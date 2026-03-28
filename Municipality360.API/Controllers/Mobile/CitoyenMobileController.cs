using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Municipality360.Application.DTOs.Mobile;
using Municipality360.Application.DTOs.PermisBatir;
using Municipality360.Application.DTOs.Reclamations;
using Municipality360.Application.Interfaces.Repositories;
using Municipality360.Application.Interfaces.Services;
using System.Security.Claims;

namespace Municipality360.API.Controllers.Mobile;

[ApiController]
[Route("api/mobile")]
public class CitoyenMobileController : ControllerBase
{
    private readonly ICitoyenAuthService _authService;
    private readonly IReclamationService _reclamationService;
    private readonly INotificationService _notificationService;
    private readonly IDemandePermisBatirService _permisService;
    private readonly ICitoyenRepository _citoyenRepo;
    private readonly IDemandeurRepository _demandeurRepo;

    public CitoyenMobileController(
        ICitoyenAuthService authService,
        IReclamationService reclamationService,
        INotificationService notificationService,
        IDemandePermisBatirService permisService,
        ICitoyenRepository citoyenRepo,       
        IDemandeurRepository demandeurRepo)
    {
        _authService = authService;
        _reclamationService = reclamationService;
        _notificationService = notificationService;
        _permisService = permisService;
        _citoyenRepo = citoyenRepo;
        _demandeurRepo = demandeurRepo;
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

    [HttpGet("mes-permis")]
    [Authorize(Roles = "Citoyen")]
    public async Task<IActionResult> GetMesPermis()
    {
        var citoyenId = GetCitoyenId();
        if (citoyenId == 0) return Unauthorized();

        // 1. جلب المواطن للحصول على CIN
        var citoyen = await _citoyenRepo.GetByIdAsync(citoyenId);
        if (citoyen == null)
            return NotFound(new { message = "المواطن غير موجود." });

        if (string.IsNullOrWhiteSpace(citoyen.CIN))
            return Ok(new MesPermisDto
            {
                NomComplet = citoyen.NomComplet,
                CIN = string.Empty,
                NombreDemandes = 0,
                Demandes = new List<PermisPublicItemDto>()
            });

        // 2. البحث عن Demandeur بنفس CIN
        var demandeurs = await _demandeurRepo.SearchAsync(citoyen.CIN);
        var demandeur = demandeurs.FirstOrDefault(d =>
            !string.IsNullOrEmpty(d.CIN) &&
            string.Equals(d.CIN.Trim(), citoyen.CIN.Trim(),
                          StringComparison.OrdinalIgnoreCase));

        // لا يوجد Demandeur مسجّل بهذا CIN → إرجاع قائمة فارغة (طبيعي)
        if (demandeur == null)
            return Ok(new MesPermisDto
            {
                NomComplet = citoyen.NomComplet,
                CIN = citoyen.CIN,
                NombreDemandes = 0,
                Demandes = new List<PermisPublicItemDto>()
            });

        // 3. جلب طلبات الرخص المرتبطة بهذا Demandeur
        var demandes = await _permisService.GetByDemandeurAsync(demandeur.Id);

        // 4. تحويل إلى DTOs مبسّطة للمواطن
        var items = demandes.Select(d => new PermisPublicItemDto
        {
            Id = d.Id,
            NumeroDemande = d.NumeroDemande,
            Statut = d.Statut,
            TypeDemande = d.TypeDemandeLibelle,
            AdresseProjet = d.AdresseProjet,
            DateDepot = d.DateDepot,
            DateDecision = d.DateDecision,
            NumeroPermis = d.NumeroPermis,
            DateDelivrance = d.DateDelivrance,
            DateValidite = d.DateValiditePermis,
            TaxesPayees = d.ToutesLTaxesPayees,
            TotalTaxes = d.TotalTaxes,
        }).OrderByDescending(d => d.DateDepot).ToList();

        return Ok(new MesPermisDto
        {
            NomComplet = citoyen.NomComplet,
            CIN = citoyen.CIN,
            NombreDemandes = items.Count,
            Demandes = items
        });
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

    [HttpGet("permis/par-cin/{cin}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetPermisByCIN(string cin)
    {
        if (string.IsNullOrWhiteSpace(cin) || cin.Length < 6)
            return BadRequest(new { message = "رقم بطاقة التعريف غير صالح." });

        // 1. البحث عن الـ Demandeur برقم CIN
        var demandeurs = await _demandeurRepo.SearchAsync(cin.Trim().ToUpper());
        var demandeur = demandeurs.FirstOrDefault(d =>
            !string.IsNullOrEmpty(d.CIN) &&
            d.CIN.Trim().ToUpper() == cin.Trim().ToUpper());

        if (demandeur == null)
            return NotFound(new { message = "لا يوجد متقدم مسجل بهذا الرقم." });

        // 2. جلب كل طلبات هذا الـ Demandeur
        var demandes = await _permisService.GetByDemandeurAsync(demandeur.Id);

        if (!demandes.Any())
            return NotFound(new { message = "لا توجد طلبات رخص مرتبطة بهذا الرقم." });

        // 3. إرجاع النتيجة
        return Ok(new PermisByCINResultDto
        {
            DemandeurId = demandeur.Id,
            NomComplet = demandeur.NomComplet,
            CIN = demandeur.CIN ?? "",
            Telephone = demandeur.Telephone,
            NombreDemandes = demandes.Count,
            Demandes = demandes.Select(d => new PermisPublicItemDto
            {
                Id = d.Id,
                NumeroDemande = d.NumeroDemande,
                Statut = d.Statut,
                TypeDemande = d.TypeDemandeLibelle,
                AdresseProjet = d.AdresseProjet,
                DateDepot = d.DateDepot,
                DateDecision = d.DateDecision,
                NumeroPermis = d.NumeroPermis,
                DateDelivrance = d.DateDelivrance,
                DateValidite = d.DateValiditePermis,
                TaxesPayees = d.ToutesLTaxesPayees,
                TotalTaxes = d.TotalTaxes,
            }).OrderByDescending(d => d.DateDepot).ToList()
        });
    }

    // ── Helper ──────────────────────────────────────────────────────
    private int GetCitoyenId()
    {
        var claim = User.FindFirstValue("citoyenId");
        return int.TryParse(claim, out var id) ? id : 0;
    }
}
