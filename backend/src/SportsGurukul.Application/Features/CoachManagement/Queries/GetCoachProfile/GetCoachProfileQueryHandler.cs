using MediatR;
using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.CoachManagement.Commands.CreateCoach;
using SportsGurukul.Application.Features.CoachManagement.DTOs;
using SportsGurukul.Domain.Entities;

namespace SportsGurukul.Application.Features.CoachManagement.Queries.GetCoachProfile;

public class GetCoachProfileQueryHandler : IRequestHandler<GetCoachProfileQuery, Result<CoachProfileDto>>
{
    private readonly ICoachRepository _coachRepository;
    private readonly ICoachAvailabilityRepository _coachAvailabilityRepository;
    private readonly ICoachCertificationRepository _coachCertificationRepository;
    private readonly IAthleteRepository _athleteRepository;
    private readonly IRepository<CoachAthlete> _coachAthleteRepository;
    private readonly ILogger<GetCoachProfileQueryHandler> _logger;

    public GetCoachProfileQueryHandler(
        ICoachRepository coachRepository,
        ICoachAvailabilityRepository coachAvailabilityRepository,
        ICoachCertificationRepository coachCertificationRepository,
        IAthleteRepository athleteRepository,
        IRepository<CoachAthlete> coachAthleteRepository,
        ILogger<GetCoachProfileQueryHandler> logger)
    {
        _coachRepository = coachRepository;
        _coachAvailabilityRepository = coachAvailabilityRepository;
        _coachCertificationRepository = coachCertificationRepository;
        _athleteRepository = athleteRepository;
        _coachAthleteRepository = coachAthleteRepository;
        _logger = logger;
    }

    public async Task<Result<CoachProfileDto>> Handle(GetCoachProfileQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting profile for coach Id: {CoachId}", request.CoachId);

        var coach = await _coachRepository.GetByIdWithDetailsAsync(request.CoachId, cancellationToken);
        if (coach is null)
            return Result<CoachProfileDto>.Failure("Coach not found.");

        var certifications = await _coachCertificationRepository.GetByCoachIdAsync(request.CoachId, cancellationToken);
        var availability = await _coachAvailabilityRepository.GetByCoachIdAsync(request.CoachId, cancellationToken);
        var coachAthletes = await _coachAthleteRepository.FindAsync(
            ca => ca.CoachId == request.CoachId && ca.IsActive, cancellationToken);

        var assignedAthletes = new List<AssignedAthleteDto>();
        foreach (var ca in coachAthletes)
        {
            var athlete = await _athleteRepository.GetByIdWithDetailsAsync(ca.AthleteId, cancellationToken);
            if (athlete is null) continue;

            assignedAthletes.Add(new AssignedAthleteDto
            {
                Id = ca.Id,
                AthleteId = athlete.Id,
                AthleteCode = athlete.AthleteCode,
                FullName = athlete.User.FullName,
                Email = athlete.User.Email,
                PhoneNumber = athlete.User.PhoneNumber,
                ProfileImageUrl = athlete.User.ProfileImageUrl,
                CurrentLevel = athlete.CurrentLevel.ToString(),
                Status = athlete.Status.ToString(),
                AssignedDate = ca.AssignedDate
            });
        }

        var profile = new CoachProfileDto
        {
            Coach = CreateCoachCommandHandler.MapToDto(coach, coach.User),
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
            Certifications = certifications.Select(c => new CertificationDto
            {
                Id = c.Id,
                CertificationName = c.CertificationName,
                IssuingAuthority = c.IssuingAuthority,
                CertificateNumber = c.CertificateNumber,
                IssueDate = c.IssueDate,
                ExpiryDate = c.ExpiryDate,
                VerificationStatus = c.VerificationStatus.ToString(),
                CertificateUrl = c.CertificateUrl,
                IsExpired = c.ExpiryDate.HasValue && c.ExpiryDate < DateTime.UtcNow,
                CreatedAt = c.CreatedAt,
                UpdatedAt = c.UpdatedAt
            }).ToList(),
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
            Availability = availability is not null ? new AvailabilityDto
            {
                Id = availability.Id,
                WeeklySchedule = availability.WeeklySchedule,
                TimeSlots = availability.TimeSlots,
                OnlineAvailable = availability.OnlineAvailable,
                OfflineAvailable = availability.OfflineAvailable,
                TravelDistance = availability.TravelDistance,
                CreatedAt = availability.CreatedAt,
                UpdatedAt = availability.UpdatedAt
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
            } : null,
            AssignedAthletes = assignedAthletes
        };

        return Result<CoachProfileDto>.Success(profile);
    }
}
