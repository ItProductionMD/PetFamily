using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PetFamily.Auth.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddIsDeleted : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "is_deleted",
                schema: "auth",
                table: "users",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "deleted_at",
                schema: "auth",
                table: "user_role",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "is_deleted",
                schema: "auth",
                table: "user_role",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "is_deleted",
                schema: "auth",
                table: "users");

            migrationBuilder.DropColumn(
                name: "deleted_at",
                schema: "auth",
                table: "user_role");

            migrationBuilder.DropColumn(
                name: "is_deleted",
                schema: "auth",
                table: "user_role");
        }
    }
}
