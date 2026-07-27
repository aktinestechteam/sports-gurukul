using MediatR;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.TrainingProgramManagement.DTOs;

namespace SportsGurukul.Application.Features.TrainingProgramManagement.Commands.Session.AssignFacility;

public record AssignFacilityCommand(
    Guid Id,
    Guid? FacilityId
) : IRequest<Result<TrainingSessionDto>>;
