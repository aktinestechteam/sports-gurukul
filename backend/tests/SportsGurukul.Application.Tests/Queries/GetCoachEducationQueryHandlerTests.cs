using FluentAssertions;
using Moq;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Features.CoachManagement.Queries.GetCoachEducation;
using SportsGurukul.Application.Tests.Common;
using SportsGurukul.Domain.Entities;

namespace SportsGurukul.Application.Tests.Queries;

public class GetCoachEducationQueryHandlerTests
{
    private readonly Mock<IRepository<CoachEducation>> _educationRepositoryMock = TestMocks.CreateCoachEducationRepository();
    private readonly Mock<ILogger<GetCoachEducationQueryHandler>> _loggerMock = TestMocks.CreateLogger<GetCoachEducationQueryHandler>();
    private readonly GetCoachEducationQueryHandler _handler;

    public GetCoachEducationQueryHandlerTests()
    {
        _handler = new GetCoachEducationQueryHandler(
            _educationRepositoryMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_ReturnsEducation()
    {
        var coachId = Guid.NewGuid();
        var education = new List<CoachEducation>
        {
            TestDataBuilder.CreateCoachEducation(coachId)
        };

        _educationRepositoryMock.Setup(r => r.FindAsync(
            It.IsAny<System.Linq.Expressions.Expression<Func<CoachEducation, bool>>>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(education);

        var result = await _handler.Handle(new GetCoachEducationQuery { CoachId = coachId }, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(1);
        result.Value![0].Degree.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task Handle_NoEducation_ReturnsEmptyList()
    {
        var coachId = Guid.NewGuid();
        _educationRepositoryMock.Setup(r => r.FindAsync(
            It.IsAny<System.Linq.Expressions.Expression<Func<CoachEducation, bool>>>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<CoachEducation>());

        var result = await _handler.Handle(new GetCoachEducationQuery { CoachId = coachId }, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEmpty();
    }
}
