using MediatR;
using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.AthleteManagement.DTOs;
using SportsGurukul.Domain.Entities;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Application.Features.AthleteManagement.Commands.CreateAthlete;

public class CreateAthleteCommandHandler : IRequestHandler<CreateAthleteCommand, Result<AthleteDto>>
{
    private readonly IUserRepository _userRepository;
    private readonly IAthleteRepository _athleteRepository;
    private readonly IRepository<MedicalProfile> _medicalProfileRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<CreateAthleteCommandHandler> _logger;

    public CreateAthleteCommandHandler(
        IUserRepository userRepository,
        IAthleteRepository athleteRepository,
        IRepository<MedicalProfile> medicalProfileRepository,
        IUnitOfWork unitOfWork,
        ILogger<CreateAthleteCommandHandler> logger)
    {
        _userRepository = userRepository;
        _athleteRepository = athleteRepository;
        _medicalProfileRepository = medicalProfileRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<AthleteDto>> Handle(CreateAthleteCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Creating athlete profile for user: {UserId}", request.UserId);

        var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);
        if (user is null)
        {
            _logger.LogWarning("User not found: {UserId}", request.UserId);
            return Result<AthleteDto>.Failure("User not found.");
        }

        var existing = await _athleteRepository.GetByUserIdAsync(request.UserId, cancellationToken);
        if (existing is not null && !existing.IsDeleted)
        {
            _logger.LogWarning("Athlete profile already exists for user: {UserId}", request.UserId);
            return Result<AthleteDto>.Failure("An athlete profile already exists for this user.");
        }

        Athlete athlete;
        if (existing is not null && existing.IsDeleted)
        {
            existing.IsDeleted = false;
            existing.CurrentLevel = request.CurrentLevel;
            existing.ExperienceYears = request.ExperienceYears;
            existing.Height = request.Height;
            existing.Weight = request.Weight;
            existing.BloodGroup = request.BloodGroup;
            existing.DominantHand = request.DominantHand;
            existing.DominantFoot = request.DominantFoot;
            existing.Biography = request.Biography;
            existing.Status = AthleteStatus.Active;
            existing.UpdatedAt = DateTime.UtcNow;
            _athleteRepository.Update(existing);
            athlete = existing;
        }
        else
        {
            athlete = new Athlete
            {
                Id = Guid.NewGuid(),
                UserId = request.UserId,
                AthleteCode = await GenerateAthleteCodeAsync(cancellationToken),
                RegistrationDate = DateTime.UtcNow,
                CurrentLevel = request.CurrentLevel,
                ExperienceYears = request.ExperienceYears,
                Height = request.Height,
                Weight = request.Weight,
                BloodGroup = request.BloodGroup,
                DominantHand = request.DominantHand,
                DominantFoot = request.DominantFoot,
                Biography = request.Biography,
                Status = AthleteStatus.Active
            };
            await _athleteRepository.AddAsync(athlete, cancellationToken);
        }

        var medicalProfile = new MedicalProfile
        {
            Id = Guid.NewGuid(),
            AthleteId = athlete.Id
        };
        await _medicalProfileRepository.AddAsync(medicalProfile, cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Athlete created: {AthleteId}, Code: {AthleteCode}", athlete.Id, athlete.AthleteCode);

        var dto = MapToDto(athlete, user);
        return Result<AthleteDto>.Success(dto);
    }

    private async Task<string> GenerateAthleteCodeAsync(CancellationToken cancellationToken)
    {
        string code;
        do
        {
            code = $"ATH-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString("N")[..6].ToUpper()}";
        } while (await _athleteRepository.GetByAthleteCodeAsync(code, cancellationToken) is not null);
        return code;
    }

    internal static AthleteDto MapToDto(Athlete athlete, User user)
    {
        return new AthleteDto
        {
            Id = athlete.Id,
            UserId = athlete.UserId,
            AthleteCode = athlete.AthleteCode,
            FullName = user.FullName,
            Email = user.Email,
            PhoneNumber = user.PhoneNumber,
            ProfileImageUrl = user.ProfileImageUrl,
            RegistrationDate = athlete.RegistrationDate,
            CurrentLevel = athlete.CurrentLevel.ToString(),
            ExperienceYears = athlete.ExperienceYears,
            Height = athlete.Height,
            Weight = athlete.Weight,
            BloodGroup = athlete.BloodGroup?.ToString(),
            DominantHand = athlete.DominantHand?.ToString(),
            DominantFoot = athlete.DominantFoot?.ToString(),
            Biography = athlete.Biography,
            Status = athlete.Status.ToString(),
            Roles = user.UserRoles.Select(ur => ur.Role.Name).ToList(),
            Sports = athlete.AthleteSports.Select(s => new SportDto
            {
                Id = s.Id,
                SportId = s.SportId,
                Name = s.Sport.Name,
                Code = s.Sport.Code,
                CategoryName = s.Sport.SportCategory.Name,
                OlympicSport = s.Sport.OlympicSport,
                IsPrimarySport = s.IsPrimarySport,
                JoinedDate = s.JoinedDate
            }).ToList(),
            Achievements = athlete.AthleteAchievements.Select(aa => new AthleteAchievementDto
            {
                Id = aa.Id,
                AchievementId = aa.AchievementId,
                Title = aa.Achievement.Title,
                Competition = aa.Achievement.Competition,
                Position = aa.Achievement.Position,
                Level = aa.Achievement.Level.ToString(),
                Date = aa.Achievement.Date,
                CertificateUrl = aa.Achievement.CertificateUrl,
                AwardedDate = aa.AwardedDate,
                Notes = aa.Notes
            }).ToList(),
            MedicalProfile = athlete.MedicalProfile is not null ? new MedicalProfileDto
            {
                Id = athlete.MedicalProfile.Id,
                MedicalConditions = athlete.MedicalProfile.MedicalConditions,
                Allergies = athlete.MedicalProfile.Allergies,
                Medications = athlete.MedicalProfile.Medications,
                BloodGroup = athlete.MedicalProfile.BloodGroup,
                InsuranceNumber = athlete.MedicalProfile.InsuranceNumber,
                DoctorName = athlete.MedicalProfile.DoctorName,
                DoctorContact = athlete.MedicalProfile.DoctorContact,
                CreatedAt = athlete.MedicalProfile.CreatedAt,
                UpdatedAt = athlete.MedicalProfile.UpdatedAt
            } : null,
            EmergencyContact = athlete.EmergencyContact is not null ? new EmergencyContactDto
            {
                Id = athlete.EmergencyContact.Id,
                Name = athlete.EmergencyContact.Name,
                Relationship = athlete.EmergencyContact.Relationship.ToString(),
                Phone = athlete.EmergencyContact.Phone,
                Email = athlete.EmergencyContact.Email,
                CreatedAt = athlete.EmergencyContact.CreatedAt,
                UpdatedAt = athlete.EmergencyContact.UpdatedAt
            } : null,
            Ranking = athlete.Ranking is not null ? new RankingDto
            {
                Id = athlete.Ranking.Id,
                CurrentRank = athlete.Ranking.CurrentRank,
                StateRank = athlete.Ranking.StateRank,
                NationalRank = athlete.Ranking.NationalRank,
                InternationalRank = athlete.Ranking.InternationalRank,
                RankingAuthority = athlete.Ranking.RankingAuthority,
                CreatedAt = athlete.Ranking.CreatedAt,
                UpdatedAt = athlete.Ranking.UpdatedAt
            } : null,
            CreatedAt = athlete.CreatedAt,
            UpdatedAt = athlete.UpdatedAt
        };
    }
}
