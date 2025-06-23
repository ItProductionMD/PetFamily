using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Volunteers.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "social_networks",
                schema: "volunteer",
                table: "volunteers");

            migrationBuilder.AddColumn<Guid>(
                name: "user_id",
                schema: "volunteer",
                table: "volunteers",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "user_id",
                schema: "volunteer",
                table: "volunteers");

            migrationBuilder.AddColumn<string>(
                name: "social_networks",
                schema: "volunteer",
                table: "volunteers",
                type: "jsonb",
                nullable: false,
                defaultValue: "");
        }
    }
}
