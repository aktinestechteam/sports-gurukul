using MediatR;
using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.FacilityManagement.DTOs;

namespace SportsGurukul.Application.Features.FacilityManagement.Commands.UpdateCourt;

public class UpdateCourtCommandHandler : IRequestHandler<UpdateCourtCommand, Result<CourtDto>>
{
    private readonly IFacilityCourtRepository _courtRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<UpdateCourtCommandHandler> _logger;

    public UpdateCourtCommandHandler(
        IFacilityCourtRepository courtRepository,
        IUnitOfWork unitOfWork,
        ILogger<UpdateCourtCommandHandler> logger)
    {
        _courtRepository = courtRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<CourtDto>> Handle(UpdateCourtCommand request, CancellationToken cancellationToken)
    {
        var court = await _courtRepository.GetByIdAsync(request.CourtId, cancellationToken);
        if (court is null)
        {
            return Result<CourtDto>.Failure("Court not found.");
        }

        if (request.CourtName is not null)
            court.CourtName = request.CourtName;
        if (request.CourtType is not null)
            court.CourtType = request.CourtType;
        if (request.Capacity is not null)
            court.Capacity = request.Capacity;
        if (request.Status is not null)
            court.Status = request.Status.Value;
        if (request.Description is not null)
            court.Description = request.Description;

        court.UpdatedAt = DateTime.UtcNow;

        _courtRepository.Update(court);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Court updated with Id: {CourtId}", court.Id);

        var dto = new CourtDto
        {
            Id = court.Id,
            FacilityId = court.FacilityId,
            CourtNumber = court.CourtNumber,
            CourtName = court.CourtName,
            CourtType = court.CourtType,
            Capacity = court.Capacity,
            Status = court.Status.ToString(),
            Description = court.Description
        };

        return Result<CourtDto>.Success(dto);
    }
}
