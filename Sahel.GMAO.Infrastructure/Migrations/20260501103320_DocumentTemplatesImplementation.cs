using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Sahel.GMAO.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class DocumentTemplatesImplementation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Consigne",
                table: "TachesEntretien",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ValeurMesuree",
                table: "TachesEntretien",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Atelier",
                table: "RapportsIncidents",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Conclusion",
                table: "RapportsIncidents",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "DegatsConstates",
                table: "RapportsIncidents",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "NomChefDeptMaintenance",
                table: "RapportsIncidents",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NomChefService",
                table: "RapportsIncidents",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NomPreparateur",
                table: "RapportsIncidents",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "FrequenceType",
                table: "MaintenancePreventives",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CauseNonExecution",
                table: "ConsommableUsages",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "QuantitePrevue",
                table: "ConsommableUsages",
                type: "float",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "QuestionnairesArretTechnique",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NumeroQuestionnaire = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    EquipementId = table.Column<int>(type: "int", nullable: false),
                    Atelier = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Section = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Annee = table.Column<int>(type: "int", nullable: false),
                    ChefAtelier = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NomConducteur = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    VisaChefAtelier = table.Column<DateTime>(type: "datetime2", nullable: true),
                    VisaConducteur = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuestionnairesArretTechnique", x => x.Id);
                    table.ForeignKey(
                        name: "FK_QuestionnairesArretTechnique_Equipements_EquipementId",
                        column: x => x.EquipementId,
                        principalTable: "Equipements",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SuivisTempsMarche",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EquipementId = table.Column<int>(type: "int", nullable: false),
                    Annee = table.Column<int>(type: "int", nullable: false),
                    Mois = table.Column<int>(type: "int", nullable: false),
                    Semaine = table.Column<int>(type: "int", nullable: false),
                    HeuresMarche = table.Column<double>(type: "float", nullable: false),
                    HeuresCumulees = table.Column<double>(type: "float", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SuivisTempsMarche", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SuivisTempsMarche_Equipements_EquipementId",
                        column: x => x.EquipementId,
                        principalTable: "Equipements",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LignesAnomalie",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    QuestionnaireId = table.Column<int>(type: "int", nullable: false),
                    Position = table.Column<int>(type: "int", nullable: false),
                    Organe = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AnomalieConstatee = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Observation = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LignesAnomalie", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LignesAnomalie_QuestionnairesArretTechnique_QuestionnaireId",
                        column: x => x.QuestionnaireId,
                        principalTable: "QuestionnairesArretTechnique",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_LignesAnomalie_QuestionnaireId",
                table: "LignesAnomalie",
                column: "QuestionnaireId");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionnairesArretTechnique_EquipementId",
                table: "QuestionnairesArretTechnique",
                column: "EquipementId");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionnairesArretTechnique_NumeroQuestionnaire",
                table: "QuestionnairesArretTechnique",
                column: "NumeroQuestionnaire",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SuivisTempsMarche_EquipementId_Annee_Mois_Semaine",
                table: "SuivisTempsMarche",
                columns: new[] { "EquipementId", "Annee", "Mois", "Semaine" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LignesAnomalie");

            migrationBuilder.DropTable(
                name: "SuivisTempsMarche");

            migrationBuilder.DropTable(
                name: "QuestionnairesArretTechnique");

            migrationBuilder.DropColumn(
                name: "Consigne",
                table: "TachesEntretien");

            migrationBuilder.DropColumn(
                name: "ValeurMesuree",
                table: "TachesEntretien");

            migrationBuilder.DropColumn(
                name: "Atelier",
                table: "RapportsIncidents");

            migrationBuilder.DropColumn(
                name: "Conclusion",
                table: "RapportsIncidents");

            migrationBuilder.DropColumn(
                name: "DegatsConstates",
                table: "RapportsIncidents");

            migrationBuilder.DropColumn(
                name: "NomChefDeptMaintenance",
                table: "RapportsIncidents");

            migrationBuilder.DropColumn(
                name: "NomChefService",
                table: "RapportsIncidents");

            migrationBuilder.DropColumn(
                name: "NomPreparateur",
                table: "RapportsIncidents");

            migrationBuilder.DropColumn(
                name: "FrequenceType",
                table: "MaintenancePreventives");

            migrationBuilder.DropColumn(
                name: "CauseNonExecution",
                table: "ConsommableUsages");

            migrationBuilder.DropColumn(
                name: "QuantitePrevue",
                table: "ConsommableUsages");
        }
    }
}
