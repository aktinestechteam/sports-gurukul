using FluentAssertions;
using Moq;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Features.EventManagement.Commands.PublishAnnouncement;
using SportsGurukul.Application.Features.EventManagement.Services;
using SportsGurukul.Application.Tests.EventManagement.Fixtures;
using SportsGurukul.Application.Tests.EventManagement.Mocks;

namespace SportsGurukul.Application.Tests.EventManagement.Commands;

public class PublishAnnouncementCommandHandlerTests
{
    private readonly Mock<IEventRepository> _eventRepo;
    private readonly Mock<IEventAnnouncementService> _announcementService;
    private readonly Mock<IUnitOfWork> _unitOfWork;
    private readonly Mock<ILogger<PublishAnnouncementCommandHandler>> _logger;
    private readonly PublishAnnouncementCommandHandler _handler;

    public PublishAnnouncementCommandHandlerTests()
    {
        _eventRepo = EventMockFactory.CreateEventRepository();
        _announcementService = EventMockFactory.CreateAnnouncementService();
        _unitOfWork = EventMockFactory.CreateUnitOfWork();
        _logger = EventMockFactory.CreateLogger<PublishAnnouncementCommandHandler>();
        _handler = new PublishAnnouncementCommandHandler(_eventRepo.Object, _announcementService.Object, _unitOfWork.Object, _logger.Object);
    }

    [Fact]
    public async Task Handle_EventNotFound_ReturnsFailure()
    {
        _eventRepo.Setup(x => x.GetWithDetailsAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Domain.Entities.Event?)null);

        var result = await _handler.Handle(new PublishAnnouncementCommand { EventId = Guid.NewGuid(), Title = "Title", Message = "Msg" }, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("Event not found");
    }

    [Fact]
    public async Task Handle_CannotPublish_ReturnsFailure()
    {
        var evt = EventDataFixture.CreateDraftEvent();
        _eventRepo.Setup(x => x.GetWithDetailsAsync(evt.Id, It.IsAny<CancellationToken>())).ReturnsAsync(evt);
        _announcementService.Setup(x => x.CanPublishAnnouncementAsync(evt, It.IsAny<CancellationToken>())).ReturnsAsync(false);

        var result = await _handler.Handle(new PublishAnnouncementCommand { EventId = evt.Id, Title = "Title", Message = "Msg" }, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("cannot be published");
    }

    [Fact]
    public async Task Handle_ValidAnnouncement_Published()
    {
        var evt = EventDataFixture.CreateRegistrationOpenEvent();
        _eventRepo.Setup(x => x.GetWithDetailsAsync(evt.Id, It.IsAny<CancellationToken>())).ReturnsAsync(evt);
        _announcementService.Setup(x => x.CanPublishAnnouncementAsync(evt, It.IsAny<CancellationToken>())).ReturnsAsync(true);

        var result = await _handler.Handle(new PublishAnnouncementCommand
        {
            EventId = evt.Id,
            Title = "Schedule Update",
            Message = "New timing",
            SendNotification = true,
            Priority = "High"
        }, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value!.Title.Should().Be("Schedule Update");
        result.Value.IsPublished.Should().BeTrue();
        _unitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}
