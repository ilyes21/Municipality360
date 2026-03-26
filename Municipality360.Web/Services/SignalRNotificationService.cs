// ═══════════════════════════════════════════════════════════════════
//  SignalRNotificationService.cs
//  Municipality360.Web/Services/SignalRNotificationService.cs
//
//  خدمة SignalR للـ Blazor Server — تستقبل الإشعارات الحية
//  وتُعلم المكوّنات المشتركة عبر حدث OnNotificationReceived
// ═══════════════════════════════════════════════════════════════════

using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.SignalR.Client;
using Municipality360.Application.DTOs.Notifications;
using System.Security.Claims;

namespace Municipality360.Web.Services;

public interface ISignalRNotificationService : IAsyncDisposable
{
    bool IsConnected { get; }
    List<NotificationDto> Notifications { get; }
    int UnreadCount { get; }

    event Action? OnChanged;
    event Action<NotificationDto>? OnNotificationReceived;

    Task StartAsync(string token);
    Task StopAsync();
    Task MarkAsReadAsync(int notificationId);
    Task MarkAllAsReadAsync();
    void LoadInitialNotifications(List<NotificationDto> notifications);
}

public class SignalRNotificationService : ISignalRNotificationService
{
    private HubConnection? _hub;
    private readonly IConfiguration _config;
    private readonly ILogger<SignalRNotificationService> _logger;

    public bool IsConnected => _hub?.State == HubConnectionState.Connected;
    public List<NotificationDto> Notifications { get; private set; } = new();
    public int UnreadCount => Notifications.Count(n => !n.EstLue);

    public event Action? OnChanged;
    public event Action<NotificationDto>? OnNotificationReceived;

    public SignalRNotificationService(
        IConfiguration config,
        ILogger<SignalRNotificationService> logger)
    {
        _config = config;
        _logger = logger;
    }

    public async Task StartAsync(string token)
    {
        if (_hub is { State: HubConnectionState.Connected }) return;

        var apiBase = _config["ApiSettings:BaseUrl"] ?? "https://localhost:7173";
        var hubUrl = $"{apiBase}/hubs/notifications?access_token={token}";

        _hub = new HubConnectionBuilder()
            .WithUrl(hubUrl)
            .WithAutomaticReconnect(new[] { TimeSpan.Zero, TimeSpan.FromSeconds(3), TimeSpan.FromSeconds(10) })
            .Build();

        // استقبال إشعار جديد
        _hub.On<NotificationDto>("NouvelleNotification", notif =>
        {
            Notifications.Insert(0, notif);
            OnNotificationReceived?.Invoke(notif);
            OnChanged?.Invoke();
        });

        // تأكيد قراءة إشعار
        _hub.On<int>("NotificationLue", id =>
        {
            var n = Notifications.FirstOrDefault(x => x.Id == id);
            if (n != null) n.EstLue = true;
            OnChanged?.Invoke();
        });

        // إعادة الاتصال
        _hub.Reconnected += _ => { OnChanged?.Invoke(); return Task.CompletedTask; };
        _hub.Reconnecting += _ => { OnChanged?.Invoke(); return Task.CompletedTask; };

        try
        {
            await _hub.StartAsync();
            _logger.LogInformation("✅ SignalR متصل: {Url}", hubUrl);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "⚠️ فشل الاتصال بـ SignalR");
        }
    }

    public async Task StopAsync()
    {
        if (_hub != null)
            await _hub.StopAsync();
    }

    public async Task MarkAsReadAsync(int notificationId)
    {
        var n = Notifications.FirstOrDefault(x => x.Id == notificationId);
        if (n != null)
        {
            n.EstLue = true;
            OnChanged?.Invoke();
        }

        if (_hub?.State == HubConnectionState.Connected)
        {
            try { await _hub.InvokeAsync("MarquerLue", notificationId); }
            catch { /* fail silently */ }
        }
    }

    public Task MarkAllAsReadAsync()
    {
        foreach (var n in Notifications) n.EstLue = true;
        OnChanged?.Invoke();
        return Task.CompletedTask;
    }

    public void LoadInitialNotifications(List<NotificationDto> notifications)
    {
        Notifications = notifications.OrderByDescending(n => n.DateCreation).ToList();
        OnChanged?.Invoke();
    }

    public async ValueTask DisposeAsync()
    {
        if (_hub != null)
        {
            await _hub.DisposeAsync();
            _hub = null;
        }
    }
}
