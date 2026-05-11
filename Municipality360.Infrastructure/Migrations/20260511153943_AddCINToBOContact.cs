using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Municipality360.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddCINToBOContact : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CIN",
                table: "BOContacts",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CIN",
                table: "BOContacts");
        }
    }
}
