using FluentAssertions;
using Moq;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Features.AthleteManagement.Commands.UpdateAthlete;
using SportsGurukul.Application.Tests.Common;
using SportsGurukul.Domain.Entities;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Application.Tests.Commands;

public class UpdateAthleteCommandHandlerTests
{
    private readonly Mock<IAthleteRepository> _athleteRepositoryMock = TestMocks.CreateAthleteRepository();
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = TestMocks.CreateUnitOfWork();
    private readonly Mock<ILogger<UpdateAthleteCommandHandler>> _loggerMock = TestMocks.CreateLogger<UpdateAthleteCommandHandler>();
    private readonly UpdateAthleteCommandHandler _handler;

    public UpdateAthleteCommandHandlerTests()
    {
        _handler = new UpdateAthleteCommandHandler(
            _athleteRepositoryMock.Object,
            _unitOfWorkMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_AthleteNotFound_ReturnsFailure()
    {
        _athleteRepositoryMock.Setup(r => r.GetByIdWithDetailsAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Athlete?)null);

        var result = await _handler.Handle(new UpdateAthleteCommand { AthleteId = Guid.NewGuid() }, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Athlete not found.");
    }

    [Fact]
    public async Task Handle_ValidUpdate_UpdatesAllFields()
    {
        var athlete = TestDataBuilder.CreateAthleteWithDetails();
        _athleteRepositoryMock.Setup(r => r.GetByIdWithDetailsAsync(athlete.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(athlete);
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var result = await _handler.Handle(new UpdateAthleteCommand
        {
            AthleteId = athlete.Id,
            CurrentLevel = AthleteLevel.Professional,
            ExperienceYears = 15,
            Height = "185cm",
            Weight = "80kg",
            BloodGroup = BloodGroup.ABNegative,
            DominantHand = DominantHand.Left,
            DominantFoot = DominantFoot.Ambidextrous,
            Biography = "Updated biography",
            Status = AthleteStatus.Inactive
        }, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        athlete.CurrentLevel.Should().Be(AthleteLevel.Professional);
        athlete.ExperienceYears.Should().Be(15);
        athlete.Height.Should().Be("185cm");
        athlete.Weight.Should().Be("80kg");
        athlete.BloodGroup.Should().Be(BloodGroup.ABNegative);
        athlete.DominantHand.Should().Be(DominantHand.Left);
        athlete.DominantFoot.Should().Be(DominantFoot.Ambidextrous);
        athlete.Biography.Should().Be("Updated biography");
        athlete.Status.Should().Be(AthleteStatus.Inactive);
        athlete.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
        _athleteRepositoryMock.Verify(r => r.Update(athlete), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_PartialUpdate_OnlyUpdatesProvidedFields()
    {
        var athlete = TestDataBuilder.CreateAthleteWithDetails();
        var originalHeight = athlete.Height;
        var originalWeight = athlete.Weight;

        _athleteRepositoryMock.Setup(r => r.GetByIdWithDetailsAsync(athlete.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(athlete);
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var result = await _handler.Handle(new UpdateAthleteCommand
        {
            AthleteId = athlete.Id,
            ExperienceYears = 20
        }, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        athlete.ExperienceYears.Should().Be(20);
        athlete.Height.Should().Be(originalHeight);
        athlete.Weight.Should().Be(originalWeight);
    }
}
