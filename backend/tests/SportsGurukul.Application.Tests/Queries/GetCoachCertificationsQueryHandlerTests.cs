using FluentAssertions;
using Moq;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Features.CoachManagement.Queries.GetCoachCertifications;
using SportsGurukul.Application.Tests.Common;
using SportsGurukul.Domain.Entities;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Application.Tests.Queries;

public class GetCoachCertificationsQueryHandlerTests
{
    private readonly Mock<ICoachCertificationRepository> _certificationRepositoryMock = TestMocks.CreateCoachCertificationRepository();
    private readonly Mock<ILogger<GetCoachCertificationsQueryHandler>> _loggerMock = TestMocks.CreateLogger<GetCoachCertificationsQueryHandler>();
    private readonly GetCoachCertificationsQueryHandler _handler;

    public GetCoachCertificationsQueryHandlerTests()
    {
        _handler = new GetCoachCertificationsQueryHandler(
            _certificationRepositoryMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_ReturnsCertifications()
    {
        var coachId = Guid.NewGuid();
        var certifications = new List<CoachCertification>
        {
            TestDataBuilder.CreateCoachCertification(coachId)
        };

        _certificationRepositoryMock.Setup(r => r.GetByCoachIdAsync(coachId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(certifications);

        var result = await _handler.Handle(new GetCoachCertificationsQuery { CoachId = coachId }, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(1);
        result.Value![0].CertificationName.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task Handle_NoCertifications_ReturnsEmptyList()
    {
        var coachId = Guid.NewGuid();
        _certificationRepositoryMock.Setup(r => r.GetByCoachIdAsync(coachId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<CoachCertification>());

        var result = await _handler.Handle(new GetCoachCertificationsQuery { CoachId = coachId }, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEmpty();
    }

    [Fact]
    public async Task Handle_ExpiredCertification_SetsIsExpiredTrue()
    {
        var coachId = Guid.NewGuid();
        var cert = TestDataBuilder.CreateCoachCertification(coachId);
        cert.ExpiryDate = DateTime.UtcNow.AddDays(-10);

        _certificationRepositoryMock.Setup(r => r.GetByCoachIdAsync(coachId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<CoachCertification> { cert });

        var result = await _handler.Handle(new GetCoachCertificationsQuery { CoachId = coachId }, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value![0].IsExpired.Should().BeTrue();
    }
}
