using SportsGurukul.Domain.Entities;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Application.Common.Interfaces;

public interface IFileRepository : IRepository<UserFile>
{
    Task<UserFile?> GetByUserIdAndTypeAsync(Guid userId, FileType fileType, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<UserFile>> GetAllByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<UserFile?> GetActiveProfilePhotoAsync(Guid userId, CancellationToken cancellationToken = default);
}
