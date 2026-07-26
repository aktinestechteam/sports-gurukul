using MediatR;
using SportsGurukul.Application.Common.Models;

namespace SportsGurukul.Application.Features.AcademyManagement.Commands.RemoveSport;

public class RemoveSportCommand : IRequest<Result<Unit>>
{
    public Guid AcademyId { get; set; }
    public Guid SportId { get; set; }
}
