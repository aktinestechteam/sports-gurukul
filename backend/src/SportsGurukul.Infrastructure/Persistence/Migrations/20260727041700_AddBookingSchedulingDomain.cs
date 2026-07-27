using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SportsGurukul.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddBookingSchedulingDomain : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Bookings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    BookingNumber = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    BookingType = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    Status = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    Title = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    AcademyId = table.Column<Guid>(type: "uuid", nullable: false),
                    BranchId = table.Column<Guid>(type: "uuid", nullable: true),
                    FacilityId = table.Column<Guid>(type: "uuid", nullable: true),
                    CoachId = table.Column<Guid>(type: "uuid", nullable: true),
                    AthleteId = table.Column<Guid>(type: "uuid", nullable: true),
                    TrainingSessionId = table.Column<Guid>(type: "uuid", nullable: true),
                    TournamentId = table.Column<Guid>(type: "uuid", nullable: true),
                    EventId = table.Column<Guid>(type: "uuid", nullable: true),
                    BookingDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    StartTime = table.Column<TimeSpan>(type: "interval", nullable: false),
                    EndTime = table.Column<TimeSpan>(type: "interval", nullable: false),
                    Duration = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    ApprovalStatus = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    BookingCreatorId = table.Column<Guid>(type: "uuid", nullable: true),
                    RowVersion = table.Column<byte[]>(type: "bytea", rowVersion: true, nullable: false, defaultValueSql: "E'\\\\x00'::bytea"),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Bookings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Bookings_Academies_AcademyId",
                        column: x => x.AcademyId,
                        principalTable: "Academies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Bookings_AcademyBranches_BranchId",
                        column: x => x.BranchId,
                        principalTable: "AcademyBranches",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Bookings_Athletes_AthleteId",
                        column: x => x.AthleteId,
                        principalTable: "Athletes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Bookings_Coaches_CoachId",
                        column: x => x.CoachId,
                        principalTable: "Coaches",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Bookings_Facilities_FacilityId",
                        column: x => x.FacilityId,
                        principalTable: "Facilities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Bookings_TrainingSessions_TrainingSessionId",
                        column: x => x.TrainingSessionId,
                        principalTable: "TrainingSessions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "BookingApprovals",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    BookingId = table.Column<Guid>(type: "uuid", nullable: false),
                    ApprovalStatus = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    ApproverUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    ReviewedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Comments = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    EscalationLevel = table.Column<int>(type: "integer", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "bytea", rowVersion: true, nullable: false, defaultValueSql: "E'\\\\x00'::bytea"),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BookingApprovals", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BookingApprovals_Bookings_BookingId",
                        column: x => x.BookingId,
                        principalTable: "Bookings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BookingAttachments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    BookingId = table.Column<Guid>(type: "uuid", nullable: false),
                    FileName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    FileType = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    FileSize = table.Column<long>(type: "bigint", nullable: false),
                    FileUrl = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false),
                    Description = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    RowVersion = table.Column<byte[]>(type: "bytea", rowVersion: true, nullable: false, defaultValueSql: "E'\\\\x00'::bytea"),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BookingAttachments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BookingAttachments_Bookings_BookingId",
                        column: x => x.BookingId,
                        principalTable: "Bookings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BookingCancellations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    BookingId = table.Column<Guid>(type: "uuid", nullable: false),
                    CancelledByUserId = table.Column<Guid>(type: "uuid", nullable: false),
                    CancelledOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Reason = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    RefundAmount = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: true),
                    IsRefundProcessed = table.Column<bool>(type: "boolean", nullable: false),
                    Notes = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    RowVersion = table.Column<byte[]>(type: "bytea", rowVersion: true, nullable: false, defaultValueSql: "E'\\\\x00'::bytea"),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BookingCancellations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BookingCancellations_Bookings_BookingId",
                        column: x => x.BookingId,
                        principalTable: "Bookings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BookingConflicts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    BookingId = table.Column<Guid>(type: "uuid", nullable: false),
                    ConflictingBookingId = table.Column<Guid>(type: "uuid", nullable: false),
                    ConflictType = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    Description = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    IsResolved = table.Column<bool>(type: "boolean", nullable: false),
                    ResolutionNotes = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    ResolvedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    RowVersion = table.Column<byte[]>(type: "bytea", rowVersion: true, nullable: false, defaultValueSql: "E'\\\\x00'::bytea"),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BookingConflicts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BookingConflicts_Bookings_BookingId",
                        column: x => x.BookingId,
                        principalTable: "Bookings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BookingHistories",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    BookingId = table.Column<Guid>(type: "uuid", nullable: false),
                    Action = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    PreviousValue = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    NewValue = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    PerformedBy = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    PerformedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Notes = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    RowVersion = table.Column<byte[]>(type: "bytea", rowVersion: true, nullable: false, defaultValueSql: "E'\\\\x00'::bytea"),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BookingHistories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BookingHistories_Bookings_BookingId",
                        column: x => x.BookingId,
                        principalTable: "Bookings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BookingItems",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    BookingId = table.Column<Guid>(type: "uuid", nullable: false),
                    ItemName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    ItemDescription = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    Quantity = table.Column<int>(type: "integer", nullable: false),
                    Unit = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    UnitPrice = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: true),
                    TotalPrice = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: true),
                    Notes = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    RowVersion = table.Column<byte[]>(type: "bytea", rowVersion: true, nullable: false, defaultValueSql: "E'\\\\x00'::bytea"),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BookingItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BookingItems_Bookings_BookingId",
                        column: x => x.BookingId,
                        principalTable: "Bookings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BookingParticipants",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    BookingId = table.Column<Guid>(type: "uuid", nullable: false),
                    ParticipantId = table.Column<Guid>(type: "uuid", nullable: false),
                    ParticipantName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    Role = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    Confirmed = table.Column<bool>(type: "boolean", nullable: false),
                    Attended = table.Column<bool>(type: "boolean", nullable: false),
                    Notes = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    RowVersion = table.Column<byte[]>(type: "bytea", rowVersion: true, nullable: false, defaultValueSql: "E'\\\\x00'::bytea"),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BookingParticipants", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BookingParticipants_Bookings_BookingId",
                        column: x => x.BookingId,
                        principalTable: "Bookings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BookingRecurrences",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    BookingId = table.Column<Guid>(type: "uuid", nullable: false),
                    RecurrenceType = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    RRule = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    EndDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    OccurrenceCount = table.Column<int>(type: "integer", nullable: true),
                    Exceptions = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "bytea", rowVersion: true, nullable: false, defaultValueSql: "E'\\\\x00'::bytea"),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BookingRecurrences", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BookingRecurrences_Bookings_BookingId",
                        column: x => x.BookingId,
                        principalTable: "Bookings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BookingReminders",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    BookingId = table.Column<Guid>(type: "uuid", nullable: false),
                    ReminderMinutesBefore = table.Column<int>(type: "integer", nullable: false),
                    ScheduledAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsSent = table.Column<bool>(type: "boolean", nullable: false),
                    SentAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Channel = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    Notes = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    RowVersion = table.Column<byte[]>(type: "bytea", rowVersion: true, nullable: false, defaultValueSql: "E'\\\\x00'::bytea"),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BookingReminders", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BookingReminders_Bookings_BookingId",
                        column: x => x.BookingId,
                        principalTable: "Bookings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BookingReschedules",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    BookingId = table.Column<Guid>(type: "uuid", nullable: false),
                    RequestedById = table.Column<Guid>(type: "uuid", nullable: false),
                    OriginalDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    OriginalStartTime = table.Column<TimeSpan>(type: "interval", nullable: false),
                    OriginalEndTime = table.Column<TimeSpan>(type: "interval", nullable: false),
                    NewDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    NewStartTime = table.Column<TimeSpan>(type: "interval", nullable: false),
                    NewEndTime = table.Column<TimeSpan>(type: "interval", nullable: false),
                    Reason = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    IsApproved = table.Column<bool>(type: "boolean", nullable: false),
                    ApprovedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Notes = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    RowVersion = table.Column<byte[]>(type: "bytea", rowVersion: true, nullable: false, defaultValueSql: "E'\\\\x00'::bytea"),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BookingReschedules", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BookingReschedules_Bookings_BookingId",
                        column: x => x.BookingId,
                        principalTable: "Bookings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BookingSchedules",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    BookingId = table.Column<Guid>(type: "uuid", nullable: false),
                    ScheduledDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    StartTime = table.Column<TimeSpan>(type: "interval", nullable: false),
                    EndTime = table.Column<TimeSpan>(type: "interval", nullable: false),
                    IsCancelled = table.Column<bool>(type: "boolean", nullable: false),
                    CancellationReason = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    Notes = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    RowVersion = table.Column<byte[]>(type: "bytea", rowVersion: true, nullable: false, defaultValueSql: "E'\\\\x00'::bytea"),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BookingSchedules", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BookingSchedules_Bookings_BookingId",
                        column: x => x.BookingId,
                        principalTable: "Bookings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BookingWaitlists",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    BookingId = table.Column<Guid>(type: "uuid", nullable: false),
                    WaitlistUserId = table.Column<Guid>(type: "uuid", nullable: false),
                    Priority = table.Column<int>(type: "integer", nullable: false),
                    RequestedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    PromotionOrder = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    Notes = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    RowVersion = table.Column<byte[]>(type: "bytea", rowVersion: true, nullable: false, defaultValueSql: "E'\\\\x00'::bytea"),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BookingWaitlists", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BookingWaitlists_Bookings_BookingId",
                        column: x => x.BookingId,
                        principalTable: "Bookings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Bookings",
                columns: new[] { "Id", "AcademyId", "ApprovalStatus", "AthleteId", "BookingCreatorId", "BookingDate", "BookingNumber", "BookingType", "BranchId", "CoachId", "CreatedAt", "Description", "Duration", "EndTime", "EventId", "FacilityId", "IsDeleted", "StartTime", "Status", "Title", "TournamentId", "TrainingSessionId", "UpdatedAt" },
                values: new object[] { new Guid("b1000000-0000-0000-0000-000000000001"), new Guid("a1000000-0000-0000-0000-000000000001"), "Approved", null, null, new DateTime(2026, 8, 1, 0, 0, 0, 0, DateTimeKind.Utc), "BK-20260727-SEED01", "TrainingSession", null, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Seed booking for development and testing.", 60, new TimeSpan(0, 10, 0, 0, 0), null, null, false, new TimeSpan(0, 9, 0, 0, 0), "Confirmed", "Seed Training Booking", null, null, null });

            migrationBuilder.InsertData(
                table: "BookingApprovals",
                columns: new[] { "Id", "ApprovalStatus", "ApproverUserId", "BookingId", "Comments", "CreatedAt", "EscalationLevel", "IsDeleted", "ReviewedOn", "UpdatedAt" },
                values: new object[] { new Guid("b6000000-0000-0000-0000-000000000001"), "Approved", null, new Guid("b1000000-0000-0000-0000-000000000001"), null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 0, false, null, null });

            migrationBuilder.InsertData(
                table: "BookingRecurrences",
                columns: new[] { "Id", "BookingId", "CreatedAt", "EndDate", "Exceptions", "IsActive", "IsDeleted", "OccurrenceCount", "RRule", "RecurrenceType", "UpdatedAt" },
                values: new object[] { new Guid("b5000000-0000-0000-0000-000000000001"), new Guid("b1000000-0000-0000-0000-000000000001"), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 12, 31, 0, 0, 0, 0, DateTimeKind.Utc), null, true, false, 24, null, "Weekly", null });

            migrationBuilder.CreateIndex(
                name: "IX_BookingApprovals_ApprovalStatus",
                table: "BookingApprovals",
                column: "ApprovalStatus");

            migrationBuilder.CreateIndex(
                name: "IX_BookingApprovals_ApproverUserId",
                table: "BookingApprovals",
                column: "ApproverUserId");

            migrationBuilder.CreateIndex(
                name: "IX_BookingApprovals_BookingId",
                table: "BookingApprovals",
                column: "BookingId");

            migrationBuilder.CreateIndex(
                name: "IX_BookingApprovals_BookingId_Status",
                table: "BookingApprovals",
                columns: new[] { "BookingId", "ApprovalStatus" });

            migrationBuilder.CreateIndex(
                name: "IX_BookingApprovals_EscalationLevel",
                table: "BookingApprovals",
                column: "EscalationLevel");

            migrationBuilder.CreateIndex(
                name: "IX_BookingAttachments_BookingId",
                table: "BookingAttachments",
                column: "BookingId");

            migrationBuilder.CreateIndex(
                name: "IX_BookingAttachments_BookingId_FileName",
                table: "BookingAttachments",
                columns: new[] { "BookingId", "FileName" });

            migrationBuilder.CreateIndex(
                name: "IX_BookingAttachments_FileName",
                table: "BookingAttachments",
                column: "FileName");

            migrationBuilder.CreateIndex(
                name: "IX_BookingCancellations_BookingId",
                table: "BookingCancellations",
                column: "BookingId");

            migrationBuilder.CreateIndex(
                name: "IX_BookingCancellations_BookingId_CancelledOn",
                table: "BookingCancellations",
                columns: new[] { "BookingId", "CancelledOn" });

            migrationBuilder.CreateIndex(
                name: "IX_BookingCancellations_CancelledByUserId",
                table: "BookingCancellations",
                column: "CancelledByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_BookingCancellations_CancelledOn",
                table: "BookingCancellations",
                column: "CancelledOn");

            migrationBuilder.CreateIndex(
                name: "IX_BookingConflicts_BookingId",
                table: "BookingConflicts",
                column: "BookingId");

            migrationBuilder.CreateIndex(
                name: "IX_BookingConflicts_BookingId_IsResolved",
                table: "BookingConflicts",
                columns: new[] { "BookingId", "IsResolved" });

            migrationBuilder.CreateIndex(
                name: "IX_BookingConflicts_ConflictingBookingId",
                table: "BookingConflicts",
                column: "ConflictingBookingId");

            migrationBuilder.CreateIndex(
                name: "IX_BookingConflicts_ConflictType",
                table: "BookingConflicts",
                column: "ConflictType");

            migrationBuilder.CreateIndex(
                name: "IX_BookingConflicts_IsResolved",
                table: "BookingConflicts",
                column: "IsResolved");

            migrationBuilder.CreateIndex(
                name: "IX_BookingConflicts_Type_IsResolved",
                table: "BookingConflicts",
                columns: new[] { "ConflictType", "IsResolved" });

            migrationBuilder.CreateIndex(
                name: "IX_BookingHistories_Action",
                table: "BookingHistories",
                column: "Action");

            migrationBuilder.CreateIndex(
                name: "IX_BookingHistories_BookingId",
                table: "BookingHistories",
                column: "BookingId");

            migrationBuilder.CreateIndex(
                name: "IX_BookingHistories_BookingId_PerformedOn",
                table: "BookingHistories",
                columns: new[] { "BookingId", "PerformedOn" });

            migrationBuilder.CreateIndex(
                name: "IX_BookingHistories_PerformedOn",
                table: "BookingHistories",
                column: "PerformedOn");

            migrationBuilder.CreateIndex(
                name: "IX_BookingItems_BookingId",
                table: "BookingItems",
                column: "BookingId");

            migrationBuilder.CreateIndex(
                name: "IX_BookingItems_BookingId_ItemName",
                table: "BookingItems",
                columns: new[] { "BookingId", "ItemName" });

            migrationBuilder.CreateIndex(
                name: "IX_BookingParticipants_BookingId",
                table: "BookingParticipants",
                column: "BookingId");

            migrationBuilder.CreateIndex(
                name: "IX_BookingParticipants_BookingId_ParticipantId",
                table: "BookingParticipants",
                columns: new[] { "BookingId", "ParticipantId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_BookingParticipants_ParticipantId",
                table: "BookingParticipants",
                column: "ParticipantId");

            migrationBuilder.CreateIndex(
                name: "IX_BookingRecurrences_BookingId",
                table: "BookingRecurrences",
                column: "BookingId");

            migrationBuilder.CreateIndex(
                name: "IX_BookingRecurrences_BookingId_IsActive",
                table: "BookingRecurrences",
                columns: new[] { "BookingId", "IsActive" });

            migrationBuilder.CreateIndex(
                name: "IX_BookingRecurrences_IsActive",
                table: "BookingRecurrences",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_BookingRecurrences_RecurrenceType",
                table: "BookingRecurrences",
                column: "RecurrenceType");

            migrationBuilder.CreateIndex(
                name: "IX_BookingReminders_BookingId",
                table: "BookingReminders",
                column: "BookingId");

            migrationBuilder.CreateIndex(
                name: "IX_BookingReminders_BookingId_IsSent",
                table: "BookingReminders",
                columns: new[] { "BookingId", "IsSent" });

            migrationBuilder.CreateIndex(
                name: "IX_BookingReminders_IsSent",
                table: "BookingReminders",
                column: "IsSent");

            migrationBuilder.CreateIndex(
                name: "IX_BookingReminders_ScheduledAt",
                table: "BookingReminders",
                column: "ScheduledAt");

            migrationBuilder.CreateIndex(
                name: "IX_BookingReschedules_BookingId",
                table: "BookingReschedules",
                column: "BookingId");

            migrationBuilder.CreateIndex(
                name: "IX_BookingReschedules_BookingId_IsApproved",
                table: "BookingReschedules",
                columns: new[] { "BookingId", "IsApproved" });

            migrationBuilder.CreateIndex(
                name: "IX_BookingReschedules_RequestedById",
                table: "BookingReschedules",
                column: "RequestedById");

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_AcademyId",
                table: "Bookings",
                column: "AcademyId");

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_AcademyId_BookingDate",
                table: "Bookings",
                columns: new[] { "AcademyId", "BookingDate" });

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_ApprovalStatus",
                table: "Bookings",
                column: "ApprovalStatus");

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_AthleteId",
                table: "Bookings",
                column: "AthleteId");

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_AthleteId_BookingDate",
                table: "Bookings",
                columns: new[] { "AthleteId", "BookingDate" });

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_BookingDate",
                table: "Bookings",
                column: "BookingDate");

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_BookingNumber",
                table: "Bookings",
                column: "BookingNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_BookingType",
                table: "Bookings",
                column: "BookingType");

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_BookingType_Status",
                table: "Bookings",
                columns: new[] { "BookingType", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_BranchId",
                table: "Bookings",
                column: "BranchId");

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_CoachId",
                table: "Bookings",
                column: "CoachId");

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_CoachId_BookingDate",
                table: "Bookings",
                columns: new[] { "CoachId", "BookingDate" });

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_FacilityId",
                table: "Bookings",
                column: "FacilityId");

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_FacilityId_BookingDate",
                table: "Bookings",
                columns: new[] { "FacilityId", "BookingDate" });

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_Status",
                table: "Bookings",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_Status_BookingDate",
                table: "Bookings",
                columns: new[] { "Status", "BookingDate" });

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_TrainingSessionId",
                table: "Bookings",
                column: "TrainingSessionId");

            migrationBuilder.CreateIndex(
                name: "IX_BookingSchedules_BookingId",
                table: "BookingSchedules",
                column: "BookingId");

            migrationBuilder.CreateIndex(
                name: "IX_BookingSchedules_BookingId_ScheduledDate",
                table: "BookingSchedules",
                columns: new[] { "BookingId", "ScheduledDate" });

            migrationBuilder.CreateIndex(
                name: "IX_BookingSchedules_ScheduledDate",
                table: "BookingSchedules",
                column: "ScheduledDate");

            migrationBuilder.CreateIndex(
                name: "IX_BookingWaitlists_BookingId",
                table: "BookingWaitlists",
                column: "BookingId");

            migrationBuilder.CreateIndex(
                name: "IX_BookingWaitlists_BookingId_Priority",
                table: "BookingWaitlists",
                columns: new[] { "BookingId", "Priority" });

            migrationBuilder.CreateIndex(
                name: "IX_BookingWaitlists_BookingId_UserId",
                table: "BookingWaitlists",
                columns: new[] { "BookingId", "WaitlistUserId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_BookingWaitlists_Priority",
                table: "BookingWaitlists",
                column: "Priority");

            migrationBuilder.CreateIndex(
                name: "IX_BookingWaitlists_Status",
                table: "BookingWaitlists",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_BookingWaitlists_WaitlistUserId",
                table: "BookingWaitlists",
                column: "WaitlistUserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BookingApprovals");

            migrationBuilder.DropTable(
                name: "BookingAttachments");

            migrationBuilder.DropTable(
                name: "BookingCancellations");

            migrationBuilder.DropTable(
                name: "BookingConflicts");

            migrationBuilder.DropTable(
                name: "BookingHistories");

            migrationBuilder.DropTable(
                name: "BookingItems");

            migrationBuilder.DropTable(
                name: "BookingParticipants");

            migrationBuilder.DropTable(
                name: "BookingRecurrences");

            migrationBuilder.DropTable(
                name: "BookingReminders");

            migrationBuilder.DropTable(
                name: "BookingReschedules");

            migrationBuilder.DropTable(
                name: "BookingSchedules");

            migrationBuilder.DropTable(
                name: "BookingWaitlists");

            migrationBuilder.DropTable(
                name: "Bookings");
        }
    }
}
