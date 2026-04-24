using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Sahel.GMAO.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddWorkingProfiles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "WorkingProfileId",
                table: "Users",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "WorkingProfiles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    HourlyRate = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Specialite = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkingProfiles", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Users_WorkingProfileId",
                table: "Users",
                column: "WorkingProfileId");

            migrationBuilder.AddForeignKey(
                name: "FK_Users_WorkingProfiles_WorkingProfileId",
                table: "Users",
                column: "WorkingProfileId",
                principalTable: "WorkingProfiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Users_WorkingProfiles_WorkingProfileId",
                table: "Users");

            migrationBuilder.DropTable(
                name: "WorkingProfiles");

            migrationBuilder.DropIndex(
                name: "IX_Users_WorkingProfileId",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "WorkingProfileId",
                table: "Users");
        }
    }
}
