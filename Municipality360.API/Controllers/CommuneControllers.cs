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
using Municipality360.Application.DTOs.Structure;
using Municipality360.Application.Interfaces.Services;
using System.Security.Claims;

namespace Municipality360.API.Controllers;


// ════════════════════════════════════════════════════════════════
//  Departements
// ════════════════════════════════════════════════════════════════
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class DepartementsController : ControllerBase
{
    private readonly IDepartementService _service;

    public DepartementsController(IDepartementService service) => _service = service;

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var result = await _service.GetAllAsync();
        return Ok(result);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _service.GetByIdAsync(id);
        return result.Succeeded ? Ok(result) : NotFound(result);
    }

    [HttpPost]
    [Authorize(Roles = "SuperAdmin,Admin")]
    public async Task<IActionResult> Create([FromBody] CreateDepartementDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        var result = await _service.CreateAsync(dto);
        return result.Succeeded ? CreatedAtAction(nameof(GetById), new { id = result.Data!.Id }, result) : BadRequest(result);
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = "SuperAdmin,Admin")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateDepartementDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        var result = await _service.UpdateAsync(id, dto);
        return result.Succeeded ? Ok(result) : BadRequest(result);
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "SuperAdmin")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _service.DeleteAsync(id);
        return result.Succeeded ? Ok(result) : BadRequest(result);
    }
}
// ════════════════════════════════════════════════════════════════
//  Services
// ════════════════════════════════════════════════════════════════
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ServicesController : ControllerBase
{
    private readonly IServiceService _service;

    public ServicesController(IServiceService service) => _service = service;

    [HttpGet]
    public async Task<IActionResult> GetAll() => Ok(await _service.GetAllAsync());

    [HttpGet("departement/{departementId:int}")]
    public async Task<IActionResult> GetByDepartement(int departementId) =>
        Ok(await _service.GetByDepartementAsync(departementId));

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _service.GetByIdAsync(id);
        return result.Succeeded ? Ok(result) : NotFound(result);
    }

    [HttpPost]
    [Authorize(Roles = "SuperAdmin,Admin")]
    public async Task<IActionResult> Create([FromBody] CreateServiceDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        var result = await _service.CreateAsync(dto);
        return result.Succeeded ? CreatedAtAction(nameof(GetById), new { id = result.Data!.Id }, result) : BadRequest(result);
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = "SuperAdmin,Admin")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateServiceDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        var result = await _service.UpdateAsync(id, dto);
        return result.Succeeded ? Ok(result) : BadRequest(result);
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "SuperAdmin")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _service.DeleteAsync(id);
        return result.Succeeded ? Ok(result) : BadRequest(result);
    }
}

// ════════════════════════════════════════════════════════════════
//  Postes
// ════════════════════════════════════════════════════════════════
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class PostesController : ControllerBase
{
    private readonly IPosteService _service;

    public PostesController(IPosteService service) => _service = service;

    [HttpGet]
    public async Task<IActionResult> GetAll() => Ok(await _service.GetAllAsync());

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _service.GetByIdAsync(id);
        return result.Succeeded ? Ok(result) : NotFound(result);
    }

    [HttpPost]
    [Authorize(Roles = "SuperAdmin,Admin")]
    public async Task<IActionResult> Create([FromBody] CreatePosteDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        var result = await _service.CreateAsync(dto);
        return result.Succeeded ? CreatedAtAction(nameof(GetById), new { id = result.Data!.Id }, result) : BadRequest(result);
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = "SuperAdmin,Admin")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdatePosteDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        var result = await _service.UpdateAsync(id, dto);
        return result.Succeeded ? Ok(result) : BadRequest(result);
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "SuperAdmin")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _service.DeleteAsync(id);
        return result.Succeeded ? Ok(result) : BadRequest(result);
    }
}

