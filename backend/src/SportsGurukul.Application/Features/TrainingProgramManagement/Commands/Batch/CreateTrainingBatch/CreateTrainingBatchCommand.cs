using MediatR;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.TrainingProgramManagement.DTOs;

namespace SportsGurukul.Application.Features.TrainingProgramManagement.Commands.Batch.CreateTrainingBatch;

public record CreateTrainingBatchCommand(
    Guid ProgramId,
    Guid CoachId,
    Guid BranchId,
    DateTime StartDate,
    DateTime? EndDate,
    int MaximumSeats
) : IRequest<Result<TrainingBatchDto>>;
