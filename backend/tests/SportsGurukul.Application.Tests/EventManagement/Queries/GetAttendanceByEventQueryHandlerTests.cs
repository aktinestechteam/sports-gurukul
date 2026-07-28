using FluentAssertions;
using Moq;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Features.EventManagement.Queries.GetAttendanceByEvent;
using SportsGurukul.Domain.Enums;
using SportsGurukul.Application.Tests.EventManagement.Fixtures;
using SportsGurukul.Application.Tests.EventManagement.Mocks;

namespace SportsGurukul.Application.Tests.EventManagement.Queries;

public class GetAttendanceByEventQueryHandlerTests
{
    private readonly Mock<IEventAttendanceRepository> _attendanceRepo;
    private readonly Mock<ILogger<GetAttendanceByEventQueryHandler>> _logger;
    private readonly GetAttendanceByEventQueryHandler _handler;

    public GetAttendanceByEventQueryHandlerTests()
    {
        _attendanceRepo = EventMockFactory.CreateAttendanceRepository();
        _logger = EventMockFactory.CreateLogger<GetAttendanceByEventQueryHandler>();
        _handler = new GetAttendanceByEventQueryHandler(_attendanceRepo.Object, _logger.Object);
    }

    [Fact]
    public async Task Handle_ReturnsPagedResults()
    {
        _attendanceRepo.Setup(x => x.GetByEventIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Domain.Entities.EventAttendance>());

        var result = await _handler.Handle(new GetAttendanceByEventQuery { EventId = Guid.NewGuid() }, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value!.Items.Should().BeEmpty();
        result.Value.TotalRecords.Should().Be(0);
    }

    [Fact]
    public async Task Handle_WithSessionId_CallsSessionRepository()
    {
        var sessionId = Guid.NewGuid();
        _attendanceRepo.Setup(x => x.GetBySessionIdAsync(sessionId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Domain.Entities.EventAttendance>());

        var result = await _handler.Handle(new GetAttendanceByEventQuery { EventId = Guid.NewGuid(), SessionId = sessionId }, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        _attendanceRepo.Verify(x => x.GetBySessionIdAsync(sessionId, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WithStatusFilter_FiltersResults()
    {
        var eventId = Guid.NewGuid();
        var records = new List<Domain.Entities.EventAttendance>
        {
            EventDataFixture.CreateAttendance(eventId: eventId, status: EventAttendanceStatus.Present),
            EventDataFixture.CreateAttendance(eventId: eventId, status: EventAttendanceStatus.Absent)
        };
        _attendanceRepo.Setup(x => x.GetByEventIdAsync(eventId, It.IsAny<CancellationToken>())).ReturnsAsync(records);

        var result = await _handler.Handle(new GetAttendanceByEventQuery { EventId = eventId, Status = EventAttendanceStatus.Present }, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value!.Items.Should().HaveCount(1);
    }
}
