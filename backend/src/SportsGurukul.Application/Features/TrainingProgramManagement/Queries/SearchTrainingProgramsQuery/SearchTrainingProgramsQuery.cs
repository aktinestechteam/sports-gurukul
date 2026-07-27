using MediatR;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.TrainingProgramManagement.DTOs;

namespace SportsGurukul.Application.Features.TrainingProgramManagement.Queries.SearchTrainingProgramsQuery
{
    public class SearchTrainingProgramsQuery : IRequest<Result<TrainingProgramSearchResponse>>
    {
        public Guid? AcademyId { get; set; }
        public Guid? SportId { get; set; }
        public string? Status { get; set; }
        public string? SearchTerm { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
