using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Sahel.GMAO.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialDomain : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ArticlesPdr",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CodeArticle = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Designation = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ReferenceConstructeur = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Unite = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PrixUnitaire = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    QuantiteEnStock = table.Column<double>(type: "float", nullable: false),
                    SeuilAlerte = table.Column<double>(type: "float", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ArticlesPdr", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Equipements",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Designation = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Marque = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Modele = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NumeroSerie = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Section = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SousSection = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DateMiseEnService = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CaracteristiquesTechniques = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Equipements", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Username = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FullName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Role = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Position = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CanManageUsers = table.Column<bool>(type: "bit", nullable: false),
                    CanViewAudit = table.Column<bool>(type: "bit", nullable: false),
                    CanEditInventory = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MaintenancePreventives",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EquipementId = table.Column<int>(type: "int", nullable: false),
                    Operation = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FrequenceJours = table.Column<int>(type: "int", nullable: false),
                    DateDerniereRealisation = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DateProchaineEcheance = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    ConsignesParticulieres = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MaintenancePreventives", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MaintenancePreventives_Equipements_EquipementId",
                        column: x => x.EquipementId,
                        principalTable: "Equipements",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DemandesTravail",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NumeroDT = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    DateEmission = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DemandeurId = table.Column<int>(type: "int", nullable: false),
                    EquipementId = table.Column<int>(type: "int", nullable: false),
                    OrganePartie = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TravailDemande = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    InstructionsPreparation = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DateReceptionPrevue = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Statut = table.Column<int>(type: "int", nullable: false),
                    TravailExecute = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DateExecutionDebut = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DateExecutionFin = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DureeArretProductionHeures = table.Column<double>(type: "float", nullable: false),
                    TotalCoutPieces = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    TotalCoutMainOeuvre = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    TotalCoutOperation = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DemandesTravail", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DemandesTravail_Equipements_EquipementId",
                        column: x => x.EquipementId,
                        principalTable: "Equipements",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DemandesTravail_Users_DemandeurId",
                        column: x => x.DemandeurId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "RapportsIncidents",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NumeroRapport = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    EquipementId = table.Column<int>(type: "int", nullable: false),
                    DateIncident = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DescriptionIncident = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CausesProbables = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ActionsCorrectives = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RedacteurId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RapportsIncidents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RapportsIncidents_Equipements_EquipementId",
                        column: x => x.EquipementId,
                        principalTable: "Equipements",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RapportsIncidents_Users_RedacteurId",
                        column: x => x.RedacteurId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ConsommableUsages",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DemandeTravailId = table.Column<int>(type: "int", nullable: false),
                    ArticlePdrId = table.Column<int>(type: "int", nullable: false),
                    Quantite = table.Column<double>(type: "float", nullable: false),
                    PrixUnitaireApplique = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConsommableUsages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ConsommableUsages_ArticlesPdr_ArticlePdrId",
                        column: x => x.ArticlePdrId,
                        principalTable: "ArticlesPdr",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ConsommableUsages_DemandesTravail_DemandeTravailId",
                        column: x => x.DemandeTravailId,
                        principalTable: "DemandesTravail",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "InterventionRoles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DemandeTravailId = table.Column<int>(type: "int", nullable: false),
                    IntervenantId = table.Column<int>(type: "int", nullable: false),
                    Qualification = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    HeuresTravaillees = table.Column<double>(type: "float", nullable: false),
                    TauxHoraire = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InterventionRoles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InterventionRoles_DemandesTravail_DemandeTravailId",
                        column: x => x.DemandeTravailId,
                        principalTable: "DemandesTravail",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_InterventionRoles_Users_IntervenantId",
                        column: x => x.IntervenantId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ArticlesPdr_CodeArticle",
                table: "ArticlesPdr",
                column: "CodeArticle",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ConsommableUsages_ArticlePdrId",
                table: "ConsommableUsages",
                column: "ArticlePdrId");

            migrationBuilder.CreateIndex(
                name: "IX_ConsommableUsages_DemandeTravailId",
                table: "ConsommableUsages",
                column: "DemandeTravailId");

            migrationBuilder.CreateIndex(
                name: "IX_DemandesTravail_DemandeurId",
                table: "DemandesTravail",
                column: "DemandeurId");

            migrationBuilder.CreateIndex(
                name: "IX_DemandesTravail_EquipementId",
                table: "DemandesTravail",
                column: "EquipementId");

            migrationBuilder.CreateIndex(
                name: "IX_DemandesTravail_NumeroDT",
                table: "DemandesTravail",
                column: "NumeroDT",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Equipements_Code",
                table: "Equipements",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_InterventionRoles_DemandeTravailId",
                table: "InterventionRoles",
                column: "DemandeTravailId");

            migrationBuilder.CreateIndex(
                name: "IX_InterventionRoles_IntervenantId",
                table: "InterventionRoles",
                column: "IntervenantId");

            migrationBuilder.CreateIndex(
                name: "IX_MaintenancePreventives_EquipementId",
                table: "MaintenancePreventives",
                column: "EquipementId");

            migrationBuilder.CreateIndex(
                name: "IX_RapportsIncidents_EquipementId",
                table: "RapportsIncidents",
                column: "EquipementId");

            migrationBuilder.CreateIndex(
                name: "IX_RapportsIncidents_NumeroRapport",
                table: "RapportsIncidents",
                column: "NumeroRapport",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RapportsIncidents_RedacteurId",
                table: "RapportsIncidents",
                column: "RedacteurId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Username",
                table: "Users",
                column: "Username",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ConsommableUsages");

            migrationBuilder.DropTable(
                name: "InterventionRoles");

            migrationBuilder.DropTable(
                name: "MaintenancePreventives");

            migrationBuilder.DropTable(
                name: "RapportsIncidents");

            migrationBuilder.DropTable(
                name: "ArticlesPdr");

            migrationBuilder.DropTable(
                name: "DemandesTravail");

            migrationBuilder.DropTable(
                name: "Equipements");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
