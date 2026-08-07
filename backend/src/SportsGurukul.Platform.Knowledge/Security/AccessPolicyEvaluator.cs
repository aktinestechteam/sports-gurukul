using SportsGurukul.Platform.Knowledge.Models;

using SportsGurukul.Platform.Knowledge.Abstractions;

namespace SportsGurukul.Platform.Knowledge.Security;

internal sealed class AccessPolicyEvaluator : IAccessPolicyEvaluator
{
    public AccessDecision Evaluate(KnowledgePrincipal principal, AccessPolicy policy, AccessPermission required)
    {
        if (policy.Scope == AccessScopeType.Public)
        {
            return new AccessDecision(true, ResolvePermission(required, AccessPermission.Read));
        }

        if (!principal.IsAuthenticated)
        {
            return new AccessDecision(false, AccessPermission.None, "Authentication required.");
        }

        switch (policy.Scope)
        {
            case AccessScopeType.Authenticated:
                return new AccessDecision(true, ResolvePermission(required, AccessPermission.Read));

            case AccessScopeType.RoleBased:
                if (policy.AllowedRoles is { Count: > 0 }
                    && !policy.AllowedRoles.Any(r => principal.Roles.Contains(r, StringComparer.OrdinalIgnoreCase)))
                {
                    return new AccessDecision(false, AccessPermission.None, "User role is not permitted.");
                }

                return new AccessDecision(true, ResolvePermission(required, AccessPermission.Read));

            case AccessScopeType.OwnerOnly:
                if (string.IsNullOrEmpty(policy.OwnerUserId)
                    || !string.Equals(policy.OwnerUserId, principal.UserId, StringComparison.Ordinal))
                {
                    return new AccessDecision(false, AccessPermission.None, "User is not the owner of this resource.");
                }

                return new AccessDecision(true, ResolvePermission(required, AccessPermission.Write));

            case AccessScopeType.Restricted:
                var allowedByRole = policy.AllowedRoles is { Count: > 0 }
                    && policy.AllowedRoles.Any(r => principal.Roles.Contains(r, StringComparer.OrdinalIgnoreCase));
                var allowedByUser = policy.AllowedUserIds is { Count: > 0 }
                    && policy.AllowedUserIds.Contains(principal.UserId);
                if (!allowedByRole && !allowedByUser)
                {
                    return new AccessDecision(false, AccessPermission.None, "Resource is restricted.");
                }

                return new AccessDecision(true, ResolvePermission(required, AccessPermission.Read));

            default:
                return new AccessDecision(false, AccessPermission.None, "Unknown access scope.");
        }
    }

    private static AccessPermission ResolvePermission(AccessPermission required, AccessPermission granted) =>
        required <= granted ? granted : AccessPermission.None;
}
