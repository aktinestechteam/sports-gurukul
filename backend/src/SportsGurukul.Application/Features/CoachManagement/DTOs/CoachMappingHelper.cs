using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Application.Features.CoachManagement.DTOs;

public static class CoachMappingHelper
{
    public static CoachSummaryDto MapToSummaryDto(Domain.Entities.Coach coach)
    {
        var primarySport = coach.CoachSports?.FirstOrDefault(cs => cs.IsPrimarySport);

        return new CoachSummaryDto
        {
            Id = coach.Id,
            UserId = coach.UserId,
            CoachCode = coach.CoachCode,
            FullName = coach.User?.FullName ?? string.Empty,
            Email = coach.User?.Email ?? string.Empty,
            PhoneNumber = coach.User?.PhoneNumber,
            ProfileImageUrl = coach.User?.ProfileImageUrl,
            CoachingLevel = coach.CoachingLevel.ToString(),
            Status = coach.Status.ToString(),
            VerificationStatus = coach.VerificationStatus.ToString(),
            PrimarySport = primarySport?.Sport?.Name,
            SportCategory = primarySport?.Sport?.SportCategory?.Name,
            YearsOfExperience = coach.YearsOfExperience,
            City = coach.Location?.City,
            State = coach.Location?.State,
            Country = coach.Location?.Country,
            IsVerified = coach.VerificationStatus == VerificationStatus.Verified,
            CertificationCount = coach.Certifications?.Count ?? 0,
            IsOnlineAvailable = coach.Availability?.OnlineAvailable ?? false,
            IsOfflineAvailable = coach.Availability?.OfflineAvailable ?? false,
            CreatedAt = coach.CreatedAt
        };
    }
}