// ════════════════════════════════════════════════════════════════
//  Employes
// ════════════════════════════════════════════════════════════════
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class EmployesController : ControllerBase
{
    private readonly IEmployeService _service;

    public EmployesController(IEmployeService service) => _service = service;

    [HttpGet]
    public async Task<IActionResult> GetPaged([FromQuery] EmployeFilterDto filter)
    {
        var result = await _service.GetPagedAsync(filter);
        return Ok(result);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _service.GetByIdAsync(id);
        return result.Succeeded ? Ok(result) : NotFound(result);
    }

    [HttpPost]
    [Authorize(Roles = "SuperAdmin,Admin,Manager")]
    public async Task<IActionResult> Create([FromBody] CreateEmployeDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        var result = await _service.CreateAsync(dto);
        return result.Succeeded ? CreatedAtAction(nameof(GetById), new { id = result.Data!.Id }, result) : BadRequest(result);
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = "SuperAdmin,Admin,Manager")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateEmployeDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        var result = await _service.UpdateAsync(id, dto);
        return result.Succeeded ? Ok(result) : BadRequest(result);
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "SuperAdmin,Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _service.DeleteAsync(id);
        return result.Succeeded ? Ok(result) : BadRequest(result);
    }
}




// ════════════════════════════════════════════════════════════════
//  BUREAU D'ORDRE — COURRIER ENTRANT
// ════════════════════════════════════════════════════════════════

[ApiController]
[Route("api/courriers-entrants")]
[Authorize]
public class CourriersEntrantsController : ControllerBase
{
    private readonly ICourrierEntrantService _service;
    private readonly IWebHostEnvironment _env;

    public CourriersEntrantsController(ICourrierEntrantService service, IWebHostEnvironment env)
    {
        _service = service;
        _env = env;
    }

    // ─── Helpers ──────────────────────────────────────────────────────────────
    private string UserId => User.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty;
    private string UserName => User.FindFirstValue(ClaimTypes.Name)
                            ?? User.FindFirstValue("name")
                            ?? "Inconnu";

    // ══════════════════════════════════════════════════════════════════════════
    //  LECTURES
    // ══════════════════════════════════════════════════════════════════════════

    /// <summary>Liste paginée avec filtres avancés</summary>
    [HttpGet]
    public async Task<IActionResult> GetPaged([FromQuery] CourrierEntrantFilterDto filter)
        => Ok(await _service.GetPagedAsync(filter));

