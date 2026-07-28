using MediatR;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.EventManagement.DTOs;

namespace SportsGurukul.Application.Features.EventManagement.Commands.ArchiveAnnouncement;

public class ArchiveAnnouncementCommand : IRequest<Result<AnnouncementDto>>
{
    public Guid AnnouncementId { get; set; }
}
