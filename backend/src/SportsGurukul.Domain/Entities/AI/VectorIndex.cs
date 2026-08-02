using SportsGurukul.Domain.Common;
using SportsGurukul.Domain.Enums.AI;

namespace SportsGurukul.Domain.Entities.AI;

public class VectorIndex : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string IndexType { get; set; } = "hnsw";
    public int Dimensions { get; set; }
    public string DistanceMetric { get; set; } = "cosine";
    public VectorIndexStatus Status { get; set; } = VectorIndexStatus.Building;
    public int TotalVectors { get; set; } = 0;
    public string? IndexConfiguration { get; set; }
    public string? TableName { get; set; }
    public byte[] RowVersion { get; set; } = [];
}
