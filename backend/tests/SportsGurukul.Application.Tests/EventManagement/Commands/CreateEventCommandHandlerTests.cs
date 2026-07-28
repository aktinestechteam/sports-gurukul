using FluentAssertions;
using Moq;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Features.EventManagement.Commands.CreateEvent;
using SportsGurukul.Application.Features.EventManagement.Services;
using SportsGurukul.Domain.Enums;
using SportsGurukul.Application.Tests.EventManagement.Mocks;

namespace SportsGurukul.Application.Tests.EventManagement.Commands;

public class CreateEventCommandHandlerTests
{
    private readonly Mock<IEventRepository> _eventRepo;
    private readonly Mock<IEventLifecycleService> _lifecycleService;
    private readonly Mock<IUnitOfWork> _unitOfWork;
    private readonly Mock<ILogger<CreateEventCommandHandler>> _logger;
    private readonly CreateEventCommandHandler _handler;

    public CreateEventCommandHandlerTests()
    {
        _eventRepo = EventMockFactory.CreateEventRepository();
        _lifecycleService = EventMockFactory.CreateLifecycleService();
        _unitOfWork = EventMockFactory.CreateUnitOfWork();
        _logger = EventMockFactory.CreateLogger<CreateEventCommandHandler>();
        _handler = new CreateEventCommandHandler(_eventRepo.Object, _lifecycleService.Object, _unitOfWork.Object, _logger.Object);
    }

    [Fact]
    public async Task Handle_ValidRequest_ReturnsSuccess()
    {
        _lifecycleService.Setup(x => x.GenerateEventCodeAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync("EVT-20260728-0001");

        var command = new CreateEventCommand
        {
            EventName = "Summer Camp",
            AcademyId = Guid.NewGuid(),
            SportId = Guid.NewGuid(),
            EventTypeId = Guid.NewGuid(),
            StartDate = DateTime.UtcNow.AddDays(30),
            EndDate = DateTime.UtcNow.AddDays(37),
            RegistrationOpenDate = DateTime.UtcNow.AddDays(1),
            RegistrationCloseDate = DateTime.UtcNow.AddDays(25)
        };

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value!.EventName.Should().Be("Summer Camp");
        result.Value.Status.Should().Be(EventStatus.Draft.ToString());
    }

    [Fact]
    public async Task Handle_EndDateBeforeStartDate_ReturnsFailure()
    {
        var command = new CreateEventCommand
        {
            EventName = "Bad Dates",
            AcademyId = Guid.NewGuid(),
            SportId = Guid.NewGuid(),
            EventTypeId = Guid.NewGuid(),
            StartDate = DateTime.UtcNow.AddDays(37),
            EndDate = DateTime.UtcNow.AddDays(30),
            RegistrationOpenDate = DateTime.UtcNow.AddDays(1),
            RegistrationCloseDate = DateTime.UtcNow.AddDays(25)
        };

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("end date must be after start date");
    }

    [Fact]
    public async Task Handle_RegistrationCloseAfterStartDate_ReturnsFailure()
    {
        var command = new CreateEventCommand
        {
            EventName = "Bad Reg Close",
            AcademyId = Guid.NewGuid(),
            SportId = Guid.NewGuid(),
            EventTypeId = Guid.NewGuid(),
            StartDate = DateTime.UtcNow.AddDays(10),
            EndDate = DateTime.UtcNow.AddDays(17),
            RegistrationOpenDate = DateTime.UtcNow.AddDays(1),
            RegistrationCloseDate = DateTime.UtcNow.AddDays(15)
        };

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("Registration close date must be before event start date");
    }

    [Fact]
    public async Task Handle_CallsRepositoryAddAndSaveChanges()
    {
        _lifecycleService.Setup(x => x.GenerateEventCodeAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync("EVT-20260728-0002");

        var command = new CreateEventCommand
        {
            EventName = "Test Event",
            AcademyId = Guid.NewGuid(),
            SportId = Guid.NewGuid(),
            EventTypeId = Guid.NewGuid(),
            StartDate = DateTime.UtcNow.AddDays(30),
            EndDate = DateTime.UtcNow.AddDays(37),
            RegistrationOpenDate = DateTime.UtcNow.AddDays(1),
            RegistrationCloseDate = DateTime.UtcNow.AddDays(25)
        };

        await _handler.Handle(command, CancellationToken.None);

        _eventRepo.Verify(x => x.AddAsync(It.IsAny<Domain.Entities.Event>(), It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_GeneratesEventCode()
    {
        _lifecycleService.Setup(x => x.GenerateEventCodeAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync("EVT-20260728-0003");

        var command = new CreateEventCommand
        {
            EventName = "Code Test",
            AcademyId = Guid.NewGuid(),
            SportId = Guid.NewGuid(),
            EventTypeId = Guid.NewGuid(),
            StartDate = DateTime.UtcNow.AddDays(30),
            EndDate = DateTime.UtcNow.AddDays(37),
            RegistrationOpenDate = DateTime.UtcNow.AddDays(1),
            RegistrationCloseDate = DateTime.UtcNow.AddDays(25)
        };

        var result = await _handler.Handle(command, CancellationToken.None);

        _lifecycleService.Verify(x => x.GenerateEventCodeAsync(It.IsAny<CancellationToken>()), Times.Once);
        result.Value!.EventCode.Should().Be("EVT-20260728-0003");
    }
}
