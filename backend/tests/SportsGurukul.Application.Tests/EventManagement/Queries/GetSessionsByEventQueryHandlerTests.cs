using FluentAssertions;
using Moq;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Features.EventManagement.Queries.GetSessionsByEvent;
using SportsGurukul.Domain.Enums;
using SportsGurukul.Application.Tests.EventManagement.Fixtures;
using SportsGurukul.Application.Tests.EventManagement.Mocks;

namespace SportsGurukul.Application.Tests.EventManagement.Queries;

public class GetSessionsByEventQueryHandlerTests
{
    private readonly Mock<IEventRepository> _eventRepo;
    private readonly Mock<ILogger<GetSessionsByEventQueryHandler>> _logger;
    private readonly GetSessionsByEventQueryHandler _handler;

    public GetSessionsByEventQueryHandlerTests()
    {
        _eventRepo = EventMockFactory.CreateEventRepository();
        _logger = EventMockFactory.CreateLogger<GetSessionsByEventQueryHandler>();
        _handler = new GetSessionsByEventQueryHandler(_eventRepo.Object, _logger.Object);
    }

    [Fact]
    public async Task Handle_EventNotFound_ReturnsFailure()
    {
        _eventRepo.Setup(x => x.GetWithDetailsAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Domain.Entities.Event?)null);

        var result = await _handler.Handle(new GetSessionsByEventQuery { EventId = Guid.NewGuid() }, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("Event not found");
    }

    [Fact]
    public async Task Handle_EventWithSessions_ReturnsSessions()
    {
        var evt = EventDataFixture.CreateCompletedEvent();
        evt.Sessions = new List<Domain.Entities.EventSession>
        {
            EventDataFixture.CreateSession(eventId: evt.Id),
            EventDataFixture.CreateSession(eventId: evt.Id)
        };
        _eventRepo.Setup(x => x.GetWithDetailsAsync(evt.Id, It.IsAny<CancellationToken>())).ReturnsAsync(evt);

        var result = await _handler.Handle(new GetSessionsByEventQuery { EventId = evt.Id }, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(2);
    }
}
