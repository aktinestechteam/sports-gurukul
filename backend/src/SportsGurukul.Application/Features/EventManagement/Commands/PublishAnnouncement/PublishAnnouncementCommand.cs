using MediatR;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.EventManagement.DTOs;

namespace SportsGurukul.Application.Features.EventManagement.Commands.PublishAnnouncement;

public class PublishAnnouncementCommand : IRequest<Result<AnnouncementDto>>
{
    public Guid EventId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public bool SendNotification { get; set; }
    public string? Priority { get; set; }
}
