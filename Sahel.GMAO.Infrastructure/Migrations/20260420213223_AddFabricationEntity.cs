using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Sahel.GMAO.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddFabricationEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DemandesFabrication",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NumeroFabrication = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    EquipementId = table.Column<int>(type: "int", nullable: false),
                    DesignationPiece = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Quantite = table.Column<double>(type: "float", nullable: false),
                    DateEmission = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DateSouhaitee = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Statut = table.Column<int>(type: "int", nullable: false),
                    PlanJointUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Observations = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DemandesFabrication", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DemandesFabrication_Equipements_EquipementId",
                        column: x => x.EquipementId,
                        principalTable: "Equipements",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DemandesFabrication_EquipementId",
                table: "DemandesFabrication",
                column: "EquipementId");

            migrationBuilder.CreateIndex(
                name: "IX_DemandesFabrication_NumeroFabrication",
                table: "DemandesFabrication",
                column: "NumeroFabrication",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DemandesFabrication");
        }
    }
}
