using FluentAssertions;
using Moq;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Features.AthleteManagement.Commands.UpdateMedicalProfile;
using SportsGurukul.Application.Tests.Common;
using SportsGurukul.Domain.Entities;

namespace SportsGurukul.Application.Tests.Commands;

public class UpdateMedicalProfileCommandHandlerTests
{
    private readonly Mock<IAthleteRepository> _athleteRepositoryMock = TestMocks.CreateAthleteRepository();
    private readonly Mock<IRepository<MedicalProfile>> _medicalProfileRepositoryMock = TestMocks.CreateMedicalProfileRepository();
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = TestMocks.CreateUnitOfWork();
    private readonly Mock<ILogger<UpdateMedicalProfileCommandHandler>> _loggerMock = TestMocks.CreateLogger<UpdateMedicalProfileCommandHandler>();
    private readonly UpdateMedicalProfileCommandHandler _handler;

    public UpdateMedicalProfileCommandHandlerTests()
    {
        _handler = new UpdateMedicalProfileCommandHandler(
            _athleteRepositoryMock.Object,
            _medicalProfileRepositoryMock.Object,
            _unitOfWorkMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_AthleteNotFound_ReturnsFailure()
    {
        _athleteRepositoryMock.Setup(r => r.GetByIdWithDetailsAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Athlete?)null);

        var result = await _handler.Handle(new UpdateMedicalProfileCommand
        {
            AthleteId = Guid.NewGuid()
        }, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Athlete not found.");
    }

    [Fact]
    public async Task Handle_NoExistingProfile_CreatesNewProfile()
    {
        var athleteId = Guid.NewGuid();
        var athlete = TestDataBuilder.CreateAthleteWithDetails(id: athleteId);
        athlete.MedicalProfile = null;

        _athleteRepositoryMock.Setup(r => r.GetByIdWithDetailsAsync(athleteId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(athlete);
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var result = await _handler.Handle(new UpdateMedicalProfileCommand
        {
            AthleteId = athleteId,
            MedicalConditions = "Asthma",
            Allergies = "Peanuts",
            Medications = "Inhaler",
            BloodGroup = "A+",
            InsuranceNumber = "INS-123",
            DoctorName = "Dr. Jones",
            DoctorContact = "+1234567890"
        }, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value!.MedicalConditions.Should().Be("Asthma");
        result.Value.Allergies.Should().Be("Peanuts");
        result.Value.Medications.Should().Be("Inhaler");
        result.Value.BloodGroup.Should().Be("A+");
        result.Value.InsuranceNumber.Should().Be("INS-123");
        result.Value.DoctorName.Should().Be("Dr. Jones");
        result.Value.DoctorContact.Should().Be("+1234567890");
        _medicalProfileRepositoryMock.Verify(r => r.AddAsync(It.IsAny<MedicalProfile>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ExistingProfile_UpdatesProfile()
    {
        var athleteId = Guid.NewGuid();
        var athlete = TestDataBuilder.CreateAthleteWithDetails(id: athleteId);
        var existingProfile = athlete.MedicalProfile!;

        _athleteRepositoryMock.Setup(r => r.GetByIdWithDetailsAsync(athleteId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(athlete);
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var result = await _handler.Handle(new UpdateMedicalProfileCommand
        {
            AthleteId = athleteId,
            MedicalConditions = "Diabetes",
            Allergies = "Latex"
        }, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        existingProfile.MedicalConditions.Should().Be("Diabetes");
        existingProfile.Allergies.Should().Be("Latex");
        _medicalProfileRepositoryMock.Verify(r => r.AddAsync(It.IsAny<MedicalProfile>(), It.IsAny<CancellationToken>()), Times.Never);
        _medicalProfileRepositoryMock.Verify(r => r.Update(existingProfile), Times.Once);
    }
}
