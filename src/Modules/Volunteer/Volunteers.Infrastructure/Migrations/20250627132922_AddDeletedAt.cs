using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Volunteers.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddDeletedAt : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "deleted_date_time",
                schema: "volunteer",
                table: "volunteers",
                newName: "deleted_at");

            migrationBuilder.RenameColumn(
                name: "deleted_date_time",
                schema: "volunteer",
                table: "pets",
                newName: "deleted_at");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "deleted_at",
                schema: "volunteer",
                table: "volunteers",
                newName: "deleted_date_time");

            migrationBuilder.RenameColumn(
                name: "deleted_at",
                schema: "volunteer",
                table: "pets",
                newName: "deleted_date_time");
        }
    }
}
