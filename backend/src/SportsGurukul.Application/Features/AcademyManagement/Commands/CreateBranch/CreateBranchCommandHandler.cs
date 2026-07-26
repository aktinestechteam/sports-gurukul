using MediatR;
using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.AcademyManagement.DTOs;
using SportsGurukul.Domain.Entities;

namespace SportsGurukul.Application.Features.AcademyManagement.Commands.CreateBranch;

public class CreateBranchCommandHandler : IRequestHandler<CreateBranchCommand, Result<BranchDto>>
{
    private readonly IAcademyRepository _academyRepository;
    private readonly IAcademyBranchRepository _academyBranchRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<CreateBranchCommandHandler> _logger;

    public CreateBranchCommandHandler(
        IAcademyRepository academyRepository,
        IAcademyBranchRepository academyBranchRepository,
        IUnitOfWork unitOfWork,
        ILogger<CreateBranchCommandHandler> logger)
    {
        _academyRepository = academyRepository;
        _academyBranchRepository = academyBranchRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<BranchDto>> Handle(CreateBranchCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Creating branch for academy: {AcademyId}", request.AcademyId);

        var academy = await _academyRepository.GetByIdAsync(request.AcademyId, cancellationToken);
        if (academy is null)
        {
            _logger.LogWarning("Academy not found: {AcademyId}", request.AcademyId);
            return Result<BranchDto>.Failure("Academy not found.");
        }

        var existingBranch = await _academyBranchRepository.GetByAcademyIdAndNameAsync(request.AcademyId, request.BranchName, cancellationToken);
        if (existingBranch is not null)
        {
            _logger.LogWarning("Branch name '{BranchName}' already exists for academy: {AcademyId}", request.BranchName, request.AcademyId);
            return Result<BranchDto>.Failure($"A branch with the name '{request.BranchName}' already exists for this academy.");
        }

        var now = DateTime.UtcNow;

        var branch = new AcademyBranch
        {
            Id = Guid.NewGuid(),
            AcademyId = request.AcademyId,
            BranchName = request.BranchName,
            Address = request.Address,
            Country = request.Country,
            State = request.State,
            City = request.City,
            District = request.District,
            PostalCode = request.PostalCode,
            Latitude = request.Latitude,
            Longitude = request.Longitude,
            CreatedAt = now
        };

        await _academyBranchRepository.AddAsync(branch, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Branch created: {BranchId} for academy: {AcademyId}", branch.Id, request.AcademyId);

        var dto = MapToDto(branch);

        return Result<BranchDto>.Success(dto);
    }

    private static BranchDto MapToDto(AcademyBranch branch)
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
