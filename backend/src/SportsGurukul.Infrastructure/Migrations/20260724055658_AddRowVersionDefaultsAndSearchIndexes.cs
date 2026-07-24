using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SportsGurukul.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddRowVersionDefaultsAndSearchIndexes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<byte[]>(
                name: "RowVersion",
                table: "Sports",
                type: "bytea",
                rowVersion: true,
                nullable: false,
                defaultValueSql: "E'\\\\x00'::bytea",
                oldClrType: typeof(byte[]),
                oldType: "bytea",
                oldRowVersion: true);

            migrationBuilder.AlterColumn<byte[]>(
                name: "RowVersion",
                table: "Rankings",
                type: "bytea",
                rowVersion: true,
                nullable: false,
                defaultValueSql: "E'\\\\x00'::bytea",
                oldClrType: typeof(byte[]),
                oldType: "bytea",
                oldRowVersion: true);

            migrationBuilder.AlterColumn<byte[]>(
                name: "RowVersion",
                table: "MedicalProfiles",
                type: "bytea",
                rowVersion: true,
                nullable: false,
                defaultValueSql: "E'\\\\x00'::bytea",
                oldClrType: typeof(byte[]),
                oldType: "bytea",
                oldRowVersion: true);

            migrationBuilder.AlterColumn<byte[]>(
                name: "RowVersion",
                table: "EmergencyContacts",
                type: "bytea",
                rowVersion: true,
                nullable: false,
                defaultValueSql: "E'\\\\x00'::bytea",
                oldClrType: typeof(byte[]),
                oldType: "bytea",
                oldRowVersion: true);

            migrationBuilder.AlterColumn<byte[]>(
                name: "RowVersion",
                table: "Coaches",
                type: "bytea",
                rowVersion: true,
                nullable: false,
                defaultValueSql: "E'\\\\x00'::bytea",
                oldClrType: typeof(byte[]),
                oldType: "bytea",
                oldRowVersion: true);

            migrationBuilder.AlterColumn<byte[]>(
                name: "RowVersion",
                table: "CoachCertifications",
                type: "bytea",
                rowVersion: true,
                nullable: false,
                defaultValueSql: "E'\\\\x00'::bytea",
                oldClrType: typeof(byte[]),
                oldType: "bytea",
                oldRowVersion: true);

            migrationBuilder.AlterColumn<byte[]>(
                name: "RowVersion",
                table: "CoachAvailabilities",
                type: "bytea",
                rowVersion: true,
                nullable: false,
                defaultValueSql: "E'\\\\x00'::bytea",
                oldClrType: typeof(byte[]),
                oldType: "bytea",
                oldRowVersion: true);

            migrationBuilder.AlterColumn<byte[]>(
                name: "RowVersion",
                table: "Athletes",
                type: "bytea",
                rowVersion: true,
                nullable: false,
                defaultValueSql: "E'\\\\x00'::bytea",
                oldClrType: typeof(byte[]),
                oldType: "bytea",
                oldRowVersion: true);

            migrationBuilder.CreateIndex(
                name: "IX_CoachLocations_Country",
                table: "CoachLocations",
                column: "Country");

            migrationBuilder.CreateIndex(
                name: "IX_CoachLocations_LatLon",
                table: "CoachLocations",
                columns: new[] { "Latitude", "Longitude" });

            migrationBuilder.CreateIndex(
                name: "IX_Coaches_YearsOfExperience",
                table: "Coaches",
                column: "YearsOfExperience");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_CoachLocations_Country",
                table: "CoachLocations");

            migrationBuilder.DropIndex(
                name: "IX_CoachLocations_LatLon",
                table: "CoachLocations");

            migrationBuilder.DropIndex(
                name: "IX_Coaches_YearsOfExperience",
                table: "Coaches");

            migrationBuilder.AlterColumn<byte[]>(
                name: "RowVersion",
                table: "Sports",
                type: "bytea",
                rowVersion: true,
                nullable: false,
                oldClrType: typeof(byte[]),
                oldType: "bytea",
                oldRowVersion: true,
                oldDefaultValueSql: "E'\\\\x00'::bytea");

            migrationBuilder.AlterColumn<byte[]>(
                name: "RowVersion",
                table: "Rankings",
                type: "bytea",
                rowVersion: true,
                nullable: false,
                oldClrType: typeof(byte[]),
                oldType: "bytea",
                oldRowVersion: true,
                oldDefaultValueSql: "E'\\\\x00'::bytea");

            migrationBuilder.AlterColumn<byte[]>(
                name: "RowVersion",
                table: "MedicalProfiles",
                type: "bytea",
                rowVersion: true,
                nullable: false,
                oldClrType: typeof(byte[]),
                oldType: "bytea",
                oldRowVersion: true,
                oldDefaultValueSql: "E'\\\\x00'::bytea");

            migrationBuilder.AlterColumn<byte[]>(
                name: "RowVersion",
                table: "EmergencyContacts",
                type: "bytea",
                rowVersion: true,
                nullable: false,
                oldClrType: typeof(byte[]),
                oldType: "bytea",
                oldRowVersion: true,
                oldDefaultValueSql: "E'\\\\x00'::bytea");

            migrationBuilder.AlterColumn<byte[]>(
                name: "RowVersion",
                table: "Coaches",
                type: "bytea",
                rowVersion: true,
                nullable: false,
                oldClrType: typeof(byte[]),
                oldType: "bytea",
                oldRowVersion: true,
                oldDefaultValueSql: "E'\\\\x00'::bytea");

            migrationBuilder.AlterColumn<byte[]>(
                name: "RowVersion",
                table: "CoachCertifications",
                type: "bytea",
                rowVersion: true,
                nullable: false,
                oldClrType: typeof(byte[]),
                oldType: "bytea",
                oldRowVersion: true,
                oldDefaultValueSql: "E'\\\\x00'::bytea");

            migrationBuilder.AlterColumn<byte[]>(
                name: "RowVersion",
                table: "CoachAvailabilities",
                type: "bytea",
                rowVersion: true,
                nullable: false,
                oldClrType: typeof(byte[]),
                oldType: "bytea",
                oldRowVersion: true,
                oldDefaultValueSql: "E'\\\\x00'::bytea");

            migrationBuilder.AlterColumn<byte[]>(
                name: "RowVersion",
                table: "Athletes",
                type: "bytea",
                rowVersion: true,
                nullable: false,
                oldClrType: typeof(byte[]),
                oldType: "bytea",
                oldRowVersion: true,
                oldDefaultValueSql: "E'\\\\x00'::bytea");
        }
    }
}
