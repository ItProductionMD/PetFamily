using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PetFamily.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Init1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "full_name_last_name",
                table: "volunteers",
                newName: "last_name");

            migrationBuilder.RenameColumn(
                name: "full_name_first_name",
                table: "volunteers",
                newName: "first_name");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "last_name",
                table: "volunteers",
                newName: "full_name_last_name");

            migrationBuilder.RenameColumn(
                name: "first_name",
                table: "volunteers",
                newName: "full_name_first_name");
        }
    }
}
