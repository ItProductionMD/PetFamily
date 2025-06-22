using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Volunteers.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RemoveEmailAndPhone : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_volunteers_email",
                schema: "volunteer",
                table: "volunteers");

            migrationBuilder.DropIndex(
                name: "ix_volunteers_phone_number_phone_region_code",
                schema: "volunteer",
                table: "volunteers");

            migrationBuilder.DropColumn(
                name: "email",
                schema: "volunteer",
                table: "volunteers");

            migrationBuilder.DropColumn(
                name: "phone_number",
                schema: "volunteer",
                table: "volunteers");

            migrationBuilder.DropColumn(
                name: "phone_region_code",
                schema: "volunteer",
                table: "volunteers");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "email",
                schema: "volunteer",
                table: "volunteers",
                type: "character varying(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "phone_number",
                schema: "volunteer",
                table: "volunteers",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "phone_region_code",
                schema: "volunteer",
                table: "volunteers",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "ix_volunteers_email",
                schema: "volunteer",
                table: "volunteers",
                column: "email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_volunteers_phone_number_phone_region_code",
                schema: "volunteer",
                table: "volunteers",
                columns: new[] { "phone_number", "phone_region_code" },
                unique: true);
        }
    }
}
