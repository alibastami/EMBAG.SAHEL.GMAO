using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Sahel.GMAO.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddInterventionJournal : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "InterventionLogs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DemandeTravailId = table.Column<int>(type: "int", nullable: false),
                    IntervenantId = table.Column<int>(type: "int", nullable: false),
                    DateIntervention = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TravailExecute = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DureeHeures = table.Column<double>(type: "float", nullable: false),
                    ArretProductionHeures = table.Column<double>(type: "float", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InterventionLogs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InterventionLogs_DemandesTravail_DemandeTravailId",
                        column: x => x.DemandeTravailId,
                        principalTable: "DemandesTravail",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_InterventionLogs_Users_IntervenantId",
                        column: x => x.IntervenantId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_InterventionLogs_DemandeTravailId",
                table: "InterventionLogs",
                column: "DemandeTravailId");

            migrationBuilder.CreateIndex(
                name: "IX_InterventionLogs_IntervenantId",
                table: "InterventionLogs",
                column: "IntervenantId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "InterventionLogs");
        }
    }
}
