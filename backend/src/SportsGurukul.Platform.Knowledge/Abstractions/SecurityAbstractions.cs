using SportsGurukul.Platform.Knowledge.Models;

namespace SportsGurukul.Platform.Knowledge.Abstractions;

public interface IAccessPolicyEvaluator
{
    AccessDecision Evaluate(KnowledgePrincipal principal, AccessPolicy policy, AccessPermission required);
}

public interface ITenantIsolationService
{
    VectorFilter ScopeFilter(VectorFilter filter, KnowledgePrincipal principal);
}

public interface IEncryptionService
{
    string Encrypt(string plaintext);
    string Decrypt(string ciphertext);
    byte[] EncryptBytes(byte[] plaintext);
    byte[] DecryptBytes(byte[] ciphertext);
}

public interface IKnowledgeAuditLogger
{
    Task LogAsync(KnowledgeAuditEvent auditEvent, CancellationToken ct = default);
}
