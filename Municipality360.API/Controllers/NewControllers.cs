// ═══════════════════════════════════════════════════════════════════
//  NewControllers.cs
//  API/Controllers/NewControllers.cs
//
//  Controllers للوحدات الجديدة:
//  BureauOrdreController · ReclamationsController
//  PermisBatirController · NotificationsController
// ═══════════════════════════════════════════════════════════════════

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Municipality360.Application.DTOs.BureauOrdre;
using Municipality360.Application.DTOs.Notifications;
using Municipality360.Application.DTOs.PermisBatir;
using Municipality360.Application.DTOs.Reclamations;
using Municipality360.Application.Interfaces.Services;
using System.Security.Claims;

namespace Municipality360.API.Controllers;

// ════════════════════════════════════════════════════════════════
//  BUREAU D'ORDRE — COURRIER ENTRANT
// ════════════════════════════════════════════════════════════════

[ApiController]
[Route("api/courriers-entrants")]
[Authorize]
public class CourriersEntrantsController : ControllerBase
{
    private readonly ICourrierEntrantService _service;
    public CourriersEntrantsController(ICourrierEntrantService service) => _service = service;

    private string UserId   => User.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty;
    private string UserName => $"{User.FindFirstValue("firstName")} {User.FindFirstValue("lastName")}".Trim();

    [HttpGet]
    public async Task<IActionResult> GetPaged([FromQuery] CourrierEntrantFilterDto filter)
    {
        var result = await _service.GetPagedAsync(filter);
        return Ok(result);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        try { return Ok(await _service.GetByIdAsync(id)); }
        catch (KeyNotFoundException ex) { return NotFound(new { message = ex.Message }); }
    }

    [HttpGet("numero/{numero}")]
    public async Task<IActionResult> GetByNumero(string numero)
    {
        try { return Ok(await _service.GetByNumeroAsync(numero)); }
        catch (KeyNotFoundException ex) { return NotFound(new { message = ex.Message }); }
    }

    [HttpGet("en-retard")]
    public async Task<IActionResult> GetEnRetard() => Ok(await _service.GetEnRetardAsync());

    [HttpGet("en-attente/service/{serviceId:int}")]
    public async Task<IActionResult> GetEnAttenteParService(int serviceId)
        => Ok(await _service.GetEnAttenteParServiceAsync(serviceId));

    [HttpGet("stats")]
    public async Task<IActionResult> GetStats([FromQuery] int? serviceId)
        => Ok(await _service.GetStatsAsync(serviceId));

    [HttpPost]
    [Authorize(Roles = "SuperAdmin,Admin,Manager,BureauOrdre")]
    public async Task<IActionResult> Enregistrer([FromBody] CreateCourrierEntrantDto dto)
    {
        try
        {
            var result = await _service.EnregistrerAsync(dto, UserId, UserName);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }
        catch (Exception ex) { return BadRequest(new { message = ex.Message }); }
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = "SuperAdmin,Admin,Manager,BureauOrdre")]
    public async Task<IActionResult> Modifier(int id, [FromBody] UpdateCourrierEntrantDto dto)
    {
        try { return Ok(await _service.ModifierAsync(id, dto, UserId)); }
        catch (KeyNotFoundException ex) { return NotFound(new { message = ex.Message }); }
        catch (InvalidOperationException ex) { return BadRequest(new { message = ex.Message }); }
    }

    [HttpPatch("{id:int}/statut")]
    [Authorize(Roles = "SuperAdmin,Admin,Manager,BureauOrdre")]
    public async Task<IActionResult> ChangerStatut(int id, [FromBody] ChangerStatutEntrantDto dto)
    {
        try { await _service.ChangerStatutAsync(id, dto, UserId, UserName); return NoContent(); }
        catch (KeyNotFoundException ex) { return NotFound(new { message = ex.Message }); }
    }

    [HttpPost("{id:int}/acheminer")]
    [Authorize(Roles = "SuperAdmin,Admin,Manager,BureauOrdre")]
    public async Task<IActionResult> Acheminer(int id, [FromBody] AcheminerCourrierDto dto)
    {
        try { return Ok(await _service.AcheminerAsync(id, dto, UserId)); }
        catch (KeyNotFoundException ex) { return NotFound(new { message = ex.Message }); }
        catch (InvalidOperationException ex) { return BadRequest(new { message = ex.Message }); }
    }

    [HttpPatch("circuit/{circuitId:int}/traiter")]
    public async Task<IActionResult> TraiterEtape(int circuitId, [FromBody] TraiterEtapeCircuitDto dto)
    {
        try { await _service.TraiterEtapeCircuitAsync(circuitId, dto, UserId, UserName); return NoContent(); }
        catch (KeyNotFoundException ex) { return NotFound(new { message = ex.Message }); }
    }

    [HttpGet("{id:int}/circuit")]
    public async Task<IActionResult> GetCircuit(int id) => Ok(await _service.GetCircuitAsync(id));

    [HttpPost("{id:int}/archiver")]
    [Authorize(Roles = "SuperAdmin,Admin,Manager,BureauOrdre")]
    public async Task<IActionResult> Archiver(int id, [FromBody] ArchiversCourrierDto dto)
    {
        try { return Ok(await _service.ArchiverAsync(id, dto, UserId)); }
        catch (KeyNotFoundException ex) { return NotFound(new { message = ex.Message }); }
        catch (InvalidOperationException ex) { return BadRequest(new { message = ex.Message }); }
    }
}

