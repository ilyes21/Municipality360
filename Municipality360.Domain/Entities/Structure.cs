using Municipality360.Domain.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Municipality360.Domain.Entities
{
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
}
