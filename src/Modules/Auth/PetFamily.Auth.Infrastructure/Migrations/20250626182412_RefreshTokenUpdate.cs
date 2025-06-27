using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PetFamily.Auth.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RefreshTokenUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RefreshTokens_Users",
                schema: "auth",
                table: "refresh_tokens");

            migrationBuilder.DropPrimaryKey(
                name: "PK_RefreshTokens",
                schema: "auth",
                table: "refresh_tokens");

            migrationBuilder.DropIndex(
                name: "ix_refresh_tokens_user_id",
                schema: "auth",
                table: "refresh_tokens");

            migrationBuilder.AddColumn<string>(
                name: "finger_print",
                schema: "auth",
                table: "refresh_tokens",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "pk_refresh_tokens",
                schema: "auth",
                table: "refresh_tokens",
                column: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "pk_refresh_tokens",
                schema: "auth",
                table: "refresh_tokens");

            migrationBuilder.DropColumn(
                name: "finger_print",
                schema: "auth",
                table: "refresh_tokens");

            migrationBuilder.AddPrimaryKey(
                name: "PK_RefreshTokens",
                schema: "auth",
                table: "refresh_tokens",
                column: "id");

            migrationBuilder.CreateIndex(
                name: "ix_refresh_tokens_user_id",
                schema: "auth",
                table: "refresh_tokens",
                column: "user_id");

            migrationBuilder.AddForeignKey(
                name: "FK_RefreshTokens_Users",
                schema: "auth",
                table: "refresh_tokens",
                column: "user_id",
                principalSchema: "auth",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
