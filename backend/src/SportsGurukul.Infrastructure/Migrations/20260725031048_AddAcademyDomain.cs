using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SportsGurukul.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddAcademyDomain : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Academies",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    AcademyCode = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    LegalName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    Description = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    RegistrationNumber = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    GSTNumber = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    EstablishedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Website = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    Email = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Phone = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Status = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    VerificationStatus = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    LogoUrl = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    BannerUrl = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    RowVersion = table.Column<byte[]>(type: "bytea", rowVersion: true, nullable: false, defaultValueSql: "E'\\\\x00'::bytea"),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Academies", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "Academies",
                columns: new[] { "Id", "AcademyCode", "BannerUrl", "CreatedAt", "Description", "Email", "EstablishedDate", "GSTNumber", "IsDeleted", "LegalName", "LogoUrl", "Name", "Phone", "RegistrationNumber", "Status", "UpdatedAt", "VerificationStatus", "Website" },
                values: new object[] { new Guid("a1000000-0000-0000-0000-000000000001"), "ACAD-SEED-001", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Seed academy for development and testing.", "academy.seed@sportsgurukul.com", null, null, false, "Seed Academy Pvt Ltd", null, "Seed Academy", "+910000000000", null, "Active", null, "Verified", null });

            migrationBuilder.CreateTable(
                name: "AcademyBranches",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    AcademyId = table.Column<Guid>(type: "uuid", nullable: false),
                    BranchName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Address = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    Country = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    State = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    City = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    District = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    PostalCode = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    Latitude = table.Column<decimal>(type: "numeric(10,8)", precision: 10, scale: 8, nullable: true),
                    Longitude = table.Column<decimal>(type: "numeric(11,8)", precision: 11, scale: 8, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AcademyBranches", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AcademyBranches_Academies_AcademyId",
                        column: x => x.AcademyId,
                        principalTable: "Academies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AcademyContacts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    AcademyId = table.Column<Guid>(type: "uuid", nullable: false),
                    PrimaryContactName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    PrimaryPhone = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    PrimaryEmail = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    SecondaryContactName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    SecondaryPhone = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    SecondaryEmail = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    Address = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    Country = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    State = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    City = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    PostalCode = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    Latitude = table.Column<decimal>(type: "numeric(10,8)", precision: 10, scale: 8, nullable: true),
                    Longitude = table.Column<decimal>(type: "numeric(11,8)", precision: 11, scale: 8, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AcademyContacts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AcademyContacts_Academies_AcademyId",
                        column: x => x.AcademyId,
                        principalTable: "Academies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AcademyDocuments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    AcademyId = table.Column<Guid>(type: "uuid", nullable: false),
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
                    table.PrimaryKey("PK_AcademyDocuments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AcademyDocuments_Academies_AcademyId",
                        column: x => x.AcademyId,
                        principalTable: "Academies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AcademyFacilities",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    AcademyId = table.Column<Guid>(type: "uuid", nullable: false),
                    FacilityName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    FacilityType = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    IndoorOutdoor = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    Capacity = table.Column<int>(type: "integer", nullable: true),
                    Available = table.Column<bool>(type: "boolean", nullable: false),
                    Description = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AcademyFacilities", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AcademyFacilities_Academies_AcademyId",
                        column: x => x.AcademyId,
                        principalTable: "Academies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AcademyGalleries",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    AcademyId = table.Column<Guid>(type: "uuid", nullable: false),
                    Title = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    ImageUrl = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    ThumbnailUrl = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    SortOrder = table.Column<int>(type: "integer", nullable: false),
                    IsFeatured = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AcademyGalleries", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AcademyGalleries_Academies_AcademyId",
                        column: x => x.AcademyId,
                        principalTable: "Academies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AcademyMemberships",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    AcademyId = table.Column<Guid>(type: "uuid", nullable: false),
                    MembershipName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    Price = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    Duration = table.Column<int>(type: "integer", nullable: false),
                    Benefits = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    Status = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AcademyMemberships", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AcademyMemberships_Academies_AcademyId",
                        column: x => x.AcademyId,
                        principalTable: "Academies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AcademyOperatingHours",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    AcademyId = table.Column<Guid>(type: "uuid", nullable: false),
                    MondayOpening = table.Column<TimeOnly>(type: "time without time zone", nullable: true),
                    MondayClosing = table.Column<TimeOnly>(type: "time without time zone", nullable: true),
                    TuesdayOpening = table.Column<TimeOnly>(type: "time without time zone", nullable: true),
                    TuesdayClosing = table.Column<TimeOnly>(type: "time without time zone", nullable: true),
                    WednesdayOpening = table.Column<TimeOnly>(type: "time without time zone", nullable: true),
                    WednesdayClosing = table.Column<TimeOnly>(type: "time without time zone", nullable: true),
                    ThursdayOpening = table.Column<TimeOnly>(type: "time without time zone", nullable: true),
                    ThursdayClosing = table.Column<TimeOnly>(type: "time without time zone", nullable: true),
                    FridayOpening = table.Column<TimeOnly>(type: "time without time zone", nullable: true),
                    FridayClosing = table.Column<TimeOnly>(type: "time without time zone", nullable: true),
                    SaturdayOpening = table.Column<TimeOnly>(type: "time without time zone", nullable: true),
                    SaturdayClosing = table.Column<TimeOnly>(type: "time without time zone", nullable: true),
                    SundayOpening = table.Column<TimeOnly>(type: "time without time zone", nullable: true),
                    SundayClosing = table.Column<TimeOnly>(type: "time without time zone", nullable: true),
                    HolidaySchedule = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AcademyOperatingHours", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AcademyOperatingHours_Academies_AcademyId",
                        column: x => x.AcademyId,
                        principalTable: "Academies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AcademySocialLinks",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    AcademyId = table.Column<Guid>(type: "uuid", nullable: false),
                    Platform = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Url = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AcademySocialLinks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AcademySocialLinks_Academies_AcademyId",
                        column: x => x.AcademyId,
                        principalTable: "Academies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AcademySports",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    AcademyId = table.Column<Guid>(type: "uuid", nullable: false),
                    SportId = table.Column<Guid>(type: "uuid", nullable: false),
                    IsPrimarySport = table.Column<bool>(type: "boolean", nullable: false),
                    JoinedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AcademySports", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AcademySports_Academies_AcademyId",
                        column: x => x.AcademyId,
                        principalTable: "Academies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AcademySports_Sports_SportId",
                        column: x => x.SportId,
                        principalTable: "Sports",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AcademyVerifications",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    AcademyId = table.Column<Guid>(type: "uuid", nullable: false),
                    VerificationStatus = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    VerifiedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    VerifiedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Remarks = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    RowVersion = table.Column<byte[]>(type: "bytea", rowVersion: true, nullable: false, defaultValueSql: "E'\\\\x00'::bytea"),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AcademyVerifications", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AcademyVerifications_Academies_AcademyId",
                        column: x => x.AcademyId,
                        principalTable: "Academies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CoachAthlete_CoachId_AthleteId",
                table: "CoachAthlete",
                columns: new[] { "CoachId", "AthleteId" },
                unique: true,
                filter: "\"IsActive\" = true");

            migrationBuilder.CreateIndex(
                name: "IX_Academies_AcademyCode",
                table: "Academies",
                column: "AcademyCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Academies_Email",
                table: "Academies",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Academies_Name",
                table: "Academies",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_Academies_Phone",
                table: "Academies",
                column: "Phone");

            migrationBuilder.CreateIndex(
                name: "IX_Academies_RegistrationNumber",
                table: "Academies",
                column: "RegistrationNumber",
                unique: true,
                filter: "\"RegistrationNumber\" IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Academies_Status",
                table: "Academies",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_Academies_Status_CreatedAt",
                table: "Academies",
                columns: new[] { "Status", "CreatedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_Academies_VerificationStatus",
                table: "Academies",
                column: "VerificationStatus");

            migrationBuilder.CreateIndex(
                name: "IX_AcademyBranches_AcademyId",
                table: "AcademyBranches",
                column: "AcademyId");

            migrationBuilder.CreateIndex(
                name: "IX_AcademyBranches_AcademyId_BranchName",
                table: "AcademyBranches",
                columns: new[] { "AcademyId", "BranchName" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AcademyBranches_Country",
                table: "AcademyBranches",
                column: "Country");

            migrationBuilder.CreateIndex(
                name: "IX_AcademyBranches_State_City",
                table: "AcademyBranches",
                columns: new[] { "State", "City" });

            migrationBuilder.CreateIndex(
                name: "IX_AcademyContacts_AcademyId",
                table: "AcademyContacts",
                column: "AcademyId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AcademyDocuments_AcademyId",
                table: "AcademyDocuments",
                column: "AcademyId");

            migrationBuilder.CreateIndex(
                name: "IX_AcademyDocuments_AcademyId_Category",
                table: "AcademyDocuments",
                columns: new[] { "AcademyId", "Category" });

            migrationBuilder.CreateIndex(
                name: "IX_AcademyDocuments_AcademyId_IsDeleted",
                table: "AcademyDocuments",
                columns: new[] { "AcademyId", "IsDeleted" });

            migrationBuilder.CreateIndex(
                name: "IX_AcademyDocuments_Category",
                table: "AcademyDocuments",
                column: "Category");

            migrationBuilder.CreateIndex(
                name: "IX_AcademyDocuments_Status",
                table: "AcademyDocuments",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_AcademyDocuments_UploadedOn",
                table: "AcademyDocuments",
                column: "UploadedOn");

            migrationBuilder.CreateIndex(
                name: "IX_AcademyFacilities_AcademyId",
                table: "AcademyFacilities",
                column: "AcademyId");

            migrationBuilder.CreateIndex(
                name: "IX_AcademyFacilities_AcademyId_FacilityType",
                table: "AcademyFacilities",
                columns: new[] { "AcademyId", "FacilityType" });

            migrationBuilder.CreateIndex(
                name: "IX_AcademyFacilities_FacilityType",
                table: "AcademyFacilities",
                column: "FacilityType");

            migrationBuilder.CreateIndex(
                name: "IX_AcademyGalleries_AcademyId",
                table: "AcademyGalleries",
                column: "AcademyId");

            migrationBuilder.CreateIndex(
                name: "IX_AcademyGalleries_AcademyId_IsFeatured",
                table: "AcademyGalleries",
                columns: new[] { "AcademyId", "IsFeatured" });

            migrationBuilder.CreateIndex(
                name: "IX_AcademyGalleries_AcademyId_SortOrder",
                table: "AcademyGalleries",
                columns: new[] { "AcademyId", "SortOrder" });

            migrationBuilder.CreateIndex(
                name: "IX_AcademyMemberships_AcademyId",
                table: "AcademyMemberships",
                column: "AcademyId");

            migrationBuilder.CreateIndex(
                name: "IX_AcademyMemberships_AcademyId_Name",
                table: "AcademyMemberships",
                columns: new[] { "AcademyId", "MembershipName" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AcademyMemberships_Status",
                table: "AcademyMemberships",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_AcademyOperatingHours_AcademyId",
                table: "AcademyOperatingHours",
                column: "AcademyId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AcademySocialLinks_AcademyId",
                table: "AcademySocialLinks",
                column: "AcademyId");

            migrationBuilder.CreateIndex(
                name: "IX_AcademySocialLinks_AcademyId_Platform",
                table: "AcademySocialLinks",
                columns: new[] { "AcademyId", "Platform" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AcademySports_AcademyId",
                table: "AcademySports",
                column: "AcademyId");

            migrationBuilder.CreateIndex(
                name: "IX_AcademySports_AcademyId_SportId",
                table: "AcademySports",
                columns: new[] { "AcademyId", "SportId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AcademySports_SportId",
                table: "AcademySports",
                column: "SportId");

            migrationBuilder.CreateIndex(
                name: "IX_AcademyVerifications_AcademyId",
                table: "AcademyVerifications",
                column: "AcademyId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AcademyVerifications_Status",
                table: "AcademyVerifications",
                column: "VerificationStatus");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AcademyBranches");

            migrationBuilder.DropTable(
                name: "AcademyContacts");

            migrationBuilder.DropTable(
                name: "AcademyDocuments");

            migrationBuilder.DropTable(
                name: "AcademyFacilities");

            migrationBuilder.DropTable(
                name: "AcademyGalleries");

            migrationBuilder.DropTable(
                name: "AcademyMemberships");

            migrationBuilder.DropTable(
                name: "AcademyOperatingHours");

            migrationBuilder.DropTable(
                name: "AcademySocialLinks");

            migrationBuilder.DropTable(
                name: "AcademySports");

            migrationBuilder.DropTable(
                name: "AcademyVerifications");

            migrationBuilder.DropTable(
                name: "Academies");

            migrationBuilder.DropIndex(
                name: "IX_CoachAthlete_CoachId_AthleteId",
                table: "CoachAthlete");
        }
    }
}
