using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace SportsGurukul.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddCoachDomain : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AthleteDocuments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    AthleteId = table.Column<Guid>(type: "uuid", nullable: false),
                    Category = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Title = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    OriginalFileName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    StoredFileName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    StorageProvider = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    StoragePath = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    MimeType = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Extension = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    FileSize = table.Column<long>(type: "bigint", nullable: false),
                    Checksum = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    Version = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    UploadedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    UploadedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    VerifiedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    VerifiedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ExpiryDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsPublic = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AthleteDocuments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AthleteDocuments_Athletes_AthleteId",
                        column: x => x.AthleteId,
                        principalTable: "Athletes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Coaches",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    CoachCode = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    RegistrationDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Biography = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    YearsOfExperience = table.Column<int>(type: "integer", nullable: false),
                    CurrentOrganization = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    HighestQualification = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    PreferredLanguage = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    CoachingLevel = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    Status = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    VerificationStatus = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    RowVersion = table.Column<byte[]>(type: "bytea", rowVersion: true, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Coaches", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Coaches_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RecentSearches",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    QueryText = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    FiltersJson = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: false),
                    ResultCount = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    SearchedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RecentSearches", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RecentSearches_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SavedSearches",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    FiltersJson = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: false),
                    UsageCount = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SavedSearches", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SavedSearches_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DocumentAudits",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    DocumentId = table.Column<Guid>(type: "uuid", nullable: false),
                    Action = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    PerformedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    PerformedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IpAddress = table.Column<string>(type: "character varying(45)", maxLength: 45, nullable: true),
                    Details = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DocumentAudits", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DocumentAudits_AthleteDocuments_DocumentId",
                        column: x => x.DocumentId,
                        principalTable: "AthleteDocuments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DocumentVersions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    DocumentId = table.Column<Guid>(type: "uuid", nullable: false),
                    VersionNumber = table.Column<int>(type: "integer", nullable: false),
                    StoredFileName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    StoragePath = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    FileSize = table.Column<long>(type: "bigint", nullable: false),
                    Checksum = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    UploadedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DocumentVersions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DocumentVersions_AthleteDocuments_DocumentId",
                        column: x => x.DocumentId,
                        principalTable: "AthleteDocuments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CoachAvailabilities",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CoachId = table.Column<Guid>(type: "uuid", nullable: false),
                    WeeklySchedule = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false),
                    TimeSlots = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false),
                    OnlineAvailable = table.Column<bool>(type: "boolean", nullable: false),
                    OfflineAvailable = table.Column<bool>(type: "boolean", nullable: false),
                    TravelDistance = table.Column<int>(type: "integer", nullable: true),
                    RowVersion = table.Column<byte[]>(type: "bytea", rowVersion: true, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CoachAvailabilities", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CoachAvailabilities_Coaches_CoachId",
                        column: x => x.CoachId,
                        principalTable: "Coaches",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CoachCertifications",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CoachId = table.Column<Guid>(type: "uuid", nullable: false),
                    CertificationName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    IssuingAuthority = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    CertificateNumber = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    IssueDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ExpiryDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    VerificationStatus = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    CertificateUrl = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    RowVersion = table.Column<byte[]>(type: "bytea", rowVersion: true, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CoachCertifications", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CoachCertifications_Coaches_CoachId",
                        column: x => x.CoachId,
                        principalTable: "Coaches",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CoachDocuments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CoachId = table.Column<Guid>(type: "uuid", nullable: false),
                    Category = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Title = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    OriginalFileName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    StoredFileName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    StorageProvider = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    StoragePath = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    MimeType = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Extension = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    FileSize = table.Column<long>(type: "bigint", nullable: false),
                    Checksum = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    Version = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    UploadedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    UploadedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    VerifiedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    VerifiedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ExpiryDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsPublic = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CoachDocuments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CoachDocuments_Coaches_CoachId",
                        column: x => x.CoachId,
                        principalTable: "Coaches",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CoachEducation",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CoachId = table.Column<Guid>(type: "uuid", nullable: false),
                    Degree = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Institution = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    FieldOfStudy = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    YearCompleted = table.Column<int>(type: "integer", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CoachEducation", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CoachEducation_Coaches_CoachId",
                        column: x => x.CoachId,
                        principalTable: "Coaches",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CoachExperiences",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CoachId = table.Column<Guid>(type: "uuid", nullable: false),
                    Organization = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Role = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    Sport = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    StartDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    EndDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Description = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CoachExperiences", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CoachExperiences_Coaches_CoachId",
                        column: x => x.CoachId,
                        principalTable: "Coaches",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CoachLocations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CoachId = table.Column<Guid>(type: "uuid", nullable: false),
                    Country = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    State = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    City = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    District = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Latitude = table.Column<decimal>(type: "numeric(10,8)", precision: 10, scale: 8, nullable: true),
                    Longitude = table.Column<decimal>(type: "numeric(11,8)", precision: 11, scale: 8, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CoachLocations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CoachLocations_Coaches_CoachId",
                        column: x => x.CoachId,
                        principalTable: "Coaches",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CoachSpecializations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CoachId = table.Column<Guid>(type: "uuid", nullable: false),
                    SpecializationName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CoachSpecializations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CoachSpecializations_Coaches_CoachId",
                        column: x => x.CoachId,
                        principalTable: "Coaches",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CoachSports",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CoachId = table.Column<Guid>(type: "uuid", nullable: false),
                    SportId = table.Column<Guid>(type: "uuid", nullable: false),
                    IsPrimarySport = table.Column<bool>(type: "boolean", nullable: false),
                    JoinedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CoachSports", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CoachSports_Coaches_CoachId",
                        column: x => x.CoachId,
                        principalTable: "Coaches",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CoachSports_Sports_SportId",
                        column: x => x.SportId,
                        principalTable: "Sports",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "Coaches",
                columns: new[] { "Id", "Biography", "CoachCode", "CoachingLevel", "CreatedAt", "CurrentOrganization", "HighestQualification", "IsDeleted", "PreferredLanguage", "RegistrationDate", "RowVersion", "Status", "UpdatedAt", "UserId", "VerificationStatus", "YearsOfExperience" },
                values: new object[] { new Guid("d1000000-0000-0000-0000-000000000001"), "Seed coach profile for development.", "COACH-20250101-SEED01", "Senior", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, false, null, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new byte[0], "Active", null, new Guid("f47ac10b-58cc-4372-a567-0e02b2c3d479"), "Verified", 5 });

            migrationBuilder.InsertData(
                table: "CoachAvailabilities",
                columns: new[] { "Id", "CoachId", "CreatedAt", "IsDeleted", "OfflineAvailable", "OnlineAvailable", "RowVersion", "TimeSlots", "TravelDistance", "UpdatedAt", "WeeklySchedule" },
                values: new object[] { new Guid("b2000000-0000-0000-0000-000000000001"), new Guid("d1000000-0000-0000-0000-000000000001"), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), false, true, true, new byte[0], "[\"06:00-08:00\",\"08:00-10:00\",\"10:00-12:00\",\"12:00-14:00\",\"14:00-16:00\",\"16:00-18:00\"]", 25, null, "{\"monday\":{\"start\":\"06:00\",\"end\":\"18:00\"},\"tuesday\":{\"start\":\"06:00\",\"end\":\"18:00\"},\"wednesday\":{\"start\":\"06:00\",\"end\":\"18:00\"},\"thursday\":{\"start\":\"06:00\",\"end\":\"18:00\"},\"friday\":{\"start\":\"06:00\",\"end\":\"18:00\"},\"saturday\":{\"start\":\"08:00\",\"end\":\"14:00\"}}" });

            migrationBuilder.InsertData(
                table: "CoachCertifications",
                columns: new[] { "Id", "CertificateNumber", "CertificateUrl", "CertificationName", "CoachId", "CreatedAt", "ExpiryDate", "IsDeleted", "IssueDate", "IssuingAuthority", "RowVersion", "UpdatedAt", "VerificationStatus" },
                values: new object[] { new Guid("e1000000-0000-0000-0000-000000000001"), "BCCI-LA-2024-001", null, "BCCI Level A Coaching", new Guid("d1000000-0000-0000-0000-000000000001"), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2027, 6, 15, 0, 0, 0, 0, DateTimeKind.Utc), false, new DateTime(2024, 6, 15, 0, 0, 0, 0, DateTimeKind.Utc), "Board of Control for Cricket in India", new byte[0], null, "Verified" });

            migrationBuilder.InsertData(
                table: "CoachEducation",
                columns: new[] { "Id", "CoachId", "CreatedAt", "Degree", "FieldOfStudy", "Institution", "IsDeleted", "UpdatedAt", "YearCompleted" },
                values: new object[] { new Guid("a2000000-0000-0000-0000-000000000001"), new Guid("d1000000-0000-0000-0000-000000000001"), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Bachelor of Physical Education", "Sports Coaching", "National Institute of Sports", false, null, 2018 });

            migrationBuilder.InsertData(
                table: "CoachExperiences",
                columns: new[] { "Id", "CoachId", "CreatedAt", "Description", "EndDate", "IsDeleted", "Organization", "Role", "Sport", "StartDate", "UpdatedAt" },
                values: new object[] { new Guid("f1000000-0000-0000-0000-000000000001"), new Guid("d1000000-0000-0000-0000-000000000001"), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Led state-level cricket training program.", new DateTime(2024, 3, 31, 0, 0, 0, 0, DateTimeKind.Utc), false, "State Cricket Academy", "Head Coach", "Cricket", new DateTime(2020, 4, 1, 0, 0, 0, 0, DateTimeKind.Utc), null });

            migrationBuilder.InsertData(
                table: "CoachLocations",
                columns: new[] { "Id", "City", "CoachId", "Country", "CreatedAt", "District", "IsDeleted", "Latitude", "Longitude", "State", "UpdatedAt" },
                values: new object[] { new Guid("c2000000-0000-0000-0000-000000000001"), "Mumbai", new Guid("d1000000-0000-0000-0000-000000000001"), "India", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Mumbai City", false, 19.0760m, 72.8777m, "Maharashtra", null });

            migrationBuilder.InsertData(
                table: "CoachSpecializations",
                columns: new[] { "Id", "CoachId", "CreatedAt", "Description", "IsDeleted", "SpecializationName", "UpdatedAt" },
                values: new object[,]
                {
                    { new Guid("d2000000-0000-0000-0000-000000000001"), new Guid("d1000000-0000-0000-0000-000000000001"), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Specialized in pace and swing bowling techniques.", false, "Fast Bowling", null },
                    { new Guid("d2000000-0000-0000-0000-000000000002"), new Guid("d1000000-0000-0000-0000-000000000001"), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Specialized in fielding drills and athleticism.", false, "Fielding", null }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Athletes_CurrentLevel",
                table: "Athletes",
                column: "CurrentLevel");

            migrationBuilder.CreateIndex(
                name: "IX_Athletes_Status_CreatedAt",
                table: "Athletes",
                columns: new[] { "Status", "CreatedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_Athletes_Status_Level",
                table: "Athletes",
                columns: new[] { "Status", "CurrentLevel" });

            migrationBuilder.CreateIndex(
                name: "IX_AthleteDocuments_AthleteId",
                table: "AthleteDocuments",
                column: "AthleteId");

            migrationBuilder.CreateIndex(
                name: "IX_AthleteDocuments_AthleteId_Category",
                table: "AthleteDocuments",
                columns: new[] { "AthleteId", "Category" });

            migrationBuilder.CreateIndex(
                name: "IX_AthleteDocuments_AthleteId_IsDeleted",
                table: "AthleteDocuments",
                columns: new[] { "AthleteId", "IsDeleted" });

            migrationBuilder.CreateIndex(
                name: "IX_AthleteDocuments_Category",
                table: "AthleteDocuments",
                column: "Category");

            migrationBuilder.CreateIndex(
                name: "IX_AthleteDocuments_Status",
                table: "AthleteDocuments",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_AthleteDocuments_UploadedOn",
                table: "AthleteDocuments",
                column: "UploadedOn");

            migrationBuilder.CreateIndex(
                name: "IX_CoachAvailabilities_CoachId",
                table: "CoachAvailabilities",
                column: "CoachId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CoachCertifications_CoachId",
                table: "CoachCertifications",
                column: "CoachId");

            migrationBuilder.CreateIndex(
                name: "IX_CoachCertifications_CoachId_Name",
                table: "CoachCertifications",
                columns: new[] { "CoachId", "CertificationName" });

            migrationBuilder.CreateIndex(
                name: "IX_CoachCertifications_VerificationStatus",
                table: "CoachCertifications",
                column: "VerificationStatus");

            migrationBuilder.CreateIndex(
                name: "IX_CoachDocuments_Category",
                table: "CoachDocuments",
                column: "Category");

            migrationBuilder.CreateIndex(
                name: "IX_CoachDocuments_CoachId",
                table: "CoachDocuments",
                column: "CoachId");

            migrationBuilder.CreateIndex(
                name: "IX_CoachDocuments_CoachId_Category",
                table: "CoachDocuments",
                columns: new[] { "CoachId", "Category" });

            migrationBuilder.CreateIndex(
                name: "IX_CoachDocuments_CoachId_IsDeleted",
                table: "CoachDocuments",
                columns: new[] { "CoachId", "IsDeleted" });

            migrationBuilder.CreateIndex(
                name: "IX_CoachDocuments_Status",
                table: "CoachDocuments",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_CoachDocuments_UploadedOn",
                table: "CoachDocuments",
                column: "UploadedOn");

            migrationBuilder.CreateIndex(
                name: "IX_CoachEducation_CoachId",
                table: "CoachEducation",
                column: "CoachId");

            migrationBuilder.CreateIndex(
                name: "IX_Coaches_CoachCode",
                table: "Coaches",
                column: "CoachCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Coaches_CoachingLevel",
                table: "Coaches",
                column: "CoachingLevel");

            migrationBuilder.CreateIndex(
                name: "IX_Coaches_Status",
                table: "Coaches",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_Coaches_Status_CoachingLevel",
                table: "Coaches",
                columns: new[] { "Status", "CoachingLevel" });

            migrationBuilder.CreateIndex(
                name: "IX_Coaches_Status_CreatedAt",
                table: "Coaches",
                columns: new[] { "Status", "CreatedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_Coaches_UserId",
                table: "Coaches",
                column: "UserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Coaches_VerificationStatus",
                table: "Coaches",
                column: "VerificationStatus");

            migrationBuilder.CreateIndex(
                name: "IX_CoachExperiences_CoachId",
                table: "CoachExperiences",
                column: "CoachId");

            migrationBuilder.CreateIndex(
                name: "IX_CoachLocations_CoachId",
                table: "CoachLocations",
                column: "CoachId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CoachLocations_State_City",
                table: "CoachLocations",
                columns: new[] { "State", "City" });

            migrationBuilder.CreateIndex(
                name: "IX_CoachSpecializations_CoachId",
                table: "CoachSpecializations",
                column: "CoachId");

            migrationBuilder.CreateIndex(
                name: "IX_CoachSpecializations_CoachId_Name",
                table: "CoachSpecializations",
                columns: new[] { "CoachId", "SpecializationName" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CoachSports_CoachId",
                table: "CoachSports",
                column: "CoachId");

            migrationBuilder.CreateIndex(
                name: "IX_CoachSports_CoachId_SportId",
                table: "CoachSports",
                columns: new[] { "CoachId", "SportId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CoachSports_SportId",
                table: "CoachSports",
                column: "SportId");

            migrationBuilder.CreateIndex(
                name: "IX_DocumentAudits_DocumentId",
                table: "DocumentAudits",
                column: "DocumentId");

            migrationBuilder.CreateIndex(
                name: "IX_DocumentAudits_PerformedOn",
                table: "DocumentAudits",
                column: "PerformedOn");

            migrationBuilder.CreateIndex(
                name: "IX_DocumentVersions_DocumentId",
                table: "DocumentVersions",
                column: "DocumentId");

            migrationBuilder.CreateIndex(
                name: "IX_DocumentVersions_DocumentId_VersionNumber",
                table: "DocumentVersions",
                columns: new[] { "DocumentId", "VersionNumber" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RecentSearches_UserId",
                table: "RecentSearches",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_RecentSearches_UserId_SearchedAt",
                table: "RecentSearches",
                columns: new[] { "UserId", "SearchedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_SavedSearches_Name",
                table: "SavedSearches",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_SavedSearches_UserId",
                table: "SavedSearches",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CoachAvailabilities");

            migrationBuilder.DropTable(
                name: "CoachCertifications");

            migrationBuilder.DropTable(
                name: "CoachDocuments");

            migrationBuilder.DropTable(
                name: "CoachEducation");

            migrationBuilder.DropTable(
                name: "CoachExperiences");

            migrationBuilder.DropTable(
                name: "CoachLocations");

            migrationBuilder.DropTable(
                name: "CoachSpecializations");

            migrationBuilder.DropTable(
                name: "CoachSports");

            migrationBuilder.DropTable(
                name: "DocumentAudits");

            migrationBuilder.DropTable(
                name: "DocumentVersions");

            migrationBuilder.DropTable(
                name: "RecentSearches");

            migrationBuilder.DropTable(
                name: "SavedSearches");

            migrationBuilder.DropTable(
                name: "Coaches");

            migrationBuilder.DropTable(
                name: "AthleteDocuments");

            migrationBuilder.DropIndex(
                name: "IX_Athletes_CurrentLevel",
                table: "Athletes");

            migrationBuilder.DropIndex(
                name: "IX_Athletes_Status_CreatedAt",
                table: "Athletes");

            migrationBuilder.DropIndex(
                name: "IX_Athletes_Status_Level",
                table: "Athletes");
        }
    }
}
