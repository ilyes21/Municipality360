// ═══════════════════════════════════════════════════════════════════
//  NotificationHub.cs
//  API/Hubs/NotificationHub.cs
// ═══════════════════════════════════════════════════════════════════

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace Municipality360.API.Hubs;

/// <summary>
/// Hub SignalR principal pour les notifications en temps réel.
///
/// Groupes utilisés :
///   agent-{userId}      → Blazor / Web agents internes
///   service-{serviceId} → tous les agents d'un service
///   role-{roleName}     → tous les agents d'un rôle
///   citoyen-{citoyenId} → Flutter app citoyens
///
/// Connexion Flutter (non authentifiée) :
///   1. Connecter au hub
///   2. Appeler InscrireCitoyen(citoyenId)
///   3. Écouter "NouvelleNotification"
/// </summary>
[Authorize]
public class NotificationHub : Hub
{
    // ── Helpers statiques utilisés par NotificationService ──────────

    public static string GroupeAgent(string userId) => $"agent-{userId}";
    public static string GroupeService(int serviceId) => $"service-{serviceId}";
    public static string GroupeRole(string roleName) => $"role-{roleName}";
    public static string GroupeCitoyen(string citoyenId) => $"citoyen-{citoyenId}";

    // ── Connexion ────────────────────────────────────────────────────

    public override async Task OnConnectedAsync()
    {
        var userId = Context.User?.FindFirstValue(ClaimTypes.NameIdentifier);

        if (userId != null)
        {
            // Groupe personnel
            await Groups.AddToGroupAsync(Context.ConnectionId, GroupeAgent(userId));

            // Groupes de rôles
            var roles = Context.User?.FindAll(ClaimTypes.Role)
                .Select(c => c.Value) ?? Enumerable.Empty<string>();

            foreach (var role in roles)
                await Groups.AddToGroupAsync(Context.ConnectionId, GroupeRole(role));

            // Groupe service (claim personnalisé)
            var serviceId = Context.User?.FindFirstValue("serviceId");
            if (serviceId != null)
                await Groups.AddToGroupAsync(Context.ConnectionId, GroupeService(int.Parse(serviceId)));
        }

        await base.OnConnectedAsync();
    }

    // ── Déconnexion ─────────────────────────────────────────────────

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var userId = Context.User?.FindFirstValue(ClaimTypes.NameIdentifier);

        if (userId != null)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, GroupeAgent(userId));

            var roles = Context.User?.FindAll(ClaimTypes.Role)
                .Select(c => c.Value) ?? Enumerable.Empty<string>();
            foreach (var role in roles)
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, GroupeRole(role));
        }

        await base.OnDisconnectedAsync(exception);
    }

    // ── Méthodes appelables par les clients ──────────────────────────

    /// <summary>
    /// [Flutter] Enregistrer un citoyen dans son groupe personnel.
    /// Appeler après connexion avec le citoyenId.
    /// </summary>
    [AllowAnonymous]
    public async Task InscrireCitoyen(string citoyenId)
    {
        if (!string.IsNullOrEmpty(citoyenId))
            await Groups.AddToGroupAsync(Context.ConnectionId, GroupeCitoyen(citoyenId));
    }

    /// <summary>
    /// [Blazor / Flutter] Marquer une notification comme lue côté client.
    /// Informer le serveur pour mise à jour en BDD.
    /// </summary>
    public async Task MarquerLue(int notificationId)
    {
        // Le client informe le hub — la logique BDD est dans INotificationService
        await Clients.Caller.SendAsync("NotificationMarquee", notificationId);
    }

    /// <summary>
    /// [Debug] Vérifier la connexion.
    /// </summary>
    public async Task Ping()
        => await Clients.Caller.SendAsync("Pong", DateTime.UtcNow);
}