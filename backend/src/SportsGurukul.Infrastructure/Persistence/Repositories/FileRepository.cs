using Microsoft.EntityFrameworkCore;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Domain.Entities;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Infrastructure.Persistence.Repositories;

public class FileRepository : Repository<UserFile>, IFileRepository
{
    public FileRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<UserFile?> GetByUserIdAndTypeAsync(Guid userId, FileType fileType, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .AsNoTracking()
            .FirstOrDefaultAsync(uf =>
                uf.UserId == userId &&
                uf.FileType == fileType &&
                !uf.IsDeleted,
                cancellationToken);
    }

    public async Task<IReadOnlyList<UserFile>> GetAllByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .AsNoTracking()
            .Where(uf => uf.UserId == userId && !uf.IsDeleted)
            .OrderByDescending(uf => uf.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<UserFile?> GetActiveProfilePhotoAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .AsNoTracking()
            .FirstOrDefaultAsync(uf =>
                uf.UserId == userId &&
                uf.FileType == FileType.ProfilePhoto &&
                !uf.IsDeleted,
                cancellationToken);
    }
}
