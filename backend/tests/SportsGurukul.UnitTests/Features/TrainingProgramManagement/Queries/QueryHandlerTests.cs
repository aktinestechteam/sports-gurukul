using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Features.TrainingProgramManagement.DTOs;
using SportsGurukul.Application.Features.TrainingProgramManagement.Queries.GetTrainingProgramByIdQuery;
using SportsGurukul.Application.Features.TrainingProgramManagement.Queries.GetTrainingBatchQuery;
using SportsGurukul.Application.Features.TrainingProgramManagement.Queries.SearchTrainingProgramsQuery;
using SportsGurukul.Application.Features.TrainingProgramManagement.Queries.GetUpcomingSessionsQuery;
using SportsGurukul.Application.Features.TrainingProgramManagement.Queries.GetTrainingProgressQuery;
using SportsGurukul.Application.Features.TrainingProgramManagement.Queries.GetSessionAttendanceQuery;
using SportsGurukul.Application.Features.TrainingProgramManagement.Queries.GetAssessmentsBySessionQuery;
using SportsGurukul.Application.Features.TrainingProgramManagement.Queries.GetTrainingStatisticsQuery;
using SportsGurukul.Domain.Entities;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.UnitTests.Features.TrainingProgramManagement.Queries;

public class QueryHandlerTests
{
    #region GetTrainingProgramByIdQueryHandler

    public class GetTrainingProgramByIdQueryHandlerTests
    {
        private readonly Mock<ITrainingProgramRepository> _repositoryMock;
        private readonly Mock<ILogger<GetTrainingProgramByIdQueryHandler>> _loggerMock;
        private readonly GetTrainingProgramByIdQueryHandler _handler;

        public GetTrainingProgramByIdQueryHandlerTests()
        {
            _repositoryMock = new Mock<ITrainingProgramRepository>();
            _loggerMock = new Mock<ILogger<GetTrainingProgramByIdQueryHandler>>();
            _handler = new GetTrainingProgramByIdQueryHandler(_repositoryMock.Object, _loggerMock.Object);
        }

