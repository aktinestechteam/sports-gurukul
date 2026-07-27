using SportsGurukul.Domain.Entities;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Application.Features.TrainingProgramManagement.DTOs;

public class TrainingProgramDto
{
    public Guid Id { get; set; }
    public string ProgramCode { get; set; } = string.Empty;
    public string ProgramName { get; set; } = string.Empty;
    public Guid SportId { get; set; }
    public string SportName { get; set; } = string.Empty;
    public Guid AcademyId { get; set; }
    public string AcademyName { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string DifficultyLevel { get; set; } = string.Empty;
    public int MinimumAge { get; set; }
    public int MaximumAge { get; set; }
    public int DurationWeeks { get; set; }
    public int Capacity { get; set; }
    public string Status { get; set; } = string.Empty;
    public List<TrainingBatchDto> Batches { get; set; } = new();
    public List<TrainingGoalDto> Goals { get; set; } = new();
    public List<TrainingMilestoneDto> Milestones { get; set; } = new();
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public byte[]? RowVersion { get; set; }
    public int TotalBatches { get; set; }
    public int ActiveBatches { get; set; }

    public static TrainingProgramDto MapToDto(TrainingProgram entity)
    {
        return new TrainingProgramDto
        {
            Id = entity.Id,
            ProgramCode = entity.ProgramCode,
            ProgramName = entity.ProgramName,
            SportId = entity.SportId,
            SportName = entity.Sport?.Name ?? string.Empty,
            AcademyId = entity.AcademyId,
            AcademyName = entity.Academy?.Name ?? string.Empty,
            Description = entity.Description ?? string.Empty,
            DifficultyLevel = entity.DifficultyLevel.ToString(),
            MinimumAge = entity.MinimumAge,
            MaximumAge = entity.MaximumAge,
            DurationWeeks = entity.DurationWeeks,
            Capacity = entity.Capacity,
            Status = entity.Status.ToString(),
            Batches = entity.Batches?.Select(TrainingBatchDto.MapToDto).ToList() ?? new List<TrainingBatchDto>(),
            Goals = entity.Goals?.Select(TrainingGoalDto.MapToDto).ToList() ?? new List<TrainingGoalDto>(),
            Milestones = entity.Milestones?.Select(TrainingMilestoneDto.MapToDto).ToList() ?? new List<TrainingMilestoneDto>(),
            CreatedAt = entity.CreatedAt,
            UpdatedAt = entity.UpdatedAt,
            RowVersion = entity.RowVersion,
            TotalBatches = entity.Batches?.Count ?? 0,
            ActiveBatches = entity.Batches?.Count(b => b.Status == BatchStatus.Active) ?? 0
        };
    }
}

public class TrainingBatchDto
{
    public Guid Id { get; set; }
    public Guid ProgramId { get; set; }
    public string BatchCode { get; set; } = string.Empty;
    public Guid CoachId { get; set; }
    public string CoachName { get; set; } = string.Empty;
    public Guid BranchId { get; set; }
    public string BranchName { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public int MaximumSeats { get; set; }
    public string Status { get; set; } = string.Empty;
    public int SessionsCount { get; set; }
    public int EnrollmentCount { get; set; }
    public List<TrainingSessionDto> Sessions { get; set; } = new();
    public string ProgramName { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public byte[]? RowVersion { get; set; }
    public int SessionCount => SessionsCount;
    public int EnrolledCount => EnrollmentCount;

    public static TrainingBatchDto MapToDto(TrainingBatch entity)
    {
        return new TrainingBatchDto
        {
            Id = entity.Id,
            ProgramId = entity.ProgramId,
            BatchCode = entity.BatchCode,
            CoachId = entity.CoachId,
            CoachName = entity.Coach?.User?.FullName ?? string.Empty,
            BranchId = entity.BranchId,
            BranchName = entity.Branch?.BranchName ?? string.Empty,
            StartDate = entity.StartDate,
            EndDate = entity.EndDate ?? entity.StartDate,
            MaximumSeats = entity.MaximumSeats,
            Status = entity.Status.ToString(),
            SessionsCount = entity.Sessions?.Count ?? 0,
            EnrollmentCount = entity.Enrollments?.Count ?? 0,
            Sessions = entity.Sessions?.Select(TrainingSessionDto.MapToDto).ToList() ?? new List<TrainingSessionDto>(),
            ProgramName = entity.Program?.ProgramName ?? string.Empty,
            CreatedAt = entity.CreatedAt,
            UpdatedAt = entity.UpdatedAt,
            RowVersion = entity.RowVersion
        };
    }
}

public class TrainingSessionDto
{
    public Guid Id { get; set; }
    public Guid BatchId { get; set; }
    public string SessionCode { get; set; } = string.Empty;
    public string SessionTitle { get; set; } = string.Empty;
    public string SessionType { get; set; } = string.Empty;
    public DateTime SessionDate { get; set; }
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }
    public Guid? FacilityId { get; set; }
    public string FacilityName { get; set; } = string.Empty;
    public Guid CoachId { get; set; }
    public string CoachName { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public int AttendanceCount { get; set; }
    public string BatchCode { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public byte[]? RowVersion { get; set; }

    public static TrainingSessionDto MapToDto(TrainingSession entity)
    {
        return new TrainingSessionDto
        {
            Id = entity.Id,
            BatchId = entity.BatchId,
            SessionCode = entity.SessionCode,
            SessionTitle = entity.SessionTitle,
            SessionType = entity.SessionType.ToString(),
            SessionDate = entity.SessionDate,
            StartTime = entity.StartTime,
            EndTime = entity.EndTime,
            FacilityId = entity.FacilityId,
            FacilityName = entity.Facility?.FacilityName ?? string.Empty,
            CoachId = entity.CoachId,
            CoachName = entity.Coach?.User?.FullName ?? string.Empty,
            Status = entity.Status.ToString(),
            AttendanceCount = entity.Attendances?.Count ?? 0,
            BatchCode = entity.Batch?.BatchCode ?? string.Empty,
            CreatedAt = entity.CreatedAt,
            UpdatedAt = entity.UpdatedAt,
            RowVersion = entity.RowVersion
        };
    }
}

public class EnrollmentDto
{
    public Guid Id { get; set; }
    public Guid BatchId { get; set; }
    public string BatchCode { get; set; } = string.Empty;
    public Guid AthleteId { get; set; }
    public string AthleteName { get; set; } = string.Empty;
    public DateTime EnrollmentDate { get; set; }
    public string Status { get; set; } = string.Empty;
    public TrainingProgressDto? Progress { get; set; }
    public List<CertificateDto> Certificates { get; set; } = new();
    public string ProgramName { get; set; } = string.Empty;
    public string AthleteCode { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public byte[]? RowVersion { get; set; }

    public static EnrollmentDto MapToDto(TrainingEnrollment entity)
    {
        return new EnrollmentDto
        {
            Id = entity.Id,
            BatchId = entity.BatchId,
            BatchCode = entity.Batch?.BatchCode ?? string.Empty,
            AthleteId = entity.AthleteId,
            AthleteName = entity.Athlete?.User?.FullName ?? string.Empty,
            EnrollmentDate = entity.EnrollmentDate,
            Status = entity.Status.ToString(),
            Progress = entity.Progress != null ? TrainingProgressDto.MapToDto(entity.Progress) : null,
            Certificates = entity.Certificates?.Select(CertificateDto.MapToDto).ToList() ?? new List<CertificateDto>(),
            ProgramName = entity.Batch?.Program?.ProgramName ?? string.Empty,
            AthleteCode = entity.Athlete?.AthleteCode ?? string.Empty,
            CreatedAt = entity.CreatedAt,
            UpdatedAt = entity.UpdatedAt,
            RowVersion = entity.RowVersion
        };
    }
}

public class AttendanceDto
{
    public Guid Id { get; set; }
    public Guid SessionId { get; set; }
    public string SessionCode { get; set; } = string.Empty;
    public Guid AthleteId { get; set; }
    public string AthleteName { get; set; } = string.Empty;
    public string AttendanceStatus { get; set; } = string.Empty;
    public DateTime? CheckInTime { get; set; }
    public DateTime? CheckOutTime { get; set; }
    public string? Remarks { get; set; }
    public string AthleteCode { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    public static AttendanceDto MapToDto(Attendance entity)
    {
        return new AttendanceDto
        {
            Id = entity.Id,
            SessionId = entity.SessionId,
            SessionCode = entity.Session?.SessionCode ?? string.Empty,
            AthleteId = entity.AthleteId,
            AthleteName = entity.Athlete?.User?.FullName ?? string.Empty,
            AttendanceStatus = entity.AttendanceStatus.ToString(),
            CheckInTime = entity.CheckInTime,
            CheckOutTime = entity.CheckOutTime,
            Remarks = entity.Remarks,
            AthleteCode = entity.Athlete?.AthleteCode ?? string.Empty,
            CreatedAt = entity.CreatedAt,
            UpdatedAt = entity.UpdatedAt
        };
    }
}

public class AssessmentDto
{
    public Guid Id { get; set; }
    public Guid SessionId { get; set; }
    public string SessionCode { get; set; } = string.Empty;
    public string AssessmentType { get; set; } = string.Empty;
    public string AssessmentName { get; set; } = string.Empty;
    public decimal MaximumScore { get; set; }
    public decimal PassingScore { get; set; }
    public List<AssessmentResultDto> Results { get; set; } = new();

    public static AssessmentDto MapToDto(TrainingAssessment entity)
    {
        return new AssessmentDto
        {
            Id = entity.Id,
            SessionId = entity.SessionId,
            SessionCode = entity.Session?.SessionCode ?? string.Empty,
            AssessmentType = entity.AssessmentType.ToString(),
            AssessmentName = entity.AssessmentName,
            MaximumScore = entity.MaximumScore,
            PassingScore = entity.PassingScore,
            Results = entity.Results?.Select(AssessmentResultDto.MapToDto).ToList() ?? new List<AssessmentResultDto>()
        };
    }
}

public class AssessmentResultDto
{
    public Guid Id { get; set; }
    public Guid AssessmentId { get; set; }
    public Guid AthleteId { get; set; }
    public string AthleteName { get; set; } = string.Empty;
    public decimal Score { get; set; }
    public bool IsPassed { get; set; }
    public string? Remarks { get; set; }
    public DateTime AssessedAt { get; set; }
    public string AssessmentName { get; set; } = string.Empty;

    public static AssessmentResultDto MapToDto(AssessmentResult entity)
    {
        return new AssessmentResultDto
        {
            Id = entity.Id,
            AssessmentId = entity.AssessmentId,
            AthleteId = entity.AthleteId,
            AthleteName = entity.Athlete?.User?.FullName ?? string.Empty,
            Score = entity.Score,
            IsPassed = entity.IsPassed,
            Remarks = entity.Remarks,
            AssessedAt = entity.AssessedAt,
            AssessmentName = entity.Assessment?.AssessmentName ?? string.Empty
        };
    }
}

public class TrainingProgressDto
{
    public Guid Id { get; set; }
    public Guid EnrollmentId { get; set; }
    public string CurrentLevel { get; set; } = string.Empty;
    public decimal CompletedPercentage { get; set; }
    public decimal? OverallRating { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public byte[]? RowVersion { get; set; }

    public static TrainingProgressDto MapToDto(TrainingProgress entity)
    {
        return new TrainingProgressDto
        {
            Id = entity.Id,
            EnrollmentId = entity.EnrollmentId,
            CurrentLevel = entity.CurrentLevel,
            CompletedPercentage = entity.CompletedPercentage,
            OverallRating = entity.OverallRating,
            CreatedAt = entity.CreatedAt,
            UpdatedAt = entity.UpdatedAt,
            RowVersion = entity.RowVersion
        };
    }
}

public class CertificateDto
{
    public Guid Id { get; set; }
    public Guid EnrollmentId { get; set; }
    public string CertificateType { get; set; } = string.Empty;
    public string CertificateNumber { get; set; } = string.Empty;
    public DateTime IssuedDate { get; set; }
    public string? FileUrl { get; set; }
    public DateTime CreatedAt { get; set; }

    public static CertificateDto MapToDto(TrainingCertificate entity)
    {
        return new CertificateDto
        {
            Id = entity.Id,
            EnrollmentId = entity.EnrollmentId,
            CertificateType = entity.CertificateType.ToString(),
            CertificateNumber = entity.CertificateNumber,
            IssuedDate = entity.IssuedDate,
            FileUrl = entity.FileUrl,
            CreatedAt = entity.CreatedAt
        };
    }
}

public class TrainingGoalDto
{
    public Guid Id { get; set; }
    public Guid ProgramId { get; set; }
    public string GoalName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int TargetWeek { get; set; }
    public bool IsAchieved { get; set; }

    public static TrainingGoalDto MapToDto(TrainingGoal entity)
    {
        return new TrainingGoalDto
        {
            Id = entity.Id,
            ProgramId = entity.ProgramId,
            GoalName = entity.GoalName,
            Description = entity.Description,
            TargetWeek = entity.TargetWeek,
            IsAchieved = entity.IsAchieved
        };
    }
}

public class TrainingMilestoneDto
{
    public Guid Id { get; set; }
    public Guid ProgramId { get; set; }
    public string MilestoneName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int WeekNumber { get; set; }
    public bool IsCompleted { get; set; }

    public static TrainingMilestoneDto MapToDto(TrainingMilestone entity)
    {
        return new TrainingMilestoneDto
        {
            Id = entity.Id,
            ProgramId = entity.ProgramId,
            MilestoneName = entity.MilestoneName,
            Description = entity.Description,
            WeekNumber = entity.WeekNumber,
            IsCompleted = entity.IsCompleted
        };
    }
}

public class TrainingStatisticsDto
{
    public int ActivePrograms { get; set; }
    public int ActiveBatches { get; set; }
    public int TotalSessions { get; set; }
    public decimal AttendancePercentage { get; set; }
    public decimal CompletionRate { get; set; }
    public decimal PassRate { get; set; }
    public int CertificatesIssued { get; set; }
    public decimal CoachUtilization { get; set; }
}

public class TrainingProgramSearchResponse
{
    public List<TrainingProgramSummaryDto> Programs { get; set; } = new();
    public int TotalCount { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public int TotalPages { get; set; }
    public List<TrainingProgramSummaryDto> Items => Programs;
    public int TotalRecords => TotalCount;
    public int CurrentPage => PageNumber;
}

public class TrainingProgramSummaryDto
{
    public Guid Id { get; set; }
    public string ProgramCode { get; set; } = string.Empty;
    public string ProgramName { get; set; } = string.Empty;
    public string SportName { get; set; } = string.Empty;
    public string AcademyName { get; set; } = string.Empty;
    public string DifficultyLevel { get; set; } = string.Empty;
    public int DurationWeeks { get; set; }
    public int Capacity { get; set; }
    public string Status { get; set; } = string.Empty;
    public int BatchesCount { get; set; }
    public int TotalEnrollments { get; set; }
    public int TotalBatches { get; set; }
    public DateTime CreatedAt { get; set; }

    public static TrainingProgramSummaryDto MapToDto(TrainingProgram entity)
    {
        return new TrainingProgramSummaryDto
        {
            Id = entity.Id,
            ProgramCode = entity.ProgramCode,
            ProgramName = entity.ProgramName,
            SportName = entity.Sport?.Name ?? string.Empty,
            AcademyName = entity.Academy?.Name ?? string.Empty,
            DifficultyLevel = entity.DifficultyLevel.ToString(),
            DurationWeeks = entity.DurationWeeks,
            Capacity = entity.Capacity,
            Status = entity.Status.ToString(),
            BatchesCount = entity.Batches?.Count ?? 0,
            TotalEnrollments = entity.Batches?.Sum(b => b.Enrollments?.Count ?? 0) ?? 0,
            TotalBatches = entity.Batches?.Count ?? 0,
            CreatedAt = entity.CreatedAt
        };
    }
}
