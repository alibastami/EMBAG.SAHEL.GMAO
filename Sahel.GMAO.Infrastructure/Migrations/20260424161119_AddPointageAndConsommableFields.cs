using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Sahel.GMAO.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddPointageAndConsommableFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PointagesIntervention_InterventionRoles_InterventionRoleId",
                table: "PointagesIntervention");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PointagesIntervention",
                table: "PointagesIntervention");

            migrationBuilder.RenameTable(
                name: "PointagesIntervention",
                newName: "PointageInterventions");

            migrationBuilder.RenameIndex(
                name: "IX_PointagesIntervention_InterventionRoleId",
                table: "PointageInterventions",
                newName: "IX_PointageInterventions_InterventionRoleId");

            migrationBuilder.AddColumn<string>(
                name: "N_BSM",
                table: "ConsommableUsages",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Observation",
                table: "ConsommableUsages",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_PointageInterventions",
                table: "PointageInterventions",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_PointageInterventions_InterventionRoles_InterventionRoleId",
                table: "PointageInterventions",
                column: "InterventionRoleId",
                principalTable: "InterventionRoles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PointageInterventions_InterventionRoles_InterventionRoleId",
                table: "PointageInterventions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PointageInterventions",
                table: "PointageInterventions");

            migrationBuilder.DropColumn(
                name: "N_BSM",
                table: "ConsommableUsages");

            migrationBuilder.DropColumn(
                name: "Observation",
                table: "ConsommableUsages");

            migrationBuilder.RenameTable(
                name: "PointageInterventions",
                newName: "PointagesIntervention");

            migrationBuilder.RenameIndex(
                name: "IX_PointageInterventions_InterventionRoleId",
                table: "PointagesIntervention",
                newName: "IX_PointagesIntervention_InterventionRoleId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PointagesIntervention",
                table: "PointagesIntervention",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_PointagesIntervention_InterventionRoles_InterventionRoleId",
                table: "PointagesIntervention",
                column: "InterventionRoleId",
                principalTable: "InterventionRoles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
