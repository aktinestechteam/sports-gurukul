using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SportsGurukul.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddAcademyTypeAndSportsSeed : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AcademyType",
                table: "Academies",
                type: "character varying(30)",
                maxLength: 30,
                nullable: false,
                defaultValue: "MultiSport");

            migrationBuilder.UpdateData(
                table: "Academies",
                keyColumn: "Id",
                keyValue: new Guid("a1000000-0000-0000-0000-000000000001"),
                column: "AcademyType",
                value: "MultiSport");

            migrationBuilder.InsertData(
                table: "Sports",
                columns: new[] { "Id", "Code", "CreatedAt", "Description", "IsDeleted", "Name", "OlympicSport", "SportCategoryId", "UpdatedAt" },
                values: new object[,]
                {
                    { new Guid("c1000000-0000-0000-0000-00000000000b"), "BOX", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, false, "Boxing", false, new Guid("b1000000-0000-0000-0000-000000000004"), null },
                    { new Guid("c1000000-0000-0000-0000-00000000000c"), "HCK", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, false, "Hockey", false, new Guid("b1000000-0000-0000-0000-000000000001"), null },
                    { new Guid("c1000000-0000-0000-0000-00000000000d"), "KBD", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, false, "Kabaddi", false, new Guid("b1000000-0000-0000-0000-000000000001"), null }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Sports",
                keyColumn: "Id",
                keyColumnType: "uuid",
                keyValues: new object[]
                {
                    new Guid("c1000000-0000-0000-0000-00000000000b"),
                    new Guid("c1000000-0000-0000-0000-00000000000c"),
                    new Guid("c1000000-0000-0000-0000-00000000000d")
                });

            migrationBuilder.DropColumn(
                name: "AcademyType",
                table: "Academies");
        }
    }
}
