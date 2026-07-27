using SportsGurukul.Api.Common.Models;
using SportsGurukul.Api.Controllers.V1;
using SportsGurukul.Application.Features.TrainingProgramManagement.Commands.Assessment.CreateAssessment;
using SportsGurukul.Application.Features.TrainingProgramManagement.Commands.Assessment.SubmitAssessmentResult;
using SportsGurukul.Application.Features.TrainingProgramManagement.Commands.Attendance.MarkAttendance;
using SportsGurukul.Application.Features.TrainingProgramManagement.Commands.Attendance.CheckInAthlete;
using SportsGurukul.Application.Features.TrainingProgramManagement.Commands.Attendance.CheckOutAthlete;
using SportsGurukul.Application.Features.TrainingProgramManagement.Commands.Attendance.UpdateAttendance;
using SportsGurukul.Application.Features.TrainingProgramManagement.Commands.Enrollment.EnrollAthlete;
using SportsGurukul.Application.Features.TrainingProgramManagement.Commands.Enrollment.TransferEnrollment;
using SportsGurukul.Application.Features.TrainingProgramManagement.Commands.Progress.IssueCertificate;
using SportsGurukul.Application.Features.TrainingProgramManagement.Commands.TrainingProgram.CreateTrainingProgram;
using SportsGurukul.Application.Features.TrainingProgramManagement.DTOs;
using SportsGurukul.Domain.Enums;
using Swashbuckle.AspNetCore.Filters;

namespace SportsGurukul.Api.Common.Models.SwaggerExamples;

#region Program Request Examples

public class CreateTrainingProgramRequestExample : IExamplesProvider<CreateTrainingProgramCommand>
{
    public CreateTrainingProgramCommand GetExamples() => new()
    {
        ProgramName = "Junior Cricket Development Program",
        SportId = Guid.Parse("a1b2c3d4-e5f6-7890-abcd-ef1234567890"),
        AcademyId = Guid.Parse("b2c3d4e5-f6a7-8901-bcde-f12345678901"),
        Description = "A 12-week intensive cricket development program for junior players aged 10-16.",
        DifficultyLevel = DifficultyLevel.Intermediate,
        MinimumAge = 10,
        MaximumAge = 16,
        DurationWeeks = 12,
        Capacity = 30
    };
}

#endregion

#region Batch Request Examples

public class CreateBatchRequestExample : IExamplesProvider<TrainingBatchesController.CreateBatchRequest>
{
    public TrainingBatchesController.CreateBatchRequest GetExamples() => new(
        CoachId: Guid.Parse("c3d4e5f6-a7b8-9012-cdef-123456789012"),
        BranchId: Guid.Parse("d4e5f6a7-b8c9-0123-defa-234567890123"),
        StartDate: DateTime.UtcNow.AddDays(7),
        EndDate: DateTime.UtcNow.AddDays(91),
        MaximumSeats: 25
    );
}

#endregion

#region Session Request Examples

public class CreateSessionRequestExample : IExamplesProvider<TrainingSessionsController.CreateSessionRequest>
{
    public TrainingSessionsController.CreateSessionRequest GetExamples() => new(
        SessionTitle: "Batting Technique Workshop",
        SessionType: SessionType.Practice,
        SessionDate: DateTime.UtcNow.AddDays(7),
        StartTime: new TimeSpan(9, 0, 0),
        EndTime: new TimeSpan(11, 0, 0),
        FacilityId: Guid.Parse("e5f6a7b8-c9d0-1234-efab-345678901234"),
        CoachId: Guid.Parse("c3d4e5f6-a7b8-9012-cdef-123456789012")
    );
}

public class RescheduleSessionRequestExample : IExamplesProvider<TrainingSessionsController.RescheduleSessionRequest>
{
    public TrainingSessionsController.RescheduleSessionRequest GetExamples() => new(
        SessionDate: DateTime.UtcNow.AddDays(14),
        StartTime: new TimeSpan(10, 0, 0),
        EndTime: new TimeSpan(12, 0, 0)
    );
}

#endregion

#region Enrollment Request Examples

public class EnrollAthleteRequestExample : IExamplesProvider<EnrollmentsController.EnrollAthleteRequest>
{
    public EnrollmentsController.EnrollAthleteRequest GetExamples() => new(
        AthleteId: Guid.Parse("f6a7b8c9-d0e1-2345-fabc-456789012345")
    );
}

