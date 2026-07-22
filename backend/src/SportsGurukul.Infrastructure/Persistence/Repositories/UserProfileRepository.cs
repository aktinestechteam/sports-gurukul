using Microsoft.EntityFrameworkCore;
using SportsGurukul.Application.Common.Interfaces;
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
            .Include(up => up.Addresses.Where(a => !a.IsDeleted))
            .Include(up => up.ContactInformation)
            .Include(up => up.UserPreference)
            .FirstOrDefaultAsync(up => up.UserId == userId && !up.IsDeleted, cancellationToken);
    }
}
