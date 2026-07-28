using MediatR;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.EventManagement.DTOs;

namespace SportsGurukul.Application.Features.EventManagement.Queries.GetAnnouncementsByEvent;

public class GetAnnouncementsByEventQuery : IRequest<Result<List<AnnouncementDto>>>
{
    public Guid EventId { get; set; }
}
