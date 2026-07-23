using Microsoft.EntityFrameworkCore;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Features.AthleteManagement.DTOs;
using SportsGurukul.Domain.Entities;

namespace SportsGurukul.Infrastructure.Persistence.Repositories;

public class AthleteRepository : Repository<Athlete>, IAthleteRepository
{
    public AthleteRepository(ApplicationDbContext context) : base(context) { }

    public async Task<Athlete?> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await Context.Athletes
            .AsNoTracking()
            .FirstOrDefaultAsync(a => a.UserId == userId, cancellationToken);
    }

    public async Task<Athlete?> GetByAthleteCodeAsync(string athleteCode, CancellationToken cancellationToken = default)
    {
        return await Context.Athletes
            .AsNoTracking()
            .FirstOrDefaultAsync(a => a.AthleteCode == athleteCode, cancellationToken);
    }

    public async Task<Athlete?> GetByUserIdWithDetailsAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await Context.Athletes
            .AsNoTracking()
            .Include(a => a.User)
            .Include(a => a.User.UserRoles).ThenInclude(ur => ur.Role)
            .Include(a => a.User.UserProfile).ThenInclude(p => p!.ContactInformation)
            .Include(a => a.AthleteSports).ThenInclude(s => s.Sport).ThenInclude(s => s!.SportCategory)
            .Include(a => a.AthleteAchievements).ThenInclude(aa => aa.Achievement)
            .Include(a => a.MedicalProfile)
            .Include(a => a.EmergencyContact)
            .Include(a => a.Ranking)
            .AsSplitQuery()
            .FirstOrDefaultAsync(a => a.UserId == userId, cancellationToken);
    }

    public async Task<Athlete?> GetByIdWithDetailsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await Context.Athletes
            .AsNoTracking()
            .Include(a => a.User)
            .Include(a => a.User.UserRoles).ThenInclude(ur => ur.Role)
            .Include(a => a.User.UserProfile).ThenInclude(p => p!.ContactInformation)
            .Include(a => a.AthleteSports).ThenInclude(s => s.Sport).ThenInclude(s => s!.SportCategory)
            .Include(a => a.AthleteAchievements).ThenInclude(aa => aa.Achievement)
            .Include(a => a.MedicalProfile)
            .Include(a => a.EmergencyContact)
            .Include(a => a.Ranking)
            .AsSplitQuery()
            .FirstOrDefaultAsync(a => a.Id == id, cancellationToken);
    }

    public async Task<Athlete?> GetDeletedByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await Context.Athletes
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(a => a.UserId == userId && a.IsDeleted, cancellationToken);
    }

    public async Task<Athlete?> GetDeletedByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await Context.Athletes
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(a => a.Id == id && a.IsDeleted, cancellationToken);
    }

    public async Task<IReadOnlyList<Athlete>> GetAllWithUserAsync(CancellationToken cancellationToken = default)
    {
        return await Context.Athletes
            .AsNoTracking()
            .Include(a => a.User)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<AthleteSport>> GetAthleteSportsAsync(Guid athleteId, CancellationToken cancellationToken = default)
    {
        return await Context.AthleteSports
            .AsNoTracking()
            .Include(s => s.Sport).ThenInclude(s => s!.SportCategory)
            .Where(s => s.AthleteId == athleteId)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<AthleteAchievement>> GetAthleteAchievementsAsync(Guid athleteId, CancellationToken cancellationToken = default)
    {
        return await Context.AthleteAchievements
            .AsNoTracking()
            .Include(aa => aa.Achievement)
            .Where(aa => aa.AthleteId == athleteId)
            .ToListAsync(cancellationToken);
    }

    public async Task<Ranking?> GetAthleteRankingAsync(Guid athleteId, CancellationToken cancellationToken = default)
    {
        return await Context.Rankings
            .AsNoTracking()
            .FirstOrDefaultAsync(r => r.AthleteId == athleteId, cancellationToken);
    }

    public async Task<IReadOnlyList<AthleteSearchSuggestionDto>> GetSearchSuggestionsAsync(
        string prefix, int limit, CancellationToken cancellationToken = default)
    {
        var term = prefix.ToLower();
        var suggestions = new List<AthleteSearchSuggestionDto>();

        var athleteMatches = await Context.Athletes
            .AsNoTracking()
            .Include(a => a.User)
            .Include(a => a.AthleteSports).ThenInclude(s => s.Sport)
            .Where(a => a.User.FullName.ToLower().Contains(term))
            .Take(limit)
            .Select(a => new AthleteSearchSuggestionDto
            {
                Text = a.User.FullName,
                Type = "athlete",
                Id = a.Id,
                SubText = a.AthleteCode
            })
            .ToListAsync(cancellationToken);

        suggestions.AddRange(athleteMatches);

        var codeMatches = await Context.Athletes
            .AsNoTracking()
            .Include(a => a.User)
            .Where(a => a.AthleteCode.ToLower().Contains(term) &&
                        !a.User.FullName.ToLower().Contains(term))
            .Take(Math.Max(0, limit - suggestions.Count))
            .Select(a => new AthleteSearchSuggestionDto
            {
                Text = a.AthleteCode,
                Type = "athlete",
                Id = a.Id,
                SubText = a.User.FullName
            })
            .ToListAsync(cancellationToken);

        suggestions.AddRange(codeMatches);

        var sportMatches = await Context.Sports
            .AsNoTracking()
            .Where(s => s.Name.ToLower().Contains(term))
            .Take(Math.Max(0, limit - suggestions.Count))
            .Select(s => new AthleteSearchSuggestionDto
            {
                Text = s.Name,
                Type = "sport",
                SubText = s.Description
            })
            .ToListAsync(cancellationToken);

        suggestions.AddRange(sportMatches);

        return suggestions.Take(limit).ToList();
    }

    public async Task<(IReadOnlyList<AthleteSummaryDto> Athletes, int TotalCount)> SearchAthletesAsync(
        AthleteSearchRequest request, CancellationToken cancellationToken = default)
    {
        var query = Context.Athletes
            .AsNoTracking()
            .Include(a => a.User)
            .ThenInclude(u => u.UserProfile)
            .ThenInclude(p => p!.ContactInformation)
            .Include(a => a.User.UserProfile)
            .ThenInclude(p => p!.Addresses)
            .Include(a => a.AthleteSports).ThenInclude(s => s.Sport)
            .Include(a => a.AthleteSports).ThenInclude(s => s.Sport).ThenInclude(s => s!.SportCategory)
            .Include(a => a.Ranking)
            .Include(a => a.AthleteAchievements).ThenInclude(aa => aa.Achievement)
            .Include(a => a.MedicalProfile)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            var term = request.SearchTerm.ToLower();
            query = query.Where(a =>
                a.User.FullName.ToLower().Contains(term) ||
                a.AthleteCode.ToLower().Contains(term) ||
                a.User.Email.ToLower().Contains(term));
        }

        if (!string.IsNullOrWhiteSpace(request.Name))
            query = query.Where(a => a.User.FullName.ToLower().Contains(request.Name.ToLower()));

        if (!string.IsNullOrWhiteSpace(request.AthleteCode))
            query = query.Where(a => a.AthleteCode.ToLower().Contains(request.AthleteCode.ToLower()));

        if (!string.IsNullOrWhiteSpace(request.Email))
            query = query.Where(a => a.User.Email.ToLower().Contains(request.Email.ToLower()));

        if (!string.IsNullOrWhiteSpace(request.Mobile))
            query = query.Where(a => a.User.UserProfile != null &&
                a.User.UserProfile!.ContactInformation != null &&
                a.User.UserProfile!.ContactInformation!.PrimaryPhoneNumber != null &&
                a.User.UserProfile!.ContactInformation!.PrimaryPhoneNumber.Contains(request.Mobile));

        if (!string.IsNullOrWhiteSpace(request.SportName))
            query = query.Where(a => a.AthleteSports.Any(s =>
                s.Sport.Name.ToLower().Contains(request.SportName!.ToLower())));

        if (!string.IsNullOrWhiteSpace(request.SportCategory))
            query = query.Where(a => a.AthleteSports.Any(s =>
                s.Sport.SportCategory != null &&
                s.Sport.SportCategory.Name.ToLower().Contains(request.SportCategory!.ToLower())));

        if (request.IsPrimarySport.HasValue)
            query = query.Where(a => a.AthleteSports.Any(s => s.IsPrimarySport == request.IsPrimarySport.Value));

        if (request.SportIds is not null && request.SportIds.Count > 0)
            query = query.Where(a => a.AthleteSports.Any(s => request.SportIds.Contains(s.SportId)));

        if (!string.IsNullOrWhiteSpace(request.City))
            query = query.Where(a => a.User.UserProfile != null &&
                a.User.UserProfile!.Addresses.Any(addr =>
                    addr.City != null && addr.City.ToLower().Contains(request.City!.ToLower())));

        if (!string.IsNullOrWhiteSpace(request.State))
            query = query.Where(a => a.User.UserProfile != null &&
                a.User.UserProfile!.Addresses.Any(addr =>
                    addr.State != null && addr.State.ToLower().Contains(request.State!.ToLower())));

        if (!string.IsNullOrWhiteSpace(request.Country))
            query = query.Where(a => a.User.UserProfile != null &&
                a.User.UserProfile!.Addresses.Any(addr =>
                    addr.Country != null && addr.Country.ToLower().Contains(request.Country!.ToLower())));

        if (!string.IsNullOrWhiteSpace(request.District))
            query = query.Where(a => a.User.UserProfile != null &&
                a.User.UserProfile!.Addresses.Any(addr =>
                    addr.Line2 != null && addr.Line2.ToLower().Contains(request.District!.ToLower())));

        if (!string.IsNullOrWhiteSpace(request.PostalCode))
            query = query.Where(a => a.User.UserProfile != null &&
                a.User.UserProfile!.Addresses.Any(addr =>
                    addr.PostalCode != null && addr.PostalCode.ToLower().Contains(request.PostalCode!.ToLower())));

        if (request.CurrentLevel.HasValue)
            query = query.Where(a => a.CurrentLevel == request.CurrentLevel.Value);

        if (!string.IsNullOrWhiteSpace(request.Ranking))
            query = query.Where(a => a.Ranking != null &&
                (a.Ranking.CurrentRank != null && a.Ranking.CurrentRank.ToLower().Contains(request.Ranking.ToLower()) ||
                 a.Ranking.StateRank != null && a.Ranking.StateRank.ToLower().Contains(request.Ranking.ToLower()) ||
                 a.Ranking.NationalRank != null && a.Ranking.NationalRank.ToLower().Contains(request.Ranking.ToLower()) ||
                 a.Ranking.InternationalRank != null && a.Ranking.InternationalRank.ToLower().Contains(request.Ranking.ToLower())));

        if (!string.IsNullOrWhiteSpace(request.StateRank))
            query = query.Where(a => a.Ranking != null && a.Ranking.StateRank != null &&
                a.Ranking.StateRank.ToLower().Contains(request.StateRank.ToLower()));

        if (!string.IsNullOrWhiteSpace(request.NationalRank))
            query = query.Where(a => a.Ranking != null && a.Ranking.NationalRank != null &&
                a.Ranking.NationalRank.ToLower().Contains(request.NationalRank.ToLower()));

        if (!string.IsNullOrWhiteSpace(request.InternationalRank))
            query = query.Where(a => a.Ranking != null && a.Ranking.InternationalRank != null &&
                a.Ranking.InternationalRank.ToLower().Contains(request.InternationalRank.ToLower()));

        if (request.Gender.HasValue)
            query = query.Where(a => a.User.UserProfile != null && a.User.UserProfile.Gender == request.Gender.Value);

        if (request.MinAge.HasValue || request.MaxAge.HasValue)
        {
            query = query.Where(a => a.User.UserProfile != null && a.User.UserProfile.DateOfBirth.HasValue);
            if (request.MinAge.HasValue)
            {
                var maxDob = DateTime.UtcNow.AddYears(-request.MinAge.Value);
                query = query.Where(a => a.User.UserProfile!.DateOfBirth <= maxDob);
            }
            if (request.MaxAge.HasValue)
            {
                var minDob = DateTime.UtcNow.AddYears(-request.MaxAge.Value - 1);
                query = query.Where(a => a.User.UserProfile!.DateOfBirth > minDob);
            }
        }

        if (!string.IsNullOrWhiteSpace(request.MinHeight))
            query = query.Where(a => a.User.UserProfile != null &&
                a.User.UserProfile!.Height != null &&
                a.User.UserProfile!.Height == request.MinHeight);

        if (!string.IsNullOrWhiteSpace(request.MaxHeight))
            query = query.Where(a => a.User.UserProfile != null &&
                a.User.UserProfile!.Height != null &&
                a.User.UserProfile!.Height == request.MaxHeight);

        if (!string.IsNullOrWhiteSpace(request.MinWeight))
            query = query.Where(a => a.User.UserProfile != null &&
                a.User.UserProfile!.Weight != null &&
                a.User.UserProfile!.Weight == request.MinWeight);

        if (!string.IsNullOrWhiteSpace(request.MaxWeight))
            query = query.Where(a => a.User.UserProfile != null &&
                a.User.UserProfile!.Weight != null &&
                a.User.UserProfile!.Weight == request.MaxWeight);

        if (request.BloodGroup.HasValue)
            query = query.Where(a => a.BloodGroup == request.BloodGroup.Value);

        if (request.MinExperience.HasValue)
            query = query.Where(a => a.ExperienceYears >= request.MinExperience.Value);

        if (request.MaxExperience.HasValue)
            query = query.Where(a => a.ExperienceYears <= request.MaxExperience.Value);

        if (request.Status.HasValue)
            query = query.Where(a => a.Status == request.Status.Value);

        if (request.MinAchievementLevel.HasValue)
            query = query.Where(a => a.AthleteAchievements.Any(aa =>
                aa.Achievement.Level >= request.MinAchievementLevel.Value));

        if (request.HasMedicalProfile.HasValue && request.HasMedicalProfile.Value)
            query = query.Where(a => a.MedicalProfile != null);

        if (request.HasMedicalProfile.HasValue && !request.HasMedicalProfile.Value)
            query = query.Where(a => a.MedicalProfile == null);

        if (request.IsVerified.HasValue && request.IsVerified.Value)
            query = query.Where(a => a.User.IsEmailVerified);

        if (request.IsVerified.HasValue && !request.IsVerified.Value)
            query = query.Where(a => !a.User.IsEmailVerified);

        if (request.CreatedFrom.HasValue)
            query = query.Where(a => a.CreatedAt >= request.CreatedFrom.Value);

        if (request.CreatedTo.HasValue)
            query = query.Where(a => a.CreatedAt <= request.CreatedTo.Value);

        if (request.UseCursorPagination && !string.IsNullOrWhiteSpace(request.Cursor))
        {
            var cursorDate = DateTime.Parse(request.Cursor);
            query = query.Where(a => a.CreatedAt < cursorDate);
        }

        var totalCount = await query.CountAsync(cancellationToken);

        if (!string.IsNullOrWhiteSpace(request.SortBy))
        {
            query = request.SortBy.ToLower() switch
            {
                "name" => request.SortDescending
                    ? query.OrderByDescending(a => a.User.FullName)
                    : query.OrderBy(a => a.User.FullName),
                "athletecode" => request.SortDescending
                    ? query.OrderByDescending(a => a.AthleteCode)
                    : query.OrderBy(a => a.AthleteCode),
                "level" => request.SortDescending
                    ? query.OrderByDescending(a => a.CurrentLevel)
                    : query.OrderBy(a => a.CurrentLevel),
                "experience" => request.SortDescending
                    ? query.OrderByDescending(a => a.ExperienceYears)
                    : query.OrderBy(a => a.ExperienceYears),
                "ranking" => request.SortDescending
                    ? query.OrderByDescending(a => a.Ranking != null ? a.Ranking.CurrentRank : null)
                    : query.OrderBy(a => a.Ranking != null ? a.Ranking.CurrentRank : null),
                "achievementcount" => request.SortDescending
                    ? query.OrderByDescending(a => a.AthleteAchievements.Count)
                    : query.OrderBy(a => a.AthleteAchievements.Count),
                "recentlyupdated" => request.SortDescending
                    ? query.OrderByDescending(a => a.UpdatedAt)
                    : query.OrderBy(a => a.UpdatedAt),
                "newest" => query.OrderByDescending(a => a.CreatedAt),
                "oldest" => query.OrderBy(a => a.CreatedAt),
                _ => request.SortDescending
                    ? query.OrderByDescending(a => a.CreatedAt)
                    : query.OrderBy(a => a.CreatedAt)
            };
        }
        else
        {
            query = request.SortDescending
                ? query.OrderByDescending(a => a.CreatedAt)
                : query.OrderBy(a => a.CreatedAt);
        }

        var athletes = await query
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(a => new AthleteSummaryDto
            {
                Id = a.Id,
                UserId = a.UserId,
                AthleteCode = a.AthleteCode,
                FullName = a.User.FullName,
                Email = a.User.Email,
                PhoneNumber = a.User.UserProfile != null && a.User.UserProfile.ContactInformation != null
                    ? a.User.UserProfile.ContactInformation.PrimaryPhoneNumber : null,
                ProfileImageUrl = a.User.ProfileImageUrl,
                CurrentLevel = a.CurrentLevel.ToString(),
                Status = a.Status.ToString(),
                PrimarySport = a.AthleteSports
                    .FirstOrDefault(s => s.IsPrimarySport) != null
                    ? a.AthleteSports.First(s => s.IsPrimarySport).Sport.Name
                    : a.AthleteSports.FirstOrDefault() != null
                        ? a.AthleteSports.First().Sport.Name
                        : null,
                SportCategory = a.AthleteSports
                    .FirstOrDefault(s => s.IsPrimarySport) != null
                    ? a.AthleteSports.First(s => s.IsPrimarySport).Sport.SportCategory.Name
                    : a.AthleteSports.FirstOrDefault() != null
                        ? a.AthleteSports.First().Sport.SportCategory != null
                            ? a.AthleteSports.First().Sport.SportCategory.Name
                            : null
                        : null,
                CurrentRank = a.Ranking != null ? a.Ranking.CurrentRank : null,
                StateRank = a.Ranking != null ? a.Ranking.StateRank : null,
                NationalRank = a.Ranking != null ? a.Ranking.NationalRank : null,
                InternationalRank = a.Ranking != null ? a.Ranking.InternationalRank : null,
                ExperienceYears = a.ExperienceYears,
                Gender = a.User.UserProfile != null
                    ? (GenderDto?)a.User.UserProfile.Gender
                    : null,
                Age = a.User.UserProfile != null && a.User.UserProfile.DateOfBirth.HasValue
                    ? (int?)((DateTime.UtcNow - a.User.UserProfile.DateOfBirth.Value).TotalDays / 365.25)
                    : null,
                City = a.User.UserProfile != null
                    ? a.User.UserProfile.Addresses.FirstOrDefault(addr => addr.IsPrimary) != null
                        ? a.User.UserProfile.Addresses.First(addr => addr.IsPrimary).City
                        : a.User.UserProfile.Addresses.FirstOrDefault() != null
                            ? a.User.UserProfile.Addresses.First().City
                            : null
                    : null,
                State = a.User.UserProfile != null
                    ? a.User.UserProfile.Addresses.FirstOrDefault(addr => addr.IsPrimary) != null
                        ? a.User.UserProfile.Addresses.First(addr => addr.IsPrimary).State
                        : a.User.UserProfile.Addresses.FirstOrDefault() != null
                            ? a.User.UserProfile.Addresses.First().State
                            : null
                    : null,
                Country = a.User.UserProfile != null
                    ? a.User.UserProfile.Addresses.FirstOrDefault(addr => addr.IsPrimary) != null
                        ? a.User.UserProfile.Addresses.First(addr => addr.IsPrimary).Country
                        : a.User.UserProfile.Addresses.FirstOrDefault() != null
                            ? a.User.UserProfile.Addresses.First().Country
                            : null
                    : null,
                IsVerified = a.User.IsEmailVerified,
                HasMedicalProfile = a.MedicalProfile != null,
                AchievementCount = a.AthleteAchievements.Count,
                CreatedAt = a.CreatedAt
            })
            .ToListAsync(cancellationToken);

        return (athletes, totalCount);
    }
}
