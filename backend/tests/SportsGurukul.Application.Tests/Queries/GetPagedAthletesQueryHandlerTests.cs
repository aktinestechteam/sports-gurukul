using FluentAssertions;
using Moq;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Features.AthleteManagement.Queries.GetPagedAthletes;
using SportsGurukul.Application.Features.AthleteManagement.DTOs;
using SportsGurukul.Application.Tests.Common;

namespace SportsGurukul.Application.Tests.Queries;

public class GetPagedAthletesQueryHandlerTests
{
    private readonly Mock<IAthleteRepository> _athleteRepositoryMock = TestMocks.CreateAthleteRepository();
    private readonly Mock<ILogger<GetPagedAthletesQueryHandler>> _loggerMock = TestMocks.CreateLogger<GetPagedAthletesQueryHandler>();
    private readonly GetPagedAthletesQueryHandler _handler;

    public GetPagedAthletesQueryHandlerTests()
    {
        _handler = new GetPagedAthletesQueryHandler(_athleteRepositoryMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_ReturnsPaginatedResults()
    {
        var athletes = new List<AthleteSummaryDto>
        {
            new() { Id = Guid.NewGuid(), FullName = "Athlete 1" }
        };
        _athleteRepositoryMock.Setup(r => r.SearchAthletesAsync(
            It.IsAny<AthleteSearchRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((athletes, 1));

        var result = await _handler.Handle(new GetPagedAthletesQuery
        {
            Page = 1,
            PageSize = 10
        }, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value!.Items.Should().HaveCount(1);
        result.Value.TotalRecords.Should().Be(1);
        result.Value.TotalPages.Should().Be(1);
    }

    [Fact]
    public async Task Handle_EmptyDatabase_ReturnsEmptyResponse()
    {
        _athleteRepositoryMock.Setup(r => r.SearchAthletesAsync(
            It.IsAny<AthleteSearchRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((new List<AthleteSummaryDto>(), 0));

        var result = await _handler.Handle(new GetPagedAthletesQuery(), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value!.Items.Should().BeEmpty();
        result.Value.TotalPages.Should().Be(0);
    }
}
