namespace SportsGurukul.Application.Features.NotificationManagement.DTOs;

public record NotificationStatisticsDto(
    int Total,
    int Queued,
    int Sending,
    int Sent,
    int Delivered,
    int Failed,
    int Cancelled,
    int Expired,
    int Read,
    double AverageDeliveryTimeMs,
    double FailureRate
);

public record DailyStatisticsDto(
    DateTime Date,
    int TotalSent,
    int TotalDelivered,
    int TotalFailed,
    int TotalRead
);

public record ChannelStatisticsDto(
    string ChannelName,
    int Total,
    int Success,
    int Failed
);
