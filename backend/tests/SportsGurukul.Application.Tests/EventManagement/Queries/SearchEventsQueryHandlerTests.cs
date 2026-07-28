using FluentAssertions;
using Moq;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Features.EventManagement.Queries.SearchEvents;
using SportsGurukul.Application.Tests.EventManagement.Mocks;

namespace SportsGurukul.Application.Tests.EventManagement.Queries;

public class SearchEventsQueryHandlerTests
{
    private readonly Mock<IEventRepository> _eventRepo;
    private readonly Mock<ILogger<SearchEventsQueryHandler>> _logger;
    private readonly SearchEventsQueryHandler _handler;

    public SearchEventsQueryHandlerTests()
    {
        _eventRepo = EventMockFactory.CreateEventRepository();
        _logger = EventMockFactory.CreateLogger<SearchEventsQueryHandler>();
        _handler = new SearchEventsQueryHandler(_eventRepo.Object, _logger.Object);
    }

    [Fact]
    public async Task Handle_ReturnsPagedResults()
    {
        _eventRepo.Setup(x => x.SearchAsync(It.IsAny<Guid?>(), It.IsAny<Guid?>(), It.IsAny<Domain.Enums.EventStatus?>(),
            It.IsAny<Domain.Enums.EventType?>(), It.IsAny<string?>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Domain.Entities.Event>());
        _eventRepo.Setup(x => x.CountSearchAsync(It.IsAny<Guid?>(), It.IsAny<Guid?>(), It.IsAny<Domain.Enums.EventStatus?>(),
            It.IsAny<Domain.Enums.EventType?>(), It.IsAny<string?>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(0);

        var result = await _handler.Handle(new SearchEventsQuery { Page = 1, PageSize = 10 }, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value!.Items.Should().BeEmpty();
        result.Value.TotalRecords.Should().Be(0);
        result.Value.CurrentPage.Should().Be(1);
        result.Value.PageSize.Should().Be(10);
    }

    [Fact]
    public async Task Handle_WithFilters_CallsRepositoryWithFilters()
    {
        var academyId = Guid.NewGuid();
        _eventRepo.Setup(x => x.SearchAsync(academyId, It.IsAny<Guid?>(), It.IsAny<Domain.Enums.EventStatus?>(),
            It.IsAny<Domain.Enums.EventType?>(), It.IsAny<string?>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Domain.Entities.Event>());
        _eventRepo.Setup(x => x.CountSearchAsync(academyId, It.IsAny<Guid?>(), It.IsAny<Domain.Enums.EventStatus?>(),
            It.IsAny<Domain.Enums.EventType?>(), It.IsAny<string?>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(0);

        await _handler.Handle(new SearchEventsQuery { AcademyId = academyId, Page = 1, PageSize = 10 }, CancellationToken.None);

        _eventRepo.Verify(x => x.SearchAsync(academyId, It.IsAny<Guid?>(), It.IsAny<Domain.Enums.EventStatus?>(),
            It.IsAny<Domain.Enums.EventType?>(), It.IsAny<string?>(), 1, 10, It.IsAny<CancellationToken>()), Times.Once);
    }
}
