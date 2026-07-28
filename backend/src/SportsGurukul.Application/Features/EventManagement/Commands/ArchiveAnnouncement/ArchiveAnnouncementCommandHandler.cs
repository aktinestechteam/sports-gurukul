using MediatR;
using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.EventManagement.Commands.PublishAnnouncement;
using SportsGurukul.Application.Features.EventManagement.DTOs;
using SportsGurukul.Domain.Entities;

namespace SportsGurukul.Application.Features.EventManagement.Commands.ArchiveAnnouncement;

public class ArchiveAnnouncementCommandHandler : IRequestHandler<ArchiveAnnouncementCommand, Result<AnnouncementDto>>
{
    private readonly IEventRepository _eventRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<ArchiveAnnouncementCommandHandler> _logger;

    public ArchiveAnnouncementCommandHandler(
        IEventRepository eventRepository,
        IUnitOfWork unitOfWork,
        ILogger<ArchiveAnnouncementCommandHandler> logger)
    {
        _eventRepository = eventRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<AnnouncementDto>> Handle(ArchiveAnnouncementCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Archiving announcement: {AnnouncementId}", request.AnnouncementId);

        EventAnnouncement? announcement = null;
        string eventName = string.Empty;

        var events = await _eventRepository.GetAllAsync(cancellationToken);
        foreach (var evt in events)
        {
            var evtWithDetails = await _eventRepository.GetWithDetailsAsync(evt.Id, cancellationToken);
            if (evtWithDetails is null) continue;

            announcement = evtWithDetails.Announcements.FirstOrDefault(a => a.Id == request.AnnouncementId);
            if (announcement is not null)
            {
                eventName = evtWithDetails.EventName;
                break;
            }
        }

        if (announcement is null)
            return Result<AnnouncementDto>.Failure("Announcement not found.");

        announcement.IsDeleted = true;
        announcement.UpdatedAt = DateTime.UtcNow;

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Announcement archived: {AnnouncementId}", request.AnnouncementId);

        var dto = PublishAnnouncementCommandHandler.MapToDto(announcement, eventName);
        return Result<AnnouncementDto>.Success(dto);
    }
}
