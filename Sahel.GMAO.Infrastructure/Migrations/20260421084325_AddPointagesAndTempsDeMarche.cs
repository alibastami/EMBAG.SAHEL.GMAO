using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Sahel.GMAO.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddPointagesAndTempsDeMarche : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "TempsDeMarcheHeures",
                table: "DemandesTravail",
                type: "float",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "PointagesIntervention",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    InterventionRoleId = table.Column<int>(type: "int", nullable: false),
                    DateIntervention = table.Column<DateTime>(type: "datetime2", nullable: false),
                    JourDuMois = table.Column<int>(type: "int", nullable: false),
                    HeuresTravaillees = table.Column<double>(type: "float", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PointagesIntervention", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PointagesIntervention_InterventionRoles_InterventionRoleId",
                        column: x => x.InterventionRoleId,
                        principalTable: "InterventionRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PointagesIntervention_InterventionRoleId",
                table: "PointagesIntervention",
                column: "InterventionRoleId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PointagesIntervention");

            migrationBuilder.DropColumn(
                name: "TempsDeMarcheHeures",
                table: "DemandesTravail");
        }
    }
}
