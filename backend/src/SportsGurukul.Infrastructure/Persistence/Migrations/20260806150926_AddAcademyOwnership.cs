using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SportsGurukul.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddAcademyOwnership : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "OwnedByUserId",
                table: "Academies",
                type: "uuid",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Academies",
                keyColumn: "Id",
                keyValue: new Guid("a1000000-0000-0000-0000-000000000001"),
                column: "OwnedByUserId",
                value: null);

            migrationBuilder.CreateIndex(
                name: "IX_Academies_OwnedByUserId",
                table: "Academies",
                column: "OwnedByUserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Academies_OwnedByUserId",
                table: "Academies");

            migrationBuilder.DropColumn(
                name: "OwnedByUserId",
                table: "Academies");
        }
    }
}
