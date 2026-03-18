// ═══════════════════════════════════════════════════════════════════
//  NotificationService.cs  ✅ FIXED
//  Infrastructure/Services/NotificationService.cs
//
//  الإصلاحات:
//  ✅ حذف: using Microsoft.AspNet.SignalR;  — مكتبة قديمة خاطئة (ASP.NET classic)
//  ✅ إضافة: using Municipality360.API.Hubs; — يحل خطأ CS0246 "NotificationHub introuvable"
// ═══════════════════════════════════════════════════════════════════

using Microsoft.AspNetCore.SignalR;
using Municipality360.Infrastructure.Hubs;               // ✅ FIXED: نفس المشروع — لا circular dependency
using Municipality360.Application.DTOs.Notifications;
using Municipality360.Application.Interfaces.Repositories;
using Municipality360.Application.Interfaces.Services;
using Municipality360.Domain.Entities;

namespace Municipality360.Infrastructure.Services;

public class NotificationService : INotificationService
{
    private readonly INotificationRepository _repo;
    private readonly IHubContext<NotificationHub> _hub;
    private readonly IServiceRepository _serviceRepo;

    public NotificationService(
        INotificationRepository repo,
        IHubContext<NotificationHub> hub,
        IServiceRepository serviceRepo)
    {
        _repo = repo;
        _hub = hub;
        _serviceRepo = serviceRepo;
    }

    // ── Lectures ────────────────────────────────────────────────────

    public Task<List<NotificationDto>> GetByAgentAsync(string agentId, bool seulementNonLues = false)
        => _repo.GetByAgentAsync(agentId, seulementNonLues);

    public Task<List<NotificationDto>> GetByCitoyenAsync(string citoyenId, bool seulementNonLues = false)
        => _repo.GetByCitoyenAsync(citoyenId, seulementNonLues);

    public Task<int> GetNombreNonLuesAsync(string destinataireId, bool estAgent = true)
        => _repo.GetNombreNonLuesAsync(destinataireId, estAgent);

    public async Task MarquerLueAsync(int notificationId, string userId)
    {
        var notif = await _repo.GetByIdAsync(notificationId)
            ?? throw new KeyNotFoundException($"Notification #{notificationId} introuvable.");

        if (notif.DestinataireAgentId != userId && notif.DestinataireCitoyenId != userId)
            throw new UnauthorizedAccessException("Notification non autorisée.");

        await _repo.MarquerLueAsync(notificationId);
    }

    public Task MarquerToutesLuesAsync(string destinataireId, bool estAgent = true)
        => _repo.MarquerToutesLuesAsync(destinataireId, estAgent);

    // ── Envoi vers un agent ─────────────────────────────────────────

    public async Task NotifierAgentAsync(
        string agentId, TypeNotification type, string message,
        string? entiteId = null, string? entiteType = null)
    {
        if (string.IsNullOrEmpty(agentId)) return;

        var notif = new Notification
        {
            Type = type,
            Cible = CibleNotification.AgentInterne,
            Titre = type.ToString(),
            Message = message,
            EntiteId = entiteId != null ? int.TryParse(entiteId, out var eid) ? eid : null : null,
            EntiteType = entiteType,
            DestinataireAgentId = agentId,
            EstLue = false
        };

        await _repo.AddAsync(notif);

        await _hub.Clients
            .Group(NotificationHub.GroupeAgent(agentId))
            .SendAsync("NouvelleNotification", MapToDto(notif));
    }

    // ── Envoi vers tous les agents d'un service ─────────────────────

    public async Task NotifierServiceAsync(
    int serviceId, TypeNotification type, string message,
    string? entiteId = null, string? entiteType = null)
    {
        // ✅ FIXED: استدعاء GetEmployesAsync المضاف إلى IServiceRepository
        var employes = await _serviceRepo.GetEmployesAsync(serviceId);

        foreach (var emp in employes.Where(e => e.UserId != null))
            await NotifierAgentAsync(emp.UserId!, type, message, entiteId, entiteType);

        // إشعار لمجموعة الخدمة الكاملة عبر SignalR
        await _hub.Clients
            .Group(NotificationHub.GroupeService(serviceId))
            .SendAsync("NouvelleNotification", new
            {
                Type = type.ToString(),
                Message = message,
                EntiteId = entiteId,
                EntiteType = entiteType,
                Date = DateTime.UtcNow
            });
    }

    // ── Envoi vers un citoyen (Flutter app) ─────────────────────────

    public async Task NotifierCitoyenAsync(
        string citoyenId, TypeNotification type, string message,
        string? entiteId = null, string? entiteType = null)
    {
        if (string.IsNullOrEmpty(citoyenId)) return;

        var notif = new Notification
        {
            Type = type,
            Cible = CibleNotification.Citoyen,
            Titre = type.ToString(),
            Message = message,
            EntiteId = entiteId != null ? int.TryParse(entiteId, out var eid) ? eid : null : null,
            EntiteType = entiteType,
            DestinataireCitoyenId = citoyenId,
            EstLue = false
        };

        await _repo.AddAsync(notif);

        await _hub.Clients
            .Group(NotificationHub.GroupeCitoyen(citoyenId))
            .SendAsync("NouvelleNotification", MapToDto(notif));
    }

    // ── Mapping ─────────────────────────────────────────────────────

    private static NotificationDto MapToDto(Notification n) => new()
    {
        Id = n.Id,
        Type = n.Type.ToString(),
        Titre = n.Titre,
        Message = n.Message,
        EntiteId = n.EntiteId,
        EntiteType = n.EntiteType,
        LienNavigation = n.LienNavigation,
        DestinataireAgentId = n.DestinataireAgentId,
        DestinataireCitoyenId = n.DestinataireCitoyenId,
        EstLue = n.EstLue,
        DateCreation = n.DateCreation
    };
}
