using FluentAssertions;
using Moq;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Features.CoachManagement.Queries.GetCoachById;
using SportsGurukul.Application.Tests.Common;
using SportsGurukul.Domain.Entities;

namespace SportsGurukul.Application.Tests.Queries;

public class GetCoachByIdQueryHandlerTests
{
    private readonly Mock<ICoachRepository> _coachRepositoryMock = TestMocks.CreateCoachRepository();
    private readonly Mock<ILogger<GetCoachByIdQueryHandler>> _loggerMock = TestMocks.CreateLogger<GetCoachByIdQueryHandler>();
    private readonly GetCoachByIdQueryHandler _handler;

    public GetCoachByIdQueryHandlerTests()
    {
        _handler = new GetCoachByIdQueryHandler(
            _coachRepositoryMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_CoachNotFound_ReturnsFailure()
    {
        _coachRepositoryMock.Setup(r => r.GetByIdWithDetailsAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Coach?)null);

        var result = await _handler.Handle(new GetCoachByIdQuery { CoachId = Guid.NewGuid() }, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Coach not found.");
    }

    [Fact]
    public async Task Handle_CoachFound_ReturnsSuccessWithCorrectDto()
    {
        var coach = TestDataBuilder.CreateCoachWithDetails();

        _coachRepositoryMock.Setup(r => r.GetByIdWithDetailsAsync(coach.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(coach);

        var result = await _handler.Handle(new GetCoachByIdQuery { CoachId = coach.Id }, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value!.Id.Should().Be(coach.Id);
        result.Value.UserId.Should().Be(coach.UserId);
        result.Value.CoachCode.Should().Be(coach.CoachCode);
        result.Value.FullName.Should().Be(coach.User.FullName);
        result.Value.Email.Should().Be(coach.User.Email);
        result.Value.Biography.Should().Be(coach.Biography);
        result.Value.YearsOfExperience.Should().Be(coach.YearsOfExperience);
        result.Value.CoachingLevel.Should().Be(coach.CoachingLevel.ToString());
        result.Value.Status.Should().Be(coach.Status.ToString());
        result.Value.VerificationStatus.Should().Be(coach.VerificationStatus.ToString());
    }

    [Fact]
    public async Task Handle_CoachFound_MapsSportsAndCertifications()
    {
        var coach = TestDataBuilder.CreateCoachWithDetails();

        _coachRepositoryMock.Setup(r => r.GetByIdWithDetailsAsync(coach.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(coach);

        var result = await _handler.Handle(new GetCoachByIdQuery { CoachId = coach.Id }, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value!.Sports.Should().HaveCount(1);
        result.Value.Certifications.Should().HaveCount(1);
        result.Value.Experiences.Should().HaveCount(1);
        result.Value.Education.Should().HaveCount(1);
        result.Value.Availability.Should().NotBeNull();
        result.Value.Location.Should().NotBeNull();
    }
}
