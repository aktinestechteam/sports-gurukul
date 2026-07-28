using MediatR;
using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.EventManagement.DTOs;
using SportsGurukul.Application.Features.EventManagement.Services;
using SportsGurukul.Domain.Entities;

namespace SportsGurukul.Application.Features.EventManagement.Commands.PublishAnnouncement;

public class PublishAnnouncementCommandHandler : IRequestHandler<PublishAnnouncementCommand, Result<AnnouncementDto>>
{
    private readonly IEventRepository _eventRepository;
    private readonly IEventAnnouncementService _announcementService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<PublishAnnouncementCommandHandler> _logger;

    public PublishAnnouncementCommandHandler(
        IEventRepository eventRepository,
        IEventAnnouncementService announcementService,
        IUnitOfWork unitOfWork,
        ILogger<PublishAnnouncementCommandHandler> logger)
    {
        _eventRepository = eventRepository;
        _announcementService = announcementService;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<AnnouncementDto>> Handle(PublishAnnouncementCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Publishing announcement for event: {EventId}", request.EventId);

        var evt = await _eventRepository.GetWithDetailsAsync(request.EventId, cancellationToken);
        if (evt is null)
            return Result<AnnouncementDto>.Failure("Event not found.");

        var canPublish = await _announcementService.CanPublishAnnouncementAsync(evt, cancellationToken);
        if (!canPublish)
            return Result<AnnouncementDto>.Failure("Announcement cannot be published for this event.");

        var announcement = new EventAnnouncement
        {
            Id = Guid.NewGuid(),
            EventId = request.EventId,
            Title = request.Title,
            Message = request.Message,
            IsPublished = true,
            PublishedAt = DateTime.UtcNow,
            SendNotification = request.SendNotification,
            Priority = request.Priority,
            CreatedAt = DateTime.UtcNow
        };

        evt.Announcements.Add(announcement);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Announcement published: {AnnouncementId}", announcement.Id);

        var dto = MapToDto(announcement, evt.EventName);
        return Result<AnnouncementDto>.Success(dto);
    }

    internal static AnnouncementDto MapToDto(EventAnnouncement ann, string eventName = "")
    {
        return new AnnouncementDto
        {
            Id = ann.Id,
            EventId = ann.EventId,
            EventName = eventName,
            Title = ann.Title,
            Message = ann.Message,
            IsPublished = ann.IsPublished,
            PublishedAt = ann.PublishedAt,
            SendNotification = ann.SendNotification,
            Priority = ann.Priority,
            CreatedAt = ann.CreatedAt,
            UpdatedAt = ann.UpdatedAt
        };
    }
}