public class TransferEnrollmentRequestExample : IExamplesProvider<EnrollmentsController.TransferEnrollmentRequest>
{
    public EnrollmentsController.TransferEnrollmentRequest GetExamples() => new(
        SourceBatchId: Guid.Parse("11111111-1111-1111-1111-111111111111"),
        TargetBatchId: Guid.Parse("22222222-2222-2222-2222-222222222222")
    );
}

#endregion

#region Attendance Request Examples

public class MarkAttendanceRequestExample : IExamplesProvider<AttendanceController.MarkAttendanceRequest>
{
    public AttendanceController.MarkAttendanceRequest GetExamples() => new()
    {
        AthleteId = Guid.Parse("f6a7b8c9-d0e1-2345-fabc-456789012345"),
        Status = AttendanceStatus.Present,
        Remarks = "Athlete attended full session."
    };
}

public class CheckInRequestExample : IExamplesProvider<AttendanceController.CheckInRequest>
{
    public AttendanceController.CheckInRequest GetExamples() => new()
    {
        AthleteId = Guid.Parse("f6a7b8c9-d0e1-2345-fabc-456789012345")
    };
}

public class CheckOutRequestExample : IExamplesProvider<AttendanceController.CheckOutRequest>
{
    public AttendanceController.CheckOutRequest GetExamples() => new()
    {
        AthleteId = Guid.Parse("f6a7b8c9-d0e1-2345-fabc-456789012345")
    };
}

public class UpdateAttendanceRequestExample : IExamplesProvider<AttendanceController.UpdateAttendanceRequest>
{
    public AttendanceController.UpdateAttendanceRequest GetExamples() => new()
    {
        Status = AttendanceStatus.Present,
        Remarks = "Corrected from absent to present."
    };
}

#endregion

#region Assessment Request Examples

public class CreateAssessmentRequestExample : IExamplesProvider<AssessmentsController.CreateAssessmentRequest>
{
    public AssessmentsController.CreateAssessmentRequest GetExamples() => new()
    {
        AssessmentType = "Skill",
        AssessmentName = "Batting Skills Assessment",
        MaximumScore = 100,
        PassingScore = 60
    };
}

public class SubmitAssessmentResultRequestExample : IExamplesProvider<AssessmentsController.SubmitResultRequest>
{
    public AssessmentsController.SubmitResultRequest GetExamples() => new()
    {
        AthleteId = Guid.Parse("f6a7b8c9-d0e1-2345-fabc-456789012345"),
        Score = 85,
        Remarks = "Excellent batting technique demonstrated."
    };
}

#endregion

#region Progress Request Examples

public class UpdateProgressRequestExample : IExamplesProvider<ProgressController.UpdateProgressRequest>
{
    public ProgressController.UpdateProgressRequest GetExamples() => new()
    {
        CurrentLevel = "Intermediate",
        CompletedPercentage = 65.5m,
        OverallRating = 4.2m
    };
}

#endregion

#region Certificate Request Examples

public class IssueCertificateRequestExample : IExamplesProvider<CertificatesController.IssueCertificateRequest>
{
    public CertificatesController.IssueCertificateRequest GetExamples() => new()
    {
        CertificateType = "Completion",
        FileUrl = "https://cdn.sportsgurukul.com/certificates/cert-2025.pdf"
    };
}

#endregion

#region Response DTO Examples

public class TrainingProgramDtoExample : IExamplesProvider<TrainingProgramDto>
{
    public TrainingProgramDto GetExamples() => new()
    {
        Id = Guid.Parse("a1b2c3d4-e5f6-7890-abcd-ef1234567890"),
        ProgramCode = "TPR-20250701-X1Y2Z3",
        ProgramName = "Junior Cricket Development Program",
        SportId = Guid.Parse("11111111-1111-1111-1111-111111111111"),
        SportName = "Cricket",
        AcademyId = Guid.Parse("22222222-2222-2222-2222-222222222222"),
        AcademyName = "Mumbai Sports Academy",
        Description = "A 12-week intensive cricket development program.",
        DifficultyLevel = "Intermediate",
        MinimumAge = 10,
        MaximumAge = 16,
        DurationWeeks = 12,
        Capacity = 30,
        Status = "Draft",
        TotalBatches = 3,
        ActiveBatches = 1,
        Batches = [],
        Goals = [],
        Milestones = [],
        CreatedAt = DateTime.UtcNow.AddDays(-5),
        UpdatedAt = DateTime.UtcNow.AddDays(-1)
    };
}

