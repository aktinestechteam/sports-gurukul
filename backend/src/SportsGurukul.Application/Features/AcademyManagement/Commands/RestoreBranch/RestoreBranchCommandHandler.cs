using MediatR;
using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.AcademyManagement.DTOs;

namespace SportsGurukul.Application.Features.AcademyManagement.Commands.RestoreBranch;

public class RestoreBranchCommandHandler : IRequestHandler<RestoreBranchCommand, Result<BranchDto>>
{
    private readonly IAcademyBranchRepository _branchRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<RestoreBranchCommandHandler> _logger;

    public RestoreBranchCommandHandler(
        IAcademyBranchRepository branchRepository,
        IUnitOfWork unitOfWork,
        ILogger<RestoreBranchCommandHandler> logger)
    {
        _branchRepository = branchRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<BranchDto>> Handle(RestoreBranchCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Restoring branch with Id: {BranchId}", request.BranchId);

        var branch = await _branchRepository.GetByIdAsync(request.BranchId, cancellationToken);
        if (branch is null)
            return Result<BranchDto>.Failure("Branch not found.");

        if (!branch.IsDeleted)
            return Result<BranchDto>.Failure("Branch is not deleted.");

        branch.IsDeleted = false;
        branch.UpdatedAt = DateTime.UtcNow;

        _branchRepository.Update(branch);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Branch restored with Id: {BranchId}", request.BranchId);

        var dto = new BranchDto
        {
            Id = branch.Id,
            AcademyId = branch.AcademyId,
            BranchName = branch.BranchName,
            Address = branch.Address,
            Country = branch.Country,
            State = branch.State,
            City = branch.City,
            District = branch.District,
            PostalCode = branch.PostalCode,
            Latitude = branch.Latitude,
            Longitude = branch.Longitude,
            CreatedAt = branch.CreatedAt,
            UpdatedAt = branch.UpdatedAt
        };

        return Result<BranchDto>.Success(dto);
    }
}
