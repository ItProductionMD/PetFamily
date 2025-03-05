using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PetFamily.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Init1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "species",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_species", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "volunteers",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    email = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    experience_years = table.Column<int>(type: "integer", nullable: false),
                    description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    requisites = table.Column<string>(type: "text", nullable: false),
                    social_networks = table.Column<string>(type: "text", nullable: false),
                    deleted_date_time = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false),
                    first_name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    last_name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    phone_number = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    phone_region_code = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_volunteers", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "breeds",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    species_id = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_breeds", x => x.id);
                    table.ForeignKey(
                        name: "fk_breeds_animal_types_species_id",
                        column: x => x.species_id,
                        principalTable: "species",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "pets",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    date_of_birth = table.Column<DateTime>(type: "date", nullable: true),
                    date_time_created = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    is_vaccinated = table.Column<bool>(type: "boolean", nullable: false),
                    is_sterilized = table.Column<bool>(type: "boolean", nullable: false),
                    weight = table.Column<double>(type: "double precision", nullable: true),
                    height = table.Column<double>(type: "double precision", nullable: true),
                    color = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    requisites = table.Column<string>(type: "text", nullable: false),
                    health_info = table.Column<string>(type: "text", nullable: true),
                    help_status = table.Column<string>(type: "text", nullable: false),
                    images = table.Column<string>(type: "text", nullable: false),
                    deleted_date_time = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false),
                    volunteer_id = table.Column<Guid>(type: "uuid", nullable: true),
                    address_city = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    address_region = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    address_street = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    owner_phone_number = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    owner_phone_region_code = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    pet_type_breed_id = table.Column<Guid>(type: "uuid", nullable: false),
                    pet_type_species_id = table.Column<Guid>(type: "uuid", nullable: false),
                    serial_number = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_pets", x => x.id);
                    table.ForeignKey(
                        name: "fk_pets_volunteers_volunteer_id",
                        column: x => x.volunteer_id,
                        principalTable: "volunteers",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "test_entity_state",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    volunteer_id = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_test_entity_state", x => x.id);
                    table.ForeignKey(
                        name: "fk_test_entity_state_volunteers_volunteer_id",
                        column: x => x.volunteer_id,
                        principalTable: "volunteers",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_breeds_species_id",
                table: "breeds",
                column: "species_id");

            migrationBuilder.CreateIndex(
                name: "ix_pets_name",
                table: "pets",
                column: "name");

            migrationBuilder.CreateIndex(
                name: "ix_pets_volunteer_id",
                table: "pets",
                column: "volunteer_id");

            migrationBuilder.CreateIndex(
                name: "ix_test_entity_state_volunteer_id",
                table: "test_entity_state",
                column: "volunteer_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "breeds");

            migrationBuilder.DropTable(
                name: "pets");

            migrationBuilder.DropTable(
                name: "test_entity_state");

            migrationBuilder.DropTable(
                name: "species");

            migrationBuilder.DropTable(
                name: "volunteers");
        }
    }
}
