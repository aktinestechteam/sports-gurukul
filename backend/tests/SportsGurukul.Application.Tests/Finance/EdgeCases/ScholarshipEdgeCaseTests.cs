using FluentAssertions;
using Moq;
using SportsGurukul.Application.Common.Interfaces.Finance.Services;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.FinanceManagement.Commands.Scholarship;
using SportsGurukul.Application.Features.FinanceManagement.DTOs;
using SportsGurukul.Domain.Enums.Finance;

namespace SportsGurukul.Application.Tests.Finance.EdgeCases;

public class CreateScholarshipEdgeCaseTests
{
    private readonly Mock<IDiscountService> _discountServiceMock;
    private readonly CreateScholarshipCommandHandler _handler;

    public CreateScholarshipEdgeCaseTests()
    {
        _discountServiceMock = new Mock<IDiscountService>();
        _handler = new CreateScholarshipCommandHandler(_discountServiceMock.Object);
    }

    [Fact]
    public async Task CreateScholarship_ZeroValue_ShouldSucceed()
    {
        var athleteId = Guid.NewGuid();
        var command = new CreateScholarshipCommand(athleteId, "Zero Scholarship", "No discount",
            DiscountType.Flat, 0m, null, DateTime.UtcNow, DateTime.UtcNow.AddDays(365));

        var discountResult = new DiscountResult("Zero Scholarship", 0m, 0m);
        _discountServiceMock.Setup(s => s.ApplyScholarshipAsync(0m, Guid.Empty, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<DiscountResult>.Success(discountResult));

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Value.Should().Be(0m);
        _discountServiceMock.Verify(s => s.ApplyScholarshipAsync(0m, Guid.Empty, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreateScholarship_VeryHighValue_ShouldSucceed()
    {
        var athleteId = Guid.NewGuid();
        var command = new CreateScholarshipCommand(athleteId, "Mega Scholarship", "Full ride",
            DiscountType.Flat, 10_000_000m, 10_000_000m, DateTime.UtcNow, DateTime.UtcNow.AddDays(365));

        var discountResult = new DiscountResult("Mega Scholarship", 10_000_000m, 0m);
        _discountServiceMock.Setup(s => s.ApplyScholarshipAsync(10_000_000m, Guid.Empty, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<DiscountResult>.Success(discountResult));

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Value.Should().Be(10_000_000m);
        result.Value.MaxAmount.Should().Be(10_000_000m);
        _discountServiceMock.Verify(s => s.ApplyScholarshipAsync(10_000_000m, Guid.Empty, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreateScholarship_DuplicateAthlete_ShouldSucceedWithPlaceholderLogic()
    {
        var athleteId = Guid.NewGuid();
        var command = new CreateScholarshipCommand(athleteId, "Second Scholarship", "Duplicate",
            DiscountType.Percentage, 10m, 5000m, DateTime.UtcNow, DateTime.UtcNow.AddDays(365));

        var discountResult = new DiscountResult("Second Scholarship", 0m, 0m);
        _discountServiceMock.Setup(s => s.ApplyScholarshipAsync(10m, Guid.Empty, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<DiscountResult>.Success(discountResult));

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.AthleteId.Should().Be(athleteId);
        _discountServiceMock.Verify(s => s.ApplyScholarshipAsync(10m, Guid.Empty, It.IsAny<CancellationToken>()), Times.Once);
    }
}