// ════════════════════════════════════════════════════════════════
//  BUREAU D'ORDRE — COURRIER SORTANT
// ════════════════════════════════════════════════════════════════

[ApiController]
[Route("api/courriers-sortants")]
[Authorize]
public class CourriersSortantsController : ControllerBase
{
    private readonly ICourrierSortantService _service;
    public CourriersSortantsController(ICourrierSortantService service) => _service = service;

    private string UserId => User.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty;

    [HttpGet]
    public async Task<IActionResult> GetPaged([FromQuery] CourrierSortantFilterDto filter)
        => Ok(await _service.GetPagedAsync(filter));

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        try { return Ok(await _service.GetByIdAsync(id)); }
        catch (KeyNotFoundException ex) { return NotFound(new { message = ex.Message }); }
    }

    [HttpGet("brouillons")]
    public async Task<IActionResult> GetBrouillons()
        => Ok(await _service.GetBrouillonsAsync(UserId));

    [HttpPost]
    [Authorize(Roles = "SuperAdmin,Admin,Manager,BureauOrdre")]
    public async Task<IActionResult> CreerBrouillon([FromBody] CreateCourrierSortantDto dto)
    {
        try
        {
            var result = await _service.CreerBrouillonAsync(dto, UserId);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }
        catch (Exception ex) { return BadRequest(new { message = ex.Message }); }
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = "SuperAdmin,Admin,Manager,BureauOrdre")]
    public async Task<IActionResult> Modifier(int id, [FromBody] CreateCourrierSortantDto dto)
    {
        try { return Ok(await _service.ModifierAsync(id, dto, UserId)); }
        catch (KeyNotFoundException ex) { return NotFound(new { message = ex.Message }); }
        catch (InvalidOperationException ex) { return BadRequest(new { message = ex.Message }); }
    }

    [HttpPatch("{id:int}/soumettre")]
    public async Task<IActionResult> Soumettre(int id)
    {
        try { await _service.SoumettreEnValidationAsync(id, UserId); return NoContent(); }
        catch (KeyNotFoundException ex) { return NotFound(new { message = ex.Message }); }
        catch (InvalidOperationException ex) { return BadRequest(new { message = ex.Message }); }
    }

    [HttpPatch("{id:int}/signer")]
    [Authorize(Roles = "SuperAdmin,Admin,Manager")]
    public async Task<IActionResult> Signer(int id, [FromBody] SignerCourrierDto dto)
    {
        try { await _service.MarquerSigneAsync(id, UserId, dto.FonctionSignataire); return NoContent(); }
        catch (KeyNotFoundException ex) { return NotFound(new { message = ex.Message }); }
    }

    [HttpPatch("{id:int}/envoyer")]
    [Authorize(Roles = "SuperAdmin,Admin,Manager,BureauOrdre")]
    public async Task<IActionResult> Envoyer(int id, [FromBody] EnvoyerCourrierDto? dto)
    {
        try { await _service.MarquerEnvoyeAsync(id, UserId, dto?.DateEnvoi); return NoContent(); }
        catch (KeyNotFoundException ex) { return NotFound(new { message = ex.Message }); }
        catch (InvalidOperationException ex) { return BadRequest(new { message = ex.Message }); }
    }

    [HttpPatch("{id:int}/annuler")]
    [Authorize(Roles = "SuperAdmin,Admin,Manager")]
    public async Task<IActionResult> Annuler(int id, [FromBody] AnnulerCourrierDto dto)
    {
        try { await _service.AnnulerAsync(id, dto.Motif, UserId); return NoContent(); }
        catch (KeyNotFoundException ex) { return NotFound(new { message = ex.Message }); }
        catch (InvalidOperationException ex) { return BadRequest(new { message = ex.Message }); }
    }

    [HttpPost("{id:int}/archiver")]
    [Authorize(Roles = "SuperAdmin,Admin,Manager,BureauOrdre")]
    public async Task<IActionResult> Archiver(int id, [FromBody] ArchiversCourrierDto dto)
    {
        try { return Ok(await _service.ArchiverAsync(id, dto, UserId)); }
        catch (KeyNotFoundException ex) { return NotFound(new { message = ex.Message }); }
        catch (InvalidOperationException ex) { return BadRequest(new { message = ex.Message }); }
    }
}

