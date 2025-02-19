using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PetFamily.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Test123456 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_pets_volunteers_volunteer_id",
                table: "Pets");

            migrationBuilder.DropIndex(
                name: "ix_pets_volunteer_id",
                table: "Pets");

            migrationBuilder.AddColumn<Guid>(
                name: "volunteer_id1",
                table: "Pets",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "ix_pets_volunteer_id",
                table: "Pets",
                column: "volunteer_id1");

            migrationBuilder.AddForeignKey(
                name: "fk_pets_volunteers_volunteer_id",
                table: "Pets",
                column: "volunteer_id1",
                principalTable: "Volunteers",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_pets_volunteers_volunteer_id",
                table: "Pets");

            migrationBuilder.DropIndex(
                name: "ix_pets_volunteer_id",
                table: "Pets");

            migrationBuilder.DropColumn(
                name: "volunteer_id1",
                table: "Pets");

            migrationBuilder.CreateIndex(
                name: "ix_pets_volunteer_id",
                table: "Pets",
                column: "volunteer_id");

            migrationBuilder.AddForeignKey(
                name: "fk_pets_volunteers_volunteer_id",
                table: "Pets",
                column: "volunteer_id",
                principalTable: "Volunteers",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
