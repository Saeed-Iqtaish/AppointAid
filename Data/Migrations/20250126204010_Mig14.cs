using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AppointAid.Data.Migrations
{
    /// <inheritdoc />
    public partial class Mig14 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "EmergencyDescription",
                table: "EmergencyResponses",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EmergencyDescription",
                table: "EmergencyResponses");
        }
    }
}
