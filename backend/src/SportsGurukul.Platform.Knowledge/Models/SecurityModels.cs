namespace SportsGurukul.Platform.Knowledge.Models;

public record KnowledgePrincipal(
    string UserId,
    string TenantId,
    IReadOnlyList<string> Roles,
    bool IsAuthenticated = true);

public record AccessPolicy(
    AccessScopeType Scope,
    IReadOnlyList<string>? AllowedRoles = null,
    IReadOnlyList<string>? AllowedUserIds = null,
    string? OwnerUserId = null,
    AccessPermission MinimumPermission = AccessPermission.Read);

public record KnowledgeAuditEvent(
    Guid Id,
    DateTime TimestampUtc,
    KnowledgeAuditAction Action,
    string ActorUserId,
    string TenantId,
    string IndexName,
    string? EntityId,
    string? EntityType,
    bool Succeeded,
    string? Reason,
    IReadOnlyDictionary<string, string>? Context = null);

public record AccessDecision(bool Allowed, AccessPermission Permission, string? Reason = null);
