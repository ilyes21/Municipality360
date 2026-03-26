// ═══════════════════════════════════════════════════════════════════
//  NotificationApiService.cs
//  Municipality360.Web/Services/NotificationApiService.cs
//
//  يتواصل مع API لجلب/تحديث الإشعارات
// ═══════════════════════════════════════════════════════════════════

using Municipality360.Application.DTOs.Notifications;

namespace Municipality360.Web.Services;

public interface INotificationApiService
{
    Task<List<NotificationDto>> GetMyNotificationsAsync(bool unreadOnly = false);
    Task<int> GetUnreadCountAsync();
    Task<bool> MarkAsReadAsync(int id);
    Task<bool> MarkAllAsReadAsync();
}

public class NotificationApiService : INotificationApiService
{
    private readonly ApiService _api;

    public NotificationApiService(ApiService api) => _api = api;

    public async Task<List<NotificationDto>> GetMyNotificationsAsync(bool unreadOnly = false)
    {
        var q = $"api/notifications?seulementNonLues={unreadOnly}";
        return await _api.GetAsync<List<NotificationDto>>(q) ?? new();
    }

    public async Task<int> GetUnreadCountAsync()
    {
        var result = await _api.GetAsync<NotificationCountDto>("api/NotificationsAgent/count");
        return result?.NonLues ?? 0;
    }

    public async Task<bool> MarkAsReadAsync(int id)
    {
        var r = await _api.PatchAsync<object, object>($"api/NotificationsAgent/{id}/lue", new { });
        return r != null;
    }

    public async Task<bool> MarkAllAsReadAsync()
    {
        var r = await _api.PatchAsync<object, object>("api/NotificationsAgent/lire-tout", new { });
        return r != null;
    }
}
