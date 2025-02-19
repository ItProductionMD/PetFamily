using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PetFamily.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Test1132564 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "images_box",
                table: "Pets");

            migrationBuilder.AddColumn<string>(
                name: "images",
                table: "Pets",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "images",
                table: "Pets");

            migrationBuilder.AddColumn<string>(
                name: "images_box",
                table: "Pets",
                type: "jsonb",
                nullable: false,
                defaultValue: "{}");
        }
    }
}
