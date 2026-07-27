using MediatR;
using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.TrainingProgramManagement.DTOs;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Application.Features.TrainingProgramManagement.Queries.GetTrainingStatisticsQuery
{
    public class GetTrainingStatisticsQueryHandler : IRequestHandler<GetTrainingStatisticsQuery, Result<TrainingStatisticsDto>>
    {
        private readonly ITrainingProgramRepository _programRepository;
        private readonly ITrainingBatchRepository _batchRepository;
        private readonly ISessionRepository _sessionRepository;
        private readonly IAttendanceRepository _attendanceRepository;
        private readonly IAssessmentRepository _assessmentRepository;
        private readonly ITrainingProgressRepository _progressRepository;
        private readonly ILogger<GetTrainingStatisticsQueryHandler> _logger;

        public GetTrainingStatisticsQueryHandler(
            ITrainingProgramRepository programRepository,
            ITrainingBatchRepository batchRepository,
            ISessionRepository sessionRepository,
            IAttendanceRepository attendanceRepository,
            IAssessmentRepository assessmentRepository,
            ITrainingProgressRepository progressRepository,
            ILogger<GetTrainingStatisticsQueryHandler> logger)
        {
            _programRepository = programRepository;
            _batchRepository = batchRepository;
            _sessionRepository = sessionRepository;
            _attendanceRepository = attendanceRepository;
            _assessmentRepository = assessmentRepository;
            _progressRepository = progressRepository;
            _logger = logger;
        }

        public async Task<Result<TrainingStatisticsDto>> Handle(GetTrainingStatisticsQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Calculating training statistics");

            var programs = await _programRepository.GetAllAsync(cancellationToken);
            var batches = await _batchRepository.GetAllAsync(cancellationToken);
            var sessions = await _sessionRepository.GetAllAsync(cancellationToken);

            var activePrograms = programs.Count(p => p.Status == TrainingProgramStatus.Active);
            var activeBatches = batches.Count(b => b.Status == BatchStatus.Active);
            var totalSessions = sessions.Count;

            var allAttendances = new List<Domain.Entities.Attendance>();
            foreach (var session in sessions)
            {
                var sessionAttendances = await _attendanceRepository.GetBySessionIdAsync(session.Id, cancellationToken);
                allAttendances.AddRange(sessionAttendances);
            }

            var attendancePercentage = allAttendances.Count > 0
                ? (decimal)allAttendances.Count(a => a.AttendanceStatus == AttendanceStatus.Present) / allAttendances.Count * 100
                : 0;

            var allProgress = await _progressRepository.GetAllAsync(cancellationToken);
            var completionRate = allProgress.Count > 0
                ? (decimal)allProgress.Count(p => p.CompletedPercentage >= 100) / allProgress.Count * 100
                : 0;

            var assessments = new List<Domain.Entities.TrainingAssessment>();
            foreach (var session in sessions)
            {
                var sessionAssessments = await _assessmentRepository.GetBySessionIdAsync(session.Id, cancellationToken);
                assessments.AddRange(sessionAssessments);
            }

            var allResults = new List<Domain.Entities.AssessmentResult>();
            foreach (var assessment in assessments)
            {
                var assessmentResults = await _assessmentRepository.GetResultsByAssessmentIdAsync(assessment.Id, cancellationToken);
                allResults.AddRange(assessmentResults);
            }

            var passRate = allResults.Count > 0
                ? (decimal)allResults.Count(r => r.IsPassed) / allResults.Count * 100
                : 0;

            var certificatesIssued = allProgress
                .SelectMany(p => p.Enrollment?.Certificates ?? [])
                .Count();

            var batchesWithCoaches = batches.Where(b => b.Status == BatchStatus.Active).ToList();
            var uniqueCoachIds = batchesWithCoaches.Select(b => b.CoachId).Distinct().ToList();
            var coachesWithActiveBatches = uniqueCoachIds.Count;
            var totalUniqueCoaches = batches.Select(b => b.CoachId).Distinct().Count();
            var coachUtilization = totalUniqueCoaches > 0
                ? (decimal)coachesWithActiveBatches / totalUniqueCoaches * 100
                : 0;

            var statistics = new TrainingStatisticsDto
            {
                ActivePrograms = activePrograms,
                ActiveBatches = activeBatches,
                TotalSessions = totalSessions,
                AttendancePercentage = Math.Round(attendancePercentage, 2),
                CompletionRate = Math.Round(completionRate, 2),
                PassRate = Math.Round(passRate, 2),
                CertificatesIssued = certificatesIssued,
                CoachUtilization = Math.Round(coachUtilization, 2)
            };

            return Result<TrainingStatisticsDto>.Success(statistics);
        }
    }
}
