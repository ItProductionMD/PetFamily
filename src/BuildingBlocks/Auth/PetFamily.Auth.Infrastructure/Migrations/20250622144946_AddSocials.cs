using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PetFamily.Auth.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddSocials : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "is_hashed_password",
                schema: "auth",
                table: "users",
                newName: "hashed_password");

            migrationBuilder.RenameColumn(
                name: "blocked_date_time",
                schema: "auth",
                table: "users",
                newName: "blocked_at");

            migrationBuilder.AlterColumn<string>(
                name: "email",
                schema: "auth",
                table: "users",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddColumn<string>(
                name: "social_networks",
                schema: "auth",
                table: "users",
                type: "jsonb",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "social_networks",
                schema: "auth",
                table: "users");

            migrationBuilder.RenameColumn(
                name: "hashed_password",
                schema: "auth",
                table: "users",
                newName: "is_hashed_password");

            migrationBuilder.RenameColumn(
                name: "blocked_at",
                schema: "auth",
                table: "users",
                newName: "blocked_date_time");

            migrationBuilder.AlterColumn<string>(
                name: "email",
                schema: "auth",
                table: "users",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(50)",
                oldMaxLength: 50);
        }
    }
}
