using FluentAssertions;
using Moq;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Features.EventManagement.Commands.CreateEvent;
using SportsGurukul.Application.Features.EventManagement.Services;
using SportsGurukul.Application.Tests.EventManagement.Mocks;

namespace SportsGurukul.Application.Tests.EventManagement.Performance;

public class EventManagementPerformanceTests
{
    [Fact]
    public async Task CreateEvent_MinimalRepositoryCalls()
    {
        var eventRepo = EventMockFactory.CreateEventRepository();
        var lifecycleService = EventMockFactory.CreateLifecycleService();
        var unitOfWork = EventMockFactory.CreateUnitOfWork();
        var logger = EventMockFactory.CreateLogger<CreateEventCommandHandler>();

        lifecycleService.Setup(x => x.GenerateEventCodeAsync(It.IsAny<CancellationToken>())).ReturnsAsync("EVT-001");

        var handler = new CreateEventCommandHandler(eventRepo.Object, lifecycleService.Object, unitOfWork.Object, logger.Object);
        var command = new CreateEventCommand
        {
            EventName = "Perf Test",
            AcademyId = Guid.NewGuid(),
            SportId = Guid.NewGuid(),
            EventTypeId = Guid.NewGuid(),
            StartDate = DateTime.UtcNow.AddDays(30),
            EndDate = DateTime.UtcNow.AddDays(37),
            RegistrationOpenDate = DateTime.UtcNow.AddDays(1),
            RegistrationCloseDate = DateTime.UtcNow.AddDays(25)
        };

        var sw = System.Diagnostics.Stopwatch.StartNew();
        await handler.Handle(command, CancellationToken.None);
        sw.Stop();

        sw.ElapsedMilliseconds.Should().BeLessThan(1000);
        eventRepo.Verify(x => x.AddAsync(It.IsAny<Domain.Entities.Event>(), It.IsAny<CancellationToken>()), Times.Once);
        unitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetEventById_MinimalRepositoryCalls()
    {
        var eventRepo = EventMockFactory.CreateEventRepository();
        var logger = EventMockFactory.CreateLogger<Features.EventManagement.Queries.GetEventById.GetEventByIdQueryHandler>();
        var handler = new Features.EventManagement.Queries.GetEventById.GetEventByIdQueryHandler(eventRepo.Object, logger.Object);

        eventRepo.Setup(x => x.GetWithDetailsAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Fixtures.EventDataFixture.CreateDraftEvent());

        var sw = System.Diagnostics.Stopwatch.StartNew();
        await handler.Handle(new Features.EventManagement.Queries.GetEventById.GetEventByIdQuery { EventId = Guid.NewGuid() }, CancellationToken.None);
        sw.Stop();

        sw.ElapsedMilliseconds.Should().BeLessThan(1000);
        eventRepo.Verify(x => x.GetWithDetailsAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task LifecycleService_StateTransitionCheck_IsFast()
    {
        var eventRepo = EventMockFactory.CreateEventRepository();
        var logger = EventMockFactory.CreateLogger<EventLifecycleService>();
        var service = new EventLifecycleService(eventRepo.Object, logger.Object);

        var sw = System.Diagnostics.Stopwatch.StartNew();
        for (int i = 0; i < 1000; i++)
        {
            await service.ValidateStateTransitionAsync(Domain.Enums.EventStatus.Draft, Domain.Enums.EventStatus.Published);
        }
        sw.Stop();

        sw.ElapsedMilliseconds.Should().BeLessThan(1000);
    }

    [Fact]
    public async Task RegistrationService_MultipleChecks_NoRepositoryLeak()
    {
        var eventRepo = EventMockFactory.CreateEventRepository();
        var regRepo = EventMockFactory.CreateRegistrationRepository();
        var logger = EventMockFactory.CreateLogger<EventRegistrationService>();
        var service = new EventRegistrationService(eventRepo.Object, regRepo.Object, logger.Object);

        var evt = Fixtures.EventDataFixture.CreateRegistrationOpenEvent();
        evt.MaxParticipants = 100;
        regRepo.Setup(x => x.GetRegistrationCountAsync(evt.Id, It.IsAny<CancellationToken>())).ReturnsAsync(50);

        var result = await service.IsCapacityAvailableAsync(evt);

        result.Should().BeTrue();
        regRepo.Verify(x => x.GetRegistrationCountAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Once);
    }
}
