using MediatR;
using SportsGurukul.Application.Common.Models;

namespace SportsGurukul.Application.Features.AcademySearchDiscovery.Commands.RecordAcademySearch;

public class RecordAcademySearchCommand : IRequest<Result<Unit>>
{
    public Guid UserId { get; set; }
    public string SearchTerm { get; set; } = string.Empty;
    public string? City { get; set; }
    public string? State { get; set; }
    public string? SportName { get; set; }
    public int AcademyCount { get; set; }
}
