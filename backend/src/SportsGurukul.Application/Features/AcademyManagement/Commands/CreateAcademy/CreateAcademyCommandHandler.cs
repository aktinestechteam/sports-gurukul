using MediatR;
using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.AcademyManagement.DTOs;
using SportsGurukul.Domain.Entities;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Application.Features.AcademyManagement.Commands.CreateAcademy;

public class CreateAcademyCommandHandler : IRequestHandler<CreateAcademyCommand, Result<AcademyDto>>
{
    private readonly IAcademyRepository _academyRepository;
    private readonly ISportRepository _sportRepository;
    private readonly IUserRepository _userRepository;
    private readonly IRoleRepository _roleRepository;
    private readonly IUserRoleRepository _userRoleRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<CreateAcademyCommandHandler> _logger;

    public CreateAcademyCommandHandler(
        IAcademyRepository academyRepository,
        ISportRepository sportRepository,
        IUserRepository userRepository,
        IRoleRepository roleRepository,
        IUserRoleRepository userRoleRepository,
        IUnitOfWork unitOfWork,
        ILogger<CreateAcademyCommandHandler> logger)
    {
        _academyRepository = academyRepository;
        _sportRepository = sportRepository;
        _userRepository = userRepository;
        _roleRepository = roleRepository;
        _userRoleRepository = userRoleRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<AcademyDto>> Handle(CreateAcademyCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Creating academy with Email: {Email}", request.Email);

        var existingAcademy = await _academyRepository.GetByEmailAsync(request.Email, cancellationToken);
        if (existingAcademy is not null)
            return Result<AcademyDto>.Failure("An academy with this email already exists.");

        var sportNames = (request.SportNames ?? [])
            .Select(name => name.Trim())
            .Where(name => name.Length > 0)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();

        var academySports = new List<AcademySport>(sportNames.Count);
        for (var index = 0; index < sportNames.Count; index++)
        {
            var sport = await _sportRepository.GetByNameAsync(sportNames[index], cancellationToken);
            if (sport is null)
            {
                _logger.LogWarning("Sport not found while creating academy: {SportName}", sportNames[index]);
                return Result<AcademyDto>.Failure($"Sport '{sportNames[index]}' is not supported.");
            }

            academySports.Add(new AcademySport
            {
                Id = Guid.NewGuid(),
                SportId = sport.Id,
                IsPrimarySport = index == 0,
                JoinedDate = DateTime.UtcNow
            });
        }

        var now = DateTime.UtcNow;
        var academyCode = await GenerateUniqueAcademyCodeAsync(cancellationToken);

        var academy = new Academy
        {
            Id = Guid.NewGuid(),
            AcademyCode = academyCode,
            Name = request.Name,
            LegalName = request.LegalName,
            Description = request.Description,
            RegistrationNumber = request.RegistrationNumber,
            GSTNumber = request.GSTNumber,
            EstablishedDate = request.EstablishedDate,
            Website = request.Website,
            Email = request.Email,
            Phone = request.Phone,
            AcademyType = request.AcademyType,
            Status = AcademyStatus.Pending,
            VerificationStatus = VerificationStatus.Pending,
            OwnedByUserId = request.UserId,
            Verification = new AcademyVerification
            {
                Id = Guid.NewGuid(),
                VerificationStatus = VerificationStatus.Pending,
                CreatedAt = now,
                UpdatedAt = now
            },
            OperatingHours = new AcademyOperatingHours
            {
                Id = Guid.NewGuid(),
                CreatedAt = now,
                UpdatedAt = now
            },
            CreatedAt = now,
            UpdatedAt = now
        };

        if (HasContactInformation(request))
        {
            academy.Contact = new AcademyContact
            {
                Id = Guid.NewGuid(),
                AcademyId = academy.Id,
                PrimaryContactName = request.PrimaryContactName,
                PrimaryPhone = request.Phone,
                PrimaryEmail = request.Email,
                Address = request.Address,
                Country = request.Country,
                State = request.State,
                City = request.City,
                PostalCode = request.PostalCode
            };
        }

        foreach (var academySport in academySports)
        {
            academySport.AcademyId = academy.Id;
            academy.AcademySports.Add(academySport);
        }

        await _academyRepository.AddAsync(academy, cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        await GrantAcademyAdminRoleAsync(request.UserId, cancellationToken);

        _logger.LogInformation("Academy created with Id: {AcademyId}, AcademyCode: {AcademyCode}", academy.Id, academyCode);

        return Result<AcademyDto>.Success(AcademyDtoMapper.Map(academy));
    }

    /// <summary>
    /// Grants the creator the <c>Academy Admin</c> role so they can manage the
    /// academy. Best-effort: a failure here must never fail the academy
    /// creation itself.
    /// </summary>
    private async Task GrantAcademyAdminRoleAsync(Guid? userId, CancellationToken cancellationToken)
    {
        if (userId is null || userId == Guid.Empty)
            return;

        try
        {
            var user = await _userRepository.GetByIdAsync(userId.Value, cancellationToken);
            if (user is null)
            {
                _logger.LogWarning("Skipping Academy Admin role grant: user {UserId} not found", userId);
                return;
            }

            var role = await _roleRepository.GetByNameAsync("Academy Admin", cancellationToken);
            if (role is null)
            {
                _logger.LogWarning("Skipping Academy Admin role grant: 'Academy Admin' role not found");
                return;
            }

            var userRoles = await _userRoleRepository.GetByUserIdAsync(userId.Value, cancellationToken);
            if (userRoles.Any(ur => ur.RoleId == role.Id))
                return;

            await _userRoleRepository.AddAsync(new UserRole
            {
                UserId = userId.Value,
                RoleId = role.Id,
                AssignedAt = DateTime.UtcNow
            }, cancellationToken);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Granted 'Academy Admin' role to user {UserId}", userId);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to grant 'Academy Admin' role to user {UserId}", userId);
        }
    }

    private static bool HasContactInformation(CreateAcademyCommand request) =>
        !string.IsNullOrWhiteSpace(request.PrimaryContactName) ||
        !string.IsNullOrWhiteSpace(request.Address) ||
        !string.IsNullOrWhiteSpace(request.Country) ||
        !string.IsNullOrWhiteSpace(request.State) ||
        !string.IsNullOrWhiteSpace(request.City) ||
        !string.IsNullOrWhiteSpace(request.PostalCode);

    private async Task<string> GenerateUniqueAcademyCodeAsync(CancellationToken cancellationToken)
    {
        const int maxAttempts = 10;

        for (var attempt = 0; attempt < maxAttempts; attempt++)
        {
            var datePart = DateTime.UtcNow.ToString("yyyyMMdd");
            var randomPart = Guid.NewGuid().ToString("N")[..4].ToUpperInvariant();
            var academyCode = $"ACAD-{datePart}-{randomPart}";

            var exists = await _academyRepository.AnyAsync(a => a.AcademyCode == academyCode, cancellationToken);
            if (!exists)
                return academyCode;
        }

        throw new InvalidOperationException("Unable to generate a unique academy code.");
    }
}
