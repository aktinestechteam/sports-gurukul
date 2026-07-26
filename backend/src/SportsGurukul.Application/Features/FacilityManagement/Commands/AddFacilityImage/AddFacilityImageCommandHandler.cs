using MediatR;
using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.FacilityManagement.DTOs;
using SportsGurukul.Domain.Entities;

namespace SportsGurukul.Application.Features.FacilityManagement.Commands.AddFacilityImage;

public class AddFacilityImageCommandHandler : IRequestHandler<AddFacilityImageCommand, Result<ImageDto>>
{
    private readonly IFacilityRepository _facilityRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<AddFacilityImageCommandHandler> _logger;

    public AddFacilityImageCommandHandler(
        IFacilityRepository facilityRepository,
        IUnitOfWork unitOfWork,
        ILogger<AddFacilityImageCommandHandler> logger)
    {
        _facilityRepository = facilityRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<ImageDto>> Handle(AddFacilityImageCommand request, CancellationToken cancellationToken)
    {
        var facility = await _facilityRepository.GetWithDetailsAsync(request.FacilityId, cancellationToken);
        if (facility is null)
        {
            return Result<ImageDto>.Failure("Facility not found.");
        }

        if (request.IsPrimary)
        {
            foreach (var existingImage in facility.Images.Where(i => i.IsPrimary))
            {
                existingImage.IsPrimary = false;
                existingImage.UpdatedAt = DateTime.UtcNow;
            }
        }

        var maxSortOrder = facility.Images.Any() ? facility.Images.Max(i => i.SortOrder) : 0;

        var image = new FacilityImage
        {
            Id = Guid.NewGuid(),
            FacilityId = request.FacilityId,
            ImageUrl = request.ImageUrl,
            Caption = request.Caption,
            IsPrimary = request.IsPrimary,
            SortOrder = maxSortOrder + 1
        };

        facility.Images.Add(image);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Image added with Id: {ImageId} for Facility: {FacilityId}", image.Id, request.FacilityId);

        var dto = new ImageDto
        {
            Id = image.Id,
            FacilityId = image.FacilityId,
            ImageUrl = image.ImageUrl,
            Caption = image.Caption,
            IsPrimary = image.IsPrimary,
            SortOrder = image.SortOrder
        };

        return Result<ImageDto>.Success(dto);
    }
}
