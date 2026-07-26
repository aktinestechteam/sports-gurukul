using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Features.AcademyManagement.Commands.CreateMembershipPlan;
using SportsGurukul.Domain.Entities;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.UnitTests.Features.AcademyManagement.Commands;

public class CreateMembershipPlanCommandHandlerTests
{
    private readonly Mock<IAcademyRepository> _academyRepositoryMock;
    private readonly Mock<IAcademyMembershipRepository> _academyMembershipRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<ILogger<CreateMembershipPlanCommandHandler>> _loggerMock;
    private readonly CreateMembershipPlanCommandHandler _handler;

    public CreateMembershipPlanCommandHandlerTests()
    {
        _academyRepositoryMock = new Mock<IAcademyRepository>();
        _academyMembershipRepositoryMock = new Mock<IAcademyMembershipRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _loggerMock = new Mock<ILogger<CreateMembershipPlanCommandHandler>>();
        _handler = new CreateMembershipPlanCommandHandler(
            _academyRepositoryMock.Object,
            _academyMembershipRepositoryMock.Object,
            _unitOfWorkMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_ValidRequest_ReturnsSuccess()
    {
        var academyId = Guid.NewGuid();
        var academy = CreateTestAcademy(academyId);

        _academyRepositoryMock
            .Setup(r => r.GetByIdAsync(academyId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(academy);
        _academyMembershipRepositoryMock
            .Setup(r => r.GetByAcademyIdAsync(academyId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<AcademyMembership>());
        _academyMembershipRepositoryMock
            .Setup(r => r.AddAsync(It.IsAny<AcademyMembership>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((AcademyMembership m, CancellationToken _) => m);
        _unitOfWorkMock
            .Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var command = new CreateMembershipPlanCommand
        {
            AcademyId = academyId,
            MembershipName = "Premium Plan",
            Description = "Premium membership plan",
            Price = 999.99m,
            Duration = 30,
            Benefits = "Access to all facilities"
        };

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value!.MembershipName.Should().Be("Premium Plan");
        result.Value.Price.Should().Be(999.99m);
        result.Value.Duration.Should().Be(30);
        result.Value.AcademyId.Should().Be(academyId);
        _academyMembershipRepositoryMock.Verify(r => r.AddAsync(It.IsAny<AcademyMembership>(), It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_AcademyNotFound_ReturnsFailure()
    {
        var academyId = Guid.NewGuid();

        _academyRepositoryMock
            .Setup(r => r.GetByIdAsync(academyId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Academy?)null);

        var command = new CreateMembershipPlanCommand
        {
            AcademyId = academyId,
            MembershipName = "Premium Plan",
            Price = 999.99m,
            Duration = 30
        };

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Academy not found.");
    }

    [Fact]
    public async Task Handle_DuplicatePlanName_ReturnsFailure()
    {
        var academyId = Guid.NewGuid();
        var academy = CreateTestAcademy(academyId);
        var existingPlans = new List<AcademyMembership>
        {
            new()
            {
                Id = Guid.NewGuid(),
                AcademyId = academyId,
                MembershipName = "Premium Plan",
                Price = 999.99m,
                Duration = 30,
                Status = AcademyMembershipStatus.Active
            }
        };

        _academyRepositoryMock
            .Setup(r => r.GetByIdAsync(academyId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(academy);
        _academyMembershipRepositoryMock
            .Setup(r => r.GetByAcademyIdAsync(academyId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingPlans);

        var command = new CreateMembershipPlanCommand
        {
            AcademyId = academyId,
            MembershipName = "Premium Plan",
            Price = 1999.99m,
            Duration = 60
        };

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("same name");
    }

    private static Academy CreateTestAcademy(Guid? id = null) => new()
    {
        Id = id ?? Guid.NewGuid(),
        AcademyCode = "ACAD-20260725-TEST",
        Name = "Test Academy",
        Email = "test@test.com",
        Phone = "1234567890",
        Status = AcademyStatus.Active,
        VerificationStatus = VerificationStatus.Pending,
        CreatedAt = DateTime.UtcNow,
        UpdatedAt = DateTime.UtcNow,
        Branches = new List<AcademyBranch>(),
        AcademySports = new List<AcademySport>(),
        Facilities = new List<AcademyFacility>(),
        Memberships = new List<AcademyMembership>(),
        Documents = new List<AcademyDocument>(),
        GalleryImages = new List<AcademyGallery>()
    };
}
