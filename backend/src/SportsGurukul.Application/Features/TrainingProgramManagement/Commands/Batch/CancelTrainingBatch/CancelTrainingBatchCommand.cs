using MediatR;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.TrainingProgramManagement.DTOs;

namespace SportsGurukul.Application.Features.TrainingProgramManagement.Commands.Batch.CancelTrainingBatch;

public record CancelTrainingBatchCommand(
    Guid Id
) : IRequest<Result<TrainingBatchDto>>;
