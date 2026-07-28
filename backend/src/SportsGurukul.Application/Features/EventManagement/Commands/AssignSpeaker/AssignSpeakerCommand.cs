using MediatR;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.EventManagement.DTOs;

namespace SportsGurukul.Application.Features.EventManagement.Commands.AssignSpeaker;

public class AssignSpeakerCommand : IRequest<Result<EventSessionDto>>
{
    public Guid SessionId { get; set; }
    public Guid SpeakerId { get; set; }
}