// ════════════════════════════════════════════════════════════════
//  CONTACTS BO
// ════════════════════════════════════════════════════════════════

[ApiController]
[Route("api/bo-contacts")]
[Authorize]
public class BOContactsController : ControllerBase
{
    private readonly IBOContactService _service;
    public BOContactsController(IBOContactService service) => _service = service;

    [HttpGet]
    public async Task<IActionResult> Search([FromQuery] string? term)
        => Ok(await _service.SearchAsync(term));

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        try { return Ok(await _service.GetByIdAsync(id)); }
        catch (KeyNotFoundException ex) { return NotFound(new { message = ex.Message }); }
    }

    [HttpPost]
    [Authorize(Roles = "SuperAdmin,Admin,Manager,BureauOrdre")]
    public async Task<IActionResult> Create([FromBody] CreateBOContactDto dto)
        => Ok(await _service.CreateAsync(dto));

    [HttpPut("{id:int}")]
    [Authorize(Roles = "SuperAdmin,Admin,Manager,BureauOrdre")]
    public async Task<IActionResult> Update(int id, [FromBody] CreateBOContactDto dto)
    {
        try { return Ok(await _service.UpdateAsync(id, dto)); }
        catch (KeyNotFoundException ex) { return NotFound(new { message = ex.Message }); }
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "SuperAdmin,Admin,Manager")]
    public async Task<IActionResult> Delete(int id)
    {
        try { await _service.DeleteAsync(id); return NoContent(); }
        catch (KeyNotFoundException ex) { return NotFound(new { message = ex.Message }); }
    }
}

// ════════════════════════════════════════════════════════════════
//  RÉCLAMATIONS
// ════════════════════════════════════════════════════════════════

[ApiController]
[Route("api/reclamations")]
[Authorize]
public class ReclamationsController : ControllerBase
{
    private readonly IReclamationService _service;
    public ReclamationsController(IReclamationService service) => _service = service;

    private string UserId   => User.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty;
    private string UserName => $"{User.FindFirstValue("firstName")} {User.FindFirstValue("lastName")}".Trim();

