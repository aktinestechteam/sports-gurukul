using FluentAssertions;
using Moq;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Features.EventManagement.Queries.GetRegistrationsByEvent;
using SportsGurukul.Domain.Enums;
using SportsGurukul.Application.Tests.EventManagement.Mocks;

namespace SportsGurukul.Application.Tests.EventManagement.Queries;

public class GetRegistrationsByEventQueryHandlerTests
{
    private readonly Mock<IEventRegistrationRepository> _regRepo;
    private readonly Mock<ILogger<GetRegistrationsByEventQueryHandler>> _logger;
    private readonly GetRegistrationsByEventQueryHandler _handler;

    public GetRegistrationsByEventQueryHandlerTests()
    {
        _regRepo = EventMockFactory.CreateRegistrationRepository();
        _logger = EventMockFactory.CreateLogger<GetRegistrationsByEventQueryHandler>();
        _handler = new GetRegistrationsByEventQueryHandler(_regRepo.Object, _logger.Object);
    }

    [Fact]
    public async Task Handle_ReturnsPagedResults()
    {
        _regRepo.Setup(x => x.SearchAsync(It.IsAny<Guid?>(), It.IsAny<EventRegistrationStatus?>(), It.IsAny<string?>(),
            It.IsAny<int>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Domain.Entities.EventRegistration>());
        _regRepo.Setup(x => x.CountSearchAsync(It.IsAny<Guid?>(), It.IsAny<EventRegistrationStatus?>(), It.IsAny<string?>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(0);

        var result = await _handler.Handle(new GetRegistrationsByEventQuery { EventId = Guid.NewGuid() }, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value!.Items.Should().BeEmpty();
    }

    [Fact]
    public async Task Handle_WithFilters_CallsRepository()
    {
        var eventId = Guid.NewGuid();
        _regRepo.Setup(x => x.SearchAsync(eventId, EventRegistrationStatus.Approved, It.IsAny<string?>(),
            It.IsAny<int>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Domain.Entities.EventRegistration>());
        _regRepo.Setup(x => x.CountSearchAsync(eventId, EventRegistrationStatus.Approved, It.IsAny<string?>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(0);

        var result = await _handler.Handle(new GetRegistrationsByEventQuery { EventId = eventId, Status = EventRegistrationStatus.Approved }, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
    }
}
