using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Volunteers.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddPhone : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "phone",
                schema: "volunteer",
                table: "volunteers",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "ix_volunteers_phone",
                schema: "volunteer",
                table: "volunteers",
                column: "phone",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_volunteers_phone",
                schema: "volunteer",
                table: "volunteers");

            migrationBuilder.DropColumn(
                name: "phone",
                schema: "volunteer",
                table: "volunteers");
        }
    }
}
