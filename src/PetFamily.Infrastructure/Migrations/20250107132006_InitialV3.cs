using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PetFamily.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialV3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_breeds_speciesies_species_id",
                table: "Breeds");

            migrationBuilder.AddForeignKey(
                name: "fk_breeds_species_species_id",
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
                name: "fk_breeds_species_species_id",
                table: "Breeds");

            migrationBuilder.AddForeignKey(
                name: "fk_breeds_speciesies_species_id",
                table: "Breeds",
                column: "species_id",
                principalTable: "Species",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
