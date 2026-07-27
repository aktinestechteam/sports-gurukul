using MediatR;
using SportsGurukul.Application.Common.Models;

namespace SportsGurukul.Application.Features.TrainingProgramManagement.Commands.Progress.UpdateTrainingProgress;

public record UpdateTrainingProgressCommand : IRequest<Result<DTOs.TrainingProgressDto>>
{
    public Guid EnrollmentId { get; init; }
    public string CurrentLevel { get; init; } = string.Empty;
    public decimal CompletedPercentage { get; init; }
    public decimal? OverallRating { get; init; }
}
