using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PetFamily.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class _124561 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "donate_details_list",
                table: "Volunteers");

            migrationBuilder.RenameColumn(
                name: "social_networks_list",
                table: "Volunteers",
                newName: "social_networks");

            migrationBuilder.RenameColumn(
                name: "donate_details",
                table: "Volunteers",
                newName: "requisites");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "social_networks",
                table: "Volunteers",
                newName: "social_networks_list");

            migrationBuilder.RenameColumn(
                name: "requisites",
                table: "Volunteers",
                newName: "donate_details");

            migrationBuilder.AddColumn<string>(
                name: "donate_details_list",
                table: "Volunteers",
                type: "jsonb",
                nullable: false,
                defaultValue: "{}");
        }
    }
}
