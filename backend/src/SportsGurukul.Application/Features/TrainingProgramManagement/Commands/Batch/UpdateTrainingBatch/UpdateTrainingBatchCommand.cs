using MediatR;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.TrainingProgramManagement.DTOs;

namespace SportsGurukul.Application.Features.TrainingProgramManagement.Commands.Batch.UpdateTrainingBatch;

public record UpdateTrainingBatchCommand(
    Guid Id,
    DateTime StartDate,
    DateTime? EndDate,
    int MaximumSeats
) : IRequest<Result<TrainingBatchDto>>;
