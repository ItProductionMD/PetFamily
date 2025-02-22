using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PetFamily.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameTable(
                name: "Volunteers",
                newName: "volunteers");

            migrationBuilder.RenameTable(
                name: "Species",
                newName: "species");

            migrationBuilder.RenameTable(
                name: "Pets",
                newName: "pets");

            migrationBuilder.RenameTable(
                name: "Breeds",
                newName: "breeds");

            migrationBuilder.RenameColumn(
                name: "phone_number_region_code",
                table: "volunteers",
                newName: "phone_region_code");

            migrationBuilder.RenameColumn(
                name: "phone_number_number",
                table: "volunteers",
                newName: "phone_number");

            migrationBuilder.RenameColumn(
                name: "adress_street",
                table: "pets",
                newName: "address_street");

            migrationBuilder.RenameColumn(
                name: "adress_region",
                table: "pets",
                newName: "address_region");

            migrationBuilder.RenameColumn(
                name: "adress_city",
                table: "pets",
                newName: "address_city");

            migrationBuilder.RenameColumn(
                name: "donate_details_box",
                table: "pets",
                newName: "requisites");

            migrationBuilder.AlterColumn<string>(
                name: "owner_phone_region_code",
                table: "pets",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "owner_phone_number",
                table: "pets",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "address_street",
                table: "pets",
                type: "character varying(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "character varying(500)",
                oldMaxLength: 500,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "address_region",
                table: "pets",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "address_city",
                table: "pets",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100,
                oldNullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameTable(
                name: "volunteers",
                newName: "Volunteers");

            migrationBuilder.RenameTable(
                name: "species",
                newName: "Species");

            migrationBuilder.RenameTable(
                name: "pets",
                newName: "Pets");

            migrationBuilder.RenameTable(
                name: "breeds",
                newName: "Breeds");

            migrationBuilder.RenameColumn(
                name: "phone_region_code",
                table: "Volunteers",
                newName: "phone_number_region_code");

            migrationBuilder.RenameColumn(
                name: "phone_number",
                table: "Volunteers",
                newName: "phone_number_number");

            migrationBuilder.RenameColumn(
                name: "address_street",
                table: "Pets",
                newName: "adress_street");

            migrationBuilder.RenameColumn(
                name: "address_region",
                table: "Pets",
                newName: "adress_region");

            migrationBuilder.RenameColumn(
                name: "address_city",
                table: "Pets",
                newName: "adress_city");

            migrationBuilder.RenameColumn(
                name: "requisites",
                table: "Pets",
                newName: "donate_details_box");

            migrationBuilder.AlterColumn<string>(
                name: "owner_phone_region_code",
                table: "Pets",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "owner_phone_number",
                table: "Pets",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "adress_street",
                table: "Pets",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(500)",
                oldMaxLength: 500);

            migrationBuilder.AlterColumn<string>(
                name: "adress_region",
                table: "Pets",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "adress_city",
                table: "Pets",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100);
        }
    }
}
