using Microsoft.EntityFrameworkCore;
using SportsGurukul.Domain.Entities;

namespace SportsGurukul.Application.Common.Interfaces;

public interface IApplicationDbContext
{
    DbSet<TrainingCertificate> Certificates { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
