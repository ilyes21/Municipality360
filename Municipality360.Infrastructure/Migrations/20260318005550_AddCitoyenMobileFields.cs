using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Municipality360.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddCitoyenMobileFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FcmToken",
                table: "Citoyens",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PasswordHash",
                table: "Citoyens",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FcmToken",
                table: "Citoyens");

            migrationBuilder.DropColumn(
                name: "PasswordHash",
                table: "Citoyens");
        }
    }
}
