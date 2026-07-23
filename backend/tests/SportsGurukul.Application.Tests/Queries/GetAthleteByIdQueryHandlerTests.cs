using FluentAssertions;
using Moq;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Features.AthleteManagement.Queries.GetAthleteById;
using SportsGurukul.Application.Tests.Common;
using SportsGurukul.Domain.Entities;

namespace SportsGurukul.Application.Tests.Queries;

public class GetAthleteByIdQueryHandlerTests
{
    private readonly Mock<IAthleteRepository> _athleteRepositoryMock = TestMocks.CreateAthleteRepository();
    private readonly Mock<ILogger<GetAthleteByIdQueryHandler>> _loggerMock = TestMocks.CreateLogger<GetAthleteByIdQueryHandler>();
    private readonly GetAthleteByIdQueryHandler _handler;

    public GetAthleteByIdQueryHandlerTests()
    {
        _handler = new GetAthleteByIdQueryHandler(_athleteRepositoryMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_AthleteNotFound_ReturnsFailure()
    {
        _athleteRepositoryMock.Setup(r => r.GetByIdWithDetailsAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Athlete?)null);

        var result = await _handler.Handle(new GetAthleteByIdQuery { AthleteId = Guid.NewGuid() }, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Athlete not found.");
    }

    [Fact]
    public async Task Handle_AthleteExists_ReturnsSuccess()
    {
        var athlete = TestDataBuilder.CreateAthleteWithDetails();
        _athleteRepositoryMock.Setup(r => r.GetByIdWithDetailsAsync(athlete.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(athlete);

        var result = await _handler.Handle(new GetAthleteByIdQuery { AthleteId = athlete.Id }, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value!.Id.Should().Be(athlete.Id);
        result.Value.AthleteCode.Should().Be(athlete.AthleteCode);
    }
}
