using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace SportsGurukul.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddAthleteDomain : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Achievements",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Title = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Competition = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    Position = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Level = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    Date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CertificateUrl = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Achievements", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Athletes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    AthleteCode = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    RegistrationDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CurrentLevel = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    ExperienceYears = table.Column<int>(type: "integer", nullable: false),
                    Height = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    Weight = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    BloodGroup = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    DominantHand = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    DominantFoot = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    Biography = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    Status = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    RowVersion = table.Column<byte[]>(type: "bytea", rowVersion: true, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Athletes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Athletes_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SportCategories",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SportCategories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AthleteAchievements",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    AthleteId = table.Column<Guid>(type: "uuid", nullable: false),
                    AchievementId = table.Column<Guid>(type: "uuid", nullable: false),
                    AwardedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Notes = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AthleteAchievements", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AthleteAchievements_Achievements_AchievementId",
                        column: x => x.AchievementId,
                        principalTable: "Achievements",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AthleteAchievements_Athletes_AthleteId",
                        column: x => x.AthleteId,
                        principalTable: "Athletes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EmergencyContacts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    AthleteId = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Relationship = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    Phone = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Email = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    RowVersion = table.Column<byte[]>(type: "bytea", rowVersion: true, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmergencyContacts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EmergencyContacts_Athletes_AthleteId",
                        column: x => x.AthleteId,
                        principalTable: "Athletes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MedicalProfiles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    AthleteId = table.Column<Guid>(type: "uuid", nullable: false),
                    MedicalConditions = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    Allergies = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    Medications = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    BloodGroup = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    InsuranceNumber = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    DoctorName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    DoctorContact = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    RowVersion = table.Column<byte[]>(type: "bytea", rowVersion: true, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MedicalProfiles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MedicalProfiles_Athletes_AthleteId",
                        column: x => x.AthleteId,
                        principalTable: "Athletes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Rankings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    AthleteId = table.Column<Guid>(type: "uuid", nullable: false),
                    CurrentRank = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    StateRank = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    NationalRank = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    InternationalRank = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    RankingAuthority = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    RowVersion = table.Column<byte[]>(type: "bytea", rowVersion: true, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Rankings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Rankings_Athletes_AthleteId",
                        column: x => x.AthleteId,
                        principalTable: "Athletes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Sports",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Code = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    OlympicSport = table.Column<bool>(type: "boolean", nullable: false),
                    Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    SportCategoryId = table.Column<Guid>(type: "uuid", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "bytea", rowVersion: true, nullable: false, defaultValueSql: "E'\\\\x00'::bytea"),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sports", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Sports_SportCategories_SportCategoryId",
                        column: x => x.SportCategoryId,
                        principalTable: "SportCategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AthleteSports",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    AthleteId = table.Column<Guid>(type: "uuid", nullable: false),
                    SportId = table.Column<Guid>(type: "uuid", nullable: false),
                    IsPrimarySport = table.Column<bool>(type: "boolean", nullable: false),
                    JoinedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AthleteSports", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AthleteSports_Athletes_AthleteId",
                        column: x => x.AthleteId,
                        principalTable: "Athletes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AthleteSports_Sports_SportId",
                        column: x => x.SportId,
                        principalTable: "Sports",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "SportCategories",
                columns: new[] { "Id", "CreatedAt", "Description", "IsDeleted", "Name", "UpdatedAt" },
                values: new object[,]
                {
                    { new Guid("b1000000-0000-0000-0000-000000000001"), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Sports played between two teams", false, "Team Sports", null },
                    { new Guid("b1000000-0000-0000-0000-000000000002"), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Sports played with racquets", false, "Racquet Sports", null },
                    { new Guid("b1000000-0000-0000-0000-000000000003"), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Individual competitive sports", false, "Individual Sports", null },
                    { new Guid("b1000000-0000-0000-0000-000000000004"), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Martial arts and combat disciplines", false, "Combat Sports", null },
                    { new Guid("b1000000-0000-0000-0000-000000000005"), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Water-based sports", false, "Aquatic Sports", null }
                });

            migrationBuilder.InsertData(
                table: "Sports",
                columns: new[] { "Id", "Code", "CreatedAt", "Description", "IsDeleted", "Name", "OlympicSport", "SportCategoryId", "UpdatedAt" },
                values: new object[,]
                {
                    { new Guid("c1000000-0000-0000-0000-000000000001"), "CRK", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, false, "Cricket", false, new Guid("b1000000-0000-0000-0000-000000000001"), null },
                    { new Guid("c1000000-0000-0000-0000-000000000002"), "FTB", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, false, "Football", true, new Guid("b1000000-0000-0000-0000-000000000001"), null },
                    { new Guid("c1000000-0000-0000-0000-000000000003"), "BDM", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, false, "Badminton", true, new Guid("b1000000-0000-0000-0000-000000000002"), null },
                    { new Guid("c1000000-0000-0000-0000-000000000004"), "TNS", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, false, "Tennis", true, new Guid("b1000000-0000-0000-0000-000000000002"), null },
                    { new Guid("c1000000-0000-0000-0000-000000000005"), "TTP", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, false, "Table Tennis", true, new Guid("b1000000-0000-0000-0000-000000000002"), null },
                    { new Guid("c1000000-0000-0000-0000-000000000006"), "ATH", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, false, "Athletics", true, new Guid("b1000000-0000-0000-0000-000000000003"), null },
                    { new Guid("c1000000-0000-0000-0000-000000000007"), "CHS", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, false, "Chess", false, new Guid("b1000000-0000-0000-0000-000000000003"), null },
                    { new Guid("c1000000-0000-0000-0000-000000000008"), "SWM", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, false, "Swimming", true, new Guid("b1000000-0000-0000-0000-000000000005"), null },
                    { new Guid("c1000000-0000-0000-0000-000000000009"), "BBL", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, false, "Basketball", true, new Guid("b1000000-0000-0000-0000-000000000001"), null },
                    { new Guid("c1000000-0000-0000-0000-00000000000a"), "VLB", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, false, "Volleyball", true, new Guid("b1000000-0000-0000-0000-000000000001"), null }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Achievements_Level",
                table: "Achievements",
                column: "Level");

            migrationBuilder.CreateIndex(
                name: "IX_Achievements_Title",
                table: "Achievements",
                column: "Title");

            migrationBuilder.CreateIndex(
                name: "IX_AthleteAchievements_AchievementId",
                table: "AthleteAchievements",
                column: "AchievementId");

            migrationBuilder.CreateIndex(
                name: "IX_AthleteAchievements_AthleteId",
                table: "AthleteAchievements",
                column: "AthleteId");

            migrationBuilder.CreateIndex(
                name: "IX_AthleteAchievements_AthleteId_AchievementId",
                table: "AthleteAchievements",
                columns: new[] { "AthleteId", "AchievementId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Athletes_AthleteCode",
                table: "Athletes",
                column: "AthleteCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Athletes_Status",
                table: "Athletes",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_Athletes_UserId",
                table: "Athletes",
                column: "UserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AthleteSports_AthleteId",
                table: "AthleteSports",
                column: "AthleteId");

            migrationBuilder.CreateIndex(
                name: "IX_AthleteSports_AthleteId_SportId",
                table: "AthleteSports",
                columns: new[] { "AthleteId", "SportId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AthleteSports_SportId",
                table: "AthleteSports",
                column: "SportId");

            migrationBuilder.CreateIndex(
                name: "IX_EmergencyContacts_AthleteId",
                table: "EmergencyContacts",
                column: "AthleteId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MedicalProfiles_AthleteId",
                table: "MedicalProfiles",
                column: "AthleteId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Rankings_AthleteId",
                table: "Rankings",
                column: "AthleteId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SportCategories_Name",
                table: "SportCategories",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Sports_Code",
                table: "Sports",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Sports_Name",
                table: "Sports",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Sports_SportCategoryId",
                table: "Sports",
                column: "SportCategoryId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AthleteAchievements");

            migrationBuilder.DropTable(
                name: "AthleteSports");

            migrationBuilder.DropTable(
                name: "EmergencyContacts");

            migrationBuilder.DropTable(
                name: "MedicalProfiles");

            migrationBuilder.DropTable(
                name: "Rankings");

            migrationBuilder.DropTable(
                name: "Achievements");

            migrationBuilder.DropTable(
                name: "Sports");

            migrationBuilder.DropTable(
                name: "Athletes");

            migrationBuilder.DropTable(
                name: "SportCategories");
        }
    }
}
