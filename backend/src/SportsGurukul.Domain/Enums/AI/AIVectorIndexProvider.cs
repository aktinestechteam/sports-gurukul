namespace SportsGurukul.Domain.Enums.AI;

public enum AIVectorIndexProvider
{
    PgVector,
    Redis,
    Qdrant,
    Pinecone,
    Milvus,
    Weaviate
}
