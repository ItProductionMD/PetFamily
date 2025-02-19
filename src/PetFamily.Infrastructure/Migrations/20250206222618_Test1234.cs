using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PetFamily.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Test1234 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "donate_details_description",
                table: "Pets");

            migrationBuilder.DropColumn(
                name: "donate_details_name",
                table: "Pets");

            migrationBuilder.AddColumn<string>(
                name: "donate_details",
                table: "Pets",
                type: "jsonb",
                nullable: false,
                defaultValue: "{}");

            migrationBuilder.AddColumn<string>(
                name: "images",
                table: "Pets",
                type: "jsonb",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "donate_details",
                table: "Pets");

            migrationBuilder.DropColumn(
                name: "images",
                table: "Pets");

            migrationBuilder.AddColumn<string>(
                name: "donate_details_description",
                table: "Pets",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "donate_details_name",
                table: "Pets",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);
        }
    }
}
