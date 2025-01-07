using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace PetFamily.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Species",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_species", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "Volunteers",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    full_name_first_name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    full_name_last_name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    email = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    phone_number_number = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    phone_number_region_code = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    expirience_years = table.Column<int>(type: "integer", nullable: false),
                    description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_volunteers", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "Breeds",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    species_id = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_breeds", x => x.id);
                    table.ForeignKey(
                        name: "fk_breeds_speciesies_species_id",
                        column: x => x.species_id,
                        principalTable: "Species",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Pets",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    date_of_birth = table.Column<DateTime>(type: "date", nullable: true),
                    date_time_created = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    is_vaccinated = table.Column<bool>(type: "boolean", nullable: true),
                    is_sterilized = table.Column<bool>(type: "boolean", nullable: true),
                    weight = table.Column<double>(type: "double precision", nullable: true),
                    height = table.Column<double>(type: "double precision", nullable: true),
                    color = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    pet_type_species_id = table.Column<int>(type: "integer", nullable: false),
                    pet_type_breed_id = table.Column<int>(type: "integer", nullable: false),
                    owner_phone_number = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    owner_phone_region_code = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    donate_details_name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    donate_details_description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    health_info = table.Column<string>(type: "text", nullable: true),
                    adress_street = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    adress_city = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    adress_country = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    help_status = table.Column<string>(type: "text", nullable: false),
                    volunteer_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_pets", x => x.id);
                    table.ForeignKey(
                        name: "fk_pets_volunteers_volunteer_id",
                        column: x => x.volunteer_id,
                        principalTable: "Volunteers",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

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
                    name = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    description = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false)
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

            migrationBuilder.CreateIndex(
                name: "ix_breeds_species_id",
                table: "Breeds",
                column: "species_id");

            migrationBuilder.CreateIndex(
                name: "ix_pets_name",
                table: "Pets",
                column: "name");

            migrationBuilder.CreateIndex(
                name: "ix_pets_volunteer_id",
                table: "Pets",
                column: "volunteer_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Breeds");

            migrationBuilder.DropTable(
                name: "Pets");

            migrationBuilder.DropTable(
                name: "social_network");

            migrationBuilder.DropTable(
                name: "volunteers_dd_list");

            migrationBuilder.DropTable(
                name: "Species");

            migrationBuilder.DropTable(
                name: "Volunteers");
        }
    }
}
