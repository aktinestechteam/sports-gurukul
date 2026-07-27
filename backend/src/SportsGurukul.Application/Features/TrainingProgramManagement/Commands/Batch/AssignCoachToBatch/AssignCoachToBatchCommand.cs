using MediatR;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.TrainingProgramManagement.DTOs;

namespace SportsGurukul.Application.Features.TrainingProgramManagement.Commands.Batch.AssignCoachToBatch;

public record AssignCoachToBatchCommand(
    Guid Id,
    Guid CoachId
) : IRequest<Result<TrainingBatchDto>>;
