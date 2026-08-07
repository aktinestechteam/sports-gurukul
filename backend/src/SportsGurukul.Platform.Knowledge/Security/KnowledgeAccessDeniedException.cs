namespace SportsGurukul.Platform.Knowledge.Security;

public sealed class KnowledgeAccessDeniedException : Exception
{
    public KnowledgeAccessDeniedException(string message, string indexName, string actorUserId)
        : base(message)
    {
        IndexName = indexName;
        ActorUserId = actorUserId;
    }

    public string IndexName { get; }
    public string ActorUserId { get; }
}
