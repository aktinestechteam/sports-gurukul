using MediatR;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.TrainingProgramManagement.DTOs;

namespace SportsGurukul.Application.Features.TrainingProgramManagement.Queries.GetTrainingStatisticsQuery
{
    public class GetTrainingStatisticsQuery : IRequest<Result<TrainingStatisticsDto>>
    {
    }
}
