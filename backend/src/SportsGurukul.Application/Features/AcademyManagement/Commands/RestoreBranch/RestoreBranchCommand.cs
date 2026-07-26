using MediatR;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.AcademyManagement.DTOs;

namespace SportsGurukul.Application.Features.AcademyManagement.Commands.RestoreBranch;

public class RestoreBranchCommand : IRequest<Result<BranchDto>>
{
    public Guid BranchId { get; set; }
}
