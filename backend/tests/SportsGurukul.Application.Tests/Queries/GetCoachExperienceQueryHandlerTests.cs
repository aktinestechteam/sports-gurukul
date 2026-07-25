using FluentAssertions;
using Moq;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Features.CoachManagement.Queries.GetCoachExperience;
using SportsGurukul.Application.Tests.Common;
using SportsGurukul.Domain.Entities;

namespace SportsGurukul.Application.Tests.Queries;

public class GetCoachExperienceQueryHandlerTests
{
    private readonly Mock<IRepository<CoachExperience>> _experienceRepositoryMock = TestMocks.CreateCoachExperienceRepository();
    private readonly Mock<ILogger<GetCoachExperienceQueryHandler>> _loggerMock = TestMocks.CreateLogger<GetCoachExperienceQueryHandler>();
    private readonly GetCoachExperienceQueryHandler _handler;

    public GetCoachExperienceQueryHandlerTests()
    {
        _handler = new GetCoachExperienceQueryHandler(
            _experienceRepositoryMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_ReturnsExperiences()
    {
        var coachId = Guid.NewGuid();
        var experiences = new List<CoachExperience>
        {
            TestDataBuilder.CreateCoachExperience(coachId)
        };

        _experienceRepositoryMock.Setup(r => r.FindAsync(
            It.IsAny<System.Linq.Expressions.Expression<Func<CoachExperience, bool>>>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(experiences);

        var result = await _handler.Handle(new GetCoachExperienceQuery { CoachId = coachId }, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(1);
        result.Value![0].Organization.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task Handle_NoExperiences_ReturnsEmptyList()
    {
        var coachId = Guid.NewGuid();
        _experienceRepositoryMock.Setup(r => r.FindAsync(
            It.IsAny<System.Linq.Expressions.Expression<Func<CoachExperience, bool>>>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<CoachExperience>());

        var result = await _handler.Handle(new GetCoachExperienceQuery { CoachId = coachId }, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEmpty();
    }
}
