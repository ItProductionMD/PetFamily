using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace PetFamily.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialV1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "social_network");

            migrationBuilder.DropTable(
                name: "volunteers_dd_list");

            migrationBuilder.AddColumn<string>(
                name: "donate_details_list",
                table: "Volunteers",
                type: "jsonb",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "social_networks_list",
                table: "Volunteers",
                type: "jsonb",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "donate_details_list",
                table: "Volunteers");

            migrationBuilder.DropColumn(
                name: "social_networks_list",
                table: "Volunteers");

            migrationBuilder.CreateTable(
                name: "social_network",
                columns: table => new
                {
                    social_networks_list_volunteer_id = table.Column<Guid>(type: "uuid", nullable: false),
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    url = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_social_network", x => new { x.social_networks_list_volunteer_id, x.id });
                    table.ForeignKey(
                        name: "fk_social_network_volunteers_social_networks_list_volunteer_id",
                        column: x => x.social_networks_list_volunteer_id,
                        principalTable: "Volunteers",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "volunteers_dd_list",
                columns: table => new
                {
                    donate_details_list_volunteer_id = table.Column<Guid>(type: "uuid", nullable: false),
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    description = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    name = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_volunteers_dd_list", x => new { x.donate_details_list_volunteer_id, x.id });
                    table.ForeignKey(
                        name: "fk_volunteers_dd_list_volunteers_donate_details_list_volunteer",
                        column: x => x.donate_details_list_volunteer_id,
                        principalTable: "Volunteers",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });
        }
    }
}
