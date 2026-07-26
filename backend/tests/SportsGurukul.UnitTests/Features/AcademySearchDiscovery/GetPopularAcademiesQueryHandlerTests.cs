using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Features.AcademySearchDiscovery.Queries.GetPopularAcademies;
using SportsGurukul.Domain.Entities;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.UnitTests.Features.AcademySearchDiscovery;

public class GetPopularAcademiesQueryHandlerTests
{
    private readonly Mock<IAcademySearchRepository> _academySearchRepositoryMock;
    private readonly Mock<ILogger<GetPopularAcademiesQueryHandler>> _loggerMock;
    private readonly GetPopularAcademiesQueryHandler _handler;

    public GetPopularAcademiesQueryHandlerTests()
    {
        _academySearchRepositoryMock = new Mock<IAcademySearchRepository>();
        _loggerMock = new Mock<ILogger<GetPopularAcademiesQueryHandler>>();
        _handler = new GetPopularAcademiesQueryHandler(
            _academySearchRepositoryMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_WithDefaultLimit_ReturnsPopularAcademies()
    {
        var academies = new List<Academy>
        {
            CreateTestAcademy("Popular Academy A"),
            CreateTestAcademy("Popular Academy B"),
            CreateTestAcademy("Popular Academy C")
        };

        _academySearchRepositoryMock
            .Setup(r => r.GetPopularAcademiesAsync(10, It.IsAny<CancellationToken>()))
            .ReturnsAsync(academies);

        var result = await _handler.Handle(
            new GetPopularAcademiesQuery { Limit = 10 },
            CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(3);
        result.Value!.First().Name.Should().Be("Popular Academy A");
    }

    private static Academy CreateTestAcademy(string name) => new()
    {
        Id = Guid.NewGuid(),
        Name = name,
        AcademyCode = $"ACD-{Guid.NewGuid().ToString()[8..16].ToUpper()}",
        Email = $"{name.ToLower().Replace(" ", "")}@example.com",
        Phone = "9876543210",
        Status = AcademyStatus.Active,
        VerificationStatus = VerificationStatus.Verified,
        Contact = new AcademyContact
        {
            Id = Guid.NewGuid(),
            City = "Mumbai",
            State = "Maharashtra",
            Country = "India"
        }
    };
}
