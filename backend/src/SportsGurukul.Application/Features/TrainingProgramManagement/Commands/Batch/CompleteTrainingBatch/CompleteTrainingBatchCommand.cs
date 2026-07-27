using MediatR;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.TrainingProgramManagement.DTOs;

namespace SportsGurukul.Application.Features.TrainingProgramManagement.Commands.Batch.CompleteTrainingBatch;

public record CompleteTrainingBatchCommand(
    Guid Id
) : IRequest<Result<TrainingBatchDto>>;