    [HttpGet]
    public async Task<IActionResult> GetPaged([FromQuery] ReclamationFilterDto filter)
        => Ok(await _service.GetPagedAsync(filter));

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        try { return Ok(await _service.GetByIdAsync(id)); }
        catch (KeyNotFoundException ex) { return NotFound(new { message = ex.Message }); }
    }

    [HttpGet("suivi-public/{numero}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetByNumeroPublic(string numero)
    {
        try { return Ok(await _service.GetByNumeroPublicAsync(numero)); }
        catch (KeyNotFoundException ex) { return NotFound(new { message = ex.Message }); }
    }

    [HttpGet("citoyen/{citoyenId:int}")]
    public async Task<IActionResult> GetByCitoyen(int citoyenId)
        => Ok(await _service.GetByCitoyenAsync(citoyenId));

    [HttpGet("en-retard")]
    public async Task<IActionResult> GetEnRetard()
        => Ok(await _service.GetEnRetardAsync());

    [HttpGet("stats")]
    public async Task<IActionResult> GetStats([FromQuery] int? serviceId)
        => Ok(await _service.GetStatsAsync(serviceId));

    [HttpPost]
    [Authorize(Roles = "SuperAdmin,Admin,Manager,Reclamations")]
    public async Task<IActionResult> Deposer([FromBody] CreateReclamationDto dto)
    {
        try
        {
            var result = await _service.DeposerAsync(dto, UserId);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }
        catch (Exception ex) { return BadRequest(new { message = ex.Message }); }
    }

    [HttpPatch("{id:int}/assigner")]
    [Authorize(Roles = "SuperAdmin,Admin,Manager,Reclamations")]
    public async Task<IActionResult> Assigner(int id, [FromBody] AssignerReclamationDto dto)
    {
        try { await _service.AssignerAsync(id, dto, UserId, UserName); return NoContent(); }
        catch (KeyNotFoundException ex) { return NotFound(new { message = ex.Message }); }
    }

    [HttpPatch("{id:int}/statut")]
    [Authorize(Roles = "SuperAdmin,Admin,Manager,Reclamations")]
    public async Task<IActionResult> ChangerStatut(int id, [FromBody] ChangerStatutReclamationDto dto)
    {
        try { await _service.ChangerStatutAsync(id, dto, UserId, UserName); return NoContent(); }
        catch (KeyNotFoundException ex) { return NotFound(new { message = ex.Message }); }
    }
}

// ════════════════════════════════════════════════════════════════
//  CITOYENS
// ════════════════════════════════════════════════════════════════

[ApiController]
[Route("api/citoyens")]
[Authorize]
public class CitoyensController : ControllerBase
{
    private readonly ICitoyenService _service;
    public CitoyensController(ICitoyenService service) => _service = service;

    [HttpGet]
    public async Task<IActionResult> GetPaged([FromQuery] CitoyenFilterDto filter)
        => Ok(await _service.GetPagedAsync(filter));

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        try { return Ok(await _service.GetByIdAsync(id)); }
        catch (KeyNotFoundException ex) { return NotFound(new { message = ex.Message }); }
    }

    [HttpGet("cin/{cin}")]
    public async Task<IActionResult> GetByCIN(string cin)
    {
        try { return Ok(await _service.GetByCINAsync(cin)); }
        catch (KeyNotFoundException ex) { return NotFound(new { message = ex.Message }); }
    }

    [HttpPost]
    [Authorize(Roles = "SuperAdmin,Admin,Manager,Reclamations")]
    public async Task<IActionResult> Create([FromBody] CreateCitoyenDto dto)
    {
        try
        {
            var result = await _service.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }
        catch (InvalidOperationException ex) { return Conflict(new { message = ex.Message }); }
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = "SuperAdmin,Admin,Manager,Reclamations")]
    public async Task<IActionResult> Update(int id, [FromBody] CreateCitoyenDto dto)
    {
        try { return Ok(await _service.UpdateAsync(id, dto)); }
        catch (KeyNotFoundException ex) { return NotFound(new { message = ex.Message }); }
        catch (InvalidOperationException ex) { return Conflict(new { message = ex.Message }); }
    }
}

// ════════════════════════════════════════════════════════════════
//  PERMIS DE BÂTIR
// ════════════════════════════════════════════════════════════════

[ApiController]
[Route("api/permis-batir")]
[Authorize]
public class PermisBatirController : ControllerBase
{
    private readonly IDemandePermisBatirService _service;
    public PermisBatirController(IDemandePermisBatirService service) => _service = service;

    private string UserId   => User.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty;
    private string UserName => $"{User.FindFirstValue("firstName")} {User.FindFirstValue("lastName")}".Trim();

