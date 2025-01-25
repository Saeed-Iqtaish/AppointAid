using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AppointAid.Data.Migrations
{
    /// <inheritdoc />
    public partial class Mig6 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "MedicalCenterId",
                table: "Nurses",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Nurses_MedicalCenterId",
                table: "Nurses",
                column: "MedicalCenterId");

            migrationBuilder.AddForeignKey(
                name: "FK_Nurses_MedicalCenters_MedicalCenterId",
                table: "Nurses",
                column: "MedicalCenterId",
                principalTable: "MedicalCenters",
                principalColumn: "MedicalCenterId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Nurses_MedicalCenters_MedicalCenterId",
                table: "Nurses");

            migrationBuilder.DropIndex(
                name: "IX_Nurses_MedicalCenterId",
                table: "Nurses");

            migrationBuilder.DropColumn(
                name: "MedicalCenterId",
                table: "Nurses");
        }
    }
}
