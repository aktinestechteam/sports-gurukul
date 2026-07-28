using FluentAssertions;
using Moq;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Features.EventManagement.Queries.GetAnnouncementsByEvent;
using SportsGurukul.Application.Tests.EventManagement.Fixtures;
using SportsGurukul.Application.Tests.EventManagement.Mocks;

namespace SportsGurukul.Application.Tests.EventManagement.Queries;

public class GetAnnouncementsByEventQueryHandlerTests
{
    private readonly Mock<IEventRepository> _eventRepo;
    private readonly Mock<ILogger<GetAnnouncementsByEventQueryHandler>> _logger;
    private readonly GetAnnouncementsByEventQueryHandler _handler;

    public GetAnnouncementsByEventQueryHandlerTests()
    {
        _eventRepo = EventMockFactory.CreateEventRepository();
        _logger = EventMockFactory.CreateLogger<GetAnnouncementsByEventQueryHandler>();
        _handler = new GetAnnouncementsByEventQueryHandler(_eventRepo.Object, _logger.Object);
    }

    [Fact]
    public async Task Handle_EventNotFound_ReturnsFailure()
    {
        _eventRepo.Setup(x => x.GetWithDetailsAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Domain.Entities.Event?)null);

        var result = await _handler.Handle(new GetAnnouncementsByEventQuery { EventId = Guid.NewGuid() }, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("Event not found");
    }

    [Fact]
    public async Task Handle_EventWithPublishedAnnouncements_ReturnsOnlyPublished()
    {
        var evt = EventDataFixture.CreateRegistrationOpenEvent();
        evt.Announcements = new List<Domain.Entities.EventAnnouncement>
        {
            EventDataFixture.CreateAnnouncement(eventId: evt.Id, isPublished: true),
            EventDataFixture.CreateAnnouncement(eventId: evt.Id, isPublished: false)
        };
        _eventRepo.Setup(x => x.GetWithDetailsAsync(evt.Id, It.IsAny<CancellationToken>())).ReturnsAsync(evt);

        var result = await _handler.Handle(new GetAnnouncementsByEventQuery { EventId = evt.Id }, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(1);
        result.Value![0].IsPublished.Should().BeTrue();
    }
}
