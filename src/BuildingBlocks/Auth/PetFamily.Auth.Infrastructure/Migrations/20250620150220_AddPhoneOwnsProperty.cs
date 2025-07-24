using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PetFamily.Auth.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddPhoneOwnsProperty : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "ix_users_email",
                schema: "auth",
                table: "users",
                column: "email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_users_login",
                schema: "auth",
                table: "users",
                column: "login",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_users_phone_number_phone_region_code",
                schema: "auth",
                table: "users",
                columns: new[] { "phone_number", "phone_region_code" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_users_email",
                schema: "auth",
                table: "users");

            migrationBuilder.DropIndex(
                name: "ix_users_login",
                schema: "auth",
                table: "users");

            migrationBuilder.DropIndex(
                name: "ix_users_phone_number_phone_region_code",
                schema: "auth",
                table: "users");
        }
    }
}
