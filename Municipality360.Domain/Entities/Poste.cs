using System.ComponentModel.DataAnnotations;
using Municipality360.Domain.Common;

namespace Municipality360.Domain.Entities;

public class Poste : BaseEntity
{
    [Required, MaxLength(100)]
    public string Titre { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? Description { get; set; }

    [MaxLength(20)]
    public string? Code { get; set; }

    public decimal? SalaireMin { get; set; }
    public decimal? SalaireMax { get; set; }

    public bool IsActive { get; set; } = true;

    // Navigation
    public ICollection<Employe> Employes { get; set; } = new List<Employe>();
}
