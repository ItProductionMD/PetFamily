using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PetFamily.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Test113256 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_breeds_species_species_id",
                table: "Breeds");

            migrationBuilder.AlterColumn<string>(
                name: "donate_details_box",
                table: "Pets",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "jsonb");

            migrationBuilder.AddForeignKey(
                name: "fk_breeds_animal_types_species_id",
                table: "Breeds",
                column: "species_id",
                principalTable: "Species",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_breeds_animal_types_species_id",
                table: "Breeds");

            migrationBuilder.AlterColumn<string>(
                name: "donate_details_box",
                table: "Pets",
                type: "jsonb",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddForeignKey(
                name: "fk_breeds_species_species_id",
                table: "Breeds",
                column: "species_id",
                principalTable: "Species",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
