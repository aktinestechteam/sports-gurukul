using MediatR;
using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.CoachManagement.DTOs;
using SportsGurukul.Domain.Entities;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Application.Features.CoachManagement.Commands.CreateCoach;

public class CreateCoachCommandHandler : IRequestHandler<CreateCoachCommand, Result<CoachDto>>
{
    private readonly IUserRepository _userRepository;
    private readonly ICoachRepository _coachRepository;
    private readonly ICoachAvailabilityRepository _coachAvailabilityRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<CreateCoachCommandHandler> _logger;

    public CreateCoachCommandHandler(
        IUserRepository userRepository,
        ICoachRepository coachRepository,
        ICoachAvailabilityRepository coachAvailabilityRepository,
        IUnitOfWork unitOfWork,
        ILogger<CreateCoachCommandHandler> logger)
    {
        _userRepository = userRepository;
        _coachRepository = coachRepository;
        _coachAvailabilityRepository = coachAvailabilityRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<CoachDto>> Handle(CreateCoachCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Creating coach for UserId: {UserId}", request.UserId);

        var user = await _userRepository.GetByIdAsync(request.UserId);
        if (user is null)
            return Result<CoachDto>.Failure("User not found.");

        var existingCoach = await _coachRepository.GetByUserIdAsync(request.UserId);
        if (existingCoach is not null)
            return Result<CoachDto>.Failure("A coach profile already exists for this user.");

        var coachCode = await GenerateUniqueCoachCodeAsync(cancellationToken);

        var coach = new Coach
        {
            Id = Guid.NewGuid(),
            UserId = request.UserId,
            CoachCode = coachCode,
            RegistrationDate = DateTime.UtcNow,
            Biography = request.Biography,
            YearsOfExperience = request.YearsOfExperience,
            CurrentOrganization = request.CurrentOrganization,
            HighestQualification = request.HighestQualification,
            PreferredLanguage = request.PreferredLanguage,
            CoachingLevel = request.CoachingLevel,
            Status = CoachStatus.Active,
            VerificationStatus = VerificationStatus.Pending,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await _coachRepository.AddAsync(coach);

        var availability = new CoachAvailability
        {
            Id = Guid.NewGuid(),
            CoachId = coach.Id,
            OnlineAvailable = false,
            OfflineAvailable = false,
            TravelDistance = 0,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await _coachAvailabilityRepository.AddAsync(availability);
        coach.Availability = availability;

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Coach created with Id: {CoachId}, CoachCode: {CoachCode}", coach.Id, coachCode);

        return Result<CoachDto>.Success(MapToDto(coach, user));
    }

    private async Task<string> GenerateUniqueCoachCodeAsync(CancellationToken cancellationToken)
    {
        string coachCode;
        do
        {
            var datePart = DateTime.UtcNow.ToString("yyyyMMdd");
            var randomPart = Guid.NewGuid().ToString("N")[..4].ToUpperInvariant();
            coachCode = $"COACH-{datePart}-{randomPart}";
        }
        while (await _coachRepository.AnyAsync(c => c.CoachCode == coachCode, cancellationToken));

        return coachCode;
    }

    internal static CoachDto MapToDto(Coach coach, User user)
    {
        return new CoachDto
        {
            Id = coach.Id,
            UserId = coach.UserId,
            CoachCode = coach.CoachCode,
            RegistrationDate = coach.RegistrationDate,
            Biography = coach.Biography,
            YearsOfExperience = coach.YearsOfExperience,
            CurrentOrganization = coach.CurrentOrganization,
            HighestQualification = coach.HighestQualification,
            PreferredLanguage = coach.PreferredLanguage,
            CoachingLevel = coach.CoachingLevel.ToString(),
            Status = coach.Status.ToString(),
            VerificationStatus = coach.VerificationStatus.ToString(),
            FullName = user.FullName,
            Email = user.Email,
            PhoneNumber = user.PhoneNumber,
            ProfileImageUrl = user.ProfileImageUrl,
            Sports = coach.CoachSports?.Select(cs => new SportDto
            {
                Id = cs.Sport.Id,
                Name = cs.Sport.Name,
                Code = cs.Sport.Code,
                OlympicSport = cs.Sport.OlympicSport,
                CategoryName = cs.Sport.SportCategory?.Name,
                IsPrimarySport = cs.IsPrimarySport,
                JoinedDate = cs.JoinedDate
            }).ToList() ?? new List<SportDto>(),
            Certifications = coach.Certifications?.Select(c => new CertificationDto
            {
                Id = c.Id,
                CertificationName = c.CertificationName,
                IssuingAuthority = c.IssuingAuthority,
                CertificateNumber = c.CertificateNumber,
                IssueDate = c.IssueDate,
                ExpiryDate = c.ExpiryDate,
                VerificationStatus = c.VerificationStatus.ToString(),
                CertificateUrl = c.CertificateUrl,
                IsExpired = c.ExpiryDate.HasValue && c.ExpiryDate < DateTime.UtcNow
            }).ToList() ?? new List<CertificationDto>(),
            Experiences = coach.Experiences?.Select(e => new ExperienceDto
            {
                Id = e.Id,
                Organization = e.Organization,
                Role = e.Role,
                Sport = e.Sport,
                StartDate = e.StartDate,
                EndDate = e.EndDate,
                Description = e.Description
            }).ToList() ?? new List<ExperienceDto>(),
            Education = coach.Education?.Select(e => new EducationDto
            {
                Id = e.Id,
                Degree = e.Degree,
                Institution = e.Institution,
                FieldOfStudy = e.FieldOfStudy,
                YearCompleted = e.YearCompleted
            }).ToList() ?? new List<EducationDto>(),
            Availability = coach.Availability is not null ? new AvailabilityDto
            {
                Id = coach.Availability.Id,
                WeeklySchedule = coach.Availability.WeeklySchedule,
                TimeSlots = coach.Availability.TimeSlots,
                OnlineAvailable = coach.Availability.OnlineAvailable,
                OfflineAvailable = coach.Availability.OfflineAvailable,
                TravelDistance = coach.Availability.TravelDistance
            } : null,
            Location = coach.Location is not null ? new LocationDto
            {
                Id = coach.Location.Id,
                Country = coach.Location.Country,
                State = coach.Location.State,
                City = coach.Location.City,
                District = coach.Location.District,
                Latitude = coach.Location.Latitude,
                Longitude = coach.Location.Longitude
            } : null
        };
    }
}
