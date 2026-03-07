namespace Municipality360.Application.DTOs.Notifications;

public class NotificationDto
{
    public int Id { get; set; }
    public string Type { get; set; } = string.Empty;
    public string Titre { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public int? EntiteId { get; set; }
    public string? EntiteType { get; set; }
    public string? LienNavigation { get; set; }
    public bool EstLue { get; set; }
    public DateTime? DateLecture { get; set; }
    public DateTime DateCreation { get; set; }
}

public class NotificationCountDto
{
    public int Total { get; set; }
    public int NonLues { get; set; }
}