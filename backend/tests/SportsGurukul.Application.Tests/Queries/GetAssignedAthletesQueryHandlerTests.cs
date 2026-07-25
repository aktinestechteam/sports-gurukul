using FluentAssertions;
using Moq;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Features.CoachManagement.Queries.GetAssignedAthletes;
using SportsGurukul.Application.Tests.Common;
using SportsGurukul.Domain.Entities;

namespace SportsGurukul.Application.Tests.Queries;

public class GetAssignedAthletesQueryHandlerTests
{
    private readonly Mock<IRepository<CoachAthlete>> _coachAthleteRepositoryMock = TestMocks.CreateCoachAthleteRepository();
    private readonly Mock<IAthleteRepository> _athleteRepositoryMock = TestMocks.CreateAthleteRepository();
    private readonly Mock<ILogger<GetAssignedAthletesQueryHandler>> _loggerMock = TestMocks.CreateLogger<GetAssignedAthletesQueryHandler>();
    private readonly GetAssignedAthletesQueryHandler _handler;

    public GetAssignedAthletesQueryHandlerTests()
    {
        _handler = new GetAssignedAthletesQueryHandler(
            _coachAthleteRepositoryMock.Object,
            _athleteRepositoryMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_NoAssignments_ReturnsEmptyList()
    {
        _coachAthleteRepositoryMock.Setup(r => r.FindAsync(
            It.IsAny<System.Linq.Expressions.Expression<Func<CoachAthlete, bool>>>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<CoachAthlete>());

        var result = await _handler.Handle(new GetAssignedAthletesQuery { CoachId = Guid.NewGuid() }, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEmpty();
    }

    [Fact]
    public async Task Handle_WithAssignments_ReturnsAthletes()
    {
        var coachId = Guid.NewGuid();
        var athleteId = Guid.NewGuid();
        var athlete = TestDataBuilder.CreateAthlete(id: athleteId);
        var coachAthlete = TestDataBuilder.CreateCoachAthlete(coachId, athleteId);
        coachAthlete.Athlete = athlete;
        coachAthlete.IsActive = true;

        _coachAthleteRepositoryMock.Setup(r => r.FindAsync(
            It.IsAny<System.Linq.Expressions.Expression<Func<CoachAthlete, bool>>>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<CoachAthlete> { coachAthlete });
        _athleteRepositoryMock.Setup(r => r.GetByIdWithDetailsAsync(athleteId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(athlete);

        var result = await _handler.Handle(new GetAssignedAthletesQuery { CoachId = coachId }, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(1);
        result.Value![0].AthleteId.Should().Be(athleteId);
        result.Value[0].FullName.Should().Be(athlete.User.FullName);
    }
}
