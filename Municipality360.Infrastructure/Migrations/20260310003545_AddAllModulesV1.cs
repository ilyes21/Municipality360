using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Municipality360.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddAllModulesV1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Architectes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NumeroOrdre = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CIN = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Nom = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Prenom = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Telephone = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Architectes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    FirstName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    UserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SecurityStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "bit", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "bit", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUsers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BOCategoriesCourrier",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    Libelle = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CouleurHex = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EstConfidentiel = table.Column<bool>(type: "bit", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BOCategoriesCourrier", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BOContacts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TypeContact = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    Nom = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Prenom = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RaisonSociale = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Fonction = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Adresse = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Ville = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Wilaya = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Telephone = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BOContacts", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CategoriesReclamation",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Libelle = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Icone = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CouleurHex = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ParentId = table.Column<int>(type: "int", nullable: true),
                    Niveau = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CategoriesReclamation", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CategoriesReclamation_CategoriesReclamation_ParentId",
                        column: x => x.ParentId,
                        principalTable: "CategoriesReclamation",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Citoyens",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CIN = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Nom = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Prenom = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    DateNaissance = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LieuNaissance = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Sexe = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    Adresse = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Ville = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CodePostal = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Telephone = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    TelephoneMobile = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SituationFamiliale = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Citoyens", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CommissionsExamen",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Libelle = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DateReunion = table.Column<DateTime>(type: "datetime2", nullable: true),
                    PresidentId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SecretaireId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StatutReunion = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ProcesVerbal = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CommissionsExamen", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Demandeurs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Type = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CIN = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Nom = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Prenom = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RaisonSociale = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Adresse = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Telephone = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Demandeurs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Departements",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nom = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Code = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Departements", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Notifications",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Type = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: false),
                    Cible = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Titre = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Message = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EntiteId = table.Column<int>(type: "int", nullable: true),
                    EntiteType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LienNavigation = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DestinataireAgentId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    DestinataireCitoyenId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    EstLue = table.Column<bool>(type: "bit", nullable: false),
                    DateLecture = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DateCreation = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Notifications", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "NumeroSequences",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Prefixe = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Annee = table.Column<int>(type: "int", nullable: false),
                    DernierNumero = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NumeroSequences", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Postes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Titre = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Code = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    SalaireMin = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    SalaireMax = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Postes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TypesDemandePermis",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Libelle = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DelaiTraitementJours = table.Column<int>(type: "int", nullable: false),
                    TarifBase = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TypesDemandePermis", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TypesTaxe",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Libelle = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TauxCalcul = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    UniteCalcul = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TypesTaxe", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ZonagesUrbanisme",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Libelle = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CoefficientOccupationSol = table.Column<decimal>(type: "decimal(5,2)", nullable: true),
                    CoefficientUtilisationSol = table.Column<decimal>(type: "decimal(5,2)", nullable: true),
                    HauteurMaximale = table.Column<decimal>(type: "decimal(5,2)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ZonagesUrbanisme", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderKey = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Services",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nom = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Code = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    DepartementId = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Services", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Services_Departements_DepartementId",
                        column: x => x.DepartementId,
                        principalTable: "Departements",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "BODossiers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NumeroDossier = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Intitule = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ServiceResponsableId = table.Column<int>(type: "int", nullable: true),
                    DateOuverture = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DateCloture = table.Column<DateTime>(type: "datetime2", nullable: true),
                    StatutDossier = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedById = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BODossiers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BODossiers_Services_ServiceResponsableId",
                        column: x => x.ServiceResponsableId,
                        principalTable: "Services",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "DemandesPermisBatir",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NumeroDemande = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    DateDepot = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TypeDemandeId = table.Column<int>(type: "int", nullable: false),
                    DemandeurId = table.Column<int>(type: "int", nullable: false),
                    Statut = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ArchitecteId = table.Column<int>(type: "int", nullable: true),
                    AdresseProjet = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NumeroParcelle = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SuperficieTerrain = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    SuperficieAConstruire = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    NombreNiveaux = table.Column<int>(type: "int", nullable: true),
                    TypeConstruction = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    CoutEstimatif = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    ZonageId = table.Column<int>(type: "int", nullable: true),
                    Longitude = table.Column<decimal>(type: "decimal(10,8)", nullable: true),
                    Latitude = table.Column<decimal>(type: "decimal(10,8)", nullable: true),
                    ServiceInstructeurId = table.Column<int>(type: "int", nullable: true),
                    AgentInstructeurId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DateDebutInstruction = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CommissionExamenId = table.Column<int>(type: "int", nullable: true),
                    DateDecision = table.Column<DateTime>(type: "datetime2", nullable: true),
                    MotifRejet = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ConditionsSpeciales = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Observations = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EnregistreParId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DemandesPermisBatir", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DemandesPermisBatir_Architectes_ArchitecteId",
                        column: x => x.ArchitecteId,
                        principalTable: "Architectes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_DemandesPermisBatir_CommissionsExamen_CommissionExamenId",
                        column: x => x.CommissionExamenId,
                        principalTable: "CommissionsExamen",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_DemandesPermisBatir_Demandeurs_DemandeurId",
                        column: x => x.DemandeurId,
                        principalTable: "Demandeurs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DemandesPermisBatir_Services_ServiceInstructeurId",
                        column: x => x.ServiceInstructeurId,
                        principalTable: "Services",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_DemandesPermisBatir_TypesDemandePermis_TypeDemandeId",
                        column: x => x.TypeDemandeId,
                        principalTable: "TypesDemandePermis",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DemandesPermisBatir_ZonagesUrbanisme_ZonageId",
                        column: x => x.ZonageId,
                        principalTable: "ZonagesUrbanisme",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "Employes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Identifiant = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Cin = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Prenom = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Nom = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Telephone = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    DateNaissance = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DateEmbauche = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DateDepart = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Genre = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Statut = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Salaire = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Adresse = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ServiceId = table.Column<int>(type: "int", nullable: false),
                    PosteId = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Employes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Employes_Postes_PosteId",
                        column: x => x.PosteId,
                        principalTable: "Postes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Employes_Services_ServiceId",
                        column: x => x.ServiceId,
                        principalTable: "Services",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TypesReclamation",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Libelle = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DelaiTraitementJours = table.Column<int>(type: "int", nullable: false),
                    ServiceResponsableId = table.Column<int>(type: "int", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TypesReclamation", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TypesReclamation_Services_ServiceResponsableId",
                        column: x => x.ServiceResponsableId,
                        principalTable: "Services",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "BOCourriersEntrants",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NumeroOrdre = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: false),
                    NumeroExterne = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DateReception = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DateCourrier = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ObjetCourrier = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TypeDocument = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CategorieId = table.Column<int>(type: "int", nullable: true),
                    DossierId = table.Column<int>(type: "int", nullable: true),
                    ExpediteurContactId = table.Column<int>(type: "int", nullable: true),
                    ExpediteurLibreNom = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ModeReception = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    NumeroRecommande = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ServiceDestinataireId = table.Column<int>(type: "int", nullable: true),
                    AgentDestinataireId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NombrePages = table.Column<short>(type: "smallint", nullable: false),
                    Priorite = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    EstConfidentiel = table.Column<bool>(type: "bit", nullable: false),
                    Statut = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    DelaiReponse = table.Column<DateTime>(type: "datetime2", nullable: true),
                    NecessiteReponse = table.Column<bool>(type: "bit", nullable: false),
                    Observation = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EnregistreParId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BOCourriersEntrants", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BOCourriersEntrants_BOCategoriesCourrier_CategorieId",
                        column: x => x.CategorieId,
                        principalTable: "BOCategoriesCourrier",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_BOCourriersEntrants_BOContacts_ExpediteurContactId",
                        column: x => x.ExpediteurContactId,
                        principalTable: "BOContacts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_BOCourriersEntrants_BODossiers_DossierId",
                        column: x => x.DossierId,
                        principalTable: "BODossiers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_BOCourriersEntrants_Services_ServiceDestinataireId",
                        column: x => x.ServiceDestinataireId,
                        principalTable: "Services",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "DocumentsPermis",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DemandeId = table.Column<int>(type: "int", nullable: false),
                    TypeDocument = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NomFichier = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CheminFichier = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TailleFichier = table.Column<long>(type: "bigint", nullable: true),
                    Statut = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Observations = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EstObligatoire = table.Column<bool>(type: "bit", nullable: false),
                    AjouteeParCitoyen = table.Column<bool>(type: "bit", nullable: false),
                    UploadedById = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DocumentsPermis", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DocumentsPermis_DemandesPermisBatir_DemandeId",
                        column: x => x.DemandeId,
                        principalTable: "DemandesPermisBatir",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "InspectionsPermis",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DemandeId = table.Column<int>(type: "int", nullable: false),
                    Objet = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DateInspection = table.Column<DateTime>(type: "datetime2", nullable: false),
                    InspecteurId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NomInspecteur = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Observations = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ReservesEmises = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Conforme = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InspectionsPermis", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InspectionsPermis_DemandesPermisBatir_DemandeId",
                        column: x => x.DemandeId,
                        principalTable: "DemandesPermisBatir",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PermisDelivres",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DemandeId = table.Column<int>(type: "int", nullable: false),
                    NumeroPermis = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    DateDelivrance = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DateValidite = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Conditions = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FichierPermis = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DelivreParId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EstRevoque = table.Column<bool>(type: "bit", nullable: false),
                    MotifRevocation = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PermisDelivres", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PermisDelivres_DemandesPermisBatir_DemandeId",
                        column: x => x.DemandeId,
                        principalTable: "DemandesPermisBatir",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SuivisPermis",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DemandeId = table.Column<int>(type: "int", nullable: false),
                    StatutPrecedent = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    NouveauStatut = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    DateChangement = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UtilisateurId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UtilisateurNom = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Commentaire = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    VisibleCitoyen = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SuivisPermis", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SuivisPermis_DemandesPermisBatir_DemandeId",
                        column: x => x.DemandeId,
                        principalTable: "DemandesPermisBatir",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TaxesPermis",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DemandeId = table.Column<int>(type: "int", nullable: false),
                    TypeTaxeId = table.Column<int>(type: "int", nullable: false),
                    Montant = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Statut = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    DatePaiement = table.Column<DateTime>(type: "datetime2", nullable: true),
                    NumeroRecu = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaxesPermis", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TaxesPermis_DemandesPermisBatir_DemandeId",
                        column: x => x.DemandeId,
                        principalTable: "DemandesPermisBatir",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TaxesPermis_TypesTaxe_TypeTaxeId",
                        column: x => x.TypeTaxeId,
                        principalTable: "TypesTaxe",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Reclamations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NumeroReclamation = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    DateDepot = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DateIncident = table.Column<DateTime>(type: "datetime2", nullable: true),
                    TypeReclamationId = table.Column<int>(type: "int", nullable: false),
                    CategorieId = table.Column<int>(type: "int", nullable: false),
                    Priorite = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Statut = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CitoyenId = table.Column<int>(type: "int", nullable: false),
                    EstAnonyme = table.Column<bool>(type: "bit", nullable: false),
                    Canal = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Objet = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Localisation = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Longitude = table.Column<decimal>(type: "decimal(10,8)", nullable: true),
                    Latitude = table.Column<decimal>(type: "decimal(10,8)", nullable: true),
                    ServiceConcerneId = table.Column<int>(type: "int", nullable: true),
                    AffecteAId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DateAffectation = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DateCloture = table.Column<DateTime>(type: "datetime2", nullable: true),
                    SolutionApportee = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SatisfactionCitoyen = table.Column<int>(type: "int", nullable: true),
                    EnregistreParId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reclamations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Reclamations_CategoriesReclamation_CategorieId",
                        column: x => x.CategorieId,
                        principalTable: "CategoriesReclamation",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Reclamations_Citoyens_CitoyenId",
                        column: x => x.CitoyenId,
                        principalTable: "Citoyens",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Reclamations_Services_ServiceConcerneId",
                        column: x => x.ServiceConcerneId,
                        principalTable: "Services",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Reclamations_TypesReclamation_TypeReclamationId",
                        column: x => x.TypeReclamationId,
                        principalTable: "TypesReclamation",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "BOCircuits",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CourrierEntrantId = table.Column<int>(type: "int", nullable: false),
                    NumeroEtape = table.Column<short>(type: "smallint", nullable: false),
                    ServiceEmetteurId = table.Column<int>(type: "int", nullable: true),
                    AgentEmetteurId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ServiceRecepteurId = table.Column<int>(type: "int", nullable: false),
                    AgentRecepteurId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DateTransmission = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DateTraitement = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DelaiTraitement = table.Column<DateTime>(type: "datetime2", nullable: true),
                    TypeAction = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                    InstructionTransmission = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CommentaireTraitement = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ActionEffectuee = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StatutEtape = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    EstRetour = table.Column<bool>(type: "bit", nullable: false),
                    MotifRetour = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedById = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BOCircuits", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BOCircuits_BOCourriersEntrants_CourrierEntrantId",
                        column: x => x.CourrierEntrantId,
                        principalTable: "BOCourriersEntrants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BOCircuits_Services_ServiceEmetteurId",
                        column: x => x.ServiceEmetteurId,
                        principalTable: "Services",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_BOCircuits_Services_ServiceRecepteurId",
                        column: x => x.ServiceRecepteurId,
                        principalTable: "Services",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "BOCourriersSortants",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NumeroOrdre = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: false),
                    NumeroReference = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CourrierEntrantRefId = table.Column<int>(type: "int", nullable: true),
                    DateRedaction = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DateSignature = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DateEnvoi = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ObjetCourrier = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TypeDocument = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CategorieId = table.Column<int>(type: "int", nullable: true),
                    DossierId = table.Column<int>(type: "int", nullable: true),
                    ServiceEmetteurId = table.Column<int>(type: "int", nullable: true),
                    RedacteurId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SignataireId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FonctionSignataire = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EstSigne = table.Column<bool>(type: "bit", nullable: false),
                    DestinataireContactId = table.Column<int>(type: "int", nullable: true),
                    DestinataireLibreNom = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ModeEnvoi = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    NumeroRecommande = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NombrePages = table.Column<short>(type: "smallint", nullable: false),
                    EstConfidentiel = table.Column<bool>(type: "bit", nullable: false),
                    Priorite = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Statut = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    AccuseReceptionRecu = table.Column<bool>(type: "bit", nullable: false),
                    DateAccuseExtRecu = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Observation = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedById = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BOCourriersSortants", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BOCourriersSortants_BOCategoriesCourrier_CategorieId",
                        column: x => x.CategorieId,
                        principalTable: "BOCategoriesCourrier",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_BOCourriersSortants_BOContacts_DestinataireContactId",
                        column: x => x.DestinataireContactId,
                        principalTable: "BOContacts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_BOCourriersSortants_BOCourriersEntrants_CourrierEntrantRefId",
                        column: x => x.CourrierEntrantRefId,
                        principalTable: "BOCourriersEntrants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_BOCourriersSortants_BODossiers_DossierId",
                        column: x => x.DossierId,
                        principalTable: "BODossiers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_BOCourriersSortants_Services_ServiceEmetteurId",
                        column: x => x.ServiceEmetteurId,
                        principalTable: "Services",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "BOPiecesJointesEntrant",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CourrierEntrantId = table.Column<int>(type: "int", nullable: false),
                    NomFichierOriginal = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NomFichierStocke = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CheminFichier = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ExtensionFichier = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TailleFichierOctets = table.Column<long>(type: "bigint", nullable: false),
                    TypePiece = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Ordre = table.Column<short>(type: "smallint", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UploadedById = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BOPiecesJointesEntrant", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BOPiecesJointesEntrant_BOCourriersEntrants_CourrierEntrantId",
                        column: x => x.CourrierEntrantId,
                        principalTable: "BOCourriersEntrants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PiecesJointesReclamation",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ReclamationId = table.Column<int>(type: "int", nullable: false),
                    TypeDocument = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NomFichier = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CheminFichier = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TailleFichier = table.Column<long>(type: "bigint", nullable: true),
                    AjouteeParCitoyen = table.Column<bool>(type: "bit", nullable: false),
                    UploadedById = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PiecesJointesReclamation", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PiecesJointesReclamation_Reclamations_ReclamationId",
                        column: x => x.ReclamationId,
                        principalTable: "Reclamations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SuivisReclamation",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ReclamationId = table.Column<int>(type: "int", nullable: false),
                    StatutPrecedent = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    NouveauStatut = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    DateChangement = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UtilisateurId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UtilisateurNom = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Commentaire = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ActionEffectuee = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    VisibleCitoyen = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SuivisReclamation", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SuivisReclamation_Reclamations_ReclamationId",
                        column: x => x.ReclamationId,
                        principalTable: "Reclamations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BOArchives",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CourrierEntrantId = table.Column<int>(type: "int", nullable: true),
                    CourrierSortantId = table.Column<int>(type: "int", nullable: true),
                    NumeroArchive = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: false),
                    CodeBarre = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SalleArchive = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Rayon = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Boite = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Classification = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    DureeConservationAns = table.Column<short>(type: "smallint", nullable: false),
                    DateDebutConservation = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DateFinConservation = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CheminArchiveNumerique = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EstDetruit = table.Column<bool>(type: "bit", nullable: false),
                    ArchiveParId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Observation = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DateArchivage = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BOArchives", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BOArchives_BOCourriersEntrants_CourrierEntrantId",
                        column: x => x.CourrierEntrantId,
                        principalTable: "BOCourriersEntrants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_BOArchives_BOCourriersSortants_CourrierSortantId",
                        column: x => x.CourrierSortantId,
                        principalTable: "BOCourriersSortants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "BOPiecesJointesSortant",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CourrierSortantId = table.Column<int>(type: "int", nullable: false),
                    NomFichierOriginal = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NomFichierStocke = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CheminFichier = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ExtensionFichier = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TailleFichierOctets = table.Column<long>(type: "bigint", nullable: false),
                    TypePiece = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Ordre = table.Column<short>(type: "smallint", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EstVersionFinale = table.Column<bool>(type: "bit", nullable: false),
                    UploadedById = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BOPiecesJointesSortant", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BOPiecesJointesSortant_BOCourriersSortants_CourrierSortantId",
                        column: x => x.CourrierSortantId,
                        principalTable: "BOCourriersSortants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Architectes_NumeroOrdre",
                table: "Architectes",
                column: "NumeroOrdre",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true,
                filter: "[NormalizedName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId",
                table: "AspNetUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                table: "AspNetUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "AspNetUsers",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true,
                filter: "[NormalizedUserName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_BOArchives_CourrierEntrantId",
                table: "BOArchives",
                column: "CourrierEntrantId",
                unique: true,
                filter: "[CourrierEntrantId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_BOArchives_CourrierSortantId",
                table: "BOArchives",
                column: "CourrierSortantId",
                unique: true,
                filter: "[CourrierSortantId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_BOArchives_NumeroArchive",
                table: "BOArchives",
                column: "NumeroArchive",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_BOCategoriesCourrier_Code",
                table: "BOCategoriesCourrier",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_BOCircuits_CourrierEntrantId_NumeroEtape",
                table: "BOCircuits",
                columns: new[] { "CourrierEntrantId", "NumeroEtape" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_BOCircuits_ServiceEmetteurId",
                table: "BOCircuits",
                column: "ServiceEmetteurId");

            migrationBuilder.CreateIndex(
                name: "IX_BOCircuits_ServiceRecepteurId",
                table: "BOCircuits",
                column: "ServiceRecepteurId");

            migrationBuilder.CreateIndex(
                name: "IX_BOCourriersEntrants_CategorieId",
                table: "BOCourriersEntrants",
                column: "CategorieId");

            migrationBuilder.CreateIndex(
                name: "IX_BOCourriersEntrants_DossierId",
                table: "BOCourriersEntrants",
                column: "DossierId");

            migrationBuilder.CreateIndex(
                name: "IX_BOCourriersEntrants_ExpediteurContactId",
                table: "BOCourriersEntrants",
                column: "ExpediteurContactId");

            migrationBuilder.CreateIndex(
                name: "IX_BOCourriersEntrants_NumeroOrdre",
                table: "BOCourriersEntrants",
                column: "NumeroOrdre",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_BOCourriersEntrants_ServiceDestinataireId",
                table: "BOCourriersEntrants",
                column: "ServiceDestinataireId");

            migrationBuilder.CreateIndex(
                name: "IX_BOCourriersSortants_CategorieId",
                table: "BOCourriersSortants",
                column: "CategorieId");

            migrationBuilder.CreateIndex(
                name: "IX_BOCourriersSortants_CourrierEntrantRefId",
                table: "BOCourriersSortants",
                column: "CourrierEntrantRefId");

            migrationBuilder.CreateIndex(
                name: "IX_BOCourriersSortants_DestinataireContactId",
                table: "BOCourriersSortants",
                column: "DestinataireContactId");

            migrationBuilder.CreateIndex(
                name: "IX_BOCourriersSortants_DossierId",
                table: "BOCourriersSortants",
                column: "DossierId");

            migrationBuilder.CreateIndex(
                name: "IX_BOCourriersSortants_NumeroOrdre",
                table: "BOCourriersSortants",
                column: "NumeroOrdre",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_BOCourriersSortants_ServiceEmetteurId",
                table: "BOCourriersSortants",
                column: "ServiceEmetteurId");

            migrationBuilder.CreateIndex(
                name: "IX_BODossiers_NumeroDossier",
                table: "BODossiers",
                column: "NumeroDossier",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_BODossiers_ServiceResponsableId",
                table: "BODossiers",
                column: "ServiceResponsableId");

            migrationBuilder.CreateIndex(
                name: "IX_BOPiecesJointesEntrant_CourrierEntrantId",
                table: "BOPiecesJointesEntrant",
                column: "CourrierEntrantId");

            migrationBuilder.CreateIndex(
                name: "IX_BOPiecesJointesSortant_CourrierSortantId",
                table: "BOPiecesJointesSortant",
                column: "CourrierSortantId");

            migrationBuilder.CreateIndex(
                name: "IX_CategoriesReclamation_Code",
                table: "CategoriesReclamation",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CategoriesReclamation_ParentId",
                table: "CategoriesReclamation",
                column: "ParentId");

            migrationBuilder.CreateIndex(
                name: "IX_Citoyens_CIN",
                table: "Citoyens",
                column: "CIN",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DemandesPermisBatir_ArchitecteId",
                table: "DemandesPermisBatir",
                column: "ArchitecteId");

            migrationBuilder.CreateIndex(
                name: "IX_DemandesPermisBatir_CommissionExamenId",
                table: "DemandesPermisBatir",
                column: "CommissionExamenId");

            migrationBuilder.CreateIndex(
                name: "IX_DemandesPermisBatir_DemandeurId",
                table: "DemandesPermisBatir",
                column: "DemandeurId");

            migrationBuilder.CreateIndex(
                name: "IX_DemandesPermisBatir_NumeroDemande",
                table: "DemandesPermisBatir",
                column: "NumeroDemande",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DemandesPermisBatir_ServiceInstructeurId",
                table: "DemandesPermisBatir",
                column: "ServiceInstructeurId");

            migrationBuilder.CreateIndex(
                name: "IX_DemandesPermisBatir_TypeDemandeId",
                table: "DemandesPermisBatir",
                column: "TypeDemandeId");

            migrationBuilder.CreateIndex(
                name: "IX_DemandesPermisBatir_ZonageId",
                table: "DemandesPermisBatir",
                column: "ZonageId");

            migrationBuilder.CreateIndex(
                name: "IX_Departements_Code",
                table: "Departements",
                column: "Code",
                unique: true,
                filter: "[Code] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_DocumentsPermis_DemandeId",
                table: "DocumentsPermis",
                column: "DemandeId");

            migrationBuilder.CreateIndex(
                name: "IX_Employes_Cin",
                table: "Employes",
                column: "Cin",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Employes_PosteId",
                table: "Employes",
                column: "PosteId");

            migrationBuilder.CreateIndex(
                name: "IX_Employes_ServiceId",
                table: "Employes",
                column: "ServiceId");

            migrationBuilder.CreateIndex(
                name: "IX_InspectionsPermis_DemandeId",
                table: "InspectionsPermis",
                column: "DemandeId");

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_DestinataireAgentId",
                table: "Notifications",
                column: "DestinataireAgentId");

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_DestinataireCitoyenId",
                table: "Notifications",
                column: "DestinataireCitoyenId");

            migrationBuilder.CreateIndex(
                name: "IX_NumeroSequences_Prefixe_Annee",
                table: "NumeroSequences",
                columns: new[] { "Prefixe", "Annee" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PermisDelivres_DemandeId",
                table: "PermisDelivres",
                column: "DemandeId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PermisDelivres_NumeroPermis",
                table: "PermisDelivres",
                column: "NumeroPermis",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PiecesJointesReclamation_ReclamationId",
                table: "PiecesJointesReclamation",
                column: "ReclamationId");

            migrationBuilder.CreateIndex(
                name: "IX_Postes_Code",
                table: "Postes",
                column: "Code",
                unique: true,
                filter: "[Code] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Reclamations_CategorieId",
                table: "Reclamations",
                column: "CategorieId");

            migrationBuilder.CreateIndex(
                name: "IX_Reclamations_CitoyenId",
                table: "Reclamations",
                column: "CitoyenId");

            migrationBuilder.CreateIndex(
                name: "IX_Reclamations_NumeroReclamation",
                table: "Reclamations",
                column: "NumeroReclamation",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Reclamations_ServiceConcerneId",
                table: "Reclamations",
                column: "ServiceConcerneId");

            migrationBuilder.CreateIndex(
                name: "IX_Reclamations_TypeReclamationId",
                table: "Reclamations",
                column: "TypeReclamationId");

            migrationBuilder.CreateIndex(
                name: "IX_Services_Code",
                table: "Services",
                column: "Code",
                unique: true,
                filter: "[Code] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Services_DepartementId",
                table: "Services",
                column: "DepartementId");

            migrationBuilder.CreateIndex(
                name: "IX_SuivisPermis_DemandeId",
                table: "SuivisPermis",
                column: "DemandeId");

            migrationBuilder.CreateIndex(
                name: "IX_SuivisReclamation_ReclamationId",
                table: "SuivisReclamation",
                column: "ReclamationId");

            migrationBuilder.CreateIndex(
                name: "IX_TaxesPermis_DemandeId",
                table: "TaxesPermis",
                column: "DemandeId");

            migrationBuilder.CreateIndex(
                name: "IX_TaxesPermis_TypeTaxeId",
                table: "TaxesPermis",
                column: "TypeTaxeId");

            migrationBuilder.CreateIndex(
                name: "IX_TypesDemandePermis_Code",
                table: "TypesDemandePermis",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TypesReclamation_Code",
                table: "TypesReclamation",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TypesReclamation_ServiceResponsableId",
                table: "TypesReclamation",
                column: "ServiceResponsableId");

            migrationBuilder.CreateIndex(
                name: "IX_TypesTaxe_Code",
                table: "TypesTaxe",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ZonagesUrbanisme_Code",
                table: "ZonagesUrbanisme",
                column: "Code",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AspNetRoleClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens");

            migrationBuilder.DropTable(
                name: "BOArchives");

            migrationBuilder.DropTable(
                name: "BOCircuits");

            migrationBuilder.DropTable(
                name: "BOPiecesJointesEntrant");

            migrationBuilder.DropTable(
                name: "BOPiecesJointesSortant");

            migrationBuilder.DropTable(
                name: "DocumentsPermis");

            migrationBuilder.DropTable(
                name: "Employes");

            migrationBuilder.DropTable(
                name: "InspectionsPermis");

            migrationBuilder.DropTable(
                name: "Notifications");

            migrationBuilder.DropTable(
                name: "NumeroSequences");

            migrationBuilder.DropTable(
                name: "PermisDelivres");

            migrationBuilder.DropTable(
                name: "PiecesJointesReclamation");

            migrationBuilder.DropTable(
                name: "SuivisPermis");

            migrationBuilder.DropTable(
                name: "SuivisReclamation");

            migrationBuilder.DropTable(
                name: "TaxesPermis");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "BOCourriersSortants");

            migrationBuilder.DropTable(
                name: "Postes");

            migrationBuilder.DropTable(
                name: "Reclamations");

            migrationBuilder.DropTable(
                name: "DemandesPermisBatir");

            migrationBuilder.DropTable(
                name: "TypesTaxe");

            migrationBuilder.DropTable(
                name: "BOCourriersEntrants");

            migrationBuilder.DropTable(
                name: "CategoriesReclamation");

            migrationBuilder.DropTable(
                name: "Citoyens");

            migrationBuilder.DropTable(
                name: "TypesReclamation");

            migrationBuilder.DropTable(
                name: "Architectes");

            migrationBuilder.DropTable(
                name: "CommissionsExamen");

            migrationBuilder.DropTable(
                name: "Demandeurs");

            migrationBuilder.DropTable(
                name: "TypesDemandePermis");

            migrationBuilder.DropTable(
                name: "ZonagesUrbanisme");

            migrationBuilder.DropTable(
                name: "BOCategoriesCourrier");

            migrationBuilder.DropTable(
                name: "BOContacts");

            migrationBuilder.DropTable(
                name: "BODossiers");

            migrationBuilder.DropTable(
                name: "Services");

            migrationBuilder.DropTable(
                name: "Departements");
        }
    }
}
