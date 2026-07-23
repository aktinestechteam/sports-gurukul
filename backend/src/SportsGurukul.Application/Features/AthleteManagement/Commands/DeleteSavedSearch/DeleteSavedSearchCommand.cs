using MediatR;
using SportsGurukul.Application.Common.Models;

namespace SportsGurukul.Application.Features.AthleteManagement.Commands.DeleteSavedSearch;

public class DeleteSavedSearchCommand : IRequest<Result<Unit>>
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
}
