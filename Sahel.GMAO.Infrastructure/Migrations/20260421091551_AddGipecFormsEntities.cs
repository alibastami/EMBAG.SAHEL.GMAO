using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Sahel.GMAO.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddGipecFormsEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DateSouhaitee",
                table: "DemandesFabrication");

            migrationBuilder.AddColumn<string>(
                name: "CodeArticle",
                table: "DemandesFabrication",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Consignes",
                table: "DemandesFabrication",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DebutTravail",
                table: "DemandesFabrication",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "DegreUrgenceImmediat",
                table: "DemandesFabrication",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "DemandeTravailId",
                table: "DemandesFabrication",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "FinTravail",
                table: "DemandesFabrication",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Matiere",
                table: "DemandesFabrication",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NDessin",
                table: "DemandesFabrication",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ReceptionDate",
                table: "DemandesFabrication",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ReferencePiece",
                table: "DemandesFabrication",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "TempsAlloueHeures",
                table: "DemandesFabrication",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "TotalCoutMainOeuvre",
                table: "DemandesFabrication",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "TotalCoutOperation",
                table: "DemandesFabrication",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "TotalCoutPieces",
                table: "DemandesFabrication",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<DateTime>(
                name: "VisaComptabiliteAnalytique",
                table: "DemandesFabrication",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "VisaExecution",
                table: "DemandesFabrication",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "VisaPreparation",
                table: "DemandesFabrication",
                type: "datetime2",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "BonsDeConsignation",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DemandeTravailId = table.Column<int>(type: "int", nullable: true),
                    EquipementId = table.Column<int>(type: "int", nullable: false),
                    Atelier = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DateHeureConsignation = table.Column<DateTime>(type: "datetime2", nullable: true),
                    MotifConsignation = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AgentConsignationId = table.Column<int>(type: "int", nullable: true),
                    ServiceUtilisateurConsignation = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    VisaConsignation = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DateHeureDeconsignation = table.Column<DateTime>(type: "datetime2", nullable: true),
                    AgentDeconsignationId = table.Column<int>(type: "int", nullable: true),
                    ServiceUtilisateurDeconsignation = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    VisaDeconsignation = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BonsDeConsignation", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BonsDeConsignation_DemandesTravail_DemandeTravailId",
                        column: x => x.DemandeTravailId,
                        principalTable: "DemandesTravail",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BonsDeConsignation_Equipements_EquipementId",
                        column: x => x.EquipementId,
                        principalTable: "Equipements",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BonsDeConsignation_Users_AgentConsignationId",
                        column: x => x.AgentConsignationId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BonsDeConsignation_Users_AgentDeconsignationId",
                        column: x => x.AgentDeconsignationId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "FichesEntretienPreventif",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NumeroOT = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Section = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Periodicite = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Partie = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EquipementId = table.Column<int>(type: "int", nullable: false),
                    MaintenancePreventiveId = table.Column<int>(type: "int", nullable: true),
                    DateFaitLe = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IntervenantId = table.Column<int>(type: "int", nullable: true),
                    VisaIntervenant = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FichesEntretienPreventif", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FichesEntretienPreventif_Equipements_EquipementId",
                        column: x => x.EquipementId,
                        principalTable: "Equipements",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_FichesEntretienPreventif_MaintenancePreventives_MaintenancePreventiveId",
                        column: x => x.MaintenancePreventiveId,
                        principalTable: "MaintenancePreventives",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_FichesEntretienPreventif_Users_IntervenantId",
                        column: x => x.IntervenantId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "IntervenantsFabrication",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DemandeFabricationId = table.Column<int>(type: "int", nullable: false),
                    NomsEtPrenoms = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Qualification = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NombreHeures = table.Column<double>(type: "float", nullable: false),
                    CoutHoraire = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IntervenantsFabrication", x => x.Id);
                    table.ForeignKey(
                        name: "FK_IntervenantsFabrication_DemandesFabrication_DemandeFabricationId",
                        column: x => x.DemandeFabricationId,
                        principalTable: "DemandesFabrication",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MatieresFabrication",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DemandeFabricationId = table.Column<int>(type: "int", nullable: false),
                    N_BSM = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DesignationMatiere = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Section = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Longueur = table.Column<double>(type: "float", nullable: false),
                    Prix = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Observation = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MatieresFabrication", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MatieresFabrication_DemandesFabrication_DemandeFabricationId",
                        column: x => x.DemandeFabricationId,
                        principalTable: "DemandesFabrication",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PointagesMachinesFabrication",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DemandeFabricationId = table.Column<int>(type: "int", nullable: false),
                    IntervenantId = table.Column<int>(type: "int", nullable: false),
                    Tour200Heures = table.Column<double>(type: "float", nullable: false),
                    Tour250Heures = table.Column<double>(type: "float", nullable: false),
                    Tour300Heures = table.Column<double>(type: "float", nullable: false),
                    FraisageHeures = table.Column<double>(type: "float", nullable: false),
                    EtauLimeurHeures = table.Column<double>(type: "float", nullable: false),
                    AffutageHeures = table.Column<double>(type: "float", nullable: false),
                    SoudureHeures = table.Column<double>(type: "float", nullable: false),
                    AutresHeures = table.Column<double>(type: "float", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PointagesMachinesFabrication", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PointagesMachinesFabrication_DemandesFabrication_DemandeFabricationId",
                        column: x => x.DemandeFabricationId,
                        principalTable: "DemandesFabrication",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PointagesMachinesFabrication_Users_IntervenantId",
                        column: x => x.IntervenantId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TachesEntretien",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FicheEntretienPreventifId = table.Column<int>(type: "int", nullable: false),
                    Position = table.Column<int>(type: "int", nullable: false),
                    Organes = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OperationAEffectuer = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EstFait = table.Column<bool>(type: "bit", nullable: false),
                    TempsPrevuHeures = table.Column<double>(type: "float", nullable: false),
                    TempsRealiseHeures = table.Column<double>(type: "float", nullable: false),
                    Observation = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SuiteADonner = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TachesEntretien", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TachesEntretien_FichesEntretienPreventif_FicheEntretienPreventifId",
                        column: x => x.FicheEntretienPreventifId,
                        principalTable: "FichesEntretienPreventif",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DemandesFabrication_DemandeTravailId",
                table: "DemandesFabrication",
                column: "DemandeTravailId");

            migrationBuilder.CreateIndex(
                name: "IX_BonsDeConsignation_AgentConsignationId",
                table: "BonsDeConsignation",
                column: "AgentConsignationId");

            migrationBuilder.CreateIndex(
                name: "IX_BonsDeConsignation_AgentDeconsignationId",
                table: "BonsDeConsignation",
                column: "AgentDeconsignationId");

            migrationBuilder.CreateIndex(
                name: "IX_BonsDeConsignation_DemandeTravailId",
                table: "BonsDeConsignation",
                column: "DemandeTravailId");

            migrationBuilder.CreateIndex(
                name: "IX_BonsDeConsignation_EquipementId",
                table: "BonsDeConsignation",
                column: "EquipementId");

            migrationBuilder.CreateIndex(
                name: "IX_FichesEntretienPreventif_EquipementId",
                table: "FichesEntretienPreventif",
                column: "EquipementId");

            migrationBuilder.CreateIndex(
                name: "IX_FichesEntretienPreventif_IntervenantId",
                table: "FichesEntretienPreventif",
                column: "IntervenantId");

            migrationBuilder.CreateIndex(
                name: "IX_FichesEntretienPreventif_MaintenancePreventiveId",
                table: "FichesEntretienPreventif",
                column: "MaintenancePreventiveId");

            migrationBuilder.CreateIndex(
                name: "IX_IntervenantsFabrication_DemandeFabricationId",
                table: "IntervenantsFabrication",
                column: "DemandeFabricationId");

            migrationBuilder.CreateIndex(
                name: "IX_MatieresFabrication_DemandeFabricationId",
                table: "MatieresFabrication",
                column: "DemandeFabricationId");

            migrationBuilder.CreateIndex(
                name: "IX_PointagesMachinesFabrication_DemandeFabricationId",
                table: "PointagesMachinesFabrication",
                column: "DemandeFabricationId");

            migrationBuilder.CreateIndex(
                name: "IX_PointagesMachinesFabrication_IntervenantId",
                table: "PointagesMachinesFabrication",
                column: "IntervenantId");

            migrationBuilder.CreateIndex(
                name: "IX_TachesEntretien_FicheEntretienPreventifId",
                table: "TachesEntretien",
                column: "FicheEntretienPreventifId");

            migrationBuilder.AddForeignKey(
                name: "FK_DemandesFabrication_DemandesTravail_DemandeTravailId",
                table: "DemandesFabrication",
                column: "DemandeTravailId",
                principalTable: "DemandesTravail",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DemandesFabrication_DemandesTravail_DemandeTravailId",
                table: "DemandesFabrication");

            migrationBuilder.DropTable(
                name: "BonsDeConsignation");

            migrationBuilder.DropTable(
                name: "IntervenantsFabrication");

            migrationBuilder.DropTable(
                name: "MatieresFabrication");

            migrationBuilder.DropTable(
                name: "PointagesMachinesFabrication");

            migrationBuilder.DropTable(
                name: "TachesEntretien");

            migrationBuilder.DropTable(
                name: "FichesEntretienPreventif");

            migrationBuilder.DropIndex(
                name: "IX_DemandesFabrication_DemandeTravailId",
                table: "DemandesFabrication");

            migrationBuilder.DropColumn(
                name: "CodeArticle",
                table: "DemandesFabrication");

            migrationBuilder.DropColumn(
                name: "Consignes",
                table: "DemandesFabrication");

            migrationBuilder.DropColumn(
                name: "DebutTravail",
                table: "DemandesFabrication");

            migrationBuilder.DropColumn(
                name: "DegreUrgenceImmediat",
                table: "DemandesFabrication");

            migrationBuilder.DropColumn(
                name: "DemandeTravailId",
                table: "DemandesFabrication");

            migrationBuilder.DropColumn(
                name: "FinTravail",
                table: "DemandesFabrication");

            migrationBuilder.DropColumn(
                name: "Matiere",
                table: "DemandesFabrication");

            migrationBuilder.DropColumn(
                name: "NDessin",
                table: "DemandesFabrication");

            migrationBuilder.DropColumn(
                name: "ReceptionDate",
                table: "DemandesFabrication");

            migrationBuilder.DropColumn(
                name: "ReferencePiece",
                table: "DemandesFabrication");

            migrationBuilder.DropColumn(
                name: "TempsAlloueHeures",
                table: "DemandesFabrication");

            migrationBuilder.DropColumn(
                name: "TotalCoutMainOeuvre",
                table: "DemandesFabrication");

            migrationBuilder.DropColumn(
                name: "TotalCoutOperation",
                table: "DemandesFabrication");

            migrationBuilder.DropColumn(
                name: "TotalCoutPieces",
                table: "DemandesFabrication");

            migrationBuilder.DropColumn(
                name: "VisaComptabiliteAnalytique",
                table: "DemandesFabrication");

            migrationBuilder.DropColumn(
                name: "VisaExecution",
                table: "DemandesFabrication");

            migrationBuilder.DropColumn(
                name: "VisaPreparation",
                table: "DemandesFabrication");

            migrationBuilder.AddColumn<DateTime>(
                name: "DateSouhaitee",
                table: "DemandesFabrication",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }
    }
}
