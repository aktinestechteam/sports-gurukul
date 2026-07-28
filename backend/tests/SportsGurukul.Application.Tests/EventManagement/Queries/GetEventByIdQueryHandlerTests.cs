using FluentAssertions;
using Moq;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Features.EventManagement.Queries.GetEventById;
using SportsGurukul.Application.Tests.EventManagement.Fixtures;
using SportsGurukul.Application.Tests.EventManagement.Mocks;

namespace SportsGurukul.Application.Tests.EventManagement.Queries;

public class GetEventByIdQueryHandlerTests
{
    private readonly Mock<IEventRepository> _eventRepo;
    private readonly Mock<ILogger<GetEventByIdQueryHandler>> _logger;
    private readonly GetEventByIdQueryHandler _handler;

    public GetEventByIdQueryHandlerTests()
    {
        _eventRepo = EventMockFactory.CreateEventRepository();
        _logger = EventMockFactory.CreateLogger<GetEventByIdQueryHandler>();
        _handler = new GetEventByIdQueryHandler(_eventRepo.Object, _logger.Object);
    }

    [Fact]
    public async Task Handle_EventNotFound_ReturnsFailure()
    {
        _eventRepo.Setup(x => x.GetWithDetailsAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Domain.Entities.Event?)null);

        var result = await _handler.Handle(new GetEventByIdQuery { EventId = Guid.NewGuid() }, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("Event not found");
    }

    [Fact]
    public async Task Handle_EventExists_ReturnsDto()
    {
        var evt = EventDataFixture.CreateDraftEvent();
        _eventRepo.Setup(x => x.GetWithDetailsAsync(evt.Id, It.IsAny<CancellationToken>())).ReturnsAsync(evt);

        var result = await _handler.Handle(new GetEventByIdQuery { EventId = evt.Id }, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value!.Id.Should().Be(evt.Id);
        result.Value.EventName.Should().Be(evt.EventName);
    }

    [Fact]
    public async Task Handle_EventExists_CallsRepositoryOnce()
    {
        var evt = EventDataFixture.CreateDraftEvent();
        _eventRepo.Setup(x => x.GetWithDetailsAsync(evt.Id, It.IsAny<CancellationToken>())).ReturnsAsync(evt);

        await _handler.Handle(new GetEventByIdQuery { EventId = evt.Id }, CancellationToken.None);

        _eventRepo.Verify(x => x.GetWithDetailsAsync(evt.Id, It.IsAny<CancellationToken>()), Times.Once);
    }
}
