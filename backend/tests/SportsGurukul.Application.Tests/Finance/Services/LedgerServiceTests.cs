using FluentAssertions;
using Moq;
using SportsGurukul.Application.Common.Interfaces.Finance;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.FinanceManagement.Services;
using SportsGurukul.Domain.Entities.Finance;
using SportsGurukul.Domain.Enums.Finance;

namespace SportsGurukul.Application.Tests.Finance.Services;

public class LedgerServiceTests
{
    private readonly Mock<ILedgerRepository> _ledgerRepoMock;
    private readonly LedgerService _service;

    public LedgerServiceTests()
    {
        _ledgerRepoMock = new Mock<ILedgerRepository>();
        _service = new LedgerService(_ledgerRepoMock.Object);
    }

    #region CreateJournalEntryAsync

    [Fact]
    public async Task CreateJournalEntryAsync_LedgerExists_ReturnsEntry()
    {
        var ledger = new Ledger { Id = Guid.NewGuid(), Code = "CASH001", Name = "Cash" };
        _ledgerRepoMock.Setup(r => r.GetByCodeAsync("CASH001", It.IsAny<CancellationToken>()))
            .ReturnsAsync(ledger);

        var result = await _service.CreateJournalEntryAsync("CASH001", 1000m, 0m, "Receipt", "REF-001", CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.DebitAmount.Should().Be(1000m);
        result.Value.CreditAmount.Should().Be(0m);
        result.Value.Description.Should().Be("Receipt");
        result.Value.Reference.Should().Be("REF-001");
    }

    [Fact]
    public async Task CreateJournalEntryAsync_LedgerNotFound_ReturnsFailure()
    {
        _ledgerRepoMock.Setup(r => r.GetByCodeAsync("UNKNOWN", It.IsAny<CancellationToken>()))
            .ReturnsAsync((Ledger?)null);

        var result = await _service.CreateJournalEntryAsync("UNKNOWN", 100m, 0m, null, null, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Ledger not found for code: UNKNOWN");
    }

    [Fact]
    public async Task CreateJournalEntryAsync_CreditEntry_ReturnsCreditEntry()
    {
        var ledger = new Ledger { Id = Guid.NewGuid(), Code = "REV001" };
        _ledgerRepoMock.Setup(r => r.GetByCodeAsync("REV001", It.IsAny<CancellationToken>()))
            .ReturnsAsync(ledger);

        var result = await _service.CreateJournalEntryAsync("REV001", 0m, 500m, "Revenue", "INV-001", CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.CreditAmount.Should().Be(500m);
        result.Value.DebitAmount.Should().Be(0m);
    }

    #endregion

    #region PostJournalAsync

    [Fact]
    public async Task PostJournalAsync_AllLedgersExist_ReturnsTrue()
    {
        var journal = new Journal
        {
            Entries = new List<JournalEntry>
            {
                new() { AccountCode = "CASH001", DebitAmount = 1000m, CreditAmount = 0m },
                new() { AccountCode = "REV001", DebitAmount = 0m, CreditAmount = 1000m }
            }
        };

        _ledgerRepoMock.Setup(r => r.GetByCodeAsync("CASH001", It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Ledger { Code = "CASH001" });
        _ledgerRepoMock.Setup(r => r.GetByCodeAsync("REV001", It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Ledger { Code = "REV001" });

        var result = await _service.PostJournalAsync(journal, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeTrue();
    }

    [Fact]
    public async Task PostJournalAsync_LedgerNotFound_ReturnsFailure()
    {
        var journal = new Journal
        {
            Entries = new List<JournalEntry>
            {
                new() { AccountCode = "UNKNOWN", DebitAmount = 100m, CreditAmount = 0m }
            }
        };

        _ledgerRepoMock.Setup(r => r.GetByCodeAsync("UNKNOWN", It.IsAny<CancellationToken>()))
            .ReturnsAsync((Ledger?)null);

        var result = await _service.PostJournalAsync(journal, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Ledger not found for account code: UNKNOWN");
    }

    [Fact]
    public async Task PostJournalAsync_EmptyEntries_ReturnsTrue()
    {
        var journal = new Journal { Entries = new List<JournalEntry>() };

        var result = await _service.PostJournalAsync(journal, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeTrue();
    }

    #endregion

    #region PostLedgerEntryAsync

    [Fact]
    public async Task PostLedgerEntryAsync_LedgerExists_AddsEntry()
    {
        var ledgerId = Guid.NewGuid();
        var existingLedger = new Ledger { Id = ledgerId, Entries = new List<LedgerEntry>() };
        var entry = new LedgerEntry { DebitAmount = 500m, CreditAmount = 0m, Description = "Test" };

        _ledgerRepoMock.Setup(r => r.GetByIdWithEntriesAsync(ledgerId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingLedger);

        var result = await _service.PostLedgerEntryAsync(new Ledger { Id = ledgerId }, entry, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeTrue();
        existingLedger.Entries.Should().HaveCount(1);
        existingLedger.Entries.First().DebitAmount.Should().Be(500m);
        _ledgerRepoMock.Verify(r => r.Update(existingLedger), Times.Once);
    }

    [Fact]
    public async Task PostLedgerEntryAsync_LedgerNotFound_ReturnsFailure()
    {
        _ledgerRepoMock.Setup(r => r.GetByIdWithEntriesAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Ledger?)null);

        var result = await _service.PostLedgerEntryAsync(new Ledger { Id = Guid.NewGuid() }, new LedgerEntry(), CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Ledger not found");
    }

    #endregion

    #region GetOrCreateLedgerAsync

    [Fact]
    public async Task GetOrCreateLedgerAsync_ExistingLedger_ReturnsExisting()
    {
        var existing = new Ledger { Id = Guid.NewGuid(), Code = "CASH", Name = "Cash", Type = LedgerType.Asset };

        _ledgerRepoMock.Setup(r => r.GetByCodeAsync("CASH", It.IsAny<CancellationToken>()))
            .ReturnsAsync(existing);

        var result = await _service.GetOrCreateLedgerAsync("CASH", "Cash", LedgerType.Asset, "Cash & Bank", CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be(existing);
        _ledgerRepoMock.Verify(r => r.AddAsync(It.IsAny<Ledger>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task GetOrCreateLedgerAsync_NewLedger_CreatesAndReturns()
    {
        _ledgerRepoMock.Setup(r => r.GetByCodeAsync("NEWLEDGER", It.IsAny<CancellationToken>()))
            .ReturnsAsync((Ledger?)null);
        _ledgerRepoMock.Setup(r => r.AddAsync(It.IsAny<Ledger>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Ledger l, CancellationToken _) => l);

        var result = await _service.GetOrCreateLedgerAsync("NEWLEDGER", "New Ledger", LedgerType.Liability, "New liabilities", CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Code.Should().Be("NEWLEDGER");
        result.Value.Name.Should().Be("New Ledger");
        result.Value.Type.Should().Be(LedgerType.Liability);
        _ledgerRepoMock.Verify(r => r.AddAsync(It.IsAny<Ledger>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetOrCreateLedgerAsync_NewLedgerWithDescription_SetsDescription()
    {
        _ledgerRepoMock.Setup(r => r.GetByCodeAsync("DESC", It.IsAny<CancellationToken>()))
            .ReturnsAsync((Ledger?)null);

        Ledger? captured = null;
        _ledgerRepoMock.Setup(r => r.AddAsync(It.IsAny<Ledger>(), It.IsAny<CancellationToken>()))
            .Callback((Ledger l, CancellationToken _) => captured = l)
            .ReturnsAsync((Ledger l, CancellationToken _) => l);

        await _service.GetOrCreateLedgerAsync("DESC", "Desc Ledger", LedgerType.Expense, "Expense description", CancellationToken.None);

        captured.Should().NotBeNull();
        captured!.Description.Should().Be("Expense description");
    }

    #endregion
}
