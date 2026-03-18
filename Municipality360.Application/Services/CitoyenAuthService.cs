using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Municipality360.Application.Common;
using Municipality360.Application.DTOs.Mobile;
using Municipality360.Application.Interfaces.Repositories;
using Municipality360.Application.Interfaces.Services;
using Org.BouncyCastle.Crypto.Generators;
using BCrypt.Net;

namespace Municipality360.Infrastructure.Services;

public class CitoyenAuthService : ICitoyenAuthService
{
    private readonly ICitoyenRepository _citoyenRepo;
    private readonly IReclamationRepository _reclamationRepo;
    private readonly IDemandePermisBatirRepository _permisRepo;
    private readonly INotificationRepository _notifRepo;
    private readonly IConfiguration _configuration;

    public CitoyenAuthService(
        ICitoyenRepository citoyenRepo,
        IReclamationRepository reclamationRepo,
        IDemandePermisBatirRepository permisRepo,
        INotificationRepository notifRepo,
        IConfiguration configuration)
    {
        _citoyenRepo = citoyenRepo;
        _reclamationRepo = reclamationRepo;
        _permisRepo = permisRepo;
        _notifRepo = notifRepo;
        _configuration = configuration;
    }

    public async Task<Result<CitoyenAuthResponseDto>> RegisterAsync(CitoyenRegisterMobileDto dto)
    {
        // التحقق من وجود CIN مسبقاً
        if (await _citoyenRepo.CINExistsAsync(dto.CIN))
            return Result<CitoyenAuthResponseDto>.Failure("رقم الهوية مسجل مسبقاً.");

        var citoyen = new Domain.Entities.Citoyen
        {
            CIN = dto.CIN,
            Nom = dto.Nom,
            Prenom = dto.Prenom,
            Telephone = dto.Telephone,
            Email = dto.Email,
            // BCrypt hash — تثبيت على WorkFactor=12 للأمان
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password, workFactor: 12),
            FcmToken = dto.FcmToken,
            IsActive = true
        };

        await _citoyenRepo.AddAsync(citoyen);

        var token = GenerateToken(citoyen);
        return Result<CitoyenAuthResponseDto>.Success(new CitoyenAuthResponseDto
        {
            Token = token,
            CitoyenId = citoyen.Id,
            NomComplet = $"{citoyen.Prenom} {citoyen.Nom}",
            Email = citoyen.Email,
            Telephone = citoyen.Telephone,
            ExpiresAt = DateTime.UtcNow.AddDays(30) // token أطول للموبايل
        }, "تم إنشاء الحساب بنجاح. أهلاً بك في ELAIN360!");
    }

    public async Task<Result<CitoyenAuthResponseDto>> LoginAsync(CitoyenLoginMobileDto dto)
    {
        // البحث بـ CIN أو الهاتف أو البريد
        var citoyens = await _citoyenRepo.FindAsync(c =>
            c.CIN == dto.Identifiant ||
            c.Telephone == dto.Identifiant ||
            c.Email == dto.Identifiant);

        var citoyen = citoyens.FirstOrDefault();
        if (citoyen is null || citoyen.PasswordHash is null)
            return Result<CitoyenAuthResponseDto>.Failure("بيانات الدخول غير صحيحة.");

        if (!BCrypt.Net.BCrypt.Verify(dto.Password, citoyen.PasswordHash))
            return Result<CitoyenAuthResponseDto>.Failure("بيانات الدخول غير صحيحة.");

        if (!citoyen.IsActive)
            return Result<CitoyenAuthResponseDto>.Failure("الحساب موقوف. تواصل مع البلدية.");

        // تحديث FCM token
        if (!string.IsNullOrEmpty(dto.FcmToken))
        {
            citoyen.FcmToken = dto.FcmToken;
            await _citoyenRepo.UpdateAsync(citoyen);
        }

        var token = GenerateToken(citoyen);
        return Result<CitoyenAuthResponseDto>.Success(new CitoyenAuthResponseDto
        {
            Token = token,
            CitoyenId = citoyen.Id,
            NomComplet = $"{citoyen.Prenom} {citoyen.Nom}",
            Email = citoyen.Email,
            Telephone = citoyen.Telephone,
            ExpiresAt = DateTime.UtcNow.AddDays(30)
        });
    }

    public async Task<Result<CitoyenProfileMobileDto>> GetProfileAsync(int citoyenId)
    {
        var citoyen = await _citoyenRepo.GetByIdAsync(citoyenId);
        if (citoyen is null) return Result<CitoyenProfileMobileDto>.Failure("المواطن غير موجود.");

        var stats = await GetDashboardStatsAsync(citoyenId);

        return Result<CitoyenProfileMobileDto>.Success(new CitoyenProfileMobileDto
        {
            Id = citoyen.Id,
            NomComplet = $"{citoyen.Prenom} {citoyen.Nom}",
            CIN = citoyen.CIN,
            Telephone = citoyen.Telephone,
            Email = citoyen.Email,
            Adresse = citoyen.Adresse,
            Ville = citoyen.Ville,
            Stats = stats.Data ?? new()
        });
    }

    public async Task<Result<CitoyenDashboardStatsDto>> GetDashboardStatsAsync(int citoyenId)
    {
        var reclamations = await _reclamationRepo.GetByCitoyenAsync(citoyenId);
        var nonLues = await _notifRepo.GetNombreNonLuesAsync(citoyenId.ToString(), estAgent: false);

        return Result<CitoyenDashboardStatsDto>.Success(new CitoyenDashboardStatsDto
        {
            ReclamationsEnCours = reclamations.Count(r => r.Statut is "Nouvelle" or "EnCours" or "Affectee"),
            ReclamationsTraitees = reclamations.Count(r => r.Statut == "Traitee"),
            NotificationsNonLues = nonLues
            // PermisEnAttente / PermisApprouves — à remplir si IDemandePermisBatirRepository a GetByCitoyenAsync
        });
    }

    public async Task<Result> UpdateFcmTokenAsync(int citoyenId, string fcmToken)
    {
        var citoyen = await _citoyenRepo.GetByIdAsync(citoyenId);
        if (citoyen is null) return Result.Failure("المواطن غير موجود.");

        citoyen.FcmToken = fcmToken;
        await _citoyenRepo.UpdateAsync(citoyen);
        return Result.Success();
    }

    // ── JWT Generator (citoyen) ──────────────────────────────────────
    private string GenerateToken(Domain.Entities.Citoyen citoyen)
    {
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub,   citoyen.Id.ToString()),
            new(JwtRegisteredClaimNames.Jti,   Guid.NewGuid().ToString()),
            new("citoyenId",  citoyen.Id.ToString()),
            new("nomComplet", $"{citoyen.Prenom} {citoyen.Nom}"),
            new(ClaimTypes.Role, "Citoyen") // دور خاص للموبايل
        };

        var jwtKey = _configuration["Jwt:Key"]
            ?? throw new InvalidOperationException("Jwt:Key manquant.");

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddDays(30),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}

