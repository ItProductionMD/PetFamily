using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Volunteers.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialVolunteer : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "volunteer");

            migrationBuilder.CreateTable(
                name: "volunteers",
                schema: "volunteer",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    email = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    phone_number = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    phone_region_code = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    experience_years = table.Column<int>(type: "integer", nullable: false),
                    description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    rating = table.Column<int>(type: "integer", nullable: false),
                    requisites = table.Column<string>(type: "jsonb", nullable: false),
                    social_networks = table.Column<string>(type: "jsonb", nullable: false),
                    deleted_date_time = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false),
                    first_name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    last_name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_volunteers", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "pets",
                schema: "volunteer",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    date_of_birth = table.Column<DateTime>(type: "date", nullable: true),
                    date_time_created = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    is_vaccinated = table.Column<bool>(type: "boolean", nullable: false),
                    is_sterilized = table.Column<bool>(type: "boolean", nullable: false),
                    weight = table.Column<double>(type: "double precision", nullable: true),
                    height = table.Column<double>(type: "double precision", nullable: true),
                    color = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    requisites = table.Column<string>(type: "jsonb", nullable: false),
                    health_info = table.Column<string>(type: "text", nullable: true),
                    help_status = table.Column<string>(type: "text", nullable: false),
                    images = table.Column<string>(type: "jsonb", nullable: false),
                    deleted_date_time = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false),
                    volunteer_id = table.Column<Guid>(type: "uuid", nullable: true),
                    address_city = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    address_number = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    address_region = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    address_street = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    owner_phone_number = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    owner_phone_region_code = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
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
                        principalSchema: "volunteer",
                        principalTable: "volunteers",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_pets_name",
                schema: "volunteer",
                table: "pets",
                column: "name");

            migrationBuilder.CreateIndex(
                name: "ix_pets_volunteer_id",
                schema: "volunteer",
                table: "pets",
                column: "volunteer_id");

            migrationBuilder.CreateIndex(
                name: "ix_volunteers_email",
                schema: "volunteer",
                table: "volunteers",
                column: "email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_volunteers_phone_number_phone_region_code",
                schema: "volunteer",
                table: "volunteers",
                columns: new[] { "phone_number", "phone_region_code" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "pets",
                schema: "volunteer");

            migrationBuilder.DropTable(
                name: "volunteers",
                schema: "volunteer");
        }
    }
}
