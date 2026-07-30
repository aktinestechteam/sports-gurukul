using SportsGurukul.Platform.Communication.Analytics.DTOs;

namespace SportsGurukul.Platform.Communication.Analytics.Abstractions;

public interface IAudienceSegmentationService
{
    Task<SegmentResultDto> EvaluateSegmentAsync(Guid segmentId, CancellationToken ct = default);
    Task<SegmentResultDto> EvaluateSegmentDefinitionAsync(SegmentDefinitionDto definition, CancellationToken ct = default);
    Task<SegmentPreviewResult> PreviewAsync(SegmentPreviewRequest request, CancellationToken ct = default);
    Task<SegmentDefinitionDto> CreateAsync(SegmentRequest request, CancellationToken ct = default);
    Task<SegmentDefinitionDto> UpdateAsync(Guid id, SegmentRequest request, CancellationToken ct = default);
    Task<bool> DeleteAsync(Guid id, CancellationToken ct = default);
    Task<SegmentDefinitionDto> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<SegmentSearchResult> SearchAsync(SegmentSearchCriteria criteria, CancellationToken ct = default);
    Task<SegmentResultDto> ResolveSegmentAsync(SegmentType type, Dictionary<string, object>? parameters, CancellationToken ct = default);
    Task<SegmentResultDto> GetAthletesAsync(Dictionary<string, object>? filters, CancellationToken ct = default);
    Task<SegmentResultDto> GetCoachesAsync(Dictionary<string, object>? filters, CancellationToken ct = default);
    Task<SegmentResultDto> GetAcademiesAsync(Dictionary<string, object>? filters, CancellationToken ct = default);
    Task<SegmentResultDto> GetParentsAsync(Dictionary<string, object>? filters, CancellationToken ct = default);
    Task<SegmentResultDto> GetEventParticipantsAsync(Guid eventId, CancellationToken ct = default);
    Task<SegmentResultDto> GetTournamentParticipantsAsync(Guid tournamentId, CancellationToken ct = default);
    Task<SegmentResultDto> GetFinanceDueUsersAsync(decimal? minAmount, DateTime? dueBefore, CancellationToken ct = default);
    Task<SegmentResultDto> GetInactiveUsersAsync(TimeSpan inactivityPeriod, CancellationToken ct = default);
    Task<SegmentResultDto> GetPremiumUsersAsync(CancellationToken ct = default);
    Task<SegmentResultDto> GetNewUsersAsync(DateTime since, CancellationToken ct = default);
    Task<List<SavedSegmentDto>> GetSavedSegmentsAsync(CancellationToken ct = default);
    Task<SegmentDefinitionDto> SaveSegmentAsync(Guid segmentId, CancellationToken ct = default);
    Task<List<RuleDefinitionDto>> GetAvailableRulesAsync(CancellationToken ct = default);
}
