using MediatR;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.AthleteManagement.DTOs;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Application.Features.AthleteManagement.Commands.UpdateEmergencyContact;

public class UpdateEmergencyContactCommand : IRequest<Result<EmergencyContactDto>>
{
    public Guid AthleteId { get; set; }
    public string Name { get; set; } = string.Empty;
    public EmergencyRelationship Relationship { get; set; }
    public string Phone { get; set; } = string.Empty;
    public string? Email { get; set; }
}
