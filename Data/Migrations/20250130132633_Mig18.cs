using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AppointAid.Data.Migrations
{
    /// <inheritdoc />
    public partial class Mig18 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MedicalTests_TimeSlots_TimeSlotId",
                table: "MedicalTests");

            migrationBuilder.DropIndex(
                name: "IX_MedicalTests_TimeSlotId",
                table: "MedicalTests");

            migrationBuilder.AlterColumn<string>(
                name: "Results",
                table: "MedicalTests",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateIndex(
                name: "IX_MedicalTests_TimeSlotId",
                table: "MedicalTests",
                column: "TimeSlotId",
                unique: true,
                filter: "[TimeSlotId] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_MedicalTests_TimeSlots_TimeSlotId",
                table: "MedicalTests",
                column: "TimeSlotId",
                principalTable: "TimeSlots",
                principalColumn: "TimeSlotId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MedicalTests_TimeSlots_TimeSlotId",
                table: "MedicalTests");

            migrationBuilder.DropIndex(
                name: "IX_MedicalTests_TimeSlotId",
                table: "MedicalTests");

            migrationBuilder.AlterColumn<string>(
                name: "Results",
                table: "MedicalTests",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_MedicalTests_TimeSlotId",
                table: "MedicalTests",
                column: "TimeSlotId");

            migrationBuilder.AddForeignKey(
                name: "FK_MedicalTests_TimeSlots_TimeSlotId",
                table: "MedicalTests",
                column: "TimeSlotId",
                principalTable: "TimeSlots",
                principalColumn: "TimeSlotId");
        }
    }
}
