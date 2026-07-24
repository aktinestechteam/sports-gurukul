using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SportsGurukul.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddCoachDocumentVersionAndAudit : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<Guid>(
                name: "UploadedBy",
                table: "CoachDocuments",
                type: "uuid",
                maxLength: 450,
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "CoachDocuments",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(30)",
                oldMaxLength: 30);

            migrationBuilder.AlterColumn<string>(
                name: "Extension",
                table: "CoachDocuments",
                type: "character varying(10)",
                maxLength: 10,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(20)",
                oldMaxLength: 20);

            migrationBuilder.AlterColumn<string>(
                name: "Checksum",
                table: "CoachDocuments",
                type: "character varying(255)",
                maxLength: 255,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(64)",
                oldMaxLength: 64,
                oldNullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "CoachId1",
                table: "CoachDocuments",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Remarks",
                table: "CoachDocuments",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.CreateTable(
                name: "CoachAthlete",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CoachId = table.Column<Guid>(type: "uuid", nullable: false),
                    AthleteId = table.Column<Guid>(type: "uuid", nullable: false),
                    AssignedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CoachAthlete", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CoachAthlete_Athletes_AthleteId",
                        column: x => x.AthleteId,
                        principalTable: "Athletes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CoachAthlete_Coaches_CoachId",
                        column: x => x.CoachId,
                        principalTable: "Coaches",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CoachDocumentAudits",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    DocumentId = table.Column<Guid>(type: "uuid", nullable: false),
                    Action = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    PerformedBy = table.Column<Guid>(type: "uuid", maxLength: 450, nullable: false),
                    PerformedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IpAddress = table.Column<string>(type: "character varying(45)", maxLength: 45, nullable: true),
                    Details = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CoachDocumentAudits", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CoachDocumentAudits_CoachDocuments_DocumentId",
                        column: x => x.DocumentId,
                        principalTable: "CoachDocuments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CoachDocumentVersions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    DocumentId = table.Column<Guid>(type: "uuid", nullable: false),
                    VersionNumber = table.Column<int>(type: "integer", nullable: false),
                    StoredFileName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    StoragePath = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    FileSize = table.Column<long>(type: "bigint", nullable: false),
                    Checksum = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    UploadedBy = table.Column<Guid>(type: "uuid", maxLength: 450, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CoachDocumentVersions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CoachDocumentVersions_CoachDocuments_DocumentId",
                        column: x => x.DocumentId,
                        principalTable: "CoachDocuments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CoachDocuments_CoachId1",
                table: "CoachDocuments",
                column: "CoachId1");

            migrationBuilder.CreateIndex(
                name: "IX_CoachAthlete_AthleteId",
                table: "CoachAthlete",
                column: "AthleteId");

            migrationBuilder.CreateIndex(
                name: "IX_CoachAthlete_CoachId",
                table: "CoachAthlete",
                column: "CoachId");

            migrationBuilder.CreateIndex(
                name: "IX_CoachDocumentAudits_DocumentId",
                table: "CoachDocumentAudits",
                column: "DocumentId");

            migrationBuilder.CreateIndex(
                name: "IX_CoachDocumentAudits_PerformedOn",
                table: "CoachDocumentAudits",
                column: "PerformedOn");

            migrationBuilder.CreateIndex(
                name: "IX_CoachDocumentVersions_DocumentId",
                table: "CoachDocumentVersions",
                column: "DocumentId");

            migrationBuilder.AddForeignKey(
                name: "FK_CoachDocuments_Coaches_CoachId1",
                table: "CoachDocuments",
                column: "CoachId1",
                principalTable: "Coaches",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CoachDocuments_Coaches_CoachId1",
                table: "CoachDocuments");

            migrationBuilder.DropTable(
                name: "CoachAthlete");

            migrationBuilder.DropTable(
                name: "CoachDocumentAudits");

            migrationBuilder.DropTable(
                name: "CoachDocumentVersions");

            migrationBuilder.DropIndex(
                name: "IX_CoachDocuments_CoachId1",
                table: "CoachDocuments");

            migrationBuilder.DropColumn(
                name: "CoachId1",
                table: "CoachDocuments");

            migrationBuilder.DropColumn(
                name: "Remarks",
                table: "CoachDocuments");

            migrationBuilder.AlterColumn<Guid>(
                name: "UploadedBy",
                table: "CoachDocuments",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldMaxLength: 450);

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "CoachDocuments",
                type: "character varying(30)",
                maxLength: 30,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<string>(
                name: "Extension",
                table: "CoachDocuments",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(10)",
                oldMaxLength: 10);

            migrationBuilder.AlterColumn<string>(
                name: "Checksum",
                table: "CoachDocuments",
                type: "character varying(64)",
                maxLength: 64,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(255)",
                oldMaxLength: 255,
                oldNullable: true);
        }
    }
}
