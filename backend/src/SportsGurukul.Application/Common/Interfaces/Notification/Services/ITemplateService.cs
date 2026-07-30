using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.NotificationManagement.DTOs;

namespace SportsGurukul.Application.Common.Interfaces.Notification.Services;

public interface ITemplateService
{
    Task<Result<TemplateDto>> CreateAsync(CreateTemplateRequest request, CancellationToken cancellationToken = default);
    Task<Result<TemplateDto>> UpdateAsync(UpdateTemplateRequest request, CancellationToken cancellationToken = default);
    Task<Result<TemplateDto>> PublishAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Result<bool>> ArchiveAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Result<TemplateVersionDto>> CreateVersionAsync(CreateTemplateVersionRequest request, CancellationToken cancellationToken = default);
    Task<Result<TemplateDto>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Result<List<TemplateVersionDto>>> GetVersionsAsync(Guid templateId, CancellationToken cancellationToken = default);
}
