using System.ComponentModel.DataAnnotations;
using Municipality360.Domain.Common;

namespace Municipality360.Domain.Entities;

public enum Genre { Masculin, Feminin }
public enum StatutEmploye { Actif, Inactif, Suspendu, Retraite }

public class Employe : BaseEntity
{
    [Required, MaxLength(50)]
    public string Identifiant { get; set; } = string.Empty;
    [Required, MaxLength(50)]
    public string Cin { get; set; } = string.Empty;

    [Required, MaxLength(50)]
    public string Prenom { get; set; } = string.Empty;

    [Required, MaxLength(50)]
    public string Nom { get; set; } = string.Empty;

    [MaxLength(100), EmailAddress]
    public string? Email { get; set; }

    [MaxLength(20)]
    public string? Telephone { get; set; }

    public DateTime DateNaissance { get; set; }
    public DateTime DateEmbauche { get; set; }
    public DateTime? DateDepart { get; set; }

    public Genre Genre { get; set; }
    public StatutEmploye Statut { get; set; } = StatutEmploye.Actif;

    public decimal Salaire { get; set; }

    [MaxLength(200)]
    public string? Adresse { get; set; }

    // FK
    public int ServiceId { get; set; }
    public int PosteId { get; set; }

    // Navigation
    public Service Service { get; set; } = null!;
    public Poste Poste { get; set; } = null!;

    // Computed
    public string NomComplet => $"{Prenom} {Nom}";
}
