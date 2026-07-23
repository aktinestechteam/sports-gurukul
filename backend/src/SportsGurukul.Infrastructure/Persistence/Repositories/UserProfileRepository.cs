using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Features.UserManagement.DTOs;
using SportsGurukul.Domain.Entities;
using SportsGurukul.Domain.Enums;

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

    public async Task<(IReadOnlyList<UserListDto> Users, int TotalCount)> SearchProfilesAsync(
        UserSearchRequest request, CancellationToken cancellationToken = default)
    {
        var query = Context.UserProfiles
            .AsNoTracking()
            .Include(up => up.User)
                .ThenInclude(u => u.UserRoles)
                    .ThenInclude(ur => ur.Role)
            .Include(up => up.ContactInformation)
            .Include(up => up.Addresses.Where(a => !a.IsDeleted))
            .AsQueryable();

        query = ApplyFilters(query, request);

        var totalCount = await query.CountAsync(cancellationToken);

        query = ApplySorting(query, request.SortBy, request.SortDescending);

        var skip = (request.Page - 1) * request.PageSize;
        var profiles = await query
            .Skip(skip)
            .Take(request.PageSize)
            .ToListAsync(cancellationToken);

        var dtos = profiles.Select(MapToUserListDto).ToList();

        return (dtos, totalCount);
    }

    private static IQueryable<UserProfile> ApplyFilters(IQueryable<UserProfile> query, UserSearchRequest request)
    {
        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            var term = request.SearchTerm.ToLower();
            query = query.Where(up =>
                up.User.FullName.ToLower().Contains(term) ||
                up.User.Email.ToLower().Contains(term) ||
                (up.ContactInformation != null &&
                 up.ContactInformation.PrimaryPhoneNumber != null &&
                 up.ContactInformation.PrimaryPhoneNumber.Contains(term)) ||
                up.PreferredSport!.ToLower().Contains(term) ||
                up.Bio!.ToLower().Contains(term));
        }

        if (!string.IsNullOrWhiteSpace(request.Name))
        {
            var name = request.Name.ToLower();
            query = query.Where(up => up.User.FullName.ToLower().Contains(name));
        }

        if (!string.IsNullOrWhiteSpace(request.Email))
        {
            var email = request.Email.ToLower();
            query = query.Where(up => up.User.Email.ToLower().Contains(email));
        }

        if (!string.IsNullOrWhiteSpace(request.Mobile))
        {
            query = query.Where(up =>
                up.ContactInformation != null &&
                up.ContactInformation.PrimaryPhoneNumber != null &&
                up.ContactInformation.PrimaryPhoneNumber.Contains(request.Mobile!));
        }

        if (!string.IsNullOrWhiteSpace(request.City))
        {
            var city = request.City.ToLower();
            query = query.Where(up =>
                up.Addresses.Any(a => a.City.ToLower().Contains(city) && !a.IsDeleted));
        }

        if (!string.IsNullOrWhiteSpace(request.State))
        {
            var state = request.State.ToLower();
            query = query.Where(up =>
                up.Addresses.Any(a => a.State.ToLower().Contains(state) && !a.IsDeleted));
        }

        if (!string.IsNullOrWhiteSpace(request.Country))
        {
            var country = request.Country.ToLower();
            query = query.Where(up =>
                up.Addresses.Any(a => a.Country.ToLower().Contains(country) && !a.IsDeleted));
        }

        if (request.Role.HasValue)
        {
            query = query.Where(up =>
                up.User.UserRoles.Any(ur => ur.Role.RoleType == request.Role.Value));
        }

        if (request.Status.HasValue)
        {
            query = query.Where(up => up.User.Status == request.Status.Value);
        }

        if (request.Gender.HasValue)
        {
            query = query.Where(up => up.Gender == request.Gender.Value);
        }

        if (request.EmailVerified.HasValue)
        {
            query = query.Where(up => up.User.IsEmailVerified == request.EmailVerified.Value);
        }

        if (request.IsActive.HasValue)
        {
            if (request.IsActive.Value)
                query = query.Where(up => up.User.Status == UserStatus.Active);
            else
                query = query.Where(up => up.User.Status != UserStatus.Active);
        }

        if (request.IsDeleted.HasValue)
        {
            query = query.Where(up => up.IsDeleted == request.IsDeleted.Value);
        }
        else
        {
            query = query.Where(up => !up.IsDeleted);
        }

        if (request.CreatedFrom.HasValue)
        {
            query = query.Where(up => up.CreatedAt >= request.CreatedFrom.Value);
        }

        if (request.CreatedTo.HasValue)
        {
            query = query.Where(up => up.CreatedAt <= request.CreatedTo.Value);
        }

        if (request.UpdatedFrom.HasValue)
        {
            query = query.Where(up => up.UpdatedAt >= request.UpdatedFrom.Value);
        }

        if (request.UpdatedTo.HasValue)
        {
            query = query.Where(up => up.UpdatedAt <= request.UpdatedTo.Value);
        }

        return query;
    }

    private static IQueryable<UserProfile> ApplySorting(IQueryable<UserProfile> query, string? sortBy, bool descending)
    {
        return sortBy?.ToLower() switch
        {
            "name" => descending
                ? query.OrderByDescending(up => up.User.FullName)
                : query.OrderBy(up => up.User.FullName),
            "email" => descending
                ? query.OrderByDescending(up => up.User.Email)
                : query.OrderBy(up => up.User.Email),
            "role" => descending
                ? query.OrderByDescending(up => up.User.UserRoles.Min(ur => ur.Role.Name))
                : query.OrderBy(up => up.User.UserRoles.Min(ur => ur.Role.Name)),
            "createddate" => descending
                ? query.OrderByDescending(up => up.CreatedAt)
                : query.OrderBy(up => up.CreatedAt),
            "updateddate" => descending
                ? query.OrderByDescending(up => up.UpdatedAt)
                : query.OrderBy(up => up.UpdatedAt),
            _ => descending
                ? query.OrderByDescending(up => up.CreatedAt)
                : query.OrderBy(up => up.CreatedAt)
        };
    }

    private static UserListDto MapToUserListDto(UserProfile up) => new()
    {
        UserId = up.UserId,
        FullName = up.User.FullName,
        Email = up.User.Email,
        PhoneNumber = up.ContactInformation?.PrimaryPhoneNumber,
        ProfileImageUrl = up.User.ProfileImageUrl ?? up.ProfileImageUrl,
        Status = up.User.Status,
        IsEmailVerified = up.User.IsEmailVerified,
        Gender = up.Gender,
        City = up.Addresses.FirstOrDefault(a => a.IsPrimary && !a.IsDeleted)?.City,
        State = up.Addresses.FirstOrDefault(a => a.IsPrimary && !a.IsDeleted)?.State,
        Country = up.Addresses.FirstOrDefault(a => a.IsPrimary && !a.IsDeleted)?.Country,
        PreferredSport = up.PreferredSport,
        Roles = up.User.UserRoles.Select(ur => ur.Role.Name).ToList(),
        CreatedAt = up.CreatedAt,
        UpdatedAt = up.UpdatedAt
    };
}