    [HttpGet]
    public async Task<IActionResult> GetPaged([FromQuery] DemandePermisFilterDto filter)
        => Ok(await _service.GetPagedAsync(filter));

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        try { return Ok(await _service.GetByIdAsync(id)); }
        catch (KeyNotFoundException ex) { return NotFound(new { message = ex.Message }); }
    }

    [HttpGet("suivi-public/{numero}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetByNumeroPublic(string numero)
    {
        try { return Ok(await _service.GetByNumeroPublicAsync(numero)); }
        catch (KeyNotFoundException ex) { return NotFound(new { message = ex.Message }); }
    }

    [HttpGet("demandeur/{demandeurId:int}")]
    public async Task<IActionResult> GetByDemandeur(int demandeurId)
        => Ok(await _service.GetByDemandeurAsync(demandeurId));

    [HttpGet("en-retard")]
    public async Task<IActionResult> GetEnRetard() => Ok(await _service.GetEnRetardAsync());

    [HttpGet("stats")]
    public async Task<IActionResult> GetStats([FromQuery] int? serviceId)
        => Ok(await _service.GetStatsAsync(serviceId));

    [HttpPost]
    [Authorize(Roles = "SuperAdmin,Admin,Manager,Urbanisme")]
    public async Task<IActionResult> Deposer([FromBody] CreateDemandePermisDto dto)
    {
        try
        {
            var result = await _service.DeposerAsync(dto, UserId);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }
        catch (Exception ex) { return BadRequest(new { message = ex.Message }); }
    }

    [HttpPatch("{id:int}/assigner-instructeur")]
    [Authorize(Roles = "SuperAdmin,Admin,Manager,Urbanisme")]
    public async Task<IActionResult> AssignerInstructeur(int id, [FromBody] AssignerInstructeurDto dto)
    {
        try { await _service.AssignerInstructeurAsync(id, dto, UserId, UserName); return NoContent(); }
        catch (KeyNotFoundException ex) { return NotFound(new { message = ex.Message }); }
    }

    [HttpPatch("{id:int}/assigner-commission")]
    [Authorize(Roles = "SuperAdmin,Admin,Manager,Urbanisme")]
    public async Task<IActionResult> AssignerCommission(int id, [FromBody] AssignerCommissionDto dto)
    {
        try { await _service.AssignerCommissionAsync(id, dto, UserId, UserName); return NoContent(); }
        catch (KeyNotFoundException ex) { return NotFound(new { message = ex.Message }); }
    }

    [HttpPatch("{id:int}/statut")]
    [Authorize(Roles = "SuperAdmin,Admin,Manager,Urbanisme")]
    public async Task<IActionResult> ChangerStatut(int id, [FromBody] ChangerStatutPermisDto dto)
    {
        try { await _service.ChangerStatutAsync(id, dto, UserId, UserName); return NoContent(); }
        catch (KeyNotFoundException ex) { return NotFound(new { message = ex.Message }); }
    }

    [HttpPost("{id:int}/delivrer-permis")]
    [Authorize(Roles = "SuperAdmin,Admin,Manager,Urbanisme")]
    public async Task<IActionResult> DelivrerPermis(int id, [FromBody] DelivrerPermisDto dto)
    {
        try { return Ok(await _service.DelivrerPermisAsync(id, dto, UserId)); }
        catch (KeyNotFoundException ex) { return NotFound(new { message = ex.Message }); }
        catch (InvalidOperationException ex) { return BadRequest(new { message = ex.Message }); }
    }

    [HttpPost("{id:int}/taxes")]
    [Authorize(Roles = "SuperAdmin,Admin,Manager,Urbanisme,Finances")]
    public async Task<IActionResult> AjouterTaxe(int id, [FromBody] AjouterTaxeDto dto)
    {
        try { await _service.AjouterTaxeAsync(id, dto); return NoContent(); }
        catch (KeyNotFoundException ex) { return NotFound(new { message = ex.Message }); }
    }

    [HttpPatch("{id:int}/taxes/payer")]
    [Authorize(Roles = "SuperAdmin,Admin,Manager,Finances")]
    public async Task<IActionResult> PayerTaxe(int id, [FromBody] PayerTaxeDto dto)
    {
        try { await _service.PayerTaxeAsync(id, dto, UserId); return NoContent(); }
        catch (KeyNotFoundException ex) { return NotFound(new { message = ex.Message }); }
    }

    [HttpPost("{id:int}/inspections")]
    [Authorize(Roles = "SuperAdmin,Admin,Manager,Urbanisme")]
    public async Task<IActionResult> AjouterInspection(int id, [FromBody] CreateInspectionDto dto)
    {
        try { await _service.AjouterInspectionAsync(id, dto, UserId); return NoContent(); }
        catch (KeyNotFoundException ex) { return NotFound(new { message = ex.Message }); }
    }
}

// ════════════════════════════════════════════════════════════════
//  DEMANDEURS & ARCHITECTES
// ════════════════════════════════════════════════════════════════

[ApiController]
[Route("api/demandeurs")]
[Authorize]
public class DemandeursController : ControllerBase
{
    private readonly IDemandeurService _service;
    public DemandeursController(IDemandeurService service) => _service = service;

    [HttpGet]
    public async Task<IActionResult> Search([FromQuery] string? term)
        => Ok(await _service.SearchAsync(term));

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        try { return Ok(await _service.GetByIdAsync(id)); }
        catch (KeyNotFoundException ex) { return NotFound(new { message = ex.Message }); }
    }

    [HttpPost]
    [Authorize(Roles = "SuperAdmin,Admin,Manager,Urbanisme")]
    public async Task<IActionResult> Create([FromBody] CreateDemandeurDto dto)
        => Ok(await _service.CreateAsync(dto));

    [HttpPut("{id:int}")]
    [Authorize(Roles = "SuperAdmin,Admin,Manager,Urbanisme")]
    public async Task<IActionResult> Update(int id, [FromBody] CreateDemandeurDto dto)
    {
        try { return Ok(await _service.UpdateAsync(id, dto)); }
        catch (KeyNotFoundException ex) { return NotFound(new { message = ex.Message }); }
    }
}

