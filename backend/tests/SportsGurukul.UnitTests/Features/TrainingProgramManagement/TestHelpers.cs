using SportsGurukul.Domain.Entities;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.UnitTests.Features.TrainingProgramManagement;

internal static class TestHelpers
{
    public static TrainingProgram CreateTestProgram(
        Guid? id = null,
        Guid? academyId = null,
        Guid? sportId = null,
        TrainingProgramStatus status = TrainingProgramStatus.Draft,
        string programName = "Test Program",
        string programCode = "TPR-20260101-ABC123") => new()
    {
        Id = id ?? Guid.NewGuid(),
        ProgramCode = programCode,
        ProgramName = programName,
        AcademyId = academyId ?? Guid.NewGuid(),
        SportId = sportId ?? Guid.NewGuid(),
        Description = "Test description",
        DifficultyLevel = DifficultyLevel.Beginner,
        MinimumAge = 8,
        MaximumAge = 18,
        DurationWeeks = 12,
        Capacity = 30,
        Status = status,
        CreatedAt = DateTime.UtcNow,
        Academy = new Academy { Id = academyId ?? Guid.NewGuid(), Name = "Test Academy" },
        Sport = new Sport { Id = sportId ?? Guid.NewGuid(), Name = "Cricket" },
        Batches = new List<TrainingBatch>(),
        Goals = new List<TrainingGoal>(),
        Milestones = new List<TrainingMilestone>()
    };

    public static TrainingBatch CreateTestBatch(
        Guid? id = null,
        Guid? programId = null,
        Guid? coachId = null,
        Guid? branchId = null,
        BatchStatus status = BatchStatus.Waitlisted,
        int maximumSeats = 30) => new()
    {
        Id = id ?? Guid.NewGuid(),
        ProgramId = programId ?? Guid.NewGuid(),
        CoachId = coachId ?? Guid.NewGuid(),
        BranchId = branchId ?? Guid.NewGuid(),
        BatchCode = "BAT-20260101-123456",
        StartDate = DateTime.UtcNow.AddDays(7),
        EndDate = DateTime.UtcNow.AddDays(90),
        MaximumSeats = maximumSeats,
        Status = status,
        CreatedAt = DateTime.UtcNow,
        Program = new TrainingProgram { Id = programId ?? Guid.NewGuid(), ProgramName = "Test Program" },
        Coach = new Coach { Id = coachId ?? Guid.NewGuid(), User = new User { FullName = "Coach Name" } },
        Branch = new AcademyBranch { Id = branchId ?? Guid.NewGuid(), BranchName = "Main Branch" },
        Enrollments = new List<TrainingEnrollment>(),
        Sessions = new List<TrainingSession>()
    };

    public static TrainingSession CreateTestSession(
        Guid? id = null,
        Guid? batchId = null,
        Guid? coachId = null,
        SessionStatus status = SessionStatus.Scheduled) => new()
    {
        Id = id ?? Guid.NewGuid(),
        BatchId = batchId ?? Guid.NewGuid(),
        SessionCode = "SES-20260101-123456",
        SessionTitle = "Test Session",
        SessionType = SessionType.Practice,
        SessionDate = DateTime.UtcNow.AddDays(1),
        StartTime = new TimeSpan(9, 0, 0),
        EndTime = new TimeSpan(11, 0, 0),
        CoachId = coachId ?? Guid.NewGuid(),
        Status = status,
        CreatedAt = DateTime.UtcNow,
        Coach = new Coach { Id = coachId ?? Guid.NewGuid(), User = new User { FullName = "Coach Name" } },
        Attendances = new List<Attendance>()
    };

    public static TrainingEnrollment CreateTestEnrollment(
        Guid? id = null,
        Guid? batchId = null,
        Guid? athleteId = null,
        EnrollmentStatus status = EnrollmentStatus.Active) => new()
    {
        Id = id ?? Guid.NewGuid(),
        BatchId = batchId ?? Guid.NewGuid(),
        AthleteId = athleteId ?? Guid.NewGuid(),
        EnrollmentDate = DateTime.UtcNow,
        Status = status,
        CreatedAt = DateTime.UtcNow,
        Athlete = new Athlete { Id = athleteId ?? Guid.NewGuid(), User = new User { FullName = "Athlete Name" }, AthleteCode = "ATH001" }
    };

    public static Attendance CreateTestAttendance(
        Guid? id = null,
        Guid? sessionId = null,
        Guid? athleteId = null,
        AttendanceStatus status = AttendanceStatus.Present) => new()
    {
        Id = id ?? Guid.NewGuid(),
        SessionId = sessionId ?? Guid.NewGuid(),
        AthleteId = athleteId ?? Guid.NewGuid(),
        AttendanceStatus = status,
        CreatedAt = DateTime.UtcNow
    };

    public static TrainingAssessment CreateTestAssessment(
        Guid? id = null,
        Guid? sessionId = null) => new()
    {
        Id = id ?? Guid.NewGuid(),
        SessionId = sessionId ?? Guid.NewGuid(),
        AssessmentType = AssessmentType.SkillTest,
        AssessmentName = "Mid-term Assessment",
        MaximumScore = 100,
        PassingScore = 50,
        CreatedAt = DateTime.UtcNow,
        Results = new List<AssessmentResult>()
    };

    public static TrainingProgress CreateTestProgress(
        Guid? id = null,
        Guid? enrollmentId = null) => new()
    {
        Id = id ?? Guid.NewGuid(),
        EnrollmentId = enrollmentId ?? Guid.NewGuid(),
        CurrentLevel = "Intermediate",
        CompletedPercentage = 50,
        CreatedAt = DateTime.UtcNow
    };

    public static TrainingMilestone CreateTestMilestone(
        Guid? id = null,
        bool isCompleted = false) => new()
    {
        Id = id ?? Guid.NewGuid(),
        MilestoneName = "Week 4 Milestone",
        Description = "Complete drills",
        WeekNumber = 4,
        IsCompleted = isCompleted
    };

    public static TrainingCertificate CreateTestCertificate(
        Guid? enrollmentId = null) => new()
    {
        Id = Guid.NewGuid(),
        EnrollmentId = enrollmentId ?? Guid.NewGuid(),
        CertificateType = CertificateType.Completion,
        CertificateNumber = $"CERT-{DateTime.UtcNow:yyyyMMdd}-123456",
        IssuedDate = DateTime.UtcNow,
        CreatedAt = DateTime.UtcNow
    };

    public static Coach CreateTestCoach(Guid? id = null) => new()
    {
        Id = id ?? Guid.NewGuid(),
        User = new User { FullName = "Test Coach" },
        CoachCode = "COA001"
    };

    public static Athlete CreateTestAthlete(Guid? id = null) => new()
    {
        Id = id ?? Guid.NewGuid(),
        User = new User { FullName = "Test Athlete" },
        AthleteCode = "ATH001"
    };

    public static Academy CreateTestAcademy(Guid? id = null) => new()
    {
        Id = id ?? Guid.NewGuid(),
        Name = "Test Academy"
    };

    public static Sport CreateTestSport(Guid? id = null) => new()
    {
        Id = id ?? Guid.NewGuid(),
        Name = "Cricket"
    };

    public static AcademyBranch CreateTestBranch(Guid? id = null) => new()
    {
        Id = id ?? Guid.NewGuid(),
        BranchName = "Main Branch"
    };

    public static Facility CreateTestFacility(Guid? id = null) => new()
    {
        Id = id ?? Guid.NewGuid(),
        FacilityName = "Indoor Court"
    };
}
