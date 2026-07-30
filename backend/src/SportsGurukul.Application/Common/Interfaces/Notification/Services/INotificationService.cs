using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.NotificationManagement.DTOs;

namespace SportsGurukul.Application.Common.Interfaces.Notification.Services;

public interface INotificationService
{
    Task<Result<NotificationDto>> CreateAsync(CreateNotificationRequest request, CancellationToken cancellationToken = default);
    Task<Result<NotificationDto>> UpdateAsync(UpdateNotificationRequest request, CancellationToken cancellationToken = default);
    Task<Result<bool>> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Result<bool>> QueueAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Result<bool>> ScheduleAsync(Guid id, DateTime scheduledAt, CancellationToken cancellationToken = default);
    Task<Result<bool>> CancelAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Result<bool>> SendAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Result<bool>> RetryAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Result<bool>> ExpireAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Result<bool>> MarkReadAsync(Guid id, Guid? userId, CancellationToken cancellationToken = default);
    Task<Result<bool>> ArchiveAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Result<NotificationDto>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
}