public class TrainingBatchDtoExample : IExamplesProvider<TrainingBatchDto>
{
    public TrainingBatchDto GetExamples() => new()
    {
        Id = Guid.Parse("b2c3d4e5-f6a7-8901-bcde-f12345678901"),
        ProgramId = Guid.Parse("a1b2c3d4-e5f6-7890-abcd-ef1234567890"),
        BatchCode = "BAT-20250701-A1B2C3",
        CoachId = Guid.Parse("33333333-3333-3333-3333-333333333333"),
        CoachName = "Vikram Singh",
        BranchId = Guid.Parse("44444444-4444-4444-4444-444444444444"),
        BranchName = "Andheri Branch",
        StartDate = DateTime.UtcNow.AddDays(7),
        EndDate = DateTime.UtcNow.AddDays(91),
        MaximumSeats = 25,
        Status = "Active",
        SessionsCount = 12,
        EnrollmentCount = 18,
        ProgramName = "Junior Cricket Development Program",
        CreatedAt = DateTime.UtcNow.AddDays(-3),
        UpdatedAt = DateTime.UtcNow.AddDays(-1),
        Sessions = []
    };
}

public class TrainingSessionDtoExample : IExamplesProvider<TrainingSessionDto>
{
    public TrainingSessionDto GetExamples() => new()
    {
        Id = Guid.Parse("c3d4e5f6-a7b8-9012-cdef-123456789012"),
        BatchId = Guid.Parse("b2c3d4e5-f6a7-8901-bcde-f12345678901"),
        SessionCode = "SES-20250701-P1Q2R3",
        SessionTitle = "Batting Technique Workshop",
        SessionType = "Practice",
        SessionDate = DateTime.UtcNow.AddDays(7),
        StartTime = new TimeSpan(9, 0, 0),
        EndTime = new TimeSpan(11, 0, 0),
        FacilityId = Guid.Parse("55555555-5555-5555-5555-555555555555"),
        FacilityName = "Main Cricket Ground",
        CoachId = Guid.Parse("33333333-3333-3333-3333-333333333333"),
        CoachName = "Vikram Singh",
        Status = "Scheduled",
        AttendanceCount = 0,
        BatchCode = "BAT-20250701-A1B2C3",
        CreatedAt = DateTime.UtcNow.AddDays(-1)
    };
}

public class EnrollmentDtoExample : IExamplesProvider<EnrollmentDto>
{
    public EnrollmentDto GetExamples() => new()
    {
        Id = Guid.Parse("d4e5f6a7-b8c9-0123-defa-234567890123"),
        BatchId = Guid.Parse("b2c3d4e5-f6a7-8901-bcde-f12345678901"),
        BatchCode = "BAT-20250701-A1B2C3",
        AthleteId = Guid.Parse("f6a7b8c9-d0e1-2345-fabc-456789012345"),
        AthleteName = "Arjun Patel",
        EnrollmentDate = DateTime.UtcNow.AddDays(-2),
        Status = "Active",
        ProgramName = "Junior Cricket Development Program",
        AthleteCode = "ATH-20250701-L1M2N3",
        Certificates = [],
        CreatedAt = DateTime.UtcNow.AddDays(-2)
    };
}

public class AttendanceDtoExample : IExamplesProvider<AttendanceDto>
{
    public AttendanceDto GetExamples() => new()
    {
        Id = Guid.Parse("e5f6a7b8-c9d0-1234-efab-345678901234"),
        SessionId = Guid.Parse("c3d4e5f6-a7b8-9012-cdef-123456789012"),
        SessionCode = "SES-20250701-P1Q2R3",
        AthleteId = Guid.Parse("f6a7b8c9-d0e1-2345-fabc-456789012345"),
        AthleteName = "Arjun Patel",
        AttendanceStatus = "Present",
        CheckInTime = DateTime.UtcNow.AddHours(-2),
        CheckOutTime = DateTime.UtcNow.AddMinutes(-30),
        Remarks = null,
        AthleteCode = "ATH-20250701-L1M2N3",
        CreatedAt = DateTime.UtcNow.AddHours(-3)
    };
}

public class AssessmentDtoExample : IExamplesProvider<AssessmentDto>
{
    public AssessmentDto GetExamples() => new()
    {
        Id = Guid.Parse("f6a7b8c9-d0e1-2345-fabc-456789012345"),
        SessionId = Guid.Parse("c3d4e5f6-a7b8-9012-cdef-123456789012"),
        SessionCode = "SES-20250701-P1Q2R3",
        AssessmentType = "Skill",
        AssessmentName = "Batting Skills Assessment",
        MaximumScore = 100,
        PassingScore = 60,
        Results = []
    };
}

