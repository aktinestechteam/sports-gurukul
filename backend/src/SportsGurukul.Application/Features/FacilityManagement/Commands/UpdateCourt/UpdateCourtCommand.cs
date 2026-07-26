using MediatR;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.FacilityManagement.DTOs;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Application.Features.FacilityManagement.Commands.UpdateCourt;

public class UpdateCourtCommand : IRequest<Result<CourtDto>>
{
    public Guid CourtId { get; set; }
    public string? CourtName { get; set; }
    public string? CourtType { get; set; }
    public int? Capacity { get; set; }
    public FacilityCourtStatus? Status { get; set; }
    public string? Description { get; set; }
}
