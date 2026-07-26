using MediatR;
using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.AcademyManagement.DTOs;

namespace SportsGurukul.Application.Features.AcademyManagement.Commands.UpdateBranch;

public class UpdateBranchCommandHandler : IRequestHandler<UpdateBranchCommand, Result<BranchDto>>
{
    private readonly IAcademyBranchRepository _academyBranchRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<UpdateBranchCommandHandler> _logger;

    public UpdateBranchCommandHandler(
        IAcademyBranchRepository academyBranchRepository,
        IUnitOfWork unitOfWork,
        ILogger<UpdateBranchCommandHandler> logger)
    {
        _academyBranchRepository = academyBranchRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<BranchDto>> Handle(UpdateBranchCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Updating branch: {BranchId}", request.BranchId);

        var branch = await _academyBranchRepository.GetByIdAsync(request.BranchId, cancellationToken);
        if (branch is null)
        {
            _logger.LogWarning("Branch not found: {BranchId}", request.BranchId);
            return Result<BranchDto>.Failure("Branch not found.");
        }

        if (branch.AcademyId != request.AcademyId)
        {
            _logger.LogWarning("Branch {BranchId} does not belong to academy: {AcademyId}", request.BranchId, request.AcademyId);
            return Result<BranchDto>.Failure("Branch does not belong to the specified academy.");
        }

        if (request.BranchName is not null && request.BranchName != branch.BranchName)
        {
            var existingBranch = await _academyBranchRepository.GetByAcademyIdAndNameAsync(request.AcademyId, request.BranchName, cancellationToken);
            if (existingBranch is not null)
            {
                _logger.LogWarning("Branch name '{BranchName}' already exists for academy: {AcademyId}", request.BranchName, request.AcademyId);
                return Result<BranchDto>.Failure($"A branch with the name '{request.BranchName}' already exists for this academy.");
            }
        }

        var now = DateTime.UtcNow;

        if (request.BranchName is not null)
            branch.BranchName = request.BranchName;

        if (request.Address is not null)
            branch.Address = request.Address;

        if (request.Country is not null)
            branch.Country = request.Country;

        if (request.State is not null)
            branch.State = request.State;

        if (request.City is not null)
            branch.City = request.City;

        if (request.District is not null)
            branch.District = request.District;

        if (request.PostalCode is not null)
            branch.PostalCode = request.PostalCode;

        if (request.Latitude.HasValue)
            branch.Latitude = request.Latitude;

        if (request.Longitude.HasValue)
            branch.Longitude = request.Longitude;

        branch.UpdatedAt = now;

        _academyBranchRepository.Update(branch);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Branch updated: {BranchId}", request.BranchId);

        var dto = MapToDto(branch);

        return Result<BranchDto>.Success(dto);
    }

    private static BranchDto MapToDto(Domain.Entities.AcademyBranch branch)
    {
        return new BranchDto
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
    }
}
