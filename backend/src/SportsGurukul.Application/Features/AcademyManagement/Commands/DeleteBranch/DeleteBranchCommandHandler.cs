using MediatR;
using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Common.Models;

namespace SportsGurukul.Application.Features.AcademyManagement.Commands.DeleteBranch;

public class DeleteBranchCommandHandler : IRequestHandler<DeleteBranchCommand, Result<Unit>>
{
    private readonly IAcademyBranchRepository _branchRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<DeleteBranchCommandHandler> _logger;

    public DeleteBranchCommandHandler(
        IAcademyBranchRepository branchRepository,
        IUnitOfWork unitOfWork,
        ILogger<DeleteBranchCommandHandler> logger)
    {
        _branchRepository = branchRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<Unit>> Handle(DeleteBranchCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Deleting branch with Id: {BranchId}", request.BranchId);

        var branch = await _branchRepository.GetByIdAsync(request.BranchId, cancellationToken);
        if (branch is null)
            return Result<Unit>.Failure("Branch not found.");

        if (branch.IsDeleted)
            return Result<Unit>.Failure("Branch is already deleted.");

        _branchRepository.Remove(branch);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Branch deleted with Id: {BranchId}", request.BranchId);

        return Result<Unit>.Success(Unit.Value);
    }
}
