using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AppointAid.Data.Migrations
{
    /// <inheritdoc />
    public partial class Mig15 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Reason",
                table: "MedicalTests",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Reason",
                table: "MedicalTests");
        }
    }
}
