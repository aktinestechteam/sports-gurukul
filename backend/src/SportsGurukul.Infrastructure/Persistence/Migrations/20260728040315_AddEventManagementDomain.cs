using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace SportsGurukul.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddEventManagementDomain : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "EventCategories",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Code = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    CategoryType = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    DisplayOrder = table.Column<int>(type: "integer", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "bytea", rowVersion: true, nullable: false, defaultValueSql: "E'\\\\x00'::bytea"),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventCategories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EventTypes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Code = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    DisplayOrder = table.Column<int>(type: "integer", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "bytea", rowVersion: true, nullable: false, defaultValueSql: "E'\\\\x00'::bytea"),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Tournaments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TournamentCode = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    TournamentName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    AcademyId = table.Column<Guid>(type: "uuid", nullable: false),
                    SportId = table.Column<Guid>(type: "uuid", nullable: false),
                    TournamentType = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    Status = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    StartDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    EndDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    RegistrationOpenDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    RegistrationCloseDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    MaxParticipants = table.Column<int>(type: "integer", nullable: true),
                    MinParticipants = table.Column<int>(type: "integer", nullable: true),
                    RegistrationFee = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: true),
                    RegistrationType = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    Venue = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    Rules = table.Column<string>(type: "character varying(5000)", maxLength: 5000, nullable: true),
                    ContactEmail = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    ContactPhone = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    Website = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    IsPublished = table.Column<bool>(type: "boolean", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "bytea", rowVersion: true, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tournaments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Tournaments_Academies_AcademyId",
                        column: x => x.AcademyId,
                        principalTable: "Academies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Tournaments_Sports_SportId",
                        column: x => x.SportId,
                        principalTable: "Sports",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Events",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    EventCode = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    EventName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "character varying(5000)", maxLength: 5000, nullable: true),
                    ShortDescription = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    AcademyId = table.Column<Guid>(type: "uuid", nullable: false),
                    SportId = table.Column<Guid>(type: "uuid", nullable: false),
                    EventTypeId = table.Column<Guid>(type: "uuid", nullable: false),
                    EventCategoryId = table.Column<Guid>(type: "uuid", nullable: true),
                    Status = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    RegistrationType = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    StartDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    EndDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    RegistrationOpenDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    RegistrationCloseDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    MaxParticipants = table.Column<int>(type: "integer", nullable: true),
                    MinParticipants = table.Column<int>(type: "integer", nullable: true),
                    RegistrationFee = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: true),
                    IsFeatured = table.Column<bool>(type: "boolean", nullable: false),
                    IsPublic = table.Column<bool>(type: "boolean", nullable: false),
                    BannerUrl = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    ContactEmail = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    ContactPhone = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    Website = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    Tags = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    Requirements = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    WhatToBring = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    CancellationPolicy = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    RowVersion = table.Column<byte[]>(type: "bytea", rowVersion: true, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Events", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Events_Academies_AcademyId",
                        column: x => x.AcademyId,
                        principalTable: "Academies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Events_EventCategories_EventCategoryId",
                        column: x => x.EventCategoryId,
                        principalTable: "EventCategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Events_EventTypes_EventTypeId",
                        column: x => x.EventTypeId,
                        principalTable: "EventTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Events_Sports_SportId",
                        column: x => x.SportId,
                        principalTable: "Sports",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TournamentCategories",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TournamentId = table.Column<Guid>(type: "uuid", nullable: false),
                    CategoryName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    CategoryType = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    Description = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    MinAge = table.Column<int>(type: "integer", nullable: false),
                    MaxAge = table.Column<int>(type: "integer", nullable: false),
                    Gender = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    MinSkillLevel = table.Column<int>(type: "integer", nullable: true),
                    MaxSkillLevel = table.Column<int>(type: "integer", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "bytea", rowVersion: true, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TournamentCategories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TournamentCategories_Tournaments_TournamentId",
                        column: x => x.TournamentId,
                        principalTable: "Tournaments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TournamentDocuments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TournamentId = table.Column<Guid>(type: "uuid", nullable: false),
                    DocumentType = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    DocumentName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    FileUrl = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    FileName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    FileSize = table.Column<long>(type: "bigint", nullable: true),
                    ContentType = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    IsPublished = table.Column<bool>(type: "boolean", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "bytea", rowVersion: true, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TournamentDocuments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TournamentDocuments_Tournaments_TournamentId",
                        column: x => x.TournamentId,
                        principalTable: "Tournaments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TournamentGallery",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TournamentId = table.Column<Guid>(type: "uuid", nullable: false),
                    MediaType = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    MediaUrl = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    Caption = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    Description = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    DisplayOrder = table.Column<int>(type: "integer", nullable: false),
                    IsFeatured = table.Column<bool>(type: "boolean", nullable: false),
                    ThumbnailUrl = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    RowVersion = table.Column<byte[]>(type: "bytea", rowVersion: true, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TournamentGallery", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TournamentGallery_Tournaments_TournamentId",
                        column: x => x.TournamentId,
                        principalTable: "Tournaments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TournamentOfficials",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TournamentId = table.Column<Guid>(type: "uuid", nullable: false),
                    CoachId = table.Column<Guid>(type: "uuid", nullable: true),
                    OfficialName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Role = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    Email = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    Phone = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "bytea", rowVersion: true, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TournamentOfficials", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TournamentOfficials_Coaches_CoachId",
                        column: x => x.CoachId,
                        principalTable: "Coaches",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_TournamentOfficials_Tournaments_TournamentId",
                        column: x => x.TournamentId,
                        principalTable: "Tournaments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TournamentRules",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TournamentId = table.Column<Guid>(type: "uuid", nullable: false),
                    RuleName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    RuleDescription = table.Column<string>(type: "character varying(5000)", maxLength: 5000, nullable: false),
                    RuleOrder = table.Column<int>(type: "integer", nullable: false),
                    Category = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "bytea", rowVersion: true, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TournamentRules", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TournamentRules_Tournaments_TournamentId",
                        column: x => x.TournamentId,
                        principalTable: "Tournaments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TournamentSponsors",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TournamentId = table.Column<Guid>(type: "uuid", nullable: false),
                    SponsorName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    SponsorType = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    Amount = table.Column<decimal>(type: "numeric(12,2)", precision: 12, scale: 2, nullable: true),
                    ContactPerson = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    ContactEmail = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    ContactPhone = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    LogoUrl = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    Website = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "bytea", rowVersion: true, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TournamentSponsors", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TournamentSponsors_Tournaments_TournamentId",
                        column: x => x.TournamentId,
                        principalTable: "Tournaments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TournamentSports",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TournamentId = table.Column<Guid>(type: "uuid", nullable: false),
                    SportId = table.Column<Guid>(type: "uuid", nullable: false),
                    SportName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    IsPrimary = table.Column<bool>(type: "boolean", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "bytea", rowVersion: true, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TournamentSports", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TournamentSports_Sports_SportId",
                        column: x => x.SportId,
                        principalTable: "Sports",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TournamentSports_Tournaments_TournamentId",
                        column: x => x.TournamentId,
                        principalTable: "Tournaments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TournamentStages",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TournamentId = table.Column<Guid>(type: "uuid", nullable: false),
                    StageName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    StageType = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    StageOrder = table.Column<int>(type: "integer", nullable: false),
                    StartDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    EndDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsCompleted = table.Column<bool>(type: "boolean", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "bytea", rowVersion: true, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TournamentStages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TournamentStages_Tournaments_TournamentId",
                        column: x => x.TournamentId,
                        principalTable: "Tournaments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TournamentTeams",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TournamentId = table.Column<Guid>(type: "uuid", nullable: false),
                    TeamName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    TeamCode = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    AcademyId = table.Column<Guid>(type: "uuid", nullable: true),
                    SeedNumber = table.Column<int>(type: "integer", nullable: true),
                    Ranking = table.Column<int>(type: "integer", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "bytea", rowVersion: true, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TournamentTeams", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TournamentTeams_Academies_AcademyId",
                        column: x => x.AcademyId,
                        principalTable: "Academies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_TournamentTeams_Tournaments_TournamentId",
                        column: x => x.TournamentId,
                        principalTable: "Tournaments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TournamentVenues",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TournamentId = table.Column<Guid>(type: "uuid", nullable: false),
                    FacilityId = table.Column<Guid>(type: "uuid", nullable: false),
                    VenueName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    Address = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    IsPrimary = table.Column<bool>(type: "boolean", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "bytea", rowVersion: true, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TournamentVenues", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TournamentVenues_Facilities_FacilityId",
                        column: x => x.FacilityId,
                        principalTable: "Facilities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TournamentVenues_Tournaments_TournamentId",
                        column: x => x.TournamentId,
                        principalTable: "Tournaments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EventAnnouncements",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    EventId = table.Column<Guid>(type: "uuid", nullable: false),
                    Title = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Message = table.Column<string>(type: "character varying(5000)", maxLength: 5000, nullable: false),
                    IsPublished = table.Column<bool>(type: "boolean", nullable: false),
                    PublishedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    PublishedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    SendNotification = table.Column<bool>(type: "boolean", nullable: false),
                    Priority = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    RowVersion = table.Column<byte[]>(type: "bytea", rowVersion: true, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventAnnouncements", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EventAnnouncements_Events_EventId",
                        column: x => x.EventId,
                        principalTable: "Events",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EventCoaches",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    EventId = table.Column<Guid>(type: "uuid", nullable: false),
                    CoachId = table.Column<Guid>(type: "uuid", nullable: false),
                    Role = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Responsibility = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    IsLeadCoach = table.Column<bool>(type: "boolean", nullable: false),
                    Notes = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    RowVersion = table.Column<byte[]>(type: "bytea", rowVersion: true, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventCoaches", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EventCoaches_Coaches_CoachId",
                        column: x => x.CoachId,
                        principalTable: "Coaches",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EventCoaches_Events_EventId",
                        column: x => x.EventId,
                        principalTable: "Events",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EventDocuments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    EventId = table.Column<Guid>(type: "uuid", nullable: false),
                    DocumentType = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    Title = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    Url = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    FileName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    FileSize = table.Column<long>(type: "bigint", nullable: true),
                    ContentType = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    IsPublic = table.Column<bool>(type: "boolean", nullable: false),
                    Version = table.Column<int>(type: "integer", nullable: false),
                    UploadedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    RowVersion = table.Column<byte[]>(type: "bytea", rowVersion: true, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventDocuments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EventDocuments_Events_EventId",
                        column: x => x.EventId,
                        principalTable: "Events",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EventMedia",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    EventId = table.Column<Guid>(type: "uuid", nullable: false),
                    MediaType = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Title = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    Url = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    ThumbnailUrl = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    FileName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    FileSize = table.Column<long>(type: "bigint", nullable: true),
                    ContentType = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    IsFeatured = table.Column<bool>(type: "boolean", nullable: false),
                    DisplayOrder = table.Column<int>(type: "integer", nullable: false),
                    UploadedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    RowVersion = table.Column<byte[]>(type: "bytea", rowVersion: true, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventMedia", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EventMedia_Events_EventId",
                        column: x => x.EventId,
                        principalTable: "Events",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EventRegistrations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    EventId = table.Column<Guid>(type: "uuid", nullable: false),
                    AthleteId = table.Column<Guid>(type: "uuid", nullable: true),
                    UserId = table.Column<Guid>(type: "uuid", nullable: true),
                    RegistrationNumber = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Status = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    AmountPaid = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: true),
                    PaymentReference = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    Notes = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    RegistrationDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ApprovalDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    RejectionReason = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    WaitlistPosition = table.Column<int>(type: "integer", nullable: true),
                    RowVersion = table.Column<byte[]>(type: "bytea", rowVersion: true, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventRegistrations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EventRegistrations_Athletes_AthleteId",
                        column: x => x.AthleteId,
                        principalTable: "Athletes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_EventRegistrations_Events_EventId",
                        column: x => x.EventId,
                        principalTable: "Events",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EventRegistrations_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "EventSchedules",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    EventId = table.Column<Guid>(type: "uuid", nullable: false),
                    ScheduleDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    StartTime = table.Column<TimeSpan>(type: "interval", nullable: false),
                    EndTime = table.Column<TimeSpan>(type: "interval", nullable: false),
                    Title = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    Description = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    IsAllDay = table.Column<bool>(type: "boolean", nullable: false),
                    RecurrenceRule = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    RowVersion = table.Column<byte[]>(type: "bytea", rowVersion: true, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventSchedules", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EventSchedules_Events_EventId",
                        column: x => x.EventId,
                        principalTable: "Events",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EventSpeakers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    EventId = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: true),
                    CoachId = table.Column<Guid>(type: "uuid", nullable: true),
                    SpeakerName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Email = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    Phone = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    Title = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    Bio = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    ProfileImageUrl = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    Organization = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    Role = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    Topic = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    DisplayOrder = table.Column<int>(type: "integer", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "bytea", rowVersion: true, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventSpeakers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EventSpeakers_Coaches_CoachId",
                        column: x => x.CoachId,
                        principalTable: "Coaches",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_EventSpeakers_Events_EventId",
                        column: x => x.EventId,
                        principalTable: "Events",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EventSpeakers_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "EventSponsors",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    EventId = table.Column<Guid>(type: "uuid", nullable: false),
                    SponsorName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    ContactPerson = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    ContactEmail = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    ContactPhone = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    Website = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    LogoUrl = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    ContributionAmount = table.Column<decimal>(type: "numeric(12,2)", precision: 12, scale: 2, nullable: true),
                    ContributionDescription = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    Tier = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    DisplayOrder = table.Column<int>(type: "integer", nullable: false),
                    Notes = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    RowVersion = table.Column<byte[]>(type: "bytea", rowVersion: true, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventSponsors", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EventSponsors_Events_EventId",
                        column: x => x.EventId,
                        principalTable: "Events",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EventTickets",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    EventId = table.Column<Guid>(type: "uuid", nullable: false),
                    TicketCode = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    TicketType = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    Price = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: false),
                    QuantityAvailable = table.Column<int>(type: "integer", nullable: false),
                    QuantitySold = table.Column<int>(type: "integer", nullable: false),
                    MaxPerPerson = table.Column<int>(type: "integer", nullable: true),
                    SaleStartDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    SaleEndDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "bytea", rowVersion: true, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventTickets", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EventTickets_Events_EventId",
                        column: x => x.EventId,
                        principalTable: "Events",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EventVenues",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    EventId = table.Column<Guid>(type: "uuid", nullable: false),
                    FacilityId = table.Column<Guid>(type: "uuid", nullable: true),
                    VenueName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Address = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    City = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    State = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Country = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    PostalCode = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    Latitude = table.Column<decimal>(type: "numeric(10,7)", precision: 10, scale: 7, nullable: true),
                    Longitude = table.Column<decimal>(type: "numeric(10,7)", precision: 10, scale: 7, nullable: true),
                    Capacity = table.Column<int>(type: "integer", nullable: true),
                    MapUrl = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    Instructions = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    IsPrimary = table.Column<bool>(type: "boolean", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "bytea", rowVersion: true, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventVenues", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EventVenues_Events_EventId",
                        column: x => x.EventId,
                        principalTable: "Events",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EventVenues_Facilities_FacilityId",
                        column: x => x.FacilityId,
                        principalTable: "Facilities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "EventVolunteers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    EventId = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: true),
                    VolunteerName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Email = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    Phone = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    Role = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    Assignment = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    CheckInTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CheckOutTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Notes = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    RowVersion = table.Column<byte[]>(type: "bytea", rowVersion: true, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventVolunteers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EventVolunteers_Events_EventId",
                        column: x => x.EventId,
                        principalTable: "Events",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EventVolunteers_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "TournamentDivisions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TournamentId = table.Column<Guid>(type: "uuid", nullable: false),
                    CategoryId = table.Column<Guid>(type: "uuid", nullable: true),
                    DivisionName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    MaxTeams = table.Column<int>(type: "integer", nullable: true),
                    MinTeams = table.Column<int>(type: "integer", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "bytea", rowVersion: true, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TournamentDivisions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TournamentDivisions_TournamentCategories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "TournamentCategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_TournamentDivisions_Tournaments_TournamentId",
                        column: x => x.TournamentId,
                        principalTable: "Tournaments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TournamentFixtures",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TournamentId = table.Column<Guid>(type: "uuid", nullable: false),
                    TournamentStageId = table.Column<Guid>(type: "uuid", nullable: true),
                    FixtureNumber = table.Column<int>(type: "integer", nullable: false),
                    ScheduledDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ScheduledTime = table.Column<TimeSpan>(type: "interval(0)", precision: 0, nullable: true),
                    VenueId = table.Column<Guid>(type: "uuid", nullable: true),
                    CourtId = table.Column<Guid>(type: "uuid", nullable: true),
                    HomeTeamName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    AwayTeamName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    IsPublished = table.Column<bool>(type: "boolean", nullable: false),
                    Notes = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    RowVersion = table.Column<byte[]>(type: "bytea", rowVersion: true, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TournamentFixtures", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TournamentFixtures_Facilities_VenueId",
                        column: x => x.VenueId,
                        principalTable: "Facilities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_TournamentFixtures_FacilityCourts_CourtId",
                        column: x => x.CourtId,
                        principalTable: "FacilityCourts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_TournamentFixtures_TournamentStages_TournamentStageId",
                        column: x => x.TournamentStageId,
                        principalTable: "TournamentStages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_TournamentFixtures_Tournaments_TournamentId",
                        column: x => x.TournamentId,
                        principalTable: "Tournaments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TournamentRounds",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TournamentStageId = table.Column<Guid>(type: "uuid", nullable: false),
                    RoundNumber = table.Column<int>(type: "integer", nullable: false),
                    RoundName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    ScheduledDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsCompleted = table.Column<bool>(type: "boolean", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "bytea", rowVersion: true, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TournamentRounds", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TournamentRounds_TournamentStages_TournamentStageId",
                        column: x => x.TournamentStageId,
                        principalTable: "TournamentStages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TournamentParticipants",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TournamentId = table.Column<Guid>(type: "uuid", nullable: false),
                    CategoryId = table.Column<Guid>(type: "uuid", nullable: true),
                    ParticipantType = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    AthleteId = table.Column<Guid>(type: "uuid", nullable: true),
                    TeamId = table.Column<Guid>(type: "uuid", nullable: true),
                    AcademyId = table.Column<Guid>(type: "uuid", nullable: true),
                    ParticipantName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    SeedNumber = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                    Ranking = table.Column<int>(type: "integer", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "bytea", rowVersion: true, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TournamentParticipants", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TournamentParticipants_Academies_AcademyId",
                        column: x => x.AcademyId,
                        principalTable: "Academies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_TournamentParticipants_Athletes_AthleteId",
                        column: x => x.AthleteId,
                        principalTable: "Athletes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_TournamentParticipants_TournamentCategories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "TournamentCategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_TournamentParticipants_TournamentTeams_TeamId",
                        column: x => x.TeamId,
                        principalTable: "TournamentTeams",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_TournamentParticipants_Tournaments_TournamentId",
                        column: x => x.TournamentId,
                        principalTable: "Tournaments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TournamentCourts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TournamentVenueId = table.Column<Guid>(type: "uuid", nullable: false),
                    CourtName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    CourtType = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    SurfaceRating = table.Column<int>(type: "integer", nullable: true),
                    IsAvailable = table.Column<bool>(type: "boolean", nullable: false),
                    Status = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    RowVersion = table.Column<byte[]>(type: "bytea", rowVersion: true, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TournamentCourts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TournamentCourts_TournamentVenues_TournamentVenueId",
                        column: x => x.TournamentVenueId,
                        principalTable: "TournamentVenues",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EventParticipants",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    EventId = table.Column<Guid>(type: "uuid", nullable: false),
                    AthleteId = table.Column<Guid>(type: "uuid", nullable: true),
                    UserId = table.Column<Guid>(type: "uuid", nullable: true),
                    RegistrationId = table.Column<Guid>(type: "uuid", nullable: true),
                    ParticipantName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Email = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    Phone = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    AttendanceStatus = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    Role = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Organization = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    DietaryRequirements = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    SpecialNeeds = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    RowVersion = table.Column<byte[]>(type: "bytea", rowVersion: true, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventParticipants", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EventParticipants_Athletes_AthleteId",
                        column: x => x.AthleteId,
                        principalTable: "Athletes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_EventParticipants_EventRegistrations_RegistrationId",
                        column: x => x.RegistrationId,
                        principalTable: "EventRegistrations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_EventParticipants_Events_EventId",
                        column: x => x.EventId,
                        principalTable: "Events",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EventParticipants_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "EventSessions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    EventId = table.Column<Guid>(type: "uuid", nullable: false),
                    SessionCode = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    Title = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    SessionDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    StartTime = table.Column<TimeSpan>(type: "interval", nullable: false),
                    EndTime = table.Column<TimeSpan>(type: "interval", nullable: false),
                    VenueId = table.Column<Guid>(type: "uuid", nullable: true),
                    SpeakerId = table.Column<Guid>(type: "uuid", nullable: true),
                    CoachId = table.Column<Guid>(type: "uuid", nullable: true),
                    Status = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    Capacity = table.Column<int>(type: "integer", nullable: true),
                    CurrentAttendeeCount = table.Column<int>(type: "integer", nullable: false),
                    IsBreak = table.Column<bool>(type: "boolean", nullable: false),
                    Notes = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    RowVersion = table.Column<byte[]>(type: "bytea", rowVersion: true, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventSessions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EventSessions_EventCoaches_CoachId",
                        column: x => x.CoachId,
                        principalTable: "EventCoaches",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_EventSessions_EventSpeakers_SpeakerId",
                        column: x => x.SpeakerId,
                        principalTable: "EventSpeakers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_EventSessions_EventVenues_VenueId",
                        column: x => x.VenueId,
                        principalTable: "EventVenues",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_EventSessions_Events_EventId",
                        column: x => x.EventId,
                        principalTable: "Events",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TournamentBrackets",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TournamentId = table.Column<Guid>(type: "uuid", nullable: false),
                    DivisionId = table.Column<Guid>(type: "uuid", nullable: true),
                    BracketName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    BracketType = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    TotalRounds = table.Column<int>(type: "integer", nullable: true),
                    IsCompleted = table.Column<bool>(type: "boolean", nullable: false),
                    BracketData = table.Column<string>(type: "character varying(10000)", maxLength: 10000, nullable: true),
                    RowVersion = table.Column<byte[]>(type: "bytea", rowVersion: true, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TournamentBrackets", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TournamentBrackets_TournamentDivisions_DivisionId",
                        column: x => x.DivisionId,
                        principalTable: "TournamentDivisions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_TournamentBrackets_Tournaments_TournamentId",
                        column: x => x.TournamentId,
                        principalTable: "Tournaments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TournamentRegistrations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TournamentId = table.Column<Guid>(type: "uuid", nullable: false),
                    CategoryId = table.Column<Guid>(type: "uuid", nullable: true),
                    DivisionId = table.Column<Guid>(type: "uuid", nullable: true),
                    RegistrationStatus = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    AthleteId = table.Column<Guid>(type: "uuid", nullable: true),
                    TeamId = table.Column<Guid>(type: "uuid", nullable: true),
                    AcademyId = table.Column<Guid>(type: "uuid", nullable: true),
                    RegistrantName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Email = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    Phone = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    FeePaid = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: true),
                    PaymentDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CheckedInDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Notes = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    RowVersion = table.Column<byte[]>(type: "bytea", rowVersion: true, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TournamentRegistrations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TournamentRegistrations_Academies_AcademyId",
                        column: x => x.AcademyId,
                        principalTable: "Academies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_TournamentRegistrations_Athletes_AthleteId",
                        column: x => x.AthleteId,
                        principalTable: "Athletes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_TournamentRegistrations_TournamentCategories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "TournamentCategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_TournamentRegistrations_TournamentDivisions_DivisionId",
                        column: x => x.DivisionId,
                        principalTable: "TournamentDivisions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_TournamentRegistrations_TournamentTeams_TeamId",
                        column: x => x.TeamId,
                        principalTable: "TournamentTeams",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_TournamentRegistrations_Tournaments_TournamentId",
                        column: x => x.TournamentId,
                        principalTable: "Tournaments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TournamentAwards",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TournamentId = table.Column<Guid>(type: "uuid", nullable: false),
                    AwardType = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    AwardName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    ParticipantId = table.Column<Guid>(type: "uuid", nullable: true),
                    ParticipantName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    TeamId = table.Column<Guid>(type: "uuid", nullable: true),
                    TeamName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    Description = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    PrizeMoney = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: true),
                    CertificateUrl = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    RowVersion = table.Column<byte[]>(type: "bytea", rowVersion: true, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TournamentAwards", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TournamentAwards_TournamentParticipants_ParticipantId",
                        column: x => x.ParticipantId,
                        principalTable: "TournamentParticipants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_TournamentAwards_TournamentTeams_TeamId",
                        column: x => x.TeamId,
                        principalTable: "TournamentTeams",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_TournamentAwards_Tournaments_TournamentId",
                        column: x => x.TournamentId,
                        principalTable: "Tournaments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TournamentRankings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TournamentId = table.Column<Guid>(type: "uuid", nullable: false),
                    CategoryId = table.Column<Guid>(type: "uuid", nullable: true),
                    ParticipantId = table.Column<Guid>(type: "uuid", nullable: false),
                    Rank = table.Column<int>(type: "integer", nullable: false),
                    Points = table.Column<int>(type: "integer", nullable: false),
                    Wins = table.Column<int>(type: "integer", nullable: false),
                    Losses = table.Column<int>(type: "integer", nullable: false),
                    Draws = table.Column<int>(type: "integer", nullable: false),
                    MatchesPlayed = table.Column<int>(type: "integer", nullable: false),
                    SetsWon = table.Column<int>(type: "integer", nullable: false),
                    SetsLost = table.Column<int>(type: "integer", nullable: false),
                    GamesWon = table.Column<int>(type: "integer", nullable: false),
                    GamesLost = table.Column<int>(type: "integer", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "bytea", rowVersion: true, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TournamentRankings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TournamentRankings_TournamentCategories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "TournamentCategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_TournamentRankings_TournamentParticipants_ParticipantId",
                        column: x => x.ParticipantId,
                        principalTable: "TournamentParticipants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TournamentRankings_Tournaments_TournamentId",
                        column: x => x.TournamentId,
                        principalTable: "Tournaments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TournamentSeeds",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TournamentId = table.Column<Guid>(type: "uuid", nullable: false),
                    CategoryId = table.Column<Guid>(type: "uuid", nullable: true),
                    ParticipantId = table.Column<Guid>(type: "uuid", nullable: false),
                    SeedPosition = table.Column<int>(type: "integer", nullable: false),
                    PreviousRanking = table.Column<int>(type: "integer", nullable: true),
                    SeedSource = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    RowVersion = table.Column<byte[]>(type: "bytea", rowVersion: true, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TournamentSeeds", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TournamentSeeds_TournamentCategories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "TournamentCategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_TournamentSeeds_TournamentParticipants_ParticipantId",
                        column: x => x.ParticipantId,
                        principalTable: "TournamentParticipants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TournamentSeeds_Tournaments_TournamentId",
                        column: x => x.TournamentId,
                        principalTable: "Tournaments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TournamentMatches",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TournamentId = table.Column<Guid>(type: "uuid", nullable: false),
                    TournamentStageId = table.Column<Guid>(type: "uuid", nullable: true),
                    TournamentRoundId = table.Column<Guid>(type: "uuid", nullable: true),
                    TournamentVenueId = table.Column<Guid>(type: "uuid", nullable: true),
                    TournamentCourtId = table.Column<Guid>(type: "uuid", nullable: true),
                    MatchNumber = table.Column<int>(type: "integer", nullable: false),
                    HomeParticipantId = table.Column<Guid>(type: "uuid", nullable: true),
                    AwayParticipantId = table.Column<Guid>(type: "uuid", nullable: true),
                    HomeParticipantName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    AwayParticipantName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    ScheduledDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ScheduledTime = table.Column<TimeSpan>(type: "interval(0)", precision: 0, nullable: true),
                    Status = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    HomeScore = table.Column<int>(type: "integer", nullable: true),
                    AwayScore = table.Column<int>(type: "integer", nullable: true),
                    ScoreDetails = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    WinnerId = table.Column<Guid>(type: "uuid", nullable: true),
                    WinnerName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    Notes = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    RowVersion = table.Column<byte[]>(type: "bytea", rowVersion: true, nullable: false),
                    TournamentCourtId1 = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TournamentMatches", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TournamentMatches_TournamentCourts_TournamentCourtId",
                        column: x => x.TournamentCourtId,
                        principalTable: "TournamentCourts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_TournamentMatches_TournamentCourts_TournamentCourtId1",
                        column: x => x.TournamentCourtId1,
                        principalTable: "TournamentCourts",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_TournamentMatches_TournamentParticipants_AwayParticipantId",
                        column: x => x.AwayParticipantId,
                        principalTable: "TournamentParticipants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_TournamentMatches_TournamentParticipants_HomeParticipantId",
                        column: x => x.HomeParticipantId,
                        principalTable: "TournamentParticipants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_TournamentMatches_TournamentParticipants_WinnerId",
                        column: x => x.WinnerId,
                        principalTable: "TournamentParticipants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_TournamentMatches_TournamentRounds_TournamentRoundId",
                        column: x => x.TournamentRoundId,
                        principalTable: "TournamentRounds",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_TournamentMatches_TournamentStages_TournamentStageId",
                        column: x => x.TournamentStageId,
                        principalTable: "TournamentStages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_TournamentMatches_TournamentVenues_TournamentVenueId",
                        column: x => x.TournamentVenueId,
                        principalTable: "TournamentVenues",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_TournamentMatches_Tournaments_TournamentId",
                        column: x => x.TournamentId,
                        principalTable: "Tournaments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EventCertificates",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    EventId = table.Column<Guid>(type: "uuid", nullable: false),
                    ParticipantId = table.Column<Guid>(type: "uuid", nullable: false),
                    CertificateNumber = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    CertificateType = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    IssuedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IssuedBy = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    DocumentUrl = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    IsPrinted = table.Column<bool>(type: "boolean", nullable: false),
                    IsSent = table.Column<bool>(type: "boolean", nullable: false),
                    Notes = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    RowVersion = table.Column<byte[]>(type: "bytea", rowVersion: true, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventCertificates", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EventCertificates_EventParticipants_ParticipantId",
                        column: x => x.ParticipantId,
                        principalTable: "EventParticipants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EventCertificates_Events_EventId",
                        column: x => x.EventId,
                        principalTable: "Events",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EventFeedbacks",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    EventId = table.Column<Guid>(type: "uuid", nullable: false),
                    ParticipantId = table.Column<Guid>(type: "uuid", nullable: true),
                    UserId = table.Column<Guid>(type: "uuid", nullable: true),
                    OverallRating = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    ContentRating = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    SpeakerRating = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    VenueRating = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    OrganizationRating = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    Comments = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    Suggestions = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    WouldRecommend = table.Column<bool>(type: "boolean", nullable: false),
                    IsAnonymous = table.Column<bool>(type: "boolean", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "bytea", rowVersion: true, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventFeedbacks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EventFeedbacks_EventParticipants_ParticipantId",
                        column: x => x.ParticipantId,
                        principalTable: "EventParticipants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_EventFeedbacks_Events_EventId",
                        column: x => x.EventId,
                        principalTable: "Events",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EventFeedbacks_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "EventAgendas",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    EventId = table.Column<Guid>(type: "uuid", nullable: false),
                    SessionId = table.Column<Guid>(type: "uuid", nullable: true),
                    DisplayOrder = table.Column<int>(type: "integer", nullable: false),
                    StartTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    EndTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Title = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    SpeakerName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    Location = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    RowVersion = table.Column<byte[]>(type: "bytea", rowVersion: true, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventAgendas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EventAgendas_EventSessions_SessionId",
                        column: x => x.SessionId,
                        principalTable: "EventSessions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_EventAgendas_Events_EventId",
                        column: x => x.EventId,
                        principalTable: "Events",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EventAttendances",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    EventId = table.Column<Guid>(type: "uuid", nullable: false),
                    SessionId = table.Column<Guid>(type: "uuid", nullable: true),
                    ParticipantId = table.Column<Guid>(type: "uuid", nullable: false),
                    Status = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    CheckInTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CheckOutTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Remarks = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    MarkedBy = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    RowVersion = table.Column<byte[]>(type: "bytea", rowVersion: true, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventAttendances", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EventAttendances_EventParticipants_ParticipantId",
                        column: x => x.ParticipantId,
                        principalTable: "EventParticipants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EventAttendances_EventSessions_SessionId",
                        column: x => x.SessionId,
                        principalTable: "EventSessions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_EventAttendances_Events_EventId",
                        column: x => x.EventId,
                        principalTable: "Events",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TournamentMatchSets",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TournamentMatchId = table.Column<Guid>(type: "uuid", nullable: false),
                    SetNumber = table.Column<int>(type: "integer", nullable: false),
                    HomeScore = table.Column<int>(type: "integer", nullable: true),
                    AwayScore = table.Column<int>(type: "integer", nullable: true),
                    SetDetails = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    WinnerId = table.Column<Guid>(type: "uuid", nullable: true),
                    WinnerName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    RowVersion = table.Column<byte[]>(type: "bytea", rowVersion: true, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TournamentMatchSets", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TournamentMatchSets_TournamentMatches_TournamentMatchId",
                        column: x => x.TournamentMatchId,
                        principalTable: "TournamentMatches",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TournamentResults",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TournamentId = table.Column<Guid>(type: "uuid", nullable: false),
                    MatchId = table.Column<Guid>(type: "uuid", nullable: false),
                    WinnerId = table.Column<Guid>(type: "uuid", nullable: true),
                    WinnerName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    HomeScore = table.Column<int>(type: "integer", nullable: true),
                    AwayScore = table.Column<int>(type: "integer", nullable: true),
                    ResultDetails = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    IsVerified = table.Column<bool>(type: "boolean", nullable: false),
                    VerifiedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    VerifiedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Notes = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    RowVersion = table.Column<byte[]>(type: "bytea", rowVersion: true, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TournamentResults", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TournamentResults_TournamentMatches_MatchId",
                        column: x => x.MatchId,
                        principalTable: "TournamentMatches",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TournamentResults_TournamentParticipants_WinnerId",
                        column: x => x.WinnerId,
                        principalTable: "TournamentParticipants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_TournamentResults_Tournaments_TournamentId",
                        column: x => x.TournamentId,
                        principalTable: "Tournaments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "EventCategories",
                columns: new[] { "Id", "CategoryType", "Code", "CreatedAt", "Description", "DisplayOrder", "IsActive", "IsDeleted", "Name", "UpdatedAt" },
                values: new object[,]
                {
                    { new Guid("ec000000-0000-0000-0000-000000000001"), "SportsTraining", "SPORTS_TRAINING", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Events focused on sports skill development", 1, true, false, "Sports Training", null },
                    { new Guid("ec000000-0000-0000-0000-000000000002"), "Education", "EDUCATION", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Educational events including seminars and workshops", 2, true, false, "Education", null },
                    { new Guid("ec000000-0000-0000-0000-000000000003"), "Networking", "NETWORKING", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Events for professional networking", 3, true, false, "Networking", null },
                    { new Guid("ec000000-0000-0000-0000-000000000004"), "Health", "HEALTH", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Health and wellness focused events", 4, true, false, "Health", null },
                    { new Guid("ec000000-0000-0000-0000-000000000005"), "TalentDevelopment", "TALENT_DEV", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Talent identification and development programs", 5, true, false, "Talent Development", null },
                    { new Guid("ec000000-0000-0000-0000-000000000006"), "CommunityOutreach", "COMMUNITY_OUTREACH", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Community engagement and outreach events", 6, true, false, "Community Outreach", null },
                    { new Guid("ec000000-0000-0000-0000-000000000007"), "Professional", "PROFESSIONAL", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Professional development events", 7, true, false, "Professional", null },
                    { new Guid("ec000000-0000-0000-0000-000000000008"), "Recreational", "RECREATIONAL", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Fun and recreational events", 8, true, false, "Recreational", null },
                    { new Guid("ec000000-0000-0000-0000-000000000009"), "Competitive", "COMPETITIVE", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Competitive events and tournaments", 9, true, false, "Competitive", null },
                    { new Guid("ec000000-0000-0000-0000-000000000010"), "Mixed", "MIXED", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Multi-purpose events combining multiple categories", 10, true, false, "Mixed", null }
                });

            migrationBuilder.InsertData(
                table: "EventTypes",
                columns: new[] { "Id", "Code", "CreatedAt", "Description", "DisplayOrder", "IsActive", "IsDeleted", "Name", "UpdatedAt" },
                values: new object[,]
                {
                    { new Guid("e1000000-0000-0000-0000-000000000001"), "CAMP", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Sports camps for training and skill development", 1, true, false, "Camp", null },
                    { new Guid("e1000000-0000-0000-0000-000000000002"), "WORKSHOP", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Interactive workshops for hands-on learning", 2, true, false, "Workshop", null },
                    { new Guid("e1000000-0000-0000-0000-000000000003"), "SEMINAR", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Educational seminars and lectures", 3, true, false, "Seminar", null },
                    { new Guid("e1000000-0000-0000-0000-000000000004"), "COACHING_CLINIC", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Professional coaching clinics for athletes and coaches", 4, true, false, "Coaching Clinic", null },
                    { new Guid("e1000000-0000-0000-0000-000000000005"), "TRIAL", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Athlete trials and assessments", 5, true, false, "Trial", null },
                    { new Guid("e1000000-0000-0000-0000-000000000006"), "TALENT_HUNT", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Talent identification and scouting programs", 6, true, false, "Talent Hunt", null },
                    { new Guid("e1000000-0000-0000-0000-000000000007"), "COMPETITION", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Sports competitions and meets", 7, true, false, "Competition", null },
                    { new Guid("e1000000-0000-0000-0000-000000000008"), "COMMUNITY_EVENT", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Community engagement and outreach events", 8, true, false, "Community Event", null },
                    { new Guid("e1000000-0000-0000-0000-000000000009"), "SPORTS_FESTIVAL", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Multi-sport festivals and celebrations", 9, true, false, "Sports Festival", null },
                    { new Guid("e1000000-0000-0000-0000-000000000010"), "WEBINAR", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Online webinars and virtual sessions", 10, true, false, "Webinar", null }
                });

            migrationBuilder.CreateIndex(
                name: "IX_EventAgendas_EventId",
                table: "EventAgendas",
                column: "EventId");

            migrationBuilder.CreateIndex(
                name: "IX_EventAgendas_EventId_DisplayOrder",
                table: "EventAgendas",
                columns: new[] { "EventId", "DisplayOrder" });

            migrationBuilder.CreateIndex(
                name: "IX_EventAgendas_SessionId",
                table: "EventAgendas",
                column: "SessionId");

            migrationBuilder.CreateIndex(
                name: "IX_EventAnnouncements_EventId",
                table: "EventAnnouncements",
                column: "EventId");

            migrationBuilder.CreateIndex(
                name: "IX_EventAnnouncements_EventId_IsPublished",
                table: "EventAnnouncements",
                columns: new[] { "EventId", "IsPublished" });

            migrationBuilder.CreateIndex(
                name: "IX_EventAnnouncements_IsPublished",
                table: "EventAnnouncements",
                column: "IsPublished");

            migrationBuilder.CreateIndex(
                name: "IX_EventAttendances_EventId",
                table: "EventAttendances",
                column: "EventId");

            migrationBuilder.CreateIndex(
                name: "IX_EventAttendances_EventId_ParticipantId",
                table: "EventAttendances",
                columns: new[] { "EventId", "ParticipantId" });

            migrationBuilder.CreateIndex(
                name: "IX_EventAttendances_ParticipantId",
                table: "EventAttendances",
                column: "ParticipantId");

            migrationBuilder.CreateIndex(
                name: "IX_EventAttendances_SessionId",
                table: "EventAttendances",
                column: "SessionId");

            migrationBuilder.CreateIndex(
                name: "IX_EventAttendances_SessionId_ParticipantId",
                table: "EventAttendances",
                columns: new[] { "SessionId", "ParticipantId" },
                unique: true,
                filter: "\"SessionId\" IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_EventAttendances_Status",
                table: "EventAttendances",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_EventCategories_CategoryType",
                table: "EventCategories",
                column: "CategoryType");

            migrationBuilder.CreateIndex(
                name: "IX_EventCategories_Code",
                table: "EventCategories",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_EventCertificates_CertificateNumber",
                table: "EventCertificates",
                column: "CertificateNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_EventCertificates_EventId",
                table: "EventCertificates",
                column: "EventId");

            migrationBuilder.CreateIndex(
                name: "IX_EventCertificates_ParticipantId",
                table: "EventCertificates",
                column: "ParticipantId");

            migrationBuilder.CreateIndex(
                name: "IX_EventCoaches_CoachId",
                table: "EventCoaches",
                column: "CoachId");

            migrationBuilder.CreateIndex(
                name: "IX_EventCoaches_EventId",
                table: "EventCoaches",
                column: "EventId");

            migrationBuilder.CreateIndex(
                name: "IX_EventCoaches_EventId_CoachId",
                table: "EventCoaches",
                columns: new[] { "EventId", "CoachId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_EventDocuments_DocumentType",
                table: "EventDocuments",
                column: "DocumentType");

            migrationBuilder.CreateIndex(
                name: "IX_EventDocuments_EventId",
                table: "EventDocuments",
                column: "EventId");

            migrationBuilder.CreateIndex(
                name: "IX_EventDocuments_EventId_DocumentType",
                table: "EventDocuments",
                columns: new[] { "EventId", "DocumentType" });

            migrationBuilder.CreateIndex(
                name: "IX_EventFeedbacks_EventId",
                table: "EventFeedbacks",
                column: "EventId");

            migrationBuilder.CreateIndex(
                name: "IX_EventFeedbacks_EventId_UserId",
                table: "EventFeedbacks",
                columns: new[] { "EventId", "UserId" });

            migrationBuilder.CreateIndex(
                name: "IX_EventFeedbacks_OverallRating",
                table: "EventFeedbacks",
                column: "OverallRating");

            migrationBuilder.CreateIndex(
                name: "IX_EventFeedbacks_ParticipantId",
                table: "EventFeedbacks",
                column: "ParticipantId");

            migrationBuilder.CreateIndex(
                name: "IX_EventFeedbacks_UserId",
                table: "EventFeedbacks",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_EventMedia_EventId",
                table: "EventMedia",
                column: "EventId");

            migrationBuilder.CreateIndex(
                name: "IX_EventMedia_EventId_MediaType",
                table: "EventMedia",
                columns: new[] { "EventId", "MediaType" });

            migrationBuilder.CreateIndex(
                name: "IX_EventMedia_MediaType",
                table: "EventMedia",
                column: "MediaType");

            migrationBuilder.CreateIndex(
                name: "IX_EventParticipants_AthleteId",
                table: "EventParticipants",
                column: "AthleteId");

            migrationBuilder.CreateIndex(
                name: "IX_EventParticipants_AttendanceStatus",
                table: "EventParticipants",
                column: "AttendanceStatus");

            migrationBuilder.CreateIndex(
                name: "IX_EventParticipants_EventId",
                table: "EventParticipants",
                column: "EventId");

            migrationBuilder.CreateIndex(
                name: "IX_EventParticipants_EventId_AthleteId",
                table: "EventParticipants",
                columns: new[] { "EventId", "AthleteId" });

            migrationBuilder.CreateIndex(
                name: "IX_EventParticipants_RegistrationId",
                table: "EventParticipants",
                column: "RegistrationId");

            migrationBuilder.CreateIndex(
                name: "IX_EventParticipants_UserId",
                table: "EventParticipants",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_EventRegistrations_AthleteId",
                table: "EventRegistrations",
                column: "AthleteId");

            migrationBuilder.CreateIndex(
                name: "IX_EventRegistrations_EventId",
                table: "EventRegistrations",
                column: "EventId");

            migrationBuilder.CreateIndex(
                name: "IX_EventRegistrations_EventId_AthleteId",
                table: "EventRegistrations",
                columns: new[] { "EventId", "AthleteId" });

            migrationBuilder.CreateIndex(
                name: "IX_EventRegistrations_EventId_Status",
                table: "EventRegistrations",
                columns: new[] { "EventId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_EventRegistrations_RegistrationNumber",
                table: "EventRegistrations",
                column: "RegistrationNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_EventRegistrations_Status",
                table: "EventRegistrations",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_EventRegistrations_UserId",
                table: "EventRegistrations",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Events_AcademyId",
                table: "Events",
                column: "AcademyId");

            migrationBuilder.CreateIndex(
                name: "IX_Events_AcademyId_Status",
                table: "Events",
                columns: new[] { "AcademyId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_Events_EventCategoryId",
                table: "Events",
                column: "EventCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Events_EventCode",
                table: "Events",
                column: "EventCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Events_EventTypeId",
                table: "Events",
                column: "EventTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Events_SportId",
                table: "Events",
                column: "SportId");

            migrationBuilder.CreateIndex(
                name: "IX_Events_SportId_Status",
                table: "Events",
                columns: new[] { "SportId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_Events_StartDate_EndDate",
                table: "Events",
                columns: new[] { "StartDate", "EndDate" });

            migrationBuilder.CreateIndex(
                name: "IX_Events_Status",
                table: "Events",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_EventSchedules_EventId",
                table: "EventSchedules",
                column: "EventId");

            migrationBuilder.CreateIndex(
                name: "IX_EventSchedules_EventId_ScheduleDate",
                table: "EventSchedules",
                columns: new[] { "EventId", "ScheduleDate" });

            migrationBuilder.CreateIndex(
                name: "IX_EventSessions_CoachId",
                table: "EventSessions",
                column: "CoachId");

            migrationBuilder.CreateIndex(
                name: "IX_EventSessions_EventId",
                table: "EventSessions",
                column: "EventId");

            migrationBuilder.CreateIndex(
                name: "IX_EventSessions_EventId_SessionDate",
                table: "EventSessions",
                columns: new[] { "EventId", "SessionDate" });

            migrationBuilder.CreateIndex(
                name: "IX_EventSessions_SessionCode",
                table: "EventSessions",
                column: "SessionCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_EventSessions_SpeakerId",
                table: "EventSessions",
                column: "SpeakerId");

            migrationBuilder.CreateIndex(
                name: "IX_EventSessions_Status",
                table: "EventSessions",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_EventSessions_VenueId",
                table: "EventSessions",
                column: "VenueId");

            migrationBuilder.CreateIndex(
                name: "IX_EventSpeakers_CoachId",
                table: "EventSpeakers",
                column: "CoachId");

            migrationBuilder.CreateIndex(
                name: "IX_EventSpeakers_EventId",
                table: "EventSpeakers",
                column: "EventId");

            migrationBuilder.CreateIndex(
                name: "IX_EventSpeakers_UserId",
                table: "EventSpeakers",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_EventSponsors_EventId",
                table: "EventSponsors",
                column: "EventId");

            migrationBuilder.CreateIndex(
                name: "IX_EventTickets_EventId",
                table: "EventTickets",
                column: "EventId");

            migrationBuilder.CreateIndex(
                name: "IX_EventTickets_EventId_TicketType",
                table: "EventTickets",
                columns: new[] { "EventId", "TicketType" });

            migrationBuilder.CreateIndex(
                name: "IX_EventTickets_TicketCode",
                table: "EventTickets",
                column: "TicketCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_EventTypes_Code",
                table: "EventTypes",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_EventTypes_Name",
                table: "EventTypes",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_EventVenues_EventId",
                table: "EventVenues",
                column: "EventId");

            migrationBuilder.CreateIndex(
                name: "IX_EventVenues_FacilityId",
                table: "EventVenues",
                column: "FacilityId");

            migrationBuilder.CreateIndex(
                name: "IX_EventVolunteers_EventId",
                table: "EventVolunteers",
                column: "EventId");

            migrationBuilder.CreateIndex(
                name: "IX_EventVolunteers_Role",
                table: "EventVolunteers",
                column: "Role");

            migrationBuilder.CreateIndex(
                name: "IX_EventVolunteers_UserId",
                table: "EventVolunteers",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_TournamentAwards_AwardType",
                table: "TournamentAwards",
                column: "AwardType");

            migrationBuilder.CreateIndex(
                name: "IX_TournamentAwards_ParticipantId",
                table: "TournamentAwards",
                column: "ParticipantId");

            migrationBuilder.CreateIndex(
                name: "IX_TournamentAwards_TeamId",
                table: "TournamentAwards",
                column: "TeamId");

            migrationBuilder.CreateIndex(
                name: "IX_TournamentAwards_TournamentId",
                table: "TournamentAwards",
                column: "TournamentId");

            migrationBuilder.CreateIndex(
                name: "IX_TournamentBrackets_DivisionId",
                table: "TournamentBrackets",
                column: "DivisionId");

            migrationBuilder.CreateIndex(
                name: "IX_TournamentBrackets_TournamentId",
                table: "TournamentBrackets",
                column: "TournamentId");

            migrationBuilder.CreateIndex(
                name: "IX_TournamentCategories_CategoryType",
                table: "TournamentCategories",
                column: "CategoryType");

            migrationBuilder.CreateIndex(
                name: "IX_TournamentCategories_TournamentId",
                table: "TournamentCategories",
                column: "TournamentId");

            migrationBuilder.CreateIndex(
                name: "IX_TournamentCourts_TournamentVenueId",
                table: "TournamentCourts",
                column: "TournamentVenueId");

            migrationBuilder.CreateIndex(
                name: "IX_TournamentDivisions_CategoryId",
                table: "TournamentDivisions",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_TournamentDivisions_TournamentId",
                table: "TournamentDivisions",
                column: "TournamentId");

            migrationBuilder.CreateIndex(
                name: "IX_TournamentDocuments_DocumentType",
                table: "TournamentDocuments",
                column: "DocumentType");

            migrationBuilder.CreateIndex(
                name: "IX_TournamentDocuments_TournamentId",
                table: "TournamentDocuments",
                column: "TournamentId");

            migrationBuilder.CreateIndex(
                name: "IX_TournamentFixtures_CourtId",
                table: "TournamentFixtures",
                column: "CourtId");

            migrationBuilder.CreateIndex(
                name: "IX_TournamentFixtures_TournamentId",
                table: "TournamentFixtures",
                column: "TournamentId");

            migrationBuilder.CreateIndex(
                name: "IX_TournamentFixtures_TournamentId_FixtureNumber",
                table: "TournamentFixtures",
                columns: new[] { "TournamentId", "FixtureNumber" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TournamentFixtures_TournamentStageId",
                table: "TournamentFixtures",
                column: "TournamentStageId");

            migrationBuilder.CreateIndex(
                name: "IX_TournamentFixtures_VenueId",
                table: "TournamentFixtures",
                column: "VenueId");

            migrationBuilder.CreateIndex(
                name: "IX_TournamentGallery_MediaType",
                table: "TournamentGallery",
                column: "MediaType");

            migrationBuilder.CreateIndex(
                name: "IX_TournamentGallery_TournamentId",
                table: "TournamentGallery",
                column: "TournamentId");

            migrationBuilder.CreateIndex(
                name: "IX_TournamentMatches_AwayParticipantId",
                table: "TournamentMatches",
                column: "AwayParticipantId");

            migrationBuilder.CreateIndex(
                name: "IX_TournamentMatches_HomeParticipantId",
                table: "TournamentMatches",
                column: "HomeParticipantId");

            migrationBuilder.CreateIndex(
                name: "IX_TournamentMatches_ScheduledDate",
                table: "TournamentMatches",
                column: "ScheduledDate");

            migrationBuilder.CreateIndex(
                name: "IX_TournamentMatches_Status",
                table: "TournamentMatches",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_TournamentMatches_TournamentCourtId",
                table: "TournamentMatches",
                column: "TournamentCourtId");

            migrationBuilder.CreateIndex(
                name: "IX_TournamentMatches_TournamentCourtId1",
                table: "TournamentMatches",
                column: "TournamentCourtId1");

            migrationBuilder.CreateIndex(
                name: "IX_TournamentMatches_TournamentId",
                table: "TournamentMatches",
                column: "TournamentId");

            migrationBuilder.CreateIndex(
                name: "IX_TournamentMatches_TournamentId_MatchNumber",
                table: "TournamentMatches",
                columns: new[] { "TournamentId", "MatchNumber" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TournamentMatches_TournamentRoundId",
                table: "TournamentMatches",
                column: "TournamentRoundId");

            migrationBuilder.CreateIndex(
                name: "IX_TournamentMatches_TournamentStageId",
                table: "TournamentMatches",
                column: "TournamentStageId");

            migrationBuilder.CreateIndex(
                name: "IX_TournamentMatches_TournamentVenueId",
                table: "TournamentMatches",
                column: "TournamentVenueId");

            migrationBuilder.CreateIndex(
                name: "IX_TournamentMatches_WinnerId",
                table: "TournamentMatches",
                column: "WinnerId");

            migrationBuilder.CreateIndex(
                name: "IX_TournamentMatchSets_TournamentMatchId",
                table: "TournamentMatchSets",
                column: "TournamentMatchId");

            migrationBuilder.CreateIndex(
                name: "IX_TournamentMatchSets_TournamentMatchId_SetNumber",
                table: "TournamentMatchSets",
                columns: new[] { "TournamentMatchId", "SetNumber" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TournamentOfficials_CoachId",
                table: "TournamentOfficials",
                column: "CoachId");

            migrationBuilder.CreateIndex(
                name: "IX_TournamentOfficials_Role",
                table: "TournamentOfficials",
                column: "Role");

            migrationBuilder.CreateIndex(
                name: "IX_TournamentOfficials_TournamentId",
                table: "TournamentOfficials",
                column: "TournamentId");

            migrationBuilder.CreateIndex(
                name: "IX_TournamentParticipants_AcademyId",
                table: "TournamentParticipants",
                column: "AcademyId");

            migrationBuilder.CreateIndex(
                name: "IX_TournamentParticipants_AthleteId",
                table: "TournamentParticipants",
                column: "AthleteId");

            migrationBuilder.CreateIndex(
                name: "IX_TournamentParticipants_CategoryId",
                table: "TournamentParticipants",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_TournamentParticipants_ParticipantType",
                table: "TournamentParticipants",
                column: "ParticipantType");

            migrationBuilder.CreateIndex(
                name: "IX_TournamentParticipants_TeamId",
                table: "TournamentParticipants",
                column: "TeamId");

            migrationBuilder.CreateIndex(
                name: "IX_TournamentParticipants_TournamentId",
                table: "TournamentParticipants",
                column: "TournamentId");

            migrationBuilder.CreateIndex(
                name: "IX_TournamentRankings_CategoryId",
                table: "TournamentRankings",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_TournamentRankings_ParticipantId",
                table: "TournamentRankings",
                column: "ParticipantId");

            migrationBuilder.CreateIndex(
                name: "IX_TournamentRankings_TournamentId",
                table: "TournamentRankings",
                column: "TournamentId");

            migrationBuilder.CreateIndex(
                name: "IX_TournamentRankings_TournamentId_ParticipantId",
                table: "TournamentRankings",
                columns: new[] { "TournamentId", "ParticipantId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TournamentRankings_TournamentId_Rank",
                table: "TournamentRankings",
                columns: new[] { "TournamentId", "Rank" });

            migrationBuilder.CreateIndex(
                name: "IX_TournamentRegistrations_AcademyId",
                table: "TournamentRegistrations",
                column: "AcademyId");

            migrationBuilder.CreateIndex(
                name: "IX_TournamentRegistrations_AthleteId",
                table: "TournamentRegistrations",
                column: "AthleteId");

            migrationBuilder.CreateIndex(
                name: "IX_TournamentRegistrations_CategoryId",
                table: "TournamentRegistrations",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_TournamentRegistrations_DivisionId",
                table: "TournamentRegistrations",
                column: "DivisionId");

            migrationBuilder.CreateIndex(
                name: "IX_TournamentRegistrations_RegistrationStatus",
                table: "TournamentRegistrations",
                column: "RegistrationStatus");

            migrationBuilder.CreateIndex(
                name: "IX_TournamentRegistrations_TeamId",
                table: "TournamentRegistrations",
                column: "TeamId");

            migrationBuilder.CreateIndex(
                name: "IX_TournamentRegistrations_TournamentId",
                table: "TournamentRegistrations",
                column: "TournamentId");

            migrationBuilder.CreateIndex(
                name: "IX_TournamentResults_MatchId",
                table: "TournamentResults",
                column: "MatchId");

            migrationBuilder.CreateIndex(
                name: "IX_TournamentResults_TournamentId",
                table: "TournamentResults",
                column: "TournamentId");

            migrationBuilder.CreateIndex(
                name: "IX_TournamentResults_WinnerId",
                table: "TournamentResults",
                column: "WinnerId");

            migrationBuilder.CreateIndex(
                name: "IX_TournamentRounds_RoundNumber",
                table: "TournamentRounds",
                column: "RoundNumber");

            migrationBuilder.CreateIndex(
                name: "IX_TournamentRounds_TournamentStageId",
                table: "TournamentRounds",
                column: "TournamentStageId");

            migrationBuilder.CreateIndex(
                name: "IX_TournamentRules_TournamentId",
                table: "TournamentRules",
                column: "TournamentId");

            migrationBuilder.CreateIndex(
                name: "IX_Tournaments_AcademyId",
                table: "Tournaments",
                column: "AcademyId");

            migrationBuilder.CreateIndex(
                name: "IX_Tournaments_SportId",
                table: "Tournaments",
                column: "SportId");

            migrationBuilder.CreateIndex(
                name: "IX_Tournaments_StartDate_EndDate",
                table: "Tournaments",
                columns: new[] { "StartDate", "EndDate" });

            migrationBuilder.CreateIndex(
                name: "IX_Tournaments_Status",
                table: "Tournaments",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_Tournaments_TournamentCode",
                table: "Tournaments",
                column: "TournamentCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Tournaments_TournamentType",
                table: "Tournaments",
                column: "TournamentType");

            migrationBuilder.CreateIndex(
                name: "IX_TournamentSeeds_CategoryId",
                table: "TournamentSeeds",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_TournamentSeeds_ParticipantId",
                table: "TournamentSeeds",
                column: "ParticipantId");

            migrationBuilder.CreateIndex(
                name: "IX_TournamentSeeds_TournamentId",
                table: "TournamentSeeds",
                column: "TournamentId");

            migrationBuilder.CreateIndex(
                name: "IX_TournamentSeeds_TournamentId_SeedPosition",
                table: "TournamentSeeds",
                columns: new[] { "TournamentId", "SeedPosition" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TournamentSponsors_TournamentId",
                table: "TournamentSponsors",
                column: "TournamentId");

            migrationBuilder.CreateIndex(
                name: "IX_TournamentSports_SportId",
                table: "TournamentSports",
                column: "SportId");

            migrationBuilder.CreateIndex(
                name: "IX_TournamentSports_TournamentId",
                table: "TournamentSports",
                column: "TournamentId");

            migrationBuilder.CreateIndex(
                name: "IX_TournamentStages_StageOrder",
                table: "TournamentStages",
                column: "StageOrder");

            migrationBuilder.CreateIndex(
                name: "IX_TournamentStages_TournamentId",
                table: "TournamentStages",
                column: "TournamentId");

            migrationBuilder.CreateIndex(
                name: "IX_TournamentTeams_AcademyId",
                table: "TournamentTeams",
                column: "AcademyId");

            migrationBuilder.CreateIndex(
                name: "IX_TournamentTeams_TournamentId",
                table: "TournamentTeams",
                column: "TournamentId");

            migrationBuilder.CreateIndex(
                name: "IX_TournamentTeams_TournamentId_TeamCode",
                table: "TournamentTeams",
                columns: new[] { "TournamentId", "TeamCode" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TournamentVenues_FacilityId",
                table: "TournamentVenues",
                column: "FacilityId");

            migrationBuilder.CreateIndex(
                name: "IX_TournamentVenues_TournamentId",
                table: "TournamentVenues",
                column: "TournamentId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EventAgendas");

            migrationBuilder.DropTable(
                name: "EventAnnouncements");

            migrationBuilder.DropTable(
                name: "EventAttendances");

            migrationBuilder.DropTable(
                name: "EventCertificates");

            migrationBuilder.DropTable(
                name: "EventDocuments");

            migrationBuilder.DropTable(
                name: "EventFeedbacks");

            migrationBuilder.DropTable(
                name: "EventMedia");

            migrationBuilder.DropTable(
                name: "EventSchedules");

            migrationBuilder.DropTable(
                name: "EventSponsors");

            migrationBuilder.DropTable(
                name: "EventTickets");

            migrationBuilder.DropTable(
                name: "EventVolunteers");

            migrationBuilder.DropTable(
                name: "TournamentAwards");

            migrationBuilder.DropTable(
                name: "TournamentBrackets");

            migrationBuilder.DropTable(
                name: "TournamentDocuments");

            migrationBuilder.DropTable(
                name: "TournamentFixtures");

            migrationBuilder.DropTable(
                name: "TournamentGallery");

            migrationBuilder.DropTable(
                name: "TournamentMatchSets");

            migrationBuilder.DropTable(
                name: "TournamentOfficials");

            migrationBuilder.DropTable(
                name: "TournamentRankings");

            migrationBuilder.DropTable(
                name: "TournamentRegistrations");

            migrationBuilder.DropTable(
                name: "TournamentResults");

            migrationBuilder.DropTable(
                name: "TournamentRules");

            migrationBuilder.DropTable(
                name: "TournamentSeeds");

            migrationBuilder.DropTable(
                name: "TournamentSponsors");

            migrationBuilder.DropTable(
                name: "TournamentSports");

            migrationBuilder.DropTable(
                name: "EventSessions");

            migrationBuilder.DropTable(
                name: "EventParticipants");

            migrationBuilder.DropTable(
                name: "TournamentDivisions");

            migrationBuilder.DropTable(
                name: "TournamentMatches");

            migrationBuilder.DropTable(
                name: "EventCoaches");

            migrationBuilder.DropTable(
                name: "EventSpeakers");

            migrationBuilder.DropTable(
                name: "EventVenues");

            migrationBuilder.DropTable(
                name: "EventRegistrations");

            migrationBuilder.DropTable(
                name: "TournamentCourts");

            migrationBuilder.DropTable(
                name: "TournamentParticipants");

            migrationBuilder.DropTable(
                name: "TournamentRounds");

            migrationBuilder.DropTable(
                name: "Events");

            migrationBuilder.DropTable(
                name: "TournamentVenues");

            migrationBuilder.DropTable(
                name: "TournamentCategories");

            migrationBuilder.DropTable(
                name: "TournamentTeams");

            migrationBuilder.DropTable(
                name: "TournamentStages");

            migrationBuilder.DropTable(
                name: "EventCategories");

            migrationBuilder.DropTable(
                name: "EventTypes");

            migrationBuilder.DropTable(
                name: "Tournaments");
        }
    }
}
