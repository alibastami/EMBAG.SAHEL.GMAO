using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Sahel.GMAO.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddLaborUnit : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Unit",
                table: "WorkingProfiles",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Unit",
                table: "InterventionRoles",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Unit",
                table: "WorkingProfiles");

            migrationBuilder.DropColumn(
                name: "Unit",
                table: "InterventionRoles");
        }
    }
}
