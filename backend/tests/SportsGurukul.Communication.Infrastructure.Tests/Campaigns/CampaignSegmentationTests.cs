using Microsoft.Extensions.Logging;
using SportsGurukul.Platform.Communication.Analytics.Abstractions;
using SportsGurukul.Platform.Communication.Analytics.DTOs;
using SportsGurukul.Platform.Communication.Analytics.Services;

namespace SportsGurukul.Communication.Infrastructure.Tests.Campaigns;

public class CampaignSegmentationTests
{
    private readonly Mock<ILogger<AudienceSegmentationService>> _loggerMock = new();
    private readonly Mock<ICacheService> _cacheMock = new();

    private AudienceSegmentationService CreateService() => new(_loggerMock.Object, _cacheMock.Object);

    [Fact]
    public async Task ResolveSegmentAsync_Athletes_ReturnsAthleteUsers()
    {
        var service = CreateService();

        var result = await service.ResolveSegmentAsync(SegmentType.Athletes, null);

        result.SegmentName.Should().Be("Athletes");
        result.UserIds.Should().AllSatisfy(id => id.Should().StartWith("athlete-"));
        result.TotalCount.Should().Be(100);
    }

    [Fact]
    public async Task ResolveSegmentAsync_Coaches_ReturnsCoachUsers()
    {
        var service = CreateService();

        var result = await service.ResolveSegmentAsync(SegmentType.Coaches, null);

        result.SegmentName.Should().Be("Coaches");
        result.UserIds.Should().AllSatisfy(id => id.Should().StartWith("coach-"));
        result.TotalCount.Should().Be(20);
    }

    [Fact]
    public async Task ResolveSegmentAsync_Academies_ReturnsAcademyUsers()
    {
        var service = CreateService();

        var result = await service.ResolveSegmentAsync(SegmentType.Academies, null);

        result.SegmentName.Should().Be("Academies");
        result.UserIds.Should().AllSatisfy(id => id.Should().StartWith("academy-org-"));
        result.TotalCount.Should().Be(10);
    }

    [Fact]
    public async Task ResolveSegmentAsync_Parents_ReturnsParentUsers()
    {
        var service = CreateService();

        var result = await service.ResolveSegmentAsync(SegmentType.Parents, null);

        result.SegmentName.Should().Be("Parents");
        result.UserIds.Should().AllSatisfy(id => id.Should().StartWith("parent-"));
        result.TotalCount.Should().Be(50);
    }

    [Fact]
    public async Task ResolveSegmentAsync_EventParticipants_ReturnsEventUsers()
    {
        var service = CreateService();

        var result = await service.ResolveSegmentAsync(SegmentType.EventParticipants, null);

        result.SegmentName.Should().Be("EventParticipants");
        result.UserIds.Should().AllSatisfy(id => id.Should().StartWith("event-participant-"));
        result.TotalCount.Should().Be(30);
    }

    [Fact]
    public async Task ResolveSegmentAsync_TournamentParticipants_ReturnsTournamentUsers()
    {
        var service = CreateService();
        var tournamentId = Guid.NewGuid();

        var result = await service.GetTournamentParticipantsAsync(tournamentId);

        result.UserIds.Should().AllSatisfy(id => id.Should().Contain("trn-"));
        result.TotalCount.Should().Be(50);
    }

    [Fact]
    public async Task ResolveSegmentAsync_FinanceDueUsers_ReturnsFinanceUsers()
    {
        var service = CreateService();

        var result = await service.GetFinanceDueUsersAsync(1000, DateTime.UtcNow);

        result.UserIds.Should().AllSatisfy(id => id.Should().StartWith("finance-due-"));
        result.TotalCount.Should().Be(15);
    }

    [Fact]
    public async Task ResolveSegmentAsync_UnknownType_ReturnsEmpty()
    {
        var service = CreateService();

        var result = await service.ResolveSegmentAsync((SegmentType)999, null);

        result.UserIds.Should().BeEmpty();
        result.TotalCount.Should().Be(0);
    }

    [Fact]
    public async Task SaveSegment_PersistsAndReturnsUpdated()
    {
        _cacheMock
            .Setup(c => c.GetAsync<SegmentResultDto>(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((SegmentResultDto?)null);

        var service = CreateService();
        var created = await service.CreateAsync(new SegmentRequest("Test Seg", null,
            SegmentType.CustomDynamic, SegmentMatchType.All, new(), false));

        var saved = await service.SaveSegmentAsync(created.Id);

        saved.IsSaved.Should().BeTrue();
    }

    [Fact]
    public async Task PreviewSegment_WithFilters_EstimatesCount()
    {
        var service = CreateService();
        var filters = new List<SegmentFilterDto>
        {
            new("user.role", "equals", "athlete", null, null)
        };

        var result = await service.PreviewAsync(new SegmentPreviewRequest(filters, SegmentMatchType.All));

        result.EstimatedCount.Should().BeGreaterThan(0);
        result.SampleUserIds.Should().NotBeNull();
    }

    [Fact]
    public async Task PreviewSegment_NoFilters_ReturnsAllUsersSample()
    {
        var service = CreateService();

        var result = await service.PreviewAsync(new SegmentPreviewRequest(new(), SegmentMatchType.All));

        result.EstimatedCount.Should().BeGreaterThanOrEqualTo(10);
        result.ValidationWarnings.Should().Contain(w => w.Contains("No filters provided"));
    }

    [Fact]
    public async Task CreateSegment_WithRoleFilter_StoresDefinition()
    {
        var service = CreateService();
        var filters = new List<SegmentFilterDto>
        {
            new("user.role", "equals", "coach", null, null)
        };

        var result = await service.CreateAsync(new SegmentRequest("Coaches Only", "All coaches",
            SegmentType.CustomDynamic, SegmentMatchType.All, filters, false));

        result.Name.Should().Be("Coaches Only");
        result.Filters.Should().HaveCount(1);
        result.Type.Should().Be(SegmentType.CustomDynamic);
    }
}
