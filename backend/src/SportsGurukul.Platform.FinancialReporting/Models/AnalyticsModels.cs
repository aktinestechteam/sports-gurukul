namespace SportsGurukul.Platform.FinancialReporting.Models;

public class RevenueTrendsResult
{
    public List<TrendDataPoint> DailyTrend { get; set; } = new();
    public List<TrendDataPoint> WeeklyTrend { get; set; } = new();
    public List<TrendDataPoint> MonthlyTrend { get; set; } = new();
    public decimal GrowthRate { get; set; }
}

public class PaymentTrendsResult
{
    public List<TrendDataPoint> VolumeTrend { get; set; } = new();
    public List<TrendDataPoint> ValueTrend { get; set; } = new();
    public decimal SuccessRateTrend { get; set; }
}

public class RefundTrendsResult
{
    public List<TrendDataPoint> RefundRateTrend { get; set; } = new();
    public List<TrendDataPoint> RefundAmountTrend { get; set; } = new();
    public decimal AverageRefundTime { get; set; }
}

public class CollectionEfficiencyResult
{
    public decimal OverallEfficiency { get; set; }
    public List<TrendDataPoint> EfficiencyTrend { get; set; } = new();
    public Dictionary<string, decimal> EfficiencyByAcademy { get; set; } = new();
}

public class OutstandingAgingResult
{
    public Dictionary<string, decimal> AgingBuckets { get; set; } = new();
    public decimal TotalOutstanding { get; set; }
    public decimal WeightedAverageAge { get; set; }
}

public class PaymentMethodDistributionResult
{
    public Dictionary<string, int> TransactionCount { get; set; } = new();
    public Dictionary<string, decimal> VolumeByMethod { get; set; } = new();
    public Dictionary<string, decimal> SuccessRateByMethod { get; set; } = new();
}

public class GatewaySuccessRateResult
{
    public Dictionary<string, decimal> OverallSuccessRate { get; set; } = new();
    public Dictionary<string, List<TrendDataPoint>> SuccessRateTrend { get; set; } = new();
    public Dictionary<string, decimal> AverageResponseTime { get; set; } = new();
}

public class SettlementPerformanceResult
{
    public decimal AverageSettlementTime { get; set; }
    public Dictionary<string, decimal> SettlementTimeByGateway { get; set; } = new();
    public Dictionary<string, decimal> SettlementSuccessRate { get; set; } = new();
}

public class ScholarshipImpactResult
{
    public decimal TotalScholarshipAmount { get; set; }
    public int StudentsBenefited { get; set; }
    public decimal RetentionRate { get; set; }
    public Dictionary<string, decimal> ImpactByType { get; set; } = new();
}

public class CouponEffectivenessResult
{
    public decimal TotalDiscountGiven { get; set; }
    public decimal RevenueLift { get; set; }
    public decimal RedemptionRate { get; set; }
    public decimal AverageOrderValueImpact { get; set; }
    public List<CouponEffectivenessItem> TopCoupons { get; set; } = new();
}

public class CouponEffectivenessItem
{
    public string CouponCode { get; set; } = string.Empty;
    public int RedemptionCount { get; set; }
    public decimal TotalDiscount { get; set; }
    public decimal RevenueGenerated { get; set; }
    public decimal RoI { get; set; }
}

public class TrendDataPoint
{
    public DateTime Date { get; set; }
    public string Label { get; set; } = string.Empty;
    public decimal Value { get; set; }
    public decimal? SecondaryValue { get; set; }
}
