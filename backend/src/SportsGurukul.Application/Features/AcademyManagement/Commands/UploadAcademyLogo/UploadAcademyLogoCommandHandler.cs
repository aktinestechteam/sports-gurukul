using MediatR;
using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.AcademyManagement.DTOs;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Application.Features.AcademyManagement.Commands.UploadAcademyLogo;

public class UploadAcademyLogoCommandHandler : IRequestHandler<UploadAcademyLogoCommand, Result<AcademyDto>>
{
    private readonly IAcademyRepository _academyRepository;
    private readonly IFileStorageService _fileStorageService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<UploadAcademyLogoCommandHandler> _logger;

    public UploadAcademyLogoCommandHandler(
        IAcademyRepository academyRepository,
        IFileStorageService fileStorageService,
        IUnitOfWork unitOfWork,
        ILogger<UploadAcademyLogoCommandHandler> logger)
    {
        _academyRepository = academyRepository;
        _fileStorageService = fileStorageService;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<AcademyDto>> Handle(UploadAcademyLogoCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Uploading logo for academy: {AcademyId}", request.AcademyId);

        var academy = await _academyRepository.GetByIdAsync(request.AcademyId, cancellationToken);
        if (academy is null)
        {
            _logger.LogWarning("Academy not found: {AcademyId}", request.AcademyId);
            return Result<AcademyDto>.Failure("Academy not found.");
        }

        using var stream = new MemoryStream(request.FileContent);
        var storageResult = await _fileStorageService.UploadAsync(
            stream, request.FileName, request.ContentType, FileCategory.Image, cancellationToken);

        academy.LogoUrl = storageResult.PublicUrl ?? storageResult.StoragePath;
        academy.UpdatedAt = DateTime.UtcNow;
        _academyRepository.Update(academy);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Logo uploaded for academy: {AcademyId}, Url: {Url}", request.AcademyId, academy.LogoUrl);

        var updated = await _academyRepository.GetByIdWithDetailsAsync(request.AcademyId, cancellationToken);
        return Result<AcademyDto>.Success(AcademyDtoMapper.Map(updated ?? academy));
    }
}
