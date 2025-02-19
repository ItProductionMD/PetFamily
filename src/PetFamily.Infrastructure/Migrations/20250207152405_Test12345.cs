using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PetFamily.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Test12345 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "images",
                table: "Pets",
                newName: "images_box");

            migrationBuilder.RenameColumn(
                name: "donate_details",
                table: "Pets",
                newName: "donate_details_box");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "images_box",
                table: "Pets",
                newName: "images");

            migrationBuilder.RenameColumn(
                name: "donate_details_box",
                table: "Pets",
                newName: "donate_details");
        }
    }
}
