using MediatR;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.FacilityManagement.DTOs;

namespace SportsGurukul.Application.Features.FacilityManagement.Commands.AddCourt;

public class AddCourtCommand : IRequest<Result<CourtDto>>
{
    public Guid FacilityId { get; set; }
    public string CourtNumber { get; set; } = string.Empty;
    public string CourtName { get; set; } = string.Empty;
    public string? CourtType { get; set; }
    public int? Capacity { get; set; }
    public string? Description { get; set; }
}
