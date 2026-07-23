using FluentAssertions;
using Moq;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Features.AthleteManagement.Queries.GetAthleteProfile;
using SportsGurukul.Application.Tests.Common;
using SportsGurukul.Domain.Entities;

namespace SportsGurukul.Application.Tests.Queries;

public class GetAthleteProfileQueryHandlerTests
{
    private readonly Mock<IAthleteRepository> _athleteRepositoryMock = TestMocks.CreateAthleteRepository();
    private readonly Mock<ILogger<GetAthleteProfileQueryHandler>> _loggerMock = TestMocks.CreateLogger<GetAthleteProfileQueryHandler>();
    private readonly GetAthleteProfileQueryHandler _handler;

    public GetAthleteProfileQueryHandlerTests()
    {
        _handler = new GetAthleteProfileQueryHandler(_athleteRepositoryMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_AthleteNotFound_ReturnsFailure()
    {
        _athleteRepositoryMock.Setup(r => r.GetByIdWithDetailsAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Athlete?)null);

        var result = await _handler.Handle(new GetAthleteProfileQuery { AthleteId = Guid.NewGuid() }, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Athlete profile not found.");
    }

    [Fact]
    public async Task Handle_AthleteExists_ReturnsSuccess()
    {
        var athlete = TestDataBuilder.CreateAthleteWithDetails();
        _athleteRepositoryMock.Setup(r => r.GetByIdWithDetailsAsync(athlete.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(athlete);

        var result = await _handler.Handle(new GetAthleteProfileQuery { AthleteId = athlete.Id }, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value!.Id.Should().Be(athlete.Id);
    }
}
