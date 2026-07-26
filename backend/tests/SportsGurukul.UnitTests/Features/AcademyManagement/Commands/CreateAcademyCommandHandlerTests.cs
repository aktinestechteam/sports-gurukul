using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Features.AcademyManagement.Commands.CreateAcademy;
using SportsGurukul.Domain.Entities;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.UnitTests.Features.AcademyManagement.Commands;

public class CreateAcademyCommandHandlerTests
{
    private readonly Mock<IAcademyRepository> _academyRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<ILogger<CreateAcademyCommandHandler>> _loggerMock;
    private readonly CreateAcademyCommandHandler _handler;

    public CreateAcademyCommandHandlerTests()
    {
        _academyRepositoryMock = new Mock<IAcademyRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _loggerMock = new Mock<ILogger<CreateAcademyCommandHandler>>();
        _handler = new CreateAcademyCommandHandler(
            _academyRepositoryMock.Object,
            _unitOfWorkMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_ValidRequest_ReturnsSuccess()
    {
        _academyRepositoryMock
            .Setup(r => r.GetByEmailAsync("test@test.com", It.IsAny<CancellationToken>()))
            .ReturnsAsync((Academy?)null);
        _academyRepositoryMock
            .Setup(r => r.AnyAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Academy, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
        _academyRepositoryMock
            .Setup(r => r.AddAsync(It.IsAny<Academy>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Academy a, CancellationToken _) => a);
        _unitOfWorkMock
            .Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var command = new CreateAcademyCommand
        {
            Name = "Test Academy",
            Email = "test@test.com",
            Phone = "1234567890"
        };

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value!.Name.Should().Be("Test Academy");
        result.Value.Email.Should().Be("test@test.com");
        result.Value.Phone.Should().Be("1234567890");
        _academyRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Academy>(), It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_DuplicateEmail_ReturnsFailure()
    {
        var existingAcademy = CreateTestAcademy();

        _academyRepositoryMock
            .Setup(r => r.GetByEmailAsync("test@test.com", It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingAcademy);

        var command = new CreateAcademyCommand
        {
            Name = "Test Academy",
            Email = "test@test.com",
            Phone = "1234567890"
        };

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("email");
        _academyRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Academy>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_VerifiesAcademyCodeGeneration()
    {
        _academyRepositoryMock
            .Setup(r => r.GetByEmailAsync("test@test.com", It.IsAny<CancellationToken>()))
            .ReturnsAsync((Academy?)null);
        _academyRepositoryMock
            .Setup(r => r.AnyAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Academy, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
        _academyRepositoryMock
            .Setup(r => r.AddAsync(It.IsAny<Academy>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Academy a, CancellationToken _) => a);
        _unitOfWorkMock
            .Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var command = new CreateAcademyCommand
        {
            Name = "Test Academy",
            Email = "test@test.com",
            Phone = "1234567890"
        };

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value!.AcademyCode.Should().StartWith("ACAD-");
    }

    [Fact]
    public async Task Handle_VerifiesInitialStatusIsPending()
    {
        _academyRepositoryMock
            .Setup(r => r.GetByEmailAsync("test@test.com", It.IsAny<CancellationToken>()))
            .ReturnsAsync((Academy?)null);
        _academyRepositoryMock
            .Setup(r => r.AnyAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Academy, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
        _academyRepositoryMock
            .Setup(r => r.AddAsync(It.IsAny<Academy>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Academy a, CancellationToken _) => a);
        _unitOfWorkMock
            .Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var command = new CreateAcademyCommand
        {
            Name = "Test Academy",
            Email = "test@test.com",
            Phone = "1234567890"
        };

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value!.Status.Should().Be("Pending");
        result.Value.VerificationStatus.Should().Be("Pending");
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
