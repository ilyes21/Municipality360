using System.ComponentModel.DataAnnotations;
using Municipality360.Domain.Common;

namespace Municipality360.Domain.Entities;

public class Service : BaseEntity
{
    [Required, MaxLength(100)]
    public string Nom { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? Description { get; set; }

    [MaxLength(20)]
    public string? Code { get; set; }

    public bool IsActive { get; set; } = true;

    // FK
    public int DepartementId { get; set; }

    // Navigation
    public Departement Departement { get; set; } = null!;
    public ICollection<Employe> Employes { get; set; } = new List<Employe>();
}
