using FluentAssertions;
using Moq;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Features.BookingSchedulingManagement.Commands.RemoveFromWaitlist;
using SportsGurukul.Application.Tests.Common;
using SportsGurukul.Domain.Entities;
using SportsGurukul.Domain.Enums;
using Microsoft.Extensions.Logging;

namespace SportsGurukul.Application.Tests.Commands;

public class RemoveFromWaitlistCommandHandlerTests
{
    private readonly Mock<IWaitlistRepository> _waitlistRepositoryMock = TestMocks.CreateWaitlistRepository();
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = TestMocks.CreateUnitOfWork();
    private readonly Mock<ILogger<RemoveFromWaitlistCommandHandler>> _loggerMock = TestMocks.CreateLogger<RemoveFromWaitlistCommandHandler>();
    private readonly RemoveFromWaitlistCommandHandler _handler;

    public RemoveFromWaitlistCommandHandlerTests()
    {
        _handler = new RemoveFromWaitlistCommandHandler(
            _waitlistRepositoryMock.Object,
            _unitOfWorkMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_EntryNotFound_ReturnsFailure()
    {
        _waitlistRepositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((BookingWaitlist?)null);

        var result = await _handler.Handle(new RemoveFromWaitlistCommand
        {
            WaitlistEntryId = Guid.NewGuid()
        }, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Waitlist entry not found.");
    }

    [Fact]
    public async Task Handle_ValidEntry_CancelsSuccessfully()
    {
        var entry = new BookingWaitlist
        {
            Id = Guid.NewGuid(),
            Status = WaitlistStatus.Active
        };
        _waitlistRepositoryMock.Setup(r => r.GetByIdAsync(entry.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(entry);
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var result = await _handler.Handle(new RemoveFromWaitlistCommand
        {
            WaitlistEntryId = entry.Id
        }, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        entry.Status.Should().Be(WaitlistStatus.Cancelled);
        _waitlistRepositoryMock.Verify(r => r.Update(entry), Times.Once);
    }
}
