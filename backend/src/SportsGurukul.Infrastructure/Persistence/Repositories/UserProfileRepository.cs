using Microsoft.EntityFrameworkCore;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Features.UserManagement.DTOs;
using SportsGurukul.Domain.Entities;

namespace SportsGurukul.Infrastructure.Persistence.Repositories;

public class UserProfileRepository : Repository<UserProfile>, IUserProfileRepository
{
    public UserProfileRepository(ApplicationDbContext context) : base(context)
    {
    }

    public override async Task<UserProfile?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await Context.UserProfiles
            .AsNoTracking()
            .Include(up => up.User)
            .FirstOrDefaultAsync(up => up.Id == id && !up.IsDeleted, cancellationToken);
    }

    public async Task<UserProfile?> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await Context.UserProfiles
            .AsNoTracking()
            .FirstOrDefaultAsync(up => up.UserId == userId && !up.IsDeleted, cancellationToken);
    }

    public async Task<UserProfile?> GetWithAddressesAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await Context.UserProfiles
            .AsNoTracking()
            .Include(up => up.Addresses.Where(a => !a.IsDeleted))
            .FirstOrDefaultAsync(up => up.UserId == userId && !up.IsDeleted, cancellationToken);
    }

    public async Task<UserProfile?> GetWithContactInformationAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await Context.UserProfiles
            .AsNoTracking()
            .Include(up => up.ContactInformation)
            .FirstOrDefaultAsync(up => up.UserId == userId && !up.IsDeleted, cancellationToken);
    }

    public async Task<UserProfile?> GetFullProfileAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await Context.UserProfiles
            .AsNoTracking()
            .Include(up => up.User)
                .ThenInclude(u => u.UserRoles)
                    .ThenInclude(ur => ur.Role)
            .Include(up => up.Addresses.Where(a => !a.IsDeleted))
            .Include(up => up.ContactInformation)
            .Include(up => up.UserPreference)
            .FirstOrDefaultAsync(up => up.UserId == userId && !up.IsDeleted, cancellationToken);
    }

    public async Task<(IReadOnlyList<UserSummaryDto> Users, int TotalCount)> SearchProfilesAsync(SearchUserRequest request, CancellationToken cancellationToken = default)
    {
        var query = Context.UserProfiles
            .AsNoTracking()
            .Include(up => up.User)
                .ThenInclude(u => u.UserRoles)
                    .ThenInclude(ur => ur.Role)
            .Include(up => up.ContactInformation)
            .Include(up => up.Addresses.Where(a => !a.IsDeleted))
            .Where(up => !up.IsDeleted);

        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            var term = request.SearchTerm.ToLower();
            query = query.Where(up =>
                up.User.FullName.ToLower().Contains(term) ||
                up.User.Email.ToLower().Contains(term) ||
                up.PreferredSport!.ToLower().Contains(term) ||
                up.Bio!.ToLower().Contains(term));
        }

        if (!string.IsNullOrWhiteSpace(request.Email))
        {
            query = query.Where(up => up.User.Email.ToLower().Contains(request.Email.ToLower()));
        }

        if (!string.IsNullOrWhiteSpace(request.PhoneNumber))
        {
            query = query.Where(up => up.ContactInformation != null && up.ContactInformation.PrimaryPhoneNumber != null && up.ContactInformation.PrimaryPhoneNumber.Contains(request.PhoneNumber!));
        }

        if (!string.IsNullOrWhiteSpace(request.City))
        {
            query = query.Where(up => up.Addresses.Any(a => a.City.ToLower().Contains(request.City!.ToLower()) && !a.IsDeleted));
        }

        if (!string.IsNullOrWhiteSpace(request.Sport))
        {
            query = query.Where(up => up.PreferredSport != null && up.PreferredSport.ToLower().Contains(request.Sport.ToLower()));
        }

        if (request.Status.HasValue)
        {
            query = query.Where(up => up.User.Status == request.Status.Value);
        }

        if (request.Role.HasValue)
        {
            query = query.Where(up => up.User.UserRoles.Any(ur => ur.Role.RoleType == request.Role.Value));
        }

        var totalCount = await query.CountAsync(cancellationToken);

        query = request.SortBy?.ToLower() switch
        {
            "name" => request.SortDescending
                ? query.OrderByDescending(up => up.User.FullName)
                : query.OrderBy(up => up.User.FullName),
            "email" => request.SortDescending
                ? query.OrderByDescending(up => up.User.Email)
                : query.OrderBy(up => up.User.Email),
            "sport" => request.SortDescending
                ? query.OrderByDescending(up => up.PreferredSport)
                : query.OrderBy(up => up.PreferredSport),
            "status" => request.SortDescending
                ? query.OrderByDescending(up => up.User.Status)
                : query.OrderBy(up => up.User.Status),
            _ => request.SortDescending
                ? query.OrderByDescending(up => up.CreatedAt)
                : query.OrderBy(up => up.CreatedAt)
        };

        var skip = (request.Page - 1) * request.PageSize;
        var profiles = await query
            .Skip(skip)
            .Take(request.PageSize)
            .ToListAsync(cancellationToken);

        var dtos = profiles.Select(up => new UserSummaryDto
        {
            UserId = up.UserId,
            FullName = up.User.FullName,
            Email = up.User.Email,
            PhoneNumber = up.ContactInformation?.PrimaryPhoneNumber,
            ProfileImageUrl = up.User.ProfileImageUrl ?? up.ProfileImageUrl,
            Status = up.User.Status,
            IsEmailVerified = up.User.IsEmailVerified,
            CreatedAt = up.CreatedAt,
            Roles = up.User.UserRoles.Select(ur => ur.Role.Name).ToList(),
            PreferredSport = up.PreferredSport,
            City = up.Addresses.FirstOrDefault(a => a.IsPrimary && !a.IsDeleted)?.City
        }).ToList();

        return (dtos, totalCount);
    }
}
