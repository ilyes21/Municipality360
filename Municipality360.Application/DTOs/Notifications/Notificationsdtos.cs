// ═══════════════════════════════════════════════════════════════════
//  NotificationsDtos.cs  ✅ FINAL
//  Application/DTOs/Notifications/NotificationsDtos.cs
//
//  يحتوي على جميع DTOs لوحدة الإشعارات.
//
//  ⚠️ تنبيه التزامن — NotificationDto:
//    الحقول تطابق تماماً NotificationService.MapToDto():
//    Id · Type · Titre · Message · EntiteId · EntiteType
//    LienNavigation · DestinataireAgentId · DestinataireCitoyenId
//    EstLue · DateCreation
//    (DateLecture محذوف لأن MapToDto لا تُعيّنها)
// ═══════════════════════════════════════════════════════════════════

namespace Municipality360.Application.DTOs.Notifications;

/// <summary>
/// DTO principal d'une notification.
/// Synchronisé avec NotificationService.MapToDto() et Notification entity.
/// </summary>
public class NotificationDto
{
    public int Id { get; set; }

    /// <summary>Valeur string de TypeNotification enum</summary>
    public string Type { get; set; } = string.Empty;

    public string Titre { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;

    /// <summary>ID de l'entité liée (courrier, réclamation, permis…)</summary>
    public int? EntiteId { get; set; }

    /// <summary>Type de l'entité liée : "CourrierEntrant", "Reclamation", "PermisBatir"…</summary>
    public string? EntiteType { get; set; }

    /// <summary>URL de navigation optionnelle (pour le frontend)</summary>
    public string? LienNavigation { get; set; }

    /// <summary>ID de l'agent destinataire (ApplicationUser.Id)</summary>
    public string? DestinataireAgentId { get; set; }

    /// <summary>ID du citoyen destinataire (app Flutter)</summary>
    public string? DestinataireCitoyenId { get; set; }

    public bool EstLue { get; set; }

    /// <summary>Date de lecture (null si non lue)</summary>
    public DateTime? DateLecture { get; set; }

    public DateTime DateCreation { get; set; }
}

/// <summary>Compteur de notifications non lues — retourné par GET /api/notifications/count</summary>
public class NotificationCountDto
{
    public int Total { get; set; }
    public int NonLues { get; set; }
}