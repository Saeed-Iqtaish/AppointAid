using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AppointAid.Data.Migrations
{
    /// <inheritdoc />
    public partial class Mig8 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "EmergencyType",
                table: "EmergencyResponses",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "IsCritical",
                table: "EmergencyResponses",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NurseInstructions",
                table: "EmergencyResponses",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ResponseTime",
                table: "EmergencyResponses",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EmergencyType",
                table: "EmergencyResponses");

            migrationBuilder.DropColumn(
                name: "IsCritical",
                table: "EmergencyResponses");

            migrationBuilder.DropColumn(
                name: "NurseInstructions",
                table: "EmergencyResponses");

            migrationBuilder.DropColumn(
                name: "ResponseTime",
                table: "EmergencyResponses");
        }
    }
}