public class AssessmentResultDtoExample : IExamplesProvider<AssessmentResultDto>
{
    public AssessmentResultDto GetExamples() => new()
    {
        Id = Guid.Parse("11111111-2222-3333-4444-555555555555"),
        AssessmentId = Guid.Parse("f6a7b8c9-d0e1-2345-fabc-456789012345"),
        AthleteId = Guid.Parse("f6a7b8c9-d0e1-2345-fabc-456789012345"),
        AthleteName = "Arjun Patel",
        Score = 85,
        IsPassed = true,
        Remarks = "Excellent batting technique demonstrated.",
        AssessedAt = DateTime.UtcNow.AddHours(-1),
        AssessmentName = "Batting Skills Assessment"
    };
}

public class TrainingProgressDtoExample : IExamplesProvider<TrainingProgressDto>
{
    public TrainingProgressDto GetExamples() => new()
    {
        Id = Guid.Parse("22222222-3333-4444-5555-666666666666"),
        EnrollmentId = Guid.Parse("d4e5f6a7-b8c9-0123-defa-234567890123"),
        CurrentLevel = "Intermediate",
        CompletedPercentage = 65.5m,
        OverallRating = 4.2m,
        CreatedAt = DateTime.UtcNow.AddDays(-10),
        UpdatedAt = DateTime.UtcNow.AddDays(-1)
    };
}

public class CertificateDtoExample : IExamplesProvider<CertificateDto>
{
    public CertificateDto GetExamples() => new()
    {
        Id = Guid.Parse("33333333-4444-5555-6666-777777777777"),
        EnrollmentId = Guid.Parse("d4e5f6a7-b8c9-0123-defa-234567890123"),
        CertificateType = "Completion",
        CertificateNumber = "CERT-20250701-A1B2C3",
        IssuedDate = DateTime.UtcNow,
        FileUrl = "https://cdn.sportsgurukul.com/certificates/cert-2025.pdf",
        CreatedAt = DateTime.UtcNow
    };
}

public class TrainingMilestoneDtoExample : IExamplesProvider<TrainingMilestoneDto>
{
    public TrainingMilestoneDto GetExamples() => new()
    {
        Id = Guid.Parse("44444444-5555-6666-7777-888888888888"),
        ProgramId = Guid.Parse("a1b2c3d4-e5f6-7890-abcd-ef1234567890"),
        MilestoneName = "Basic Batting Technique",
        Description = "Master forward and defensive shots.",
        WeekNumber = 4,
        IsCompleted = false
    };
}

public class TrainingGoalDtoExample : IExamplesProvider<TrainingGoalDto>
{
    public TrainingGoalDto GetExamples() => new()
    {
        Id = Guid.Parse("55555555-6666-7777-8888-999999999999"),
        ProgramId = Guid.Parse("a1b2c3d4-e5f6-7890-abcd-ef1234567890"),
        GoalName = "Score 50+ runs in practice match",
        Description = "Demonstrate consistent batting in match conditions.",
        TargetWeek = 8,
        IsAchieved = false
    };
}

public class TrainingStatisticsDtoExample : IExamplesProvider<TrainingStatisticsDto>
{
    public TrainingStatisticsDto GetExamples() => new()
    {
        ActivePrograms = 12,
        ActiveBatches = 28,
        TotalSessions = 156,
        AttendancePercentage = 87.5m,
        CompletionRate = 72.3m,
        PassRate = 81.2m,
        CertificatesIssued = 145,
        CoachUtilization = 68.9m
    };
}

public class TrainingProgramSearchResponseExample : IExamplesProvider<TrainingProgramSearchResponse>
{
    public TrainingProgramSearchResponse GetExamples() => new()
    {
        Programs =
        [
            new TrainingProgramSummaryDto
            {
                Id = Guid.Parse("a1b2c3d4-e5f6-7890-abcd-ef1234567890"),
                ProgramCode = "TPR-20250701-X1Y2Z3",
                ProgramName = "Junior Cricket Development Program",
                SportName = "Cricket",
                AcademyName = "Mumbai Sports Academy",
                DifficultyLevel = "Intermediate",
                DurationWeeks = 12,
                Capacity = 30,
                Status = "Active",
                BatchesCount = 3,
                TotalEnrollments = 54,
                TotalBatches = 3,
                CreatedAt = DateTime.UtcNow.AddDays(-30)
            }
        ],
        TotalCount = 1,
        PageNumber = 1,
        PageSize = 20,
        TotalPages = 1
    };
}

#endregion
