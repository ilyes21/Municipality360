using System.ComponentModel.DataAnnotations;
using Municipality360.Domain.Common;

namespace Municipality360.Domain.Entities;

public class Departement : BaseEntity
{
    [Required, MaxLength(100)]
    public string Nom { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? Description { get; set; }

    [MaxLength(20)]
    public string? Code { get; set; }

    public bool IsActive { get; set; } = true;

    // Navigation
    public ICollection<Service> Services { get; set; } = new List<Service>();
}
