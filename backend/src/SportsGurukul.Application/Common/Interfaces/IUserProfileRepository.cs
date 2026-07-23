using SportsGurukul.Application.Features.UserManagement.DTOs;
using SportsGurukul.Domain.Entities;

namespace SportsGurukul.Application.Common.Interfaces;

public interface IUserProfileRepository : IRepository<UserProfile>
{
    Task<UserProfile?> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<UserProfile?> GetWithAddressesAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<UserProfile?> GetWithContactInformationAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<UserProfile?> GetFullProfileAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<(IReadOnlyList<UserListDto> Users, int TotalCount)> SearchProfilesAsync(UserSearchRequest request, CancellationToken cancellationToken = default);
    Task<UserProfile?> GetDeletedByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
}
