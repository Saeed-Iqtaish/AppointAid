using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AppointAid.Data.Migrations
{
    /// <inheritdoc />
    public partial class Mig19 : Migration
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

            migrationBuilder.DropColumn(
                name: "TimeSlotId",
                table: "MedicalTests");

            migrationBuilder.RenameColumn(
                name: "Time",
                table: "MedicalTests",
                newName: "ScheduledDate");

            migrationBuilder.AddColumn<TimeSpan>(
                name: "ScheduledTime",
                table: "MedicalTests",
                type: "time",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ScheduledTime",
                table: "MedicalTests");

            migrationBuilder.RenameColumn(
                name: "ScheduledDate",
                table: "MedicalTests",
                newName: "Time");

            migrationBuilder.AddColumn<int>(
                name: "TimeSlotId",
                table: "MedicalTests",
                type: "int",
                nullable: true);

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
    }
}
