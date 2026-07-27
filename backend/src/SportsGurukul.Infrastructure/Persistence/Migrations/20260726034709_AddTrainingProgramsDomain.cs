using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SportsGurukul.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddTrainingProgramsDomain : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AcademyViews",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    AcademyId = table.Column<Guid>(type: "uuid", nullable: false),
                    ViewedByUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    ViewedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Source = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AcademyViews", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AcademyViews_Academies_AcademyId",
                        column: x => x.AcademyId,
                        principalTable: "Academies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AthleteAcademies",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    AthleteId = table.Column<Guid>(type: "uuid", nullable: false),
                    AcademyId = table.Column<Guid>(type: "uuid", nullable: false),
                    RegisteredDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AthleteAcademies", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AthleteAcademies_Academies_AcademyId",
                        column: x => x.AcademyId,
                        principalTable: "Academies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AthleteAcademies_Athletes_AthleteId",
                        column: x => x.AthleteId,
                        principalTable: "Athletes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CoachAcademies",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CoachId = table.Column<Guid>(type: "uuid", nullable: false),
                    AcademyId = table.Column<Guid>(type: "uuid", nullable: false),
                    AssignedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CoachAcademies", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CoachAcademies_Academies_AcademyId",
                        column: x => x.AcademyId,
                        principalTable: "Academies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CoachAcademies_Coaches_CoachId",
                        column: x => x.CoachId,
                        principalTable: "Coaches",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Facilities",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    AcademyId = table.Column<Guid>(type: "uuid", nullable: false),
                    BranchId = table.Column<Guid>(type: "uuid", nullable: true),
                    FacilityCode = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    FacilityName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    FacilityType = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    Description = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    Capacity = table.Column<int>(type: "integer", nullable: false),
                    IndoorOutdoor = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    SurfaceType = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    LightingAvailable = table.Column<bool>(type: "boolean", nullable: false),
                    ParkingAvailable = table.Column<bool>(type: "boolean", nullable: false),
                    ChangingRoomAvailable = table.Column<bool>(type: "boolean", nullable: false),
                    WashroomAvailable = table.Column<bool>(type: "boolean", nullable: false),
                    MedicalRoomAvailable = table.Column<bool>(type: "boolean", nullable: false),
                    Status = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    RowVersion = table.Column<byte[]>(type: "bytea", rowVersion: true, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Facilities", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Facilities_Academies_AcademyId",
                        column: x => x.AcademyId,
                        principalTable: "Academies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Facilities_AcademyBranches_BranchId",
                        column: x => x.BranchId,
                        principalTable: "AcademyBranches",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "RecentAcademySearches",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    SearchTerm = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    City = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    State = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    SportName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    AcademyCount = table.Column<int>(type: "integer", nullable: false),
                    SearchedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RecentAcademySearches", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SavedAcademySearches",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    SearchName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    SearchTerm = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    City = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    State = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Country = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    District = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    PinCode = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    SportName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    SportCategory = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    FacilityType = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    HasSwimmingPool = table.Column<bool>(type: "boolean", nullable: true),
                    HasIndoorStadium = table.Column<bool>(type: "boolean", nullable: true),
                    HasCricketGround = table.Column<bool>(type: "boolean", nullable: true),
                    HasFootballGround = table.Column<bool>(type: "boolean", nullable: true),
                    HasGym = table.Column<bool>(type: "boolean", nullable: true),
                    HasYogaHall = table.Column<bool>(type: "boolean", nullable: true),
                    HasParking = table.Column<bool>(type: "boolean", nullable: true),
                    HasMedicalRoom = table.Column<bool>(type: "boolean", nullable: true),
                    HasWifi = table.Column<bool>(type: "boolean", nullable: true),
                    HasCafeteria = table.Column<bool>(type: "boolean", nullable: true),
                    VerifiedOnly = table.Column<bool>(type: "boolean", nullable: true),
                    GovernmentRegisteredOnly = table.Column<bool>(type: "boolean", nullable: true),
                    OpenNow = table.Column<bool>(type: "boolean", nullable: true),
                    WeekendOpen = table.Column<bool>(type: "boolean", nullable: true),
                    MinMembershipPrice = table.Column<decimal>(type: "numeric", nullable: true),
                    MaxMembershipPrice = table.Column<decimal>(type: "numeric", nullable: true),
                    MinRating = table.Column<decimal>(type: "numeric", nullable: true),
                    ResultCount = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SavedAcademySearches", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TrainingPrograms",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ProgramCode = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    ProgramName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    SportId = table.Column<Guid>(type: "uuid", nullable: false),
                    AcademyId = table.Column<Guid>(type: "uuid", nullable: false),
                    Description = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    DifficultyLevel = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    MinimumAge = table.Column<int>(type: "integer", nullable: false),
                    MaximumAge = table.Column<int>(type: "integer", nullable: false),
                    DurationWeeks = table.Column<int>(type: "integer", nullable: false),
                    Capacity = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    RowVersion = table.Column<byte[]>(type: "bytea", rowVersion: true, nullable: false, defaultValueSql: "E'\\\\x00'::bytea"),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TrainingPrograms", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TrainingPrograms_Academies_AcademyId",
                        column: x => x.AcademyId,
                        principalTable: "Academies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TrainingPrograms_Sports_SportId",
                        column: x => x.SportId,
                        principalTable: "Sports",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "FacilityAmenities",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    FacilityId = table.Column<Guid>(type: "uuid", nullable: false),
                    AmenityName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    IsAvailable = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FacilityAmenities", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FacilityAmenities_Facilities_FacilityId",
                        column: x => x.FacilityId,
                        principalTable: "Facilities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FacilityAreas",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    FacilityId = table.Column<Guid>(type: "uuid", nullable: false),
                    AreaName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    Capacity = table.Column<int>(type: "integer", nullable: true),
                    AreaType = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FacilityAreas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FacilityAreas_Facilities_FacilityId",
                        column: x => x.FacilityId,
                        principalTable: "Facilities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FacilityEquipment",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    FacilityId = table.Column<Guid>(type: "uuid", nullable: false),
                    EquipmentName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Category = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    PurchaseDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Condition = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    MaintenanceSchedule = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    WarrantyExpiry = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Quantity = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    Description = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FacilityEquipment", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FacilityEquipment_Facilities_FacilityId",
                        column: x => x.FacilityId,
                        principalTable: "Facilities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FacilityImages",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    FacilityId = table.Column<Guid>(type: "uuid", nullable: false),
                    ImageUrl = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false),
                    Caption = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    IsPrimary = table.Column<bool>(type: "boolean", nullable: false),
                    SortOrder = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FacilityImages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FacilityImages_Facilities_FacilityId",
                        column: x => x.FacilityId,
                        principalTable: "Facilities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FacilityPricing",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    FacilityId = table.Column<Guid>(type: "uuid", nullable: false),
                    PricingName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    HourlyRate = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    DailyRate = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    MonthlyRate = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    PeakHourlyRate = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: true),
                    OffPeakHourlyRate = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: true),
                    Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FacilityPricing", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FacilityPricing_Facilities_FacilityId",
                        column: x => x.FacilityId,
                        principalTable: "Facilities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FacilityReviews",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    FacilityId = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    Rating = table.Column<int>(type: "integer", nullable: false),
                    ReviewText = table.Column<string>(type: "character varying(5000)", maxLength: 5000, nullable: true),
                    IsApproved = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FacilityReviews", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FacilityReviews_Facilities_FacilityId",
                        column: x => x.FacilityId,
                        principalTable: "Facilities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TrainingBatches",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ProgramId = table.Column<Guid>(type: "uuid", nullable: false),
                    BatchCode = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    CoachId = table.Column<Guid>(type: "uuid", nullable: false),
                    BranchId = table.Column<Guid>(type: "uuid", nullable: false),
                    StartDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    EndDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    MaximumSeats = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    RowVersion = table.Column<byte[]>(type: "bytea", rowVersion: true, nullable: false, defaultValueSql: "E'\\\\x00'::bytea"),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TrainingBatches", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TrainingBatches_AcademyBranches_BranchId",
                        column: x => x.BranchId,
                        principalTable: "AcademyBranches",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TrainingBatches_Coaches_CoachId",
                        column: x => x.CoachId,
                        principalTable: "Coaches",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TrainingBatches_TrainingPrograms_ProgramId",
                        column: x => x.ProgramId,
                        principalTable: "TrainingPrograms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TrainingGoals",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ProgramId = table.Column<Guid>(type: "uuid", nullable: false),
                    GoalName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    TargetWeek = table.Column<int>(type: "integer", nullable: false),
                    IsAchieved = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TrainingGoals", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TrainingGoals_TrainingPrograms_ProgramId",
                        column: x => x.ProgramId,
                        principalTable: "TrainingPrograms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TrainingMilestones",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ProgramId = table.Column<Guid>(type: "uuid", nullable: false),
                    MilestoneName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    WeekNumber = table.Column<int>(type: "integer", nullable: false),
                    IsCompleted = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TrainingMilestones", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TrainingMilestones_TrainingPrograms_ProgramId",
                        column: x => x.ProgramId,
                        principalTable: "TrainingPrograms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TrainingProgramSports",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TrainingProgramId = table.Column<Guid>(type: "uuid", nullable: false),
                    SportId = table.Column<Guid>(type: "uuid", nullable: false),
                    IsPrimarySport = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TrainingProgramSports", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TrainingProgramSports_Sports_SportId",
                        column: x => x.SportId,
                        principalTable: "Sports",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TrainingProgramSports_TrainingPrograms_TrainingProgramId",
                        column: x => x.TrainingProgramId,
                        principalTable: "TrainingPrograms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FacilityCourts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    FacilityId = table.Column<Guid>(type: "uuid", nullable: false),
                    FacilityAreaId = table.Column<Guid>(type: "uuid", nullable: true),
                    CourtNumber = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    CourtName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    CourtType = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    Capacity = table.Column<int>(type: "integer", nullable: true),
                    Status = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    Description = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FacilityCourts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FacilityCourts_Facilities_FacilityId",
                        column: x => x.FacilityId,
                        principalTable: "Facilities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FacilityCourts_FacilityAreas_FacilityAreaId",
                        column: x => x.FacilityAreaId,
                        principalTable: "FacilityAreas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "EquipmentMaintenance",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    FacilityEquipmentId = table.Column<Guid>(type: "uuid", nullable: false),
                    ScheduledDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CompletedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    MaintenanceType = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    Cost = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: true),
                    PerformedBy = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    Notes = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    IsCompleted = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EquipmentMaintenance", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EquipmentMaintenance_FacilityEquipment_FacilityEquipmentId",
                        column: x => x.FacilityEquipmentId,
                        principalTable: "FacilityEquipment",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TrainingEnrollments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    BatchId = table.Column<Guid>(type: "uuid", nullable: false),
                    AthleteId = table.Column<Guid>(type: "uuid", nullable: false),
                    EnrollmentDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Status = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    RowVersion = table.Column<byte[]>(type: "bytea", rowVersion: true, nullable: false, defaultValueSql: "E'\\\\x00'::bytea"),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TrainingEnrollments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TrainingEnrollments_Athletes_AthleteId",
                        column: x => x.AthleteId,
                        principalTable: "Athletes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TrainingEnrollments_TrainingBatches_BatchId",
                        column: x => x.BatchId,
                        principalTable: "TrainingBatches",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TrainingSessions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    BatchId = table.Column<Guid>(type: "uuid", nullable: false),
                    SessionCode = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    SessionTitle = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    SessionType = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    SessionDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    StartTime = table.Column<TimeSpan>(type: "interval", nullable: false),
                    EndTime = table.Column<TimeSpan>(type: "interval", nullable: false),
                    FacilityId = table.Column<Guid>(type: "uuid", nullable: true),
                    CoachId = table.Column<Guid>(type: "uuid", nullable: false),
                    Status = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    RowVersion = table.Column<byte[]>(type: "bytea", rowVersion: true, nullable: false, defaultValueSql: "E'\\\\x00'::bytea"),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TrainingSessions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TrainingSessions_Coaches_CoachId",
                        column: x => x.CoachId,
                        principalTable: "Coaches",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TrainingSessions_Facilities_FacilityId",
                        column: x => x.FacilityId,
                        principalTable: "Facilities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_TrainingSessions_TrainingBatches_BatchId",
                        column: x => x.BatchId,
                        principalTable: "TrainingBatches",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FacilitySchedules",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    FacilityId = table.Column<Guid>(type: "uuid", nullable: false),
                    FacilityCourtId = table.Column<Guid>(type: "uuid", nullable: true),
                    DayOfWeek = table.Column<int>(type: "integer", nullable: false),
                    OpeningTime = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    ClosingTime = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    IsClosed = table.Column<bool>(type: "boolean", nullable: false),
                    IsMaintenanceWindow = table.Column<bool>(type: "boolean", nullable: false),
                    Notes = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FacilitySchedules", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FacilitySchedules_Facilities_FacilityId",
                        column: x => x.FacilityId,
                        principalTable: "Facilities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FacilitySchedules_FacilityCourts_FacilityCourtId",
                        column: x => x.FacilityCourtId,
                        principalTable: "FacilityCourts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TrainingCertificates",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    EnrollmentId = table.Column<Guid>(type: "uuid", nullable: false),
                    CertificateType = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    CertificateNumber = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    IssuedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    FileUrl = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    RowVersion = table.Column<byte[]>(type: "bytea", rowVersion: true, nullable: false, defaultValueSql: "E'\\\\x00'::bytea"),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TrainingCertificates", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TrainingCertificates_TrainingEnrollments_EnrollmentId",
                        column: x => x.EnrollmentId,
                        principalTable: "TrainingEnrollments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TrainingProgresses",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    EnrollmentId = table.Column<Guid>(type: "uuid", nullable: false),
                    CurrentLevel = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    CompletedPercentage = table.Column<decimal>(type: "numeric(5,2)", precision: 5, scale: 2, nullable: false),
                    OverallRating = table.Column<decimal>(type: "numeric(5,2)", precision: 5, scale: 2, nullable: true),
                    RowVersion = table.Column<byte[]>(type: "bytea", rowVersion: true, nullable: false, defaultValueSql: "E'\\\\x00'::bytea"),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TrainingProgresses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TrainingProgresses_TrainingEnrollments_EnrollmentId",
                        column: x => x.EnrollmentId,
                        principalTable: "TrainingEnrollments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Attendances",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    SessionId = table.Column<Guid>(type: "uuid", nullable: false),
                    AthleteId = table.Column<Guid>(type: "uuid", nullable: false),
                    AttendanceStatus = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    CheckInTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CheckOutTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Remarks = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Attendances", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Attendances_Athletes_AthleteId",
                        column: x => x.AthleteId,
                        principalTable: "Athletes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Attendances_TrainingSessions_SessionId",
                        column: x => x.SessionId,
                        principalTable: "TrainingSessions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SessionSchedules",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    SessionId = table.Column<Guid>(type: "uuid", nullable: false),
                    DayOfWeek = table.Column<int>(type: "integer", nullable: false),
                    StartTime = table.Column<TimeSpan>(type: "interval", nullable: false),
                    EndTime = table.Column<TimeSpan>(type: "interval", nullable: false),
                    IsRecurring = table.Column<bool>(type: "boolean", nullable: false),
                    Notes = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SessionSchedules", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SessionSchedules_TrainingSessions_SessionId",
                        column: x => x.SessionId,
                        principalTable: "TrainingSessions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TrainingAssessments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    SessionId = table.Column<Guid>(type: "uuid", nullable: false),
                    AssessmentType = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    AssessmentName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    MaximumScore = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: false),
                    PassingScore = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: false),
                    RowVersion = table.Column<byte[]>(type: "bytea", rowVersion: true, nullable: false, defaultValueSql: "E'\\\\x00'::bytea"),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TrainingAssessments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TrainingAssessments_TrainingSessions_SessionId",
                        column: x => x.SessionId,
                        principalTable: "TrainingSessions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TrainingMaterials",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ProgramId = table.Column<Guid>(type: "uuid", nullable: false),
                    SessionId = table.Column<Guid>(type: "uuid", nullable: true),
                    Title = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    MaterialType = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    FileUrl = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    SortOrder = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TrainingMaterials", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TrainingMaterials_TrainingPrograms_ProgramId",
                        column: x => x.ProgramId,
                        principalTable: "TrainingPrograms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TrainingMaterials_TrainingSessions_SessionId",
                        column: x => x.SessionId,
                        principalTable: "TrainingSessions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "AssessmentResults",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    AssessmentId = table.Column<Guid>(type: "uuid", nullable: false),
                    AthleteId = table.Column<Guid>(type: "uuid", nullable: false),
                    Score = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: false),
                    IsPassed = table.Column<bool>(type: "boolean", nullable: false),
                    Remarks = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    AssessedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AssessmentResults", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AssessmentResults_Athletes_AthleteId",
                        column: x => x.AthleteId,
                        principalTable: "Athletes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AssessmentResults_TrainingAssessments_AssessmentId",
                        column: x => x.AssessmentId,
                        principalTable: "TrainingAssessments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AcademyViews_AcademyId",
                table: "AcademyViews",
                column: "AcademyId");

            migrationBuilder.CreateIndex(
                name: "IX_AcademyViews_AcademyId_ViewedAt",
                table: "AcademyViews",
                columns: new[] { "AcademyId", "ViewedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_AcademyViews_ViewedByUserId",
                table: "AcademyViews",
                column: "ViewedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_AssessmentResults_AssessmentId",
                table: "AssessmentResults",
                column: "AssessmentId");

            migrationBuilder.CreateIndex(
                name: "IX_AssessmentResults_AssessmentId_AthleteId",
                table: "AssessmentResults",
                columns: new[] { "AssessmentId", "AthleteId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AssessmentResults_AthleteId",
                table: "AssessmentResults",
                column: "AthleteId");

            migrationBuilder.CreateIndex(
                name: "IX_AssessmentResults_IsPassed",
                table: "AssessmentResults",
                column: "IsPassed");

            migrationBuilder.CreateIndex(
                name: "IX_AthleteAcademies_AcademyId",
                table: "AthleteAcademies",
                column: "AcademyId");

            migrationBuilder.CreateIndex(
                name: "IX_AthleteAcademies_AthleteId",
                table: "AthleteAcademies",
                column: "AthleteId");

            migrationBuilder.CreateIndex(
                name: "IX_AthleteAcademies_AthleteId_AcademyId",
                table: "AthleteAcademies",
                columns: new[] { "AthleteId", "AcademyId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Attendances_AthleteId",
                table: "Attendances",
                column: "AthleteId");

            migrationBuilder.CreateIndex(
                name: "IX_Attendances_AttendanceStatus",
                table: "Attendances",
                column: "AttendanceStatus");

            migrationBuilder.CreateIndex(
                name: "IX_Attendances_SessionId",
                table: "Attendances",
                column: "SessionId");

            migrationBuilder.CreateIndex(
                name: "IX_Attendances_SessionId_AthleteId",
                table: "Attendances",
                columns: new[] { "SessionId", "AthleteId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Attendances_SessionId_Status",
                table: "Attendances",
                columns: new[] { "SessionId", "AttendanceStatus" });

            migrationBuilder.CreateIndex(
                name: "IX_CoachAcademies_AcademyId",
                table: "CoachAcademies",
                column: "AcademyId");

            migrationBuilder.CreateIndex(
                name: "IX_CoachAcademies_CoachId",
                table: "CoachAcademies",
                column: "CoachId");

            migrationBuilder.CreateIndex(
                name: "IX_CoachAcademies_CoachId_AcademyId",
                table: "CoachAcademies",
                columns: new[] { "CoachId", "AcademyId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_EquipmentMaintenance_EquipmentId",
                table: "EquipmentMaintenance",
                column: "FacilityEquipmentId");

            migrationBuilder.CreateIndex(
                name: "IX_EquipmentMaintenance_ScheduledDate",
                table: "EquipmentMaintenance",
                column: "ScheduledDate");

            migrationBuilder.CreateIndex(
                name: "IX_Facilities_AcademyId",
                table: "Facilities",
                column: "AcademyId");

            migrationBuilder.CreateIndex(
                name: "IX_Facilities_AcademyId_BranchId_Name",
                table: "Facilities",
                columns: new[] { "AcademyId", "BranchId", "FacilityName" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Facilities_AcademyId_FacilityType",
                table: "Facilities",
                columns: new[] { "AcademyId", "FacilityType" });

            migrationBuilder.CreateIndex(
                name: "IX_Facilities_BranchId",
                table: "Facilities",
                column: "BranchId");

            migrationBuilder.CreateIndex(
                name: "IX_Facilities_FacilityCode",
                table: "Facilities",
                column: "FacilityCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Facilities_FacilityType",
                table: "Facilities",
                column: "FacilityType");

            migrationBuilder.CreateIndex(
                name: "IX_Facilities_Status",
                table: "Facilities",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_FacilityAmenities_FacilityId",
                table: "FacilityAmenities",
                column: "FacilityId");

            migrationBuilder.CreateIndex(
                name: "IX_FacilityAmenities_FacilityId_Name",
                table: "FacilityAmenities",
                columns: new[] { "FacilityId", "AmenityName" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_FacilityAreas_FacilityId",
                table: "FacilityAreas",
                column: "FacilityId");

            migrationBuilder.CreateIndex(
                name: "IX_FacilityAreas_FacilityId_Name",
                table: "FacilityAreas",
                columns: new[] { "FacilityId", "AreaName" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_FacilityCourts_FacilityAreaId",
                table: "FacilityCourts",
                column: "FacilityAreaId");

            migrationBuilder.CreateIndex(
                name: "IX_FacilityCourts_FacilityId",
                table: "FacilityCourts",
                column: "FacilityId");

            migrationBuilder.CreateIndex(
                name: "IX_FacilityCourts_FacilityId_CourtNumber",
                table: "FacilityCourts",
                columns: new[] { "FacilityId", "CourtNumber" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_FacilityEquipment_FacilityId",
                table: "FacilityEquipment",
                column: "FacilityId");

            migrationBuilder.CreateIndex(
                name: "IX_FacilityEquipment_FacilityId_Name",
                table: "FacilityEquipment",
                columns: new[] { "FacilityId", "EquipmentName" });

            migrationBuilder.CreateIndex(
                name: "IX_FacilityImages_FacilityId",
                table: "FacilityImages",
                column: "FacilityId");

            migrationBuilder.CreateIndex(
                name: "IX_FacilityPricing_FacilityId",
                table: "FacilityPricing",
                column: "FacilityId");

            migrationBuilder.CreateIndex(
                name: "IX_FacilityReviews_FacilityId",
                table: "FacilityReviews",
                column: "FacilityId");

            migrationBuilder.CreateIndex(
                name: "IX_FacilityReviews_FacilityId_UserId",
                table: "FacilityReviews",
                columns: new[] { "FacilityId", "UserId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_FacilityReviews_UserId",
                table: "FacilityReviews",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_FacilitySchedules_Facility_Day_Court",
                table: "FacilitySchedules",
                columns: new[] { "FacilityId", "DayOfWeek", "FacilityCourtId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_FacilitySchedules_FacilityCourtId",
                table: "FacilitySchedules",
                column: "FacilityCourtId");

            migrationBuilder.CreateIndex(
                name: "IX_FacilitySchedules_FacilityId",
                table: "FacilitySchedules",
                column: "FacilityId");

            migrationBuilder.CreateIndex(
                name: "IX_RecentAcademySearches_UserId",
                table: "RecentAcademySearches",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_RecentAcademySearches_UserId_SearchedAt",
                table: "RecentAcademySearches",
                columns: new[] { "UserId", "SearchedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_SavedAcademySearches_UserId",
                table: "SavedAcademySearches",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_SavedAcademySearches_UserId_SearchName",
                table: "SavedAcademySearches",
                columns: new[] { "UserId", "SearchName" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SessionSchedules_SessionId",
                table: "SessionSchedules",
                column: "SessionId");

            migrationBuilder.CreateIndex(
                name: "IX_SessionSchedules_SessionId_DayOfWeek",
                table: "SessionSchedules",
                columns: new[] { "SessionId", "DayOfWeek" });

            migrationBuilder.CreateIndex(
                name: "IX_TrainingAssessments_AssessmentType",
                table: "TrainingAssessments",
                column: "AssessmentType");

            migrationBuilder.CreateIndex(
                name: "IX_TrainingAssessments_SessionId",
                table: "TrainingAssessments",
                column: "SessionId");

            migrationBuilder.CreateIndex(
                name: "IX_TrainingAssessments_SessionId_Type",
                table: "TrainingAssessments",
                columns: new[] { "SessionId", "AssessmentType" });

            migrationBuilder.CreateIndex(
                name: "IX_TrainingBatches_BatchCode",
                table: "TrainingBatches",
                column: "BatchCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TrainingBatches_BranchId",
                table: "TrainingBatches",
                column: "BranchId");

            migrationBuilder.CreateIndex(
                name: "IX_TrainingBatches_CoachId",
                table: "TrainingBatches",
                column: "CoachId");

            migrationBuilder.CreateIndex(
                name: "IX_TrainingBatches_CoachId_StartDate",
                table: "TrainingBatches",
                columns: new[] { "CoachId", "StartDate" });

            migrationBuilder.CreateIndex(
                name: "IX_TrainingBatches_ProgramId",
                table: "TrainingBatches",
                column: "ProgramId");

            migrationBuilder.CreateIndex(
                name: "IX_TrainingBatches_ProgramId_Status",
                table: "TrainingBatches",
                columns: new[] { "ProgramId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_TrainingBatches_Status",
                table: "TrainingBatches",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_TrainingCertificates_CertificateNumber",
                table: "TrainingCertificates",
                column: "CertificateNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TrainingCertificates_CertificateType",
                table: "TrainingCertificates",
                column: "CertificateType");

            migrationBuilder.CreateIndex(
                name: "IX_TrainingCertificates_EnrollmentId",
                table: "TrainingCertificates",
                column: "EnrollmentId");

            migrationBuilder.CreateIndex(
                name: "IX_TrainingEnrollments_AthleteId",
                table: "TrainingEnrollments",
                column: "AthleteId");

            migrationBuilder.CreateIndex(
                name: "IX_TrainingEnrollments_AthleteId_Status",
                table: "TrainingEnrollments",
                columns: new[] { "AthleteId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_TrainingEnrollments_BatchId",
                table: "TrainingEnrollments",
                column: "BatchId");

            migrationBuilder.CreateIndex(
                name: "IX_TrainingEnrollments_BatchId_AthleteId",
                table: "TrainingEnrollments",
                columns: new[] { "BatchId", "AthleteId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TrainingEnrollments_Status",
                table: "TrainingEnrollments",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_TrainingGoals_ProgramId",
                table: "TrainingGoals",
                column: "ProgramId");

            migrationBuilder.CreateIndex(
                name: "IX_TrainingGoals_ProgramId_TargetWeek",
                table: "TrainingGoals",
                columns: new[] { "ProgramId", "TargetWeek" });

            migrationBuilder.CreateIndex(
                name: "IX_TrainingMaterials_MaterialType",
                table: "TrainingMaterials",
                column: "MaterialType");

            migrationBuilder.CreateIndex(
                name: "IX_TrainingMaterials_ProgramId",
                table: "TrainingMaterials",
                column: "ProgramId");

            migrationBuilder.CreateIndex(
                name: "IX_TrainingMaterials_ProgramId_SortOrder",
                table: "TrainingMaterials",
                columns: new[] { "ProgramId", "SortOrder" });

            migrationBuilder.CreateIndex(
                name: "IX_TrainingMaterials_SessionId",
                table: "TrainingMaterials",
                column: "SessionId");

            migrationBuilder.CreateIndex(
                name: "IX_TrainingMilestones_ProgramId",
                table: "TrainingMilestones",
                column: "ProgramId");

            migrationBuilder.CreateIndex(
                name: "IX_TrainingMilestones_ProgramId_WeekNumber",
                table: "TrainingMilestones",
                columns: new[] { "ProgramId", "WeekNumber" });

            migrationBuilder.CreateIndex(
                name: "IX_TrainingPrograms_AcademyId",
                table: "TrainingPrograms",
                column: "AcademyId");

            migrationBuilder.CreateIndex(
                name: "IX_TrainingPrograms_AcademyId_Status",
                table: "TrainingPrograms",
                columns: new[] { "AcademyId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_TrainingPrograms_DifficultyLevel",
                table: "TrainingPrograms",
                column: "DifficultyLevel");

            migrationBuilder.CreateIndex(
                name: "IX_TrainingPrograms_ProgramCode",
                table: "TrainingPrograms",
                column: "ProgramCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TrainingPrograms_ProgramName",
                table: "TrainingPrograms",
                column: "ProgramName");

            migrationBuilder.CreateIndex(
                name: "IX_TrainingPrograms_SportId",
                table: "TrainingPrograms",
                column: "SportId");

            migrationBuilder.CreateIndex(
                name: "IX_TrainingPrograms_SportId_Status",
                table: "TrainingPrograms",
                columns: new[] { "SportId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_TrainingPrograms_Status",
                table: "TrainingPrograms",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_TrainingProgramSports_ProgramId_SportId",
                table: "TrainingProgramSports",
                columns: new[] { "TrainingProgramId", "SportId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TrainingProgramSports_SportId",
                table: "TrainingProgramSports",
                column: "SportId");

            migrationBuilder.CreateIndex(
                name: "IX_TrainingProgramSports_TrainingProgramId",
                table: "TrainingProgramSports",
                column: "TrainingProgramId");

            migrationBuilder.CreateIndex(
                name: "IX_TrainingProgresses_EnrollmentId",
                table: "TrainingProgresses",
                column: "EnrollmentId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TrainingSessions_BatchId",
                table: "TrainingSessions",
                column: "BatchId");

            migrationBuilder.CreateIndex(
                name: "IX_TrainingSessions_BatchId_SessionDate",
                table: "TrainingSessions",
                columns: new[] { "BatchId", "SessionDate" });

            migrationBuilder.CreateIndex(
                name: "IX_TrainingSessions_CoachId",
                table: "TrainingSessions",
                column: "CoachId");

            migrationBuilder.CreateIndex(
                name: "IX_TrainingSessions_CoachId_SessionDate",
                table: "TrainingSessions",
                columns: new[] { "CoachId", "SessionDate" });

            migrationBuilder.CreateIndex(
                name: "IX_TrainingSessions_FacilityId",
                table: "TrainingSessions",
                column: "FacilityId");

            migrationBuilder.CreateIndex(
                name: "IX_TrainingSessions_SessionCode",
                table: "TrainingSessions",
                column: "SessionCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TrainingSessions_SessionDate",
                table: "TrainingSessions",
                column: "SessionDate");

            migrationBuilder.CreateIndex(
                name: "IX_TrainingSessions_SessionType",
                table: "TrainingSessions",
                column: "SessionType");

            migrationBuilder.CreateIndex(
                name: "IX_TrainingSessions_Status",
                table: "TrainingSessions",
                column: "Status");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AcademyViews");

            migrationBuilder.DropTable(
                name: "AssessmentResults");

            migrationBuilder.DropTable(
                name: "AthleteAcademies");

            migrationBuilder.DropTable(
                name: "Attendances");

            migrationBuilder.DropTable(
                name: "CoachAcademies");

            migrationBuilder.DropTable(
                name: "EquipmentMaintenance");

            migrationBuilder.DropTable(
                name: "FacilityAmenities");

            migrationBuilder.DropTable(
                name: "FacilityImages");

            migrationBuilder.DropTable(
                name: "FacilityPricing");

            migrationBuilder.DropTable(
                name: "FacilityReviews");

            migrationBuilder.DropTable(
                name: "FacilitySchedules");

            migrationBuilder.DropTable(
                name: "RecentAcademySearches");

            migrationBuilder.DropTable(
                name: "SavedAcademySearches");

            migrationBuilder.DropTable(
                name: "SessionSchedules");

            migrationBuilder.DropTable(
                name: "TrainingCertificates");

            migrationBuilder.DropTable(
                name: "TrainingGoals");

            migrationBuilder.DropTable(
                name: "TrainingMaterials");

            migrationBuilder.DropTable(
                name: "TrainingMilestones");

            migrationBuilder.DropTable(
                name: "TrainingProgramSports");

            migrationBuilder.DropTable(
                name: "TrainingProgresses");

            migrationBuilder.DropTable(
                name: "TrainingAssessments");

            migrationBuilder.DropTable(
                name: "FacilityEquipment");

            migrationBuilder.DropTable(
                name: "FacilityCourts");

            migrationBuilder.DropTable(
                name: "TrainingEnrollments");

            migrationBuilder.DropTable(
                name: "TrainingSessions");

            migrationBuilder.DropTable(
                name: "FacilityAreas");

            migrationBuilder.DropTable(
                name: "TrainingBatches");

            migrationBuilder.DropTable(
                name: "Facilities");

            migrationBuilder.DropTable(
                name: "TrainingPrograms");
        }
    }
}
