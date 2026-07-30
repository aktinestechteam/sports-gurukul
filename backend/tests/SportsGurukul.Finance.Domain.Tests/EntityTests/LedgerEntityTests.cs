using SportsGurukul.Domain.Entities.Finance;
using SportsGurukul.Domain.Enums.Finance;
using SportsGurukul.Finance.Domain.Tests.Builders;

namespace SportsGurukul.Finance.Domain.Tests.Entities;

public class LedgerEntityTests
{
    [Fact]
    public void CreateLedger_HasCorrectDefaults()
    {
        var l = FinanceEntityBuilder.CreateLedger();
        l.IsActive.Should().BeTrue();
        l.Type.Should().Be(LedgerType.Asset);
    }

    [Fact]
    public void LedgerEntry_WithDebit_RecordsCorrectAmount()
    {
        var l = FinanceEntityBuilder.CreateLedger();
        var e = FinanceEntityBuilder.CreateLedgerEntry(l.Id, debit: 1000, credit: 0);
        e.DebitAmount.Should().Be(1000);
        e.CreditAmount.Should().Be(0);
    }

    [Fact]
    public void LedgerEntry_WithCredit_RecordsCorrectAmount()
    {
        var l = FinanceEntityBuilder.CreateLedger();
        var e = FinanceEntityBuilder.CreateLedgerEntry(l.Id, debit: 0, credit: 500);
        e.DebitAmount.Should().Be(0);
        e.CreditAmount.Should().Be(500);
    }
}
