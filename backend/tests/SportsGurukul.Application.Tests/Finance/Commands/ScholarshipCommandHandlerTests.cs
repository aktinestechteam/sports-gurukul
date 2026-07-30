using FluentAssertions;
using Moq;
using SportsGurukul.Application.Common.Interfaces.Finance.Services;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.FinanceManagement.Commands.Scholarship;
using SportsGurukul.Application.Features.FinanceManagement.DTOs;
using SportsGurukul.Domain.Enums.Finance;

namespace SportsGurukul.Application.Tests.Finance.Commands;

public class CreateScholarshipCommandHandlerTests
{
    private readonly Mock<IDiscountService> _serviceMock;
    private readonly CreateScholarshipCommandHandler _handler;

    public CreateScholarshipCommandHandlerTests()
    {
        _serviceMock = new Mock<IDiscountService>();
        _handler = new CreateScholarshipCommandHandler(_serviceMock.Object);
    }

    [Fact]
    public async Task Handle_ValidRequest_ReturnsSuccess()
    {
        var athleteId = Guid.NewGuid();
        var command = new CreateScholarshipCommand(athleteId, "Merit", "Academic merit", DiscountType.Percentage, 25m, 5000m, DateTime.UtcNow, DateTime.UtcNow.AddYears(1));
        _serviceMock.Setup(s => s.ApplyScholarshipAsync(It.IsAny<decimal>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<DiscountResult>.Success(new DiscountResult("Merit", 250m, 750m)));

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        _serviceMock.Verify(s => s.ApplyScholarshipAsync(It.IsAny<decimal>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ServiceFailure_ReturnsSuccessWithDefaultScholarship()
    {
        var athleteId = Guid.NewGuid();
        var command = new CreateScholarshipCommand(athleteId, "Merit", null, DiscountType.Flat, 1000m, null, null, null);
        _serviceMock.Setup(s => s.ApplyScholarshipAsync(It.IsAny<decimal>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<DiscountResult>.Failure("Discount service error"));

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        _serviceMock.Verify(s => s.ApplyScholarshipAsync(It.IsAny<decimal>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Once);
    }
}

public class ApproveScholarshipCommandHandlerTests
{
    private readonly ApproveScholarshipCommandHandler _handler;

    public ApproveScholarshipCommandHandlerTests()
    {
        _handler = new ApproveScholarshipCommandHandler();
    }

    [Fact]
    public async Task Handle_ValidRequest_ReturnsSuccess()
    {
        var scholarshipId = Guid.NewGuid();
        var command = new ApproveScholarshipCommand(scholarshipId);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value!.Id.Should().Be(scholarshipId);
    }
}