    /// <summary>Détail complet (circuit + pièces jointes)</summary>
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        try { return Ok(await _service.GetByIdAsync(id)); }
        catch (KeyNotFoundException ex) { return NotFound(new { message = ex.Message }); }
    }

    /// <summary>Recherche par numéro d'ordre (ENT-YYYYNNN)</summary>
    [HttpGet("numero/{numero}")]
    public async Task<IActionResult> GetByNumero(string numero)
    {
        try { return Ok(await _service.GetByNumeroAsync(numero)); }
        catch (KeyNotFoundException ex) { return NotFound(new { message = ex.Message }); }
    }

    /// <summary>Courriers dont le délai de réponse est dépassé</summary>
    [HttpGet("en-retard")]
    public async Task<IActionResult> GetEnRetard()
        => Ok(await _service.GetEnRetardAsync());

    /// <summary>Courriers en attente pour un service donné</summary>
    [HttpGet("en-attente/service/{serviceId:int}")]
    public async Task<IActionResult> GetEnAttenteParService(int serviceId)
        => Ok(await _service.GetEnAttenteParServiceAsync(serviceId));

    /// <summary>Statistiques globales ou par service</summary>
    [HttpGet("stats")]
    public async Task<IActionResult> GetStats([FromQuery] int? serviceId)
        => Ok(await _service.GetStatsAsync(serviceId));

    // ══════════════════════════════════════════════════════════════════════════
    //  ENREGISTREMENT
    // ══════════════════════════════════════════════════════════════════════════

    /// <summary>
    /// Enregistrer un courrier entrant (corps JSON).
    /// Les pièces jointes peuvent être ajoutées séparément via POST /{id}/pieces-jointes
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "SuperAdmin,Admin,Manager,BOAgent")]
    public async Task<IActionResult> Enregistrer([FromBody] CreateCourrierEntrantDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        try
        {
            var result = await _service.EnregistrerAsync(dto, UserId, UserName);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }
        catch (Exception ex) { return BadRequest(new { message = ex.Message }); }
    }

    /// <summary>
    /// Enregistrer un courrier entrant avec pièces jointes en une seule requête multipart/form-data.
    /// Champs: tous les champs de CreateCourrierEntrantDto + fichiers (Files[])
    /// </summary>
    [HttpPost("avec-fichiers")]
    [Authorize(Roles = "SuperAdmin,Admin,Manager,BOAgent")]
    [RequestSizeLimit(52_428_800)] // 50 MB
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> EnregistrerAvecFichiers([FromForm] CreateCourrierEntrantAvecFichiersDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        try
        {
            // 1. Enregistrer le courrier
            var createDto = dto.ToCreateDto();
            var result = await _service.EnregistrerAsync(createDto, UserId, UserName);

            // 2. Uploader les fichiers si présents
            if (dto.Fichiers != null && dto.Fichiers.Count > 0)
            {
                var uploadDir = Path.Combine(_env.ContentRootPath, "uploads", "courriers-entrants", result.Id.ToString());
                Directory.CreateDirectory(uploadDir);

                short ordre = 1;
                foreach (var fichier in dto.Fichiers)
                {
                    if (fichier.Length == 0) continue;

                    var ext = Path.GetExtension(fichier.FileName).ToLowerInvariant();
                    if (ext is not ".pdf" and not ".png" and not ".jpg" and not ".jpeg" and not ".tiff")
                        continue;

                    var nomStocke = $"{Guid.NewGuid()}{ext}";
                    var chemin = Path.Combine(uploadDir, nomStocke);

                    await using (var stream = new FileStream(chemin, FileMode.Create))
                        await fichier.CopyToAsync(stream);

                    await _service.AjouterPieceJointeAsync(result.Id, new AjouterPieceJointeDto
                    {
                        NomFichierOriginal = fichier.FileName,
                        NomFichierStocke = nomStocke,
                        CheminFichier = chemin,
                        ExtensionFichier = ext,
                        TailleFichierOctets = fichier.Length,
                        TypePiece = ordre == 1 ? "DocumentPrincipal" : "Annexe",
                        Ordre = ordre++,
                        UploadedById = UserId
                    });
                }

                // Recharger avec PJ à jour
                result = await _service.GetByIdAsync(result.Id);
            }

            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }
        catch (Exception ex) { return BadRequest(new { message = ex.Message }); }
    }

    // ══════════════════════════════════════════════════════════════════════════
    //  MODIFICATION
    // ══════════════════════════════════════════════════════════════════════════

    /// <summary>Modifier les champs d'un courrier (hors pièces jointes)</summary>
    [HttpPut("{id:int}")]
    [Authorize(Roles = "SuperAdmin,Admin,Manager,BOAgent")]
    public async Task<IActionResult> Modifier(int id, [FromBody] UpdateCourrierEntrantDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        try
        {
            var result = await _service.ModifierAsync(id, dto, UserId);
            return Ok(result);
        }
        catch (KeyNotFoundException ex) { return NotFound(new { message = ex.Message }); }
        catch (InvalidOperationException ex) { return BadRequest(new { message = ex.Message }); }
    }

    // ══════════════════════════════════════════════════════════════════════════
    //  STATUT & AFFECTATION
    // ══════════════════════════════════════════════════════════════════════════

    /// <summary>Changer le statut (Enregistre → EnCours → Traite → Archive…)</summary>
    [HttpPatch("{id:int}/statut")]
    [Authorize(Roles = "SuperAdmin,Admin,Manager,BOAgent")]
    public async Task<IActionResult> ChangerStatut(int id, [FromBody] ChangerStatutEntrantDto dto)
    {
        try
        {
            await _service.ChangerStatutAsync(id, dto, UserId, UserName);
            return NoContent();
        }
        catch (KeyNotFoundException ex) { return NotFound(new { message = ex.Message }); }
        catch (InvalidOperationException ex) { return BadRequest(new { message = ex.Message }); }
    }

    /// <summary>
    /// Affecter (ou ré-affecter) à un service / agent sans passer par le circuit.
    /// Pratique pour les corrections rapides après enregistrement.
    /// </summary>
    [HttpPatch("{id:int}/affecter")]
    [Authorize(Roles = "SuperAdmin,Admin,Manager,BOAgent")]
    public async Task<IActionResult> Affecter(int id, [FromBody] AffecterCourrierEntrantDto dto)
    {
        try
        {
            await _service.AffecterAsync(id, dto, UserId, UserName);
            return NoContent();
        }
        catch (KeyNotFoundException ex) { return NotFound(new { message = ex.Message }); }
        catch (InvalidOperationException ex) { return BadRequest(new { message = ex.Message }); }
    }

    // ══════════════════════════════════════════════════════════════════════════
    //  CIRCUIT DE TRAITEMENT
    // ══════════════════════════════════════════════════════════════════════════

    /// <summary>Acheminer vers un autre service (crée une étape dans le circuit)</summary>
    [HttpPost("{id:int}/acheminer")]
    [Authorize(Roles = "SuperAdmin,Admin,Manager,BOAgent")]
    public async Task<IActionResult> Acheminer(int id, [FromBody] AcheminerCourrierDto dto)
    {
        try
        {
            var result = await _service.AcheminerAsync(id, dto, UserId);
            return Ok(result);
        }
        catch (KeyNotFoundException ex) { return NotFound(new { message = ex.Message }); }
        catch (InvalidOperationException ex) { return BadRequest(new { message = ex.Message }); }
    }

    /// <summary>Traiter une étape du circuit (ajouter commentaire / action effectuée)</summary>
    [HttpPatch("circuit/{circuitId:int}/traiter")]
    [Authorize(Roles = "SuperAdmin,Admin,Manager,BOAgent")]
    public async Task<IActionResult> TraiterEtapeCircuit(int circuitId, [FromBody] TraiterEtapeCircuitDto dto)
    {
        try
        {
            await _service.TraiterEtapeCircuitAsync(circuitId, dto, UserId, UserName);
            return NoContent();
        }
        catch (KeyNotFoundException ex) { return NotFound(new { message = ex.Message }); }
        catch (InvalidOperationException ex) { return BadRequest(new { message = ex.Message }); }
    }

    /// <summary>Retourner une étape (renvoi vers l'émetteur)</summary>
    [HttpPatch("circuit/{circuitId:int}/retourner")]
    [Authorize(Roles = "SuperAdmin,Admin,Manager,BOAgent")]
    public async Task<IActionResult> RetournerEtapeCircuit(int circuitId, [FromBody] RetournerEtapeCircuitDto dto)
    {
        try
        {
            await _service.RetournerEtapeCircuitAsync(circuitId, dto, UserId, UserName);
            return NoContent();
        }
        catch (KeyNotFoundException ex) { return NotFound(new { message = ex.Message }); }
        catch (InvalidOperationException ex) { return BadRequest(new { message = ex.Message }); }
    }

    /// <summary>Historique complet du circuit pour un courrier</summary>
    [HttpGet("{id:int}/circuit")]
    public async Task<IActionResult> GetCircuit(int id)
        => Ok(await _service.GetCircuitAsync(id));

    // ══════════════════════════════════════════════════════════════════════════
    //  PIÈCES JOINTES
    // ══════════════════════════════════════════════════════════════════════════

    /// <summary>
    /// Ajouter des pièces jointes à un courrier existant (multipart/form-data).
    /// Accepte: PDF, PNG, JPG, JPEG, TIFF — max 10 MB par fichier.
    /// </summary>
    [HttpPost("{id:int}/pieces-jointes")]
    [Authorize(Roles = "SuperAdmin,Admin,Manager,BOAgent")]
    [RequestSizeLimit(52_428_800)]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> AjouterPiecesJointes(int id, [FromForm] UploadPiecesJointesDto dto)
    {
        if (dto.Fichiers == null || dto.Fichiers.Count == 0)
            return BadRequest(new { message = "Aucun fichier fourni." });

        try
        {
            var uploadDir = Path.Combine(_env.ContentRootPath, "uploads", "courriers-entrants", id.ToString());
            Directory.CreateDirectory(uploadDir);

            // Récupérer le prochain ordre
            var detail = await _service.GetByIdAsync(id);
            short ordre = (short)((detail.PiecesJointes?.Count ?? 0) + 1);

            var resultPJ = new List<BOPieceJointeDto>();
            foreach (var fichier in dto.Fichiers)
            {
                if (fichier.Length == 0) continue;

                var ext = Path.GetExtension(fichier.FileName).ToLowerInvariant();
                if (ext is not ".pdf" and not ".png" and not ".jpg" and not ".jpeg" and not ".tiff")
                    return BadRequest(new { message = $"Type de fichier non autorisé: {ext}. Acceptés: pdf, png, jpg, jpeg, tiff" });

                if (fichier.Length > 10_485_760) // 10 MB
                    return BadRequest(new { message = $"Fichier {fichier.FileName} dépasse 10 MB." });

                var nomStocke = $"{Guid.NewGuid()}{ext}";
                var chemin = Path.Combine(uploadDir, nomStocke);

                await using (var stream = new FileStream(chemin, FileMode.Create))
                    await fichier.CopyToAsync(stream);

                var pj = await _service.AjouterPieceJointeAsync(id, new AjouterPieceJointeDto
                {
                    NomFichierOriginal = fichier.FileName,
                    NomFichierStocke = nomStocke,
                    CheminFichier = chemin,
                    ExtensionFichier = ext,
                    TailleFichierOctets = fichier.Length,
                    TypePiece = dto.TypePiece ?? (ordre == 1 ? "DocumentPrincipal" : "Annexe"),
                    Description = dto.Description,
                    Ordre = ordre++,
                    UploadedById = UserId
                });
                resultPJ.Add(pj);
            }

            return Ok(new { message = $"{resultPJ.Count} pièce(s) jointe(s) ajoutée(s).", pieces = resultPJ });
        }
        catch (KeyNotFoundException ex) { return NotFound(new { message = ex.Message }); }
        catch (InvalidOperationException ex) { return BadRequest(new { message = ex.Message }); }
    }

    /// <summary>Télécharger une pièce jointe</summary>
    [HttpGet("{id:int}/pieces-jointes/{pjId:int}/telecharger")]
    public async Task<IActionResult> TelechargerPieceJointe(int id, int pjId)
    {
        try
        {
            var pj = await _service.GetPieceJointeAsync(id, pjId);
            if (!System.IO.File.Exists(pj.CheminFichier))
                return NotFound(new { message = "Fichier introuvable sur le serveur." });

            var contentType = pj.ExtensionFichier switch
            {
                ".pdf" => "application/pdf",
                ".png" => "image/png",
                ".jpg" or ".jpeg" => "image/jpeg",
                ".tiff" => "image/tiff",
                _ => "application/octet-stream"
            };
            var bytes = await System.IO.File.ReadAllBytesAsync(pj.CheminFichier);
            return File(bytes, contentType, pj.NomFichierOriginal);
        }
        catch (KeyNotFoundException ex) { return NotFound(new { message = ex.Message }); }
    }

    /// <summary>Supprimer une pièce jointe</summary>
    [HttpDelete("{id:int}/pieces-jointes/{pjId:int}")]
    [Authorize(Roles = "SuperAdmin,Admin,Manager,BOAgent")]
    public async Task<IActionResult> SupprimerPieceJointe(int id, int pjId)
    {
        try
        {
            var pj = await _service.GetPieceJointeAsync(id, pjId);
            await _service.SupprimerPieceJointeAsync(id, pjId, UserId);

            // Supprimer le fichier physique
            if (System.IO.File.Exists(pj.CheminFichier))
                System.IO.File.Delete(pj.CheminFichier);

            return NoContent();
        }
        catch (KeyNotFoundException ex) { return NotFound(new { message = ex.Message }); }
        catch (UnauthorizedAccessException) { return Forbid(); }
    }

    // ══════════════════════════════════════════════════════════════════════════
    //  ARCHIVAGE
    // ══════════════════════════════════════════════════════════════════════════

    /// <summary>Archiver un courrier traité</summary>
    [HttpPost("{id:int}/archiver")]
    [Authorize(Roles = "SuperAdmin,Admin,Manager,BOAgent")]
    public async Task<IActionResult> Archiver(int id, [FromBody] ArchiversCourrierDto dto)
    {
        try
        {
            var result = await _service.ArchiverAsync(id, dto, UserId);
            return Ok(result);
        }
        catch (KeyNotFoundException ex) { return NotFound(new { message = ex.Message }); }
        catch (InvalidOperationException ex) { return BadRequest(new { message = ex.Message }); }
    }

    // ══════════════════════════════════════════════════════════════════════════
    //  SUPPRESSION (SuperAdmin uniquement — soft delete)
    // ══════════════════════════════════════════════════════════════════════════

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "SuperAdmin")]
    public async Task<IActionResult> Supprimer(int id)
    {
        try
        {
            await _service.SupprimerAsync(id, UserId);
            return NoContent();
        }
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
//  TYPES DE RÉCLAMATION
// ════════════════════════════════════════════════════════════════

[ApiController]
[Route("api/types-reclamation")]
[Authorize]
public class TypesReclamationController : ControllerBase
{
    private readonly ITypeReclamationService _service;
    public TypesReclamationController(ITypeReclamationService service) => _service = service;

    /// <summary>Liste tous les types actifs</summary>
    [HttpGet]
    public async Task<IActionResult> GetAll()
        => Ok(await _service.GetAllActiveAsync());

    /// <summary>Détail d'un type</summary>
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        try { return Ok(await _service.GetByIdAsync(id)); }
        catch (KeyNotFoundException ex) { return NotFound(new { message = ex.Message }); }
    }

    /// <summary>Créer un type</summary>
    [HttpPost]
    [Authorize(Roles = "SuperAdmin,Admin,Manager")]
    public async Task<IActionResult> Create([FromBody] CreateTypeReclamationDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        try
        {
            var result = await _service.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }
        catch (InvalidOperationException ex) { return Conflict(new { message = ex.Message }); }
    }

    /// <summary>Modifier un type</summary>
    [HttpPut("{id:int}")]
    [Authorize(Roles = "SuperAdmin,Admin,Manager")]
    public async Task<IActionResult> Update(int id, [FromBody] CreateTypeReclamationDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        try { return Ok(await _service.UpdateAsync(id, dto)); }
        catch (KeyNotFoundException ex) { return NotFound(new { message = ex.Message }); }
        catch (InvalidOperationException ex) { return Conflict(new { message = ex.Message }); }
    }

    /// <summary>Supprimer (soft delete)</summary>
    [HttpDelete("{id:int}")]
    [Authorize(Roles = "SuperAdmin,Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        try { await _service.DeleteAsync(id); return NoContent(); }
        catch (KeyNotFoundException ex) { return NotFound(new { message = ex.Message }); }
    }
}

// ════════════════════════════════════════════════════════════════
//  CATÉGORIES DE RÉCLAMATION
// ════════════════════════════════════════════════════════════════

[ApiController]
[Route("api/categories-reclamation")]
[Authorize]
public class CategoriesReclamationController : ControllerBase
{
    private readonly ICategorieReclamationService _service;
    public CategoriesReclamationController(ICategorieReclamationService service) => _service = service;

    /// <summary>Hiérarchie complète (parents + sous-catégories)</summary>
    [HttpGet]
    public async Task<IActionResult> GetHierarchie()
        => Ok(await _service.GetHierarchieAsync());

    /// <summary>Toutes les catégories à plat (pour dropdowns)</summary>
    [HttpGet("flat")]
    public async Task<IActionResult> GetFlat()
        => Ok(await _service.GetFlatAsync());

    /// <summary>Détail</summary>
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        try { return Ok(await _service.GetByIdAsync(id)); }
        catch (KeyNotFoundException ex) { return NotFound(new { message = ex.Message }); }
    }

    /// <summary>Créer une catégorie</summary>
    [HttpPost]
    [Authorize(Roles = "SuperAdmin,Admin,Manager")]
    public async Task<IActionResult> Create([FromBody] CreateCategorieReclamationDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        try
        {
            var result = await _service.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }
        catch (InvalidOperationException ex) { return Conflict(new { message = ex.Message }); }
    }

    /// <summary>Supprimer</summary>
    [HttpDelete("{id:int}")]
    [Authorize(Roles = "SuperAdmin,Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        try { await _service.DeleteAsync(id); return NoContent(); }
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

    [HttpGet("cin-exists/{cin}")]
    public async Task<IActionResult> CINExists(string cin, [FromQuery] int? excludeId)
        => Ok(await _service.CINExistsAsync(cin, excludeId));

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