[ApiController]
[Route("api/architectes")]
[Authorize]
public class ArchitectesController : ControllerBase
{
    private readonly IArchitecteService _service;
    public ArchitectesController(IArchitecteService service) => _service = service;

    [HttpGet]
    public async Task<IActionResult> GetAll() => Ok(await _service.GetAllAsync());

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        try { return Ok(await _service.GetByIdAsync(id)); }
        catch (KeyNotFoundException ex) { return NotFound(new { message = ex.Message }); }
    }

    [HttpPost]
    [Authorize(Roles = "SuperAdmin,Admin,Manager,Urbanisme")]
    public async Task<IActionResult> Create([FromBody] CreateArchitecteDto dto)
    {
        try { return Ok(await _service.CreateAsync(dto)); }
        catch (InvalidOperationException ex) { return Conflict(new { message = ex.Message }); }
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = "SuperAdmin,Admin,Manager,Urbanisme")]
    public async Task<IActionResult> Update(int id, [FromBody] CreateArchitecteDto dto)
    {
        try { return Ok(await _service.UpdateAsync(id, dto)); }
        catch (KeyNotFoundException ex) { return NotFound(new { message = ex.Message }); }
        catch (InvalidOperationException ex) { return Conflict(new { message = ex.Message }); }
    }
}

// ════════════════════════════════════════════════════════════════
//  NOTIFICATIONS
// ════════════════════════════════════════════════════════════════

[ApiController]
[Route("api/notifications")]
[Authorize]
public class NotificationsController : ControllerBase
{
    private readonly INotificationService _service;
    public NotificationsController(INotificationService service) => _service = service;

    private string UserId => User.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty;

    [HttpGet]
    public async Task<IActionResult> GetMesNotifications([FromQuery] bool seulementNonLues = false)
        => Ok(await _service.GetByAgentAsync(UserId, seulementNonLues));

    [HttpGet("count")]
    public async Task<IActionResult> GetCount()
    {
        var count = await _service.GetNombreNonLuesAsync(UserId, estAgent: true);
        return Ok(new NotificationCountDto { NonLues = count });
    }

    [HttpPatch("{id:int}/lue")]
    public async Task<IActionResult> MarquerLue(int id)
    {
        try { await _service.MarquerLueAsync(id, UserId); return NoContent(); }
        catch (KeyNotFoundException ex) { return NotFound(new { message = ex.Message }); }
        catch (UnauthorizedAccessException) { return Forbid(); }
    }

    [HttpPatch("toutes-lues")]
    public async Task<IActionResult> MarquerToutesLues()
    {
        await _service.MarquerToutesLuesAsync(UserId, estAgent: true);
        return NoContent();
    }

    // Flutter — citoyens (sans auth ou avec token citoyen custom)
    [HttpGet("citoyen/{citoyenId}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetByCitoyen(string citoyenId, [FromQuery] bool seulementNonLues = false)
        => Ok(await _service.GetByCitoyenAsync(citoyenId, seulementNonLues));
}
