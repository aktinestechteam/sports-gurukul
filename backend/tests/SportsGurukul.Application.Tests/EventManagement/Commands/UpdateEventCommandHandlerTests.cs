using FluentAssertions;
using Moq;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Features.EventManagement.Commands.UpdateEvent;
using SportsGurukul.Application.Tests.EventManagement.Fixtures;
using SportsGurukul.Application.Tests.EventManagement.Mocks;

namespace SportsGurukul.Application.Tests.EventManagement.Commands;

public class UpdateEventCommandHandlerTests
{
    private readonly Mock<IEventRepository> _eventRepo;
    private readonly Mock<IUnitOfWork> _unitOfWork;
    private readonly Mock<ILogger<UpdateEventCommandHandler>> _logger;
    private readonly UpdateEventCommandHandler _handler;

    public UpdateEventCommandHandlerTests()
    {
        _eventRepo = EventMockFactory.CreateEventRepository();
        _unitOfWork = EventMockFactory.CreateUnitOfWork();
        _logger = EventMockFactory.CreateLogger<UpdateEventCommandHandler>();
        _handler = new UpdateEventCommandHandler(_eventRepo.Object, _unitOfWork.Object, _logger.Object);
    }

    [Fact]
    public async Task Handle_EventNotFound_ReturnsFailure()
    {
        _eventRepo.Setup(x => x.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Domain.Entities.Event?)null);

        var result = await _handler.Handle(new UpdateEventCommand { EventId = Guid.NewGuid(), EventName = "Updated" }, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("Event not found");
    }

    [Fact]
    public async Task Handle_ValidUpdate_ReturnsSuccess()
    {
        var evt = EventDataFixture.CreateDraftEvent();
        _eventRepo.Setup(x => x.GetByIdAsync(evt.Id, It.IsAny<CancellationToken>())).ReturnsAsync(evt);

        var result = await _handler.Handle(new UpdateEventCommand { EventId = evt.Id, EventName = "Updated Name", Description = "Updated Desc" }, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value!.EventName.Should().Be("Updated Name");
        result.Value.Description.Should().Be("Updated Desc");
    }

    [Fact]
    public async Task Handle_PartialUpdate_OnlyUpdatesNonNullFields()
    {
        var evt = EventDataFixture.CreateDraftEvent();
        var originalName = evt.EventName;
        _eventRepo.Setup(x => x.GetByIdAsync(evt.Id, It.IsAny<CancellationToken>())).ReturnsAsync(evt);

        var result = await _handler.Handle(new UpdateEventCommand { EventId = evt.Id, Description = "Only desc" }, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value!.EventName.Should().Be(originalName);
        result.Value.Description.Should().Be("Only desc");
    }

    [Fact]
    public async Task Handle_ValidUpdate_SetsUpdatedAt()
    {
        var evt = EventDataFixture.CreateDraftEvent();
        _eventRepo.Setup(x => x.GetByIdAsync(evt.Id, It.IsAny<CancellationToken>())).ReturnsAsync(evt);

        var before = DateTime.UtcNow;
        await _handler.Handle(new UpdateEventCommand { EventId = evt.Id, EventName = "Updated" }, CancellationToken.None);

        evt.UpdatedAt.Should().BeOnOrAfter(before);
    }

    [Fact]
    public async Task Handle_ValidUpdate_CallsUpdateAndSave()
    {
        var evt = EventDataFixture.CreateDraftEvent();
        _eventRepo.Setup(x => x.GetByIdAsync(evt.Id, It.IsAny<CancellationToken>())).ReturnsAsync(evt);

        await _handler.Handle(new UpdateEventCommand { EventId = evt.Id, EventName = "Updated" }, CancellationToken.None);

        _eventRepo.Verify(x => x.Update(It.IsAny<Domain.Entities.Event>()), Times.Once);
        _unitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}
