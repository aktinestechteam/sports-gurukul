using FluentAssertions;
using MediatR;
using Moq;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Features.AthleteManagement.Commands.CreateSavedSearch;
using SportsGurukul.Application.Tests.Common;
using SportsGurukul.Domain.Entities;

namespace SportsGurukul.Application.Tests.Commands;

public class CreateSavedSearchCommandHandlerTests
{
    private readonly Mock<ISavedSearchRepository> _repositoryMock = TestMocks.CreateSavedSearchRepository();
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = TestMocks.CreateUnitOfWork();
    private readonly Mock<ILogger<CreateSavedSearchCommandHandler>> _loggerMock = TestMocks.CreateLogger<CreateSavedSearchCommandHandler>();
    private readonly CreateSavedSearchCommandHandler _handler;

    public CreateSavedSearchCommandHandlerTests()
    {
        _handler = new CreateSavedSearchCommandHandler(
            _repositoryMock.Object,
            _unitOfWorkMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_ValidCommand_CreatesSavedSearchAndReturnsSuccess()
    {
        var userId = Guid.NewGuid();
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var result = await _handler.Handle(new CreateSavedSearchCommand
        {
            UserId = userId,
            Name = "Cricket Players in Mumbai",
            FiltersJson = "{\"city\":\"Mumbai\",\"sport\":\"Cricket\"}"
        }, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value!.Name.Should().Be("Cricket Players in Mumbai");
        result.Value.FiltersJson.Should().Be("{\"city\":\"Mumbai\",\"sport\":\"Cricket\"}");
        result.Value.UsageCount.Should().Be(0);
        _repositoryMock.Verify(r => r.AddAsync(It.IsAny<SavedSearch>(), It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ValidCommand_SetsCorrectFields()
    {
        var userId = Guid.NewGuid();
        SavedSearch? captured = null;
        _repositoryMock.Setup(r => r.AddAsync(It.IsAny<SavedSearch>(), It.IsAny<CancellationToken>()))
            .Callback<SavedSearch, CancellationToken>((s, _) => captured = s)
            .Returns<SavedSearch, CancellationToken>((s, _) => Task.FromResult(s));
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        await _handler.Handle(new CreateSavedSearchCommand
        {
            UserId = userId,
            Name = "My Search",
            FiltersJson = "{}"
        }, CancellationToken.None);

        captured.Should().NotBeNull();
        captured!.UserId.Should().Be(userId);
        captured.Name.Should().Be("My Search");
        captured.UsageCount.Should().Be(0);
        captured.Id.Should().NotBe(Guid.Empty);
    }
}