        [Fact]
        public async Task Handle_Should_ReturnSuccess_When_ProgramFound()
        {
            var programId = Guid.NewGuid();
            var program = TestHelpers.CreateTestProgram(programId);
            program.Batches = new List<TrainingBatch>();

            _repositoryMock.Setup(r => r.GetByIdWithDetailsAsync(programId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(program);

            var query = new GetTrainingProgramByIdQuery { Id = programId };
            var result = await _handler.Handle(query, CancellationToken.None);

            result.IsSuccess.Should().BeTrue();
            result.Value.Should().NotBeNull();
            result.Value.Id.Should().Be(programId);
            result.Value.ProgramName.Should().Be(program.ProgramName);
        }

        [Fact]
        public async Task Handle_Should_ReturnFailure_When_ProgramNotFound()
        {
            var programId = Guid.NewGuid();
            _repositoryMock.Setup(r => r.GetByIdWithDetailsAsync(programId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((TrainingProgram?)null);

            var query = new GetTrainingProgramByIdQuery { Id = programId };
            var result = await _handler.Handle(query, CancellationToken.None);

            result.IsSuccess.Should().BeFalse();
            result.Error.Should().Contain("not found");
        }
    }

    #endregion

    #region GetTrainingBatchQueryHandler

    public class GetTrainingBatchQueryHandlerTests
    {
        private readonly Mock<ITrainingBatchRepository> _repositoryMock;
        private readonly Mock<ILogger<GetTrainingBatchQueryHandler>> _loggerMock;
        private readonly GetTrainingBatchQueryHandler _handler;

        public GetTrainingBatchQueryHandlerTests()
        {
            _repositoryMock = new Mock<ITrainingBatchRepository>();
            _loggerMock = new Mock<ILogger<GetTrainingBatchQueryHandler>>();
            _handler = new GetTrainingBatchQueryHandler(_repositoryMock.Object, _loggerMock.Object);
        }

        [Fact]
        public async Task Handle_Should_ReturnSuccess_When_BatchFound()
        {
            var batchId = Guid.NewGuid();
            var batch = TestHelpers.CreateTestBatch(id: batchId);
            batch.Sessions = new List<TrainingSession>();
            batch.Enrollments = new List<TrainingEnrollment>();

            _repositoryMock.Setup(r => r.GetByIdWithDetailsAsync(batchId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(batch);

            var query = new GetTrainingBatchQuery { Id = batchId };
            var result = await _handler.Handle(query, CancellationToken.None);

            result.IsSuccess.Should().BeTrue();
            result.Value.Should().NotBeNull();
            result.Value.Id.Should().Be(batchId);
            result.Value.BatchCode.Should().Be(batch.BatchCode);
        }

        [Fact]
        public async Task Handle_Should_ReturnFailure_When_BatchNotFound()
        {
            var batchId = Guid.NewGuid();
            _repositoryMock.Setup(r => r.GetByIdWithDetailsAsync(batchId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((TrainingBatch?)null);

            var query = new GetTrainingBatchQuery { Id = batchId };
            var result = await _handler.Handle(query, CancellationToken.None);

            result.IsSuccess.Should().BeFalse();
            result.Error.Should().Contain("not found");
        }
    }

    #endregion

    #region SearchTrainingProgramsQueryHandler

    public class SearchTrainingProgramsQueryHandlerTests
    {
        private readonly Mock<ITrainingProgramRepository> _repositoryMock;
        private readonly Mock<ILogger<SearchTrainingProgramsQueryHandler>> _loggerMock;
        private readonly SearchTrainingProgramsQueryHandler _handler;

        public SearchTrainingProgramsQueryHandlerTests()
        {
            _repositoryMock = new Mock<ITrainingProgramRepository>();
            _loggerMock = new Mock<ILogger<SearchTrainingProgramsQueryHandler>>();
            _handler = new SearchTrainingProgramsQueryHandler(_repositoryMock.Object, _loggerMock.Object);
        }

        [Fact]
        public async Task Handle_Should_ReturnFilteredResults_When_AcademyIdFilter()
        {
            var academyId = Guid.NewGuid();
            var programs = new List<TrainingProgram>
            {
                TestHelpers.CreateTestProgram(academyId: academyId),
                TestHelpers.CreateTestProgram()
            };

            _repositoryMock.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(programs);

            var query = new SearchTrainingProgramsQuery { AcademyId = academyId, Page = 1, PageSize = 10 };
            var result = await _handler.Handle(query, CancellationToken.None);

            result.IsSuccess.Should().BeTrue();
            result.Value.Programs.Should().HaveCount(1);
            result.Value.TotalCount.Should().Be(1);
        }

        [Fact]
        public async Task Handle_Should_PaginateResults()
        {
            var programs = Enumerable.Range(1, 5)
                .Select(i => TestHelpers.CreateTestProgram(programName: $"Program {i}"))
                .ToList();

            _repositoryMock.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(programs);

            var query = new SearchTrainingProgramsQuery { Page = 1, PageSize = 2 };
            var result = await _handler.Handle(query, CancellationToken.None);

            result.IsSuccess.Should().BeTrue();
            result.Value.Programs.Should().HaveCount(2);
            result.Value.TotalCount.Should().Be(5);
            result.Value.TotalPages.Should().Be(3);
        }

        [Fact]
        public async Task Handle_Should_HandleEmptyResults()
        {
            _repositoryMock.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<TrainingProgram>());

            var query = new SearchTrainingProgramsQuery { Page = 1, PageSize = 10 };
            var result = await _handler.Handle(query, CancellationToken.None);

            result.IsSuccess.Should().BeTrue();
            result.Value.Programs.Should().BeEmpty();
            result.Value.TotalCount.Should().Be(0);
        }
    }

    #endregion

    #region GetUpcomingSessionsQueryHandler

    public class GetUpcomingSessionsQueryHandlerTests
    {
        private readonly Mock<ISessionRepository> _repositoryMock;
        private readonly Mock<ILogger<GetUpcomingSessionsQueryHandler>> _loggerMock;
        private readonly GetUpcomingSessionsQueryHandler _handler;

        public GetUpcomingSessionsQueryHandlerTests()
        {
            _repositoryMock = new Mock<ISessionRepository>();
            _loggerMock = new Mock<ILogger<GetUpcomingSessionsQueryHandler>>();
            _handler = new GetUpcomingSessionsQueryHandler(_repositoryMock.Object, _loggerMock.Object);
        }

        [Fact]
        public async Task Handle_Should_FilterByBatchId_When_Provided()
        {
            var batchId = Guid.NewGuid();
            var sessions = new List<TrainingSession>
            {
                TestHelpers.CreateTestSession(batchId: batchId, status: SessionStatus.Scheduled)
            };

            _repositoryMock.Setup(r => r.GetByBatchIdAsync(batchId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(sessions);

            var query = new GetUpcomingSessionsQuery { BatchId = batchId };
            var result = await _handler.Handle(query, CancellationToken.None);

            result.IsSuccess.Should().BeTrue();
            result.Value.Should().HaveCount(1);
        }

        [Fact]
        public async Task Handle_Should_FilterByCoachId_When_Provided()
        {
            var coachId = Guid.NewGuid();
            var sessions = new List<TrainingSession>
            {
                TestHelpers.CreateTestSession(coachId: coachId)
            };

            _repositoryMock.Setup(r => r.GetByCoachIdAsync(coachId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(sessions);

            var query = new GetUpcomingSessionsQuery { CoachId = coachId };
            var result = await _handler.Handle(query, CancellationToken.None);

            result.IsSuccess.Should().BeTrue();
            result.Value.Should().HaveCount(1);
        }

        [Fact]
        public async Task Handle_Should_ReturnAllUpcomingSessions_When_NoFilter()
        {
            var futureSession = TestHelpers.CreateTestSession();
            futureSession.SessionDate = DateTime.UtcNow.AddDays(7);

            _repositoryMock.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<TrainingSession> { futureSession });

            var query = new GetUpcomingSessionsQuery();
            var result = await _handler.Handle(query, CancellationToken.None);

            result.IsSuccess.Should().BeTrue();
            result.Value.Should().HaveCount(1);
        }

        [Fact]
        public async Task Handle_Should_FilterOutPastSessions()
        {
            var futureSession = TestHelpers.CreateTestSession();
            futureSession.SessionDate = DateTime.UtcNow.AddDays(7);

            var pastSession = TestHelpers.CreateTestSession();
            pastSession.SessionDate = DateTime.UtcNow.AddDays(-7);

            _repositoryMock.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<TrainingSession> { futureSession, pastSession });

            var query = new GetUpcomingSessionsQuery();
            var result = await _handler.Handle(query, CancellationToken.None);

            result.IsSuccess.Should().BeTrue();
            result.Value.Should().HaveCount(1);
        }
    }

    #endregion

    #region GetTrainingProgressQueryHandler

    public class GetTrainingProgressQueryHandlerTests
    {
        private readonly Mock<ITrainingProgressRepository> _repositoryMock;
        private readonly Mock<ILogger<GetTrainingProgressQueryHandler>> _loggerMock;
        private readonly GetTrainingProgressQueryHandler _handler;

        public GetTrainingProgressQueryHandlerTests()
        {
            _repositoryMock = new Mock<ITrainingProgressRepository>();
            _loggerMock = new Mock<ILogger<GetTrainingProgressQueryHandler>>();
            _handler = new GetTrainingProgressQueryHandler(_repositoryMock.Object, _loggerMock.Object);
        }

        [Fact]
        public async Task Handle_Should_ReturnSuccess_When_ProgressFound()
        {
            var enrollmentId = Guid.NewGuid();
            var progress = TestHelpers.CreateTestProgress(enrollmentId: enrollmentId);

            _repositoryMock.Setup(r => r.GetByEnrollmentIdAsync(enrollmentId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(progress);

            var query = new GetTrainingProgressQuery { EnrollmentId = enrollmentId };
            var result = await _handler.Handle(query, CancellationToken.None);

            result.IsSuccess.Should().BeTrue();
            result.Value.Should().NotBeNull();
            result.Value.EnrollmentId.Should().Be(enrollmentId);
        }

        [Fact]
        public async Task Handle_Should_ReturnFailure_When_ProgressNotFound()
        {
            var enrollmentId = Guid.NewGuid();
            _repositoryMock.Setup(r => r.GetByEnrollmentIdAsync(enrollmentId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((TrainingProgress?)null);

            var query = new GetTrainingProgressQuery { EnrollmentId = enrollmentId };
            var result = await _handler.Handle(query, CancellationToken.None);

            result.IsSuccess.Should().BeFalse();
            result.Error.Should().Contain("not found");
        }
    }

    #endregion

    #region GetSessionAttendanceQueryHandler

    public class GetSessionAttendanceQueryHandlerTests
    {
        private readonly Mock<IAttendanceRepository> _repositoryMock;
        private readonly Mock<ILogger<GetSessionAttendanceQueryHandler>> _loggerMock;
        private readonly GetSessionAttendanceQueryHandler _handler;

        public GetSessionAttendanceQueryHandlerTests()
        {
            _repositoryMock = new Mock<IAttendanceRepository>();
            _loggerMock = new Mock<ILogger<GetSessionAttendanceQueryHandler>>();
            _handler = new GetSessionAttendanceQueryHandler(_repositoryMock.Object, _loggerMock.Object);
        }

        [Fact]
        public async Task Handle_Should_ReturnAttendanceList()
        {
            var sessionId = Guid.NewGuid();
            var attendances = new List<Attendance>
            {
                TestHelpers.CreateTestAttendance(sessionId: sessionId, status: AttendanceStatus.Present),
                TestHelpers.CreateTestAttendance(sessionId: sessionId, status: AttendanceStatus.Absent)
            };

            _repositoryMock.Setup(r => r.GetBySessionIdAsync(sessionId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(attendances);

            var query = new GetSessionAttendanceQuery { SessionId = sessionId };
            var result = await _handler.Handle(query, CancellationToken.None);

            result.IsSuccess.Should().BeTrue();
            result.Value.Should().HaveCount(2);
        }
    }

    #endregion

    #region GetAssessmentsBySessionQueryHandler

    public class GetAssessmentsBySessionQueryHandlerTests
    {
        private readonly Mock<IAssessmentRepository> _repositoryMock;
        private readonly Mock<ILogger<GetAssessmentsBySessionQueryHandler>> _loggerMock;
        private readonly GetAssessmentsBySessionQueryHandler _handler;

        public GetAssessmentsBySessionQueryHandlerTests()
        {
            _repositoryMock = new Mock<IAssessmentRepository>();
            _loggerMock = new Mock<ILogger<GetAssessmentsBySessionQueryHandler>>();
            _handler = new GetAssessmentsBySessionQueryHandler(_repositoryMock.Object, _loggerMock.Object);
        }

        [Fact]
        public async Task Handle_Should_ReturnAssessmentList()
        {
            var sessionId = Guid.NewGuid();
            var assessments = new List<TrainingAssessment>
            {
                TestHelpers.CreateTestAssessment(sessionId: sessionId)
            };

            _repositoryMock.Setup(r => r.GetBySessionIdAsync(sessionId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(assessments);

            var query = new GetAssessmentsBySessionQuery { SessionId = sessionId };
            var result = await _handler.Handle(query, CancellationToken.None);

            result.IsSuccess.Should().BeTrue();
            result.Value.Should().HaveCount(1);
        }
    }

    #endregion

    #region GetTrainingStatisticsQueryHandler

    public class GetTrainingStatisticsQueryHandlerTests
    {
        private readonly Mock<ITrainingProgramRepository> _programRepositoryMock;
        private readonly Mock<ITrainingBatchRepository> _batchRepositoryMock;
        private readonly Mock<ISessionRepository> _sessionRepositoryMock;
        private readonly Mock<IAttendanceRepository> _attendanceRepositoryMock;
        private readonly Mock<IAssessmentRepository> _assessmentRepositoryMock;
        private readonly Mock<ITrainingProgressRepository> _progressRepositoryMock;
        private readonly Mock<ILogger<GetTrainingStatisticsQueryHandler>> _loggerMock;
        private readonly GetTrainingStatisticsQueryHandler _handler;

        public GetTrainingStatisticsQueryHandlerTests()
        {
            _programRepositoryMock = new Mock<ITrainingProgramRepository>();
            _batchRepositoryMock = new Mock<ITrainingBatchRepository>();
            _sessionRepositoryMock = new Mock<ISessionRepository>();
            _attendanceRepositoryMock = new Mock<IAttendanceRepository>();
            _assessmentRepositoryMock = new Mock<IAssessmentRepository>();
            _progressRepositoryMock = new Mock<ITrainingProgressRepository>();
            _loggerMock = new Mock<ILogger<GetTrainingStatisticsQueryHandler>>();
            _handler = new GetTrainingStatisticsQueryHandler(
                _programRepositoryMock.Object,
                _batchRepositoryMock.Object,
                _sessionRepositoryMock.Object,
                _attendanceRepositoryMock.Object,
                _assessmentRepositoryMock.Object,
                _progressRepositoryMock.Object,
                _loggerMock.Object);
        }

        [Fact]
        public async Task Handle_Should_ReturnStatistics_WithCorrectCalculations()
        {
            var programs = new List<TrainingProgram>
            {
                TestHelpers.CreateTestProgram(status: TrainingProgramStatus.Active),
                TestHelpers.CreateTestProgram(status: TrainingProgramStatus.Draft)
            };
            var batchId1 = Guid.NewGuid();
            var batchId2 = Guid.NewGuid();
            var sessionId1 = Guid.NewGuid();
            var sessionId2 = Guid.NewGuid();
            var batches = new List<TrainingBatch>
            {
                TestHelpers.CreateTestBatch(id: batchId1, status: BatchStatus.Active),
                TestHelpers.CreateTestBatch(id: batchId2, status: BatchStatus.Active)
            };
            var sessions = new List<TrainingSession>
            {
                TestHelpers.CreateTestSession(id: sessionId1),
                TestHelpers.CreateTestSession(id: sessionId2)
            };
            var attendances = new List<Attendance>
            {
                TestHelpers.CreateTestAttendance(sessionId: sessionId1, status: AttendanceStatus.Present),
                TestHelpers.CreateTestAttendance(sessionId: sessionId1, status: AttendanceStatus.Absent)
            };
            var completedProgress = TestHelpers.CreateTestProgress();
            completedProgress.CompletedPercentage = 100;
            var progress = new List<TrainingProgress> { completedProgress };

            _programRepositoryMock.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(programs);
            _batchRepositoryMock.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(batches);
            _sessionRepositoryMock.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(sessions);
            _attendanceRepositoryMock.Setup(r => r.GetBySessionIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(attendances);
            _assessmentRepositoryMock.Setup(r => r.GetBySessionIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<TrainingAssessment>());
            _assessmentRepositoryMock.Setup(r => r.GetResultsByAssessmentIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<AssessmentResult>());
            _progressRepositoryMock.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(progress);

            var query = new GetTrainingStatisticsQuery();
            var result = await _handler.Handle(query, CancellationToken.None);

            result.IsSuccess.Should().BeTrue();
            result.Value.Should().NotBeNull();
            result.Value.ActivePrograms.Should().Be(1);
            result.Value.ActiveBatches.Should().Be(2);
            result.Value.TotalSessions.Should().Be(2);
            result.Value.AttendancePercentage.Should().Be(50m);
            result.Value.CompletionRate.Should().Be(100m);
        }

        [Fact]
        public async Task Handle_Should_ReturnZeroStatistics_When_NoData()
        {
            _programRepositoryMock.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<TrainingProgram>());
            _batchRepositoryMock.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<TrainingBatch>());
            _sessionRepositoryMock.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<TrainingSession>());
            _attendanceRepositoryMock.Setup(r => r.GetBySessionIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<Attendance>());
            _assessmentRepositoryMock.Setup(r => r.GetBySessionIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<TrainingAssessment>());
            _assessmentRepositoryMock.Setup(r => r.GetResultsByAssessmentIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<AssessmentResult>());
            _progressRepositoryMock.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<TrainingProgress>());

            var query = new GetTrainingStatisticsQuery();
            var result = await _handler.Handle(query, CancellationToken.None);

            result.IsSuccess.Should().BeTrue();
            result.Value.ActivePrograms.Should().Be(0);
            result.Value.ActiveBatches.Should().Be(0);
            result.Value.TotalSessions.Should().Be(0);
            result.Value.AttendancePercentage.Should().Be(0);
            result.Value.CompletionRate.Should().Be(0);
            result.Value.PassRate.Should().Be(0);
        }
    }

    #endregion
}
