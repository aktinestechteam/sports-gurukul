using FluentAssertions;
using Moq;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Features.AthleteManagement.Commands.UpdateEmergencyContact;
using SportsGurukul.Application.Tests.Common;
using SportsGurukul.Domain.Entities;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Application.Tests.Commands;

public class UpdateEmergencyContactCommandHandlerTests
{
    private readonly Mock<IAthleteRepository> _athleteRepositoryMock = TestMocks.CreateAthleteRepository();
    private readonly Mock<IRepository<EmergencyContact>> _emergencyContactRepositoryMock = TestMocks.CreateEmergencyContactRepository();
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = TestMocks.CreateUnitOfWork();
    private readonly Mock<ILogger<UpdateEmergencyContactCommandHandler>> _loggerMock = TestMocks.CreateLogger<UpdateEmergencyContactCommandHandler>();
    private readonly UpdateEmergencyContactCommandHandler _handler;

    public UpdateEmergencyContactCommandHandlerTests()
    {
        _handler = new UpdateEmergencyContactCommandHandler(
            _athleteRepositoryMock.Object,
            _emergencyContactRepositoryMock.Object,
            _unitOfWorkMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_AthleteNotFound_ReturnsFailure()
    {
        _athleteRepositoryMock.Setup(r => r.GetByIdWithDetailsAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Athlete?)null);

        var result = await _handler.Handle(new UpdateEmergencyContactCommand
        {
            AthleteId = Guid.NewGuid(),
            Name = "John",
            Relationship = EmergencyRelationship.Parent,
            Phone = "+1234567890"
        }, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Athlete not found.");
    }

    [Fact]
    public async Task Handle_NoExistingContact_CreatesNewContact()
    {
        var athleteId = Guid.NewGuid();
        var athlete = TestDataBuilder.CreateAthleteWithDetails(id: athleteId);
        athlete.EmergencyContact = null;

        _athleteRepositoryMock.Setup(r => r.GetByIdWithDetailsAsync(athleteId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(athlete);
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var result = await _handler.Handle(new UpdateEmergencyContactCommand
        {
            AthleteId = athleteId,
            Name = "Jane Doe",
            Relationship = EmergencyRelationship.Spouse,
            Phone = "+1987654321",
            Email = "jane@example.com"
        }, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value!.Name.Should().Be("Jane Doe");
        result.Value.Relationship.Should().Be("Spouse");
        result.Value.Phone.Should().Be("+1987654321");
        result.Value.Email.Should().Be("jane@example.com");
        _emergencyContactRepositoryMock.Verify(r => r.AddAsync(It.IsAny<EmergencyContact>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ExistingContact_UpdatesContact()
    {
        var athleteId = Guid.NewGuid();
        var athlete = TestDataBuilder.CreateAthleteWithDetails(id: athleteId);
        var existingContact = athlete.EmergencyContact!;

        _athleteRepositoryMock.Setup(r => r.GetByIdWithDetailsAsync(athleteId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(athlete);
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var result = await _handler.Handle(new UpdateEmergencyContactCommand
        {
            AthleteId = athleteId,
            Name = "Updated Name",
            Relationship = EmergencyRelationship.Coach,
            Phone = "+9999999999"
        }, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        existingContact.Name.Should().Be("Updated Name");
        existingContact.Relationship.Should().Be(EmergencyRelationship.Coach);
        existingContact.Phone.Should().Be("+9999999999");
        _emergencyContactRepositoryMock.Verify(r => r.AddAsync(It.IsAny<EmergencyContact>(), It.IsAny<CancellationToken>()), Times.Never);
    }
}
