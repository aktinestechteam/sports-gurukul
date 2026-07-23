using MediatR;
using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.AthleteManagement.DTOs;
using SportsGurukul.Domain.Entities;

namespace SportsGurukul.Application.Features.AthleteManagement.Commands.UpdateMedicalProfile;

public class UpdateMedicalProfileCommandHandler : IRequestHandler<UpdateMedicalProfileCommand, Result<MedicalProfileDto>>
{
    private readonly IAthleteRepository _athleteRepository;
    private readonly IRepository<MedicalProfile> _medicalProfileRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<UpdateMedicalProfileCommandHandler> _logger;

    public UpdateMedicalProfileCommandHandler(
        IAthleteRepository athleteRepository,
        IRepository<MedicalProfile> medicalProfileRepository,
        IUnitOfWork unitOfWork,
        ILogger<UpdateMedicalProfileCommandHandler> logger)
    {
        _athleteRepository = athleteRepository;
        _medicalProfileRepository = medicalProfileRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<MedicalProfileDto>> Handle(UpdateMedicalProfileCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Updating medical profile for athlete: {AthleteId}", request.AthleteId);

        var athlete = await _athleteRepository.GetByIdWithDetailsAsync(request.AthleteId, cancellationToken);
        if (athlete is null)
        {
            _logger.LogWarning("Athlete not found: {AthleteId}", request.AthleteId);
            return Result<MedicalProfileDto>.Failure("Athlete not found.");
        }

        var medicalProfile = athlete.MedicalProfile;
        if (medicalProfile is null)
        {
            medicalProfile = new MedicalProfile
            {
                Id = Guid.NewGuid(),
                AthleteId = request.AthleteId
            };
            await _medicalProfileRepository.AddAsync(medicalProfile, cancellationToken);
        }

        medicalProfile.MedicalConditions = request.MedicalConditions;
        medicalProfile.Allergies = request.Allergies;
        medicalProfile.Medications = request.Medications;
        medicalProfile.BloodGroup = request.BloodGroup;
        medicalProfile.InsuranceNumber = request.InsuranceNumber;
        medicalProfile.DoctorName = request.DoctorName;
        medicalProfile.DoctorContact = request.DoctorContact;
        medicalProfile.UpdatedAt = DateTime.UtcNow;

        _medicalProfileRepository.Update(medicalProfile);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Medical profile updated for athlete: {AthleteId}", request.AthleteId);

        var dto = new MedicalProfileDto
        {
            Id = medicalProfile.Id,
            MedicalConditions = medicalProfile.MedicalConditions,
            Allergies = medicalProfile.Allergies,
            Medications = medicalProfile.Medications,
            BloodGroup = medicalProfile.BloodGroup,
            InsuranceNumber = medicalProfile.InsuranceNumber,
            DoctorName = medicalProfile.DoctorName,
            DoctorContact = medicalProfile.DoctorContact,
            CreatedAt = medicalProfile.CreatedAt,
            UpdatedAt = medicalProfile.UpdatedAt
        };

        return Result<MedicalProfileDto>.Success(dto);
    }
}
