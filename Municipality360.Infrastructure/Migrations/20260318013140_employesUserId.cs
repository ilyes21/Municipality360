using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Municipality360.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class employesUserId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "Employes",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Employes");
        }
    }
}
