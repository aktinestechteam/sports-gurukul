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
    private readonly Mock<ISportRepository> _sportRepositoryMock;
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly Mock<IRoleRepository> _roleRepositoryMock;
    private readonly Mock<IUserRoleRepository> _userRoleRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<ILogger<CreateAcademyCommandHandler>> _loggerMock;
    private readonly CreateAcademyCommandHandler _handler;

    public CreateAcademyCommandHandlerTests()
    {
        _academyRepositoryMock = new Mock<IAcademyRepository>();
        _sportRepositoryMock = new Mock<ISportRepository>();
        _userRepositoryMock = new Mock<IUserRepository>();
        _roleRepositoryMock = new Mock<IRoleRepository>();
        _userRoleRepositoryMock = new Mock<IUserRoleRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _loggerMock = new Mock<ILogger<CreateAcademyCommandHandler>>();
        _handler = new CreateAcademyCommandHandler(
            _academyRepositoryMock.Object,
            _sportRepositoryMock.Object,
            _userRepositoryMock.Object,
            _roleRepositoryMock.Object,
            _userRoleRepositoryMock.Object,
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

    [Fact]
    public async Task Handle_WithUserId_GrantsAcademyAdminRole()
    {
        var userId = Guid.NewGuid();
        var academyAdminRole = new Role
        {
            Id = Guid.NewGuid(),
            Name = "Academy Admin"
        };
        SetupSuccessfulAcademyCreation();
        _userRepositoryMock
            .Setup(r => r.GetByIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new User { Id = userId });
        _roleRepositoryMock
            .Setup(r => r.GetByNameAsync("Academy Admin", It.IsAny<CancellationToken>()))
            .ReturnsAsync(academyAdminRole);
        _userRoleRepositoryMock
            .Setup(r => r.GetByUserIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<UserRole>());

        var command = new CreateAcademyCommand
        {
            Name = "Test Academy",
            Email = "test@test.com",
            Phone = "1234567890",
            UserId = userId
        };

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        _userRoleRepositoryMock.Verify(
            r => r.AddAsync(It.Is<UserRole>(ur => ur.UserId == userId && ur.RoleId == academyAdminRole.Id),
                It.IsAny<CancellationToken>()),
            Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Exactly(2));
    }

    [Fact]
    public async Task Handle_UserAlreadyHasRole_DoesNotDuplicateRole()
    {
        var userId = Guid.NewGuid();
        var academyAdminRole = new Role
        {
            Id = Guid.NewGuid(),
            Name = "Academy Admin"
        };
        SetupSuccessfulAcademyCreation();
        _userRepositoryMock
            .Setup(r => r.GetByIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new User { Id = userId });
        _roleRepositoryMock
            .Setup(r => r.GetByNameAsync("Academy Admin", It.IsAny<CancellationToken>()))
            .ReturnsAsync(academyAdminRole);
        _userRoleRepositoryMock
            .Setup(r => r.GetByUserIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<UserRole>
            {
                new() { UserId = userId, RoleId = academyAdminRole.Id }
            });

        var command = new CreateAcademyCommand
        {
            Name = "Test Academy",
            Email = "test@test.com",
            Phone = "1234567890",
            UserId = userId
        };

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        _userRoleRepositoryMock.Verify(
            r => r.AddAsync(It.IsAny<UserRole>(), It.IsAny<CancellationToken>()),
            Times.Never);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_RoleGrantFails_DoesNotFailAcademyCreation()
    {
        var userId = Guid.NewGuid();
        SetupSuccessfulAcademyCreation();
        _userRepositoryMock
            .Setup(r => r.GetByIdAsync(userId, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException("boom"));

        var command = new CreateAcademyCommand
        {
            Name = "Test Academy",
            Email = "test@test.com",
            Phone = "1234567890",
            UserId = userId
        };

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        _academyRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Academy>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    private void SetupSuccessfulAcademyCreation()
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
