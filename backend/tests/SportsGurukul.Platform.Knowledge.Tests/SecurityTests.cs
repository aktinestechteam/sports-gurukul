using Microsoft.Extensions.Logging.Abstractions;
using SportsGurukul.Platform.Knowledge.Configuration;
using SportsGurukul.Platform.Knowledge.Models;
using SportsGurukul.Platform.Knowledge.Security;
using Xunit;

namespace SportsGurukul.Platform.Knowledge.Tests;

public class SecurityTests
{
    [Fact]
    public void AccessPolicyEvaluator_PublicPolicy_AllowsAnonymous()
    {
        var evaluator = new AccessPolicyEvaluator();
        var principal = new KnowledgePrincipal("", "", Array.Empty<string>(), IsAuthenticated: false);

        var decision = evaluator.Evaluate(principal, new AccessPolicy(AccessScopeType.Public), AccessPermission.Read);

        Assert.True(decision.Allowed);
    }

    [Fact]
    public void AccessPolicyEvaluator_RestrictedPolicy_DeniesWithoutRoleOrUser()
    {
        var evaluator = new AccessPolicyEvaluator();
        var principal = new KnowledgePrincipal("u1", "t1", new[] { "coach" });
        var policy = new AccessPolicy(
            AccessScopeType.Restricted,
            AllowedRoles: new[] { "admin" },
            AllowedUserIds: new[] { "someone-else" });

        var decision = evaluator.Evaluate(principal, policy, AccessPermission.Read);

        Assert.False(decision.Allowed);
        Assert.Equal(AccessPermission.None, decision.Permission);
    }

    [Fact]
    public void AccessPolicyEvaluator_RoleBasedPolicy_AllowsMatchingRole()
    {
        var evaluator = new AccessPolicyEvaluator();
        var principal = new KnowledgePrincipal("u1", "t1", new[] { "admin" });

        var decision = evaluator.Evaluate(
            principal,
            new AccessPolicy(AccessScopeType.RoleBased, AllowedRoles: new[] { "admin", "system" }),
            AccessPermission.Write);

        Assert.True(decision.Allowed);
    }

    [Fact]
    public void AccessPolicyEvaluator_OwnerOnly_AllowsOnlyOwner()
    {
        var evaluator = new AccessPolicyEvaluator();
        var owner = new KnowledgePrincipal("owner-1", "t1", Array.Empty<string>());
        var stranger = new KnowledgePrincipal("stranger", "t1", Array.Empty<string>());
        var policy = new AccessPolicy(AccessScopeType.OwnerOnly, OwnerUserId: "owner-1");

        Assert.True(evaluator.Evaluate(owner, policy, AccessPermission.Read).Allowed);
        Assert.False(evaluator.Evaluate(stranger, policy, AccessPermission.Read).Allowed);
    }

    [Fact]
    public void TenantIsolationService_ScopesFilter_ToPrincipalTenant()
    {
        var options = new KnowledgePlatformOptions { Security = new SecurityOptions { EnforceTenantIsolation = true } };
        var service = new TenantIsolationService(options);
        var principal = new KnowledgePrincipal("u1", "tenant-a", Array.Empty<string>());

        var scoped = service.ScopeFilter(new VectorFilter("sports"), principal);

        Assert.Equal("tenant-a", scoped.TenantId);
    }

    [Fact]
    public void TenantIsolationService_AdminBypasses_TenantScoping()
    {
        var options = new KnowledgePlatformOptions { Security = new SecurityOptions { EnforceTenantIsolation = true } };
        var service = new TenantIsolationService(options);
        var principal = new KnowledgePrincipal("admin-1", "tenant-a", new[] { "admin" });

        var scoped = service.ScopeFilter(new VectorFilter("sports", "tenant-b"), principal);

        Assert.Equal("tenant-b", scoped.TenantId);
    }

    [Fact]
    public void TenantIsolationService_Disabled_ReturnsFilterUnchanged()
    {
        var options = new KnowledgePlatformOptions { Security = new SecurityOptions { EnforceTenantIsolation = false } };
        var service = new TenantIsolationService(options);
        var principal = new KnowledgePrincipal("u1", "tenant-a", Array.Empty<string>());

        var scoped = service.ScopeFilter(new VectorFilter("sports", "tenant-b"), principal);

        Assert.Equal("tenant-b", scoped.TenantId);
    }

    [Fact]
    public void EncryptionService_RoundTrips_Plaintext()
    {
        var options = new KnowledgePlatformOptions
        {
            Security = new SecurityOptions { EncryptionKeyBase64 = EncryptionService.GenerateKey() }
        };
        var service = new EncryptionService(options);

        var ciphertext = service.Encrypt("sensitive athlete record");

        Assert.NotEqual("sensitive athlete record", ciphertext);
        Assert.Equal("sensitive athlete record", service.Decrypt(ciphertext));
    }

    [Fact]
    public void EncryptionService_TwoEncryptions_ProduceDifferentCiphertexts()
    {
        var options = new KnowledgePlatformOptions
        {
            Security = new SecurityOptions { EncryptionKeyBase64 = EncryptionService.GenerateKey() }
        };
        var service = new EncryptionService(options);

        var a = service.Encrypt("same value");
        var b = service.Encrypt("same value");

        Assert.NotEqual(a, b);
        Assert.Equal("same value", service.Decrypt(b));
    }

    [Fact]
    public void EncryptionService_MissingKey_Throws()
    {
        var options = new KnowledgePlatformOptions { Security = new SecurityOptions() };

        Assert.Throws<InvalidOperationException>(() => new EncryptionService(options));
    }

    [Fact]
    public async Task AuditLogger_Records_EventsToBuffer()
    {
        var options = new KnowledgePlatformOptions();
        var logger = new KnowledgeAuditLogger(options, NullLogger<KnowledgeAuditLogger>.Instance);

        await logger.LogAsync(new KnowledgeAuditEvent(
            Guid.NewGuid(),
            DateTime.UtcNow,
            KnowledgeAuditAction.Search,
            "u1",
            "t1",
            "sports",
            null,
            "knowledge",
            true,
            null));

        var snapshot = logger.Snapshot();

        Assert.Single(snapshot);
        Assert.Equal(KnowledgeAuditAction.Search, snapshot[0].Action);
    }
}
