using FluentAssertions;
using Moq;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Features.CoachManagement.Queries.GetCoachProfile;
using SportsGurukul.Application.Tests.Common;
using SportsGurukul.Domain.Entities;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Application.Tests.Queries;

public class GetCoachProfileQueryHandlerTests
{
    private readonly Mock<ICoachRepository> _coachRepositoryMock = TestMocks.CreateCoachRepository();
    private readonly Mock<ICoachAvailabilityRepository> _availabilityRepositoryMock = TestMocks.CreateCoachAvailabilityRepository();
    private readonly Mock<ICoachCertificationRepository> _certificationRepositoryMock = TestMocks.CreateCoachCertificationRepository();
    private readonly Mock<IAthleteRepository> _athleteRepositoryMock = TestMocks.CreateAthleteRepository();
    private readonly Mock<IRepository<CoachAthlete>> _coachAthleteRepositoryMock = TestMocks.CreateCoachAthleteRepository();
    private readonly Mock<ILogger<GetCoachProfileQueryHandler>> _loggerMock = TestMocks.CreateLogger<GetCoachProfileQueryHandler>();
    private readonly GetCoachProfileQueryHandler _handler;

    public GetCoachProfileQueryHandlerTests()
    {
        _handler = new GetCoachProfileQueryHandler(
            _coachRepositoryMock.Object,
            _availabilityRepositoryMock.Object,
            _certificationRepositoryMock.Object,
            _athleteRepositoryMock.Object,
            _coachAthleteRepositoryMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_CoachNotFound_ReturnsFailure()
    {
        _coachRepositoryMock.Setup(r => r.GetByIdWithDetailsAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Coach?)null);

        var result = await _handler.Handle(new GetCoachProfileQuery { CoachId = Guid.NewGuid() }, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Coach not found.");
    }

    [Fact]
    public async Task Handle_CoachFound_ReturnsFullProfile()
    {
        var coach = TestDataBuilder.CreateCoachWithDetails();
        var certifications = new List<CoachCertification>
        {
            TestDataBuilder.CreateCoachCertification(coach.Id)
        };
        var availability = TestDataBuilder.CreateCoachAvailability(coach.Id);

        _coachRepositoryMock.Setup(r => r.GetByIdWithDetailsAsync(coach.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(coach);
        _certificationRepositoryMock.Setup(r => r.GetByCoachIdAsync(coach.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(certifications);
        _availabilityRepositoryMock.Setup(r => r.GetByCoachIdAsync(coach.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(availability);
        _coachAthleteRepositoryMock.Setup(r => r.FindAsync(
            It.IsAny<System.Linq.Expressions.Expression<Func<CoachAthlete, bool>>>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<CoachAthlete>());

        var result = await _handler.Handle(new GetCoachProfileQuery { CoachId = coach.Id }, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value!.Coach.Should().NotBeNull();
        result.Value.Coach.Id.Should().Be(coach.Id);
        result.Value.Sports.Should().NotBeNull();
        result.Value.Certifications.Should().HaveCount(1);
        result.Value.Experiences.Should().HaveCount(1);
        result.Value.Education.Should().HaveCount(1);
        result.Value.Availability.Should().NotBeNull();
        result.Value.Location.Should().NotBeNull();
        result.Value.AssignedAthletes.Should().BeEmpty();
    }

    [Fact]
    public async Task Handle_CoachWithAssignedAthletes_ReturnsAthletesInProfile()
    {
        var coach = TestDataBuilder.CreateCoachWithDetails();
        var athlete = TestDataBuilder.CreateAthlete();
        var coachAthlete = new CoachAthlete
        {
            Id = Guid.NewGuid(),
            CoachId = coach.Id,
            AthleteId = athlete.Id,
            AssignedDate = DateTime.UtcNow,
            IsActive = true,
            Athlete = athlete
        };

        _coachRepositoryMock.Setup(r => r.GetByIdWithDetailsAsync(coach.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(coach);
        _certificationRepositoryMock.Setup(r => r.GetByCoachIdAsync(coach.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<CoachCertification>());
        _availabilityRepositoryMock.Setup(r => r.GetByCoachIdAsync(coach.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync((CoachAvailability?)null);
        _coachAthleteRepositoryMock.Setup(r => r.FindAsync(
            It.IsAny<System.Linq.Expressions.Expression<Func<CoachAthlete, bool>>>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<CoachAthlete> { coachAthlete });
        _athleteRepositoryMock.Setup(r => r.GetByIdWithDetailsAsync(athlete.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(athlete);

        var result = await _handler.Handle(new GetCoachProfileQuery { CoachId = coach.Id }, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value!.AssignedAthletes.Should().HaveCount(1);
        result.Value.AssignedAthletes[0].AthleteId.Should().Be(athlete.Id);
    }
}
