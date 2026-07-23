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
            .FirstOrDefaultAsync(a => a.Id == id, cancellationToken);
    }

    public async Task<Athlete?> GetDeletedByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await Context.Athletes
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(a => a.UserId == userId && a.IsDeleted, cancellationToken);
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

    public async Task<(IReadOnlyList<AthleteSummaryDto> Athletes, int TotalCount)> SearchAthletesAsync(
        AthleteSearchRequest request, CancellationToken cancellationToken = default)
    {
        var query = Context.Athletes
            .AsNoTracking()
            .Include(a => a.User)
            .Include(a => a.AthleteSports).ThenInclude(s => s.Sport)
            .Include(a => a.Ranking)
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
        {
            query = query.Where(a => a.User.FullName.ToLower().Contains(request.Name.ToLower()));
        }

        if (!string.IsNullOrWhiteSpace(request.SportName))
        {
            query = query.Where(a => a.AthleteSports.Any(s =>
                s.Sport.Name.ToLower().Contains(request.SportName!.ToLower())));
        }

        if (!string.IsNullOrWhiteSpace(request.City) ||
            !string.IsNullOrWhiteSpace(request.State) ||
            !string.IsNullOrWhiteSpace(request.Country))
        {
            query = query.Where(a => a.User.UserProfile != null);
        }

        if (!string.IsNullOrWhiteSpace(request.City))
        {
            query = query.Where(a => a.User.UserProfile != null &&
                a.User.UserProfile!.Addresses.Any(addr =>
                    addr.City != null && addr.City.ToLower().Contains(request.City!.ToLower())));
        }

        if (!string.IsNullOrWhiteSpace(request.State))
        {
            query = query.Where(a => a.User.UserProfile != null &&
                a.User.UserProfile!.Addresses.Any(addr =>
                    addr.State != null && addr.State.ToLower().Contains(request.State!.ToLower())));
        }

        if (!string.IsNullOrWhiteSpace(request.Country))
        {
            query = query.Where(a => a.User.UserProfile != null &&
                a.User.UserProfile!.Addresses.Any(addr =>
                    addr.Country != null && addr.Country.ToLower().Contains(request.Country!.ToLower())));
        }

        if (request.CurrentLevel.HasValue)
        {
            query = query.Where(a => a.CurrentLevel == request.CurrentLevel.Value);
        }

        if (!string.IsNullOrWhiteSpace(request.Ranking))
        {
            query = query.Where(a => a.Ranking != null &&
                (a.Ranking.CurrentRank != null && a.Ranking.CurrentRank.ToLower().Contains(request.Ranking.ToLower()) ||
                 a.Ranking.StateRank != null && a.Ranking.StateRank.ToLower().Contains(request.Ranking.ToLower()) ||
                 a.Ranking.NationalRank != null && a.Ranking.NationalRank.ToLower().Contains(request.Ranking.ToLower()) ||
                 a.Ranking.InternationalRank != null && a.Ranking.InternationalRank.ToLower().Contains(request.Ranking.ToLower())));
        }

        if (request.Gender.HasValue)
        {
            query = query.Where(a => a.User.UserProfile != null && a.User.UserProfile.Gender == request.Gender.Value);
        }

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

        if (request.MinExperience.HasValue)
        {
            query = query.Where(a => a.ExperienceYears >= request.MinExperience.Value);
        }

        if (request.MaxExperience.HasValue)
        {
            query = query.Where(a => a.ExperienceYears <= request.MaxExperience.Value);
        }

        if (request.Status.HasValue)
        {
            query = query.Where(a => a.Status == request.Status.Value);
        }

        if (request.IsDeleted.HasValue)
        {
            query = query.Where(a => a.IsDeleted == request.IsDeleted.Value);
        }

        if (request.CreatedFrom.HasValue)
        {
            query = query.Where(a => a.CreatedAt >= request.CreatedFrom.Value);
        }

        if (request.CreatedTo.HasValue)
        {
            query = query.Where(a => a.CreatedAt <= request.CreatedTo.Value);
        }

        var totalCount = await query.CountAsync(cancellationToken);

        query = request.SortBy?.ToLower() switch
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
            "updateddate" => request.SortDescending
                ? query.OrderByDescending(a => a.UpdatedAt)
                : query.OrderBy(a => a.UpdatedAt),
            _ => request.SortDescending
                ? query.OrderByDescending(a => a.CreatedAt)
                : query.OrderBy(a => a.CreatedAt)
        };

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
                ProfileImageUrl = a.User.ProfileImageUrl,
                CurrentLevel = a.CurrentLevel.ToString(),
                Status = a.Status.ToString(),
                PrimarySport = a.AthleteSports
                    .FirstOrDefault(s => s.IsPrimarySport) != null
                    ? a.AthleteSports.First(s => s.IsPrimarySport).Sport.Name
                    : a.AthleteSports.FirstOrDefault() != null
                        ? a.AthleteSports.First().Sport.Name
                        : null,
                CurrentRank = a.Ranking != null ? a.Ranking.CurrentRank : null,
                ExperienceYears = a.ExperienceYears,
                CreatedAt = a.CreatedAt
            })
            .ToListAsync(cancellationToken);

        return (athletes, totalCount);
    }
}
