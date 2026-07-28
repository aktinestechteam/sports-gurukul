using Microsoft.Extensions.Logging;
using Moq;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Features.EventManagement.Services;

namespace SportsGurukul.Application.Tests.EventManagement.Mocks;

public static class EventMockFactory
{
    public static Mock<IEventRepository> CreateEventRepository() => new();
    public static Mock<IEventRegistrationRepository> CreateRegistrationRepository() => new();
    public static Mock<IEventAttendanceRepository> CreateAttendanceRepository() => new();
    public static Mock<IEventFeedbackRepository> CreateFeedbackRepository() => new();
    public static Mock<IUnitOfWork> CreateUnitOfWork() => new();
    public static Mock<ILogger<T>> CreateLogger<T>() where T : class => new();
    public static Mock<IEventLifecycleService> CreateLifecycleService() => new();
    public static Mock<IEventRegistrationService> CreateRegistrationService() => new();
    public static Mock<IEventAttendanceService> CreateAttendanceService() => new();
    public static Mock<IEventFeedbackService> CreateFeedbackService() => new();
    public static Mock<IEventCertificateService> CreateCertificateService() => new();
    public static Mock<IEventAnnouncementService> CreateAnnouncementService() => new();
}
