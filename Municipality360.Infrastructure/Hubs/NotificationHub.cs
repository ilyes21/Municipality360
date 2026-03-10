// ═══════════════════════════════════════════════════════════════════
//  NotificationHub.cs  ✅ FIXED — نُقل إلى Infrastructure
//  Infrastructure/Hubs/NotificationHub.cs
//
//  السبب: Infrastructure لا يمكنها الاعتماد على API (circular dependency)
//  الحل: نقل NotificationHub من API/Hubs/ إلى Infrastructure/Hubs/
//        ثم في API/Program.cs: app.MapHub<NotificationHub>("/hubs/notifications");
// ═══════════════════════════════════════════════════════════════════

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace Municipality360.Infrastructure.Hubs;

[Authorize]
public class NotificationHub : Hub
{
    // ── Noms de groupes statiques ────────────────────────────────────
    public static string GroupeAgent(string userId) => $"agent_{userId}";
    public static string GroupeService(int serviceId) => $"service_{serviceId}";
    public static string GroupeRole(string roleName) => $"role_{roleName}";
    public static string GroupeCitoyen(string citoyenId) => $"citoyen_{citoyenId}";

    // ── Connexion ────────────────────────────────────────────────────

    public override async Task OnConnectedAsync()
    {
        var userId = Context.UserIdentifier;
        if (userId != null)
            await Groups.AddToGroupAsync(Context.ConnectionId, GroupeAgent(userId));

        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var userId = Context.UserIdentifier;
        if (userId != null)
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, GroupeAgent(userId));

        await base.OnDisconnectedAsync(exception);
    }

    // ── Méthodes appelables par le client ────────────────────────────

    /// <summary>Flutter — s'abonner aux notifs citoyen (AllowAnonymous via JWT custom)</summary>
    [AllowAnonymous]
    public async Task InscrireCitoyen(string citoyenId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, GroupeCitoyen(citoyenId));
    }

    /// <summary>Marquer une notification comme lue</summary>
    public async Task MarquerLue(int notificationId)
    {
        await Clients.Caller.SendAsync("NotificationLue", notificationId);
    }

    /// <summary>Test de connexion</summary>
    public async Task Ping()
    {
        await Clients.Caller.SendAsync("Pong", DateTime.UtcNow);
    }
}
