using MediatR;
using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.AthleteManagement.DTOs;
using SportsGurukul.Domain.Entities;

namespace SportsGurukul.Application.Features.AthleteManagement.Commands.UpdateEmergencyContact;

public class UpdateEmergencyContactCommandHandler : IRequestHandler<UpdateEmergencyContactCommand, Result<EmergencyContactDto>>
{
    private readonly IAthleteRepository _athleteRepository;
    private readonly IRepository<EmergencyContact> _emergencyContactRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<UpdateEmergencyContactCommandHandler> _logger;

    public UpdateEmergencyContactCommandHandler(
        IAthleteRepository athleteRepository,
        IRepository<EmergencyContact> emergencyContactRepository,
        IUnitOfWork unitOfWork,
        ILogger<UpdateEmergencyContactCommandHandler> logger)
    {
        _athleteRepository = athleteRepository;
        _emergencyContactRepository = emergencyContactRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<EmergencyContactDto>> Handle(UpdateEmergencyContactCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Updating emergency contact for athlete: {AthleteId}", request.AthleteId);

        var athlete = await _athleteRepository.GetByIdWithDetailsAsync(request.AthleteId, cancellationToken);
        if (athlete is null)
        {
            _logger.LogWarning("Athlete not found: {AthleteId}", request.AthleteId);
            return Result<EmergencyContactDto>.Failure("Athlete not found.");
        }

        var emergencyContact = athlete.EmergencyContact;
        if (emergencyContact is null)
        {
            emergencyContact = new EmergencyContact
            {
                Id = Guid.NewGuid(),
                AthleteId = request.AthleteId
            };
            await _emergencyContactRepository.AddAsync(emergencyContact, cancellationToken);
        }

        emergencyContact.Name = request.Name;
        emergencyContact.Relationship = request.Relationship;
        emergencyContact.Phone = request.Phone;
        emergencyContact.Email = request.Email;
        emergencyContact.UpdatedAt = DateTime.UtcNow;

        _emergencyContactRepository.Update(emergencyContact);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Emergency contact updated for athlete: {AthleteId}", request.AthleteId);

        var dto = new EmergencyContactDto
        {
            Id = emergencyContact.Id,
            Name = emergencyContact.Name,
            Relationship = emergencyContact.Relationship.ToString(),
            Phone = emergencyContact.Phone,
            Email = emergencyContact.Email,
            CreatedAt = emergencyContact.CreatedAt,
            UpdatedAt = emergencyContact.UpdatedAt
        };

        return Result<EmergencyContactDto>.Success(dto);
    }
}
