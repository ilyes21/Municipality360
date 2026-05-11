using System.ComponentModel.DataAnnotations;

namespace Municipality360.Application.DTOs.Structure;

// ============ DEPARTEMENT ============
public class DepartementDto
{
    public int Id { get; set; }
    public string Nom { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Code { get; set; }
    public bool IsActive { get; set; }
    public int NombreServices { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class CreateDepartementDto
{
    [Required, MaxLength(100)]
    public string Nom { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? Description { get; set; }

    [MaxLength(20)]
    public string? Code { get; set; }
}

public class UpdateDepartementDto
{
    [Required, MaxLength(100)]
    public string Nom { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? Description { get; set; }

    [MaxLength(20)]
    public string? Code { get; set; }

    public bool IsActive { get; set; }
}

// ============ SERVICE ============
public class ServiceDto
{
    public int Id { get; set; }
    public string Nom { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Code { get; set; }
    public bool IsActive { get; set; }
    public int DepartementId { get; set; }
    public string DepartementNom { get; set; } = string.Empty;
    public int NombreEmployes { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class CreateServiceDto
{
    [Required, MaxLength(100)]
    public string Nom { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? Description { get; set; }

    [MaxLength(20)]
    public string? Code { get; set; }

    [Required]
    public int DepartementId { get; set; }
}

public class UpdateServiceDto
{
    [Required, MaxLength(100)]
    public string Nom { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? Description { get; set; }

    [MaxLength(20)]
    public string? Code { get; set; }

    [Required]
    public int DepartementId { get; set; }

    public bool IsActive { get; set; }
}

// ============ POSTE ============
public class PosteDto
{
    public int Id { get; set; }
    public string Titre { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Code { get; set; }
    public decimal? SalaireMin { get; set; }
    public decimal? SalaireMax { get; set; }
    public bool IsActive { get; set; }
    public int NombreEmployes { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class CreatePosteDto
{
    [Required, MaxLength(100)]
    public string Titre { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? Description { get; set; }

    [MaxLength(20)]
    public string? Code { get; set; }

    public decimal? SalaireMin { get; set; }
    public decimal? SalaireMax { get; set; }
}

public class UpdatePosteDto
{
    [Required, MaxLength(100)]
    public string Titre { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? Description { get; set; }

    [MaxLength(20)]
    public string? Code { get; set; }

    public decimal? SalaireMin { get; set; }
    public decimal? SalaireMax { get; set; }

    public bool IsActive { get; set; }
}

// ============ EMPLOYE ============
public class EmployeDto
{
    public int Id { get; set; }
    public string Identifiant { get; set; } = string.Empty;
    public string Cin { get; set; } = string.Empty;
    public string Prenom { get; set; } = string.Empty;
    public string Nom { get; set; } = string.Empty;
    public string NomComplet { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? Telephone { get; set; }
    public DateTime DateNaissance { get; set; }
    public DateTime DateEmbauche { get; set; }
    public DateTime? DateDepart { get; set; }
    public string Genre { get; set; } = string.Empty;
    public string Statut { get; set; } = string.Empty;
    public decimal Salaire { get; set; }
    public string? Adresse { get; set; }
    public int ServiceId { get; set; }
    public string ServiceNom { get; set; } = string.Empty;
    public string DepartementNom { get; set; } = string.Empty;
    public int PosteId { get; set; }
    public string PosteTitre { get; set; } = string.Empty;
    public string? UserId { get; set; }         // ✅ الحساب المرتبط
    public bool HasAccount => !string.IsNullOrEmpty(UserId);
    public DateTime CreatedAt { get; set; }
}

// ── ربط / فك ربط حساب بموظف ────────────────────────────────────
public class LinkUserDto
{
    [Required]
    public string UserId { get; set; } = string.Empty;
}

public class CreateEmployeDto
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

    [Required]
    public DateTime DateNaissance { get; set; }

    [Required]
    public DateTime DateEmbauche { get; set; }

    [Required]
    public string Genre { get; set; } = string.Empty;

    [Required]
    public decimal Salaire { get; set; }

    [MaxLength(200)]
    public string? Adresse { get; set; }

    [Required]
    public int ServiceId { get; set; }

    [Required]
    public int PosteId { get; set; }
}

public class UpdateEmployeDto
{
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

    [Required]
    public string Genre { get; set; } = string.Empty;

    [Required]
    public string Statut { get; set; } = string.Empty;

    public decimal Salaire { get; set; }

    [MaxLength(200)]
    public string? Adresse { get; set; }

    public int ServiceId { get; set; }
    public int PosteId { get; set; }
}

public class EmployeFilterDto
{
    public int? ServiceId { get; set; }
    public int? DepartementId { get; set; }
    public int? PosteId { get; set; }
    public string? Statut { get; set; }
    public string? SearchTerm { get; set; }
    public string? UserId { get; set; }
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}
