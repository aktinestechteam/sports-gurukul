using System.Linq.Expressions;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Common.Interfaces.Finance;
using SportsGurukul.Domain.Common;
using SportsGurukul.Domain.Entities.Finance;

namespace SportsGurukul.Finance.Infrastructure.Tests.Builders;

public static class MockRepositoryBuilder
{
    public static Mock<IInvoiceRepository> CreateInvoiceRepository(
        IReadOnlyList<Invoice>? data = null)
    {
        var mock = new Mock<IInvoiceRepository>();
        SetupBaseCalls(mock, data ?? []);
        return mock;
    }

    public static Mock<IPaymentRepository> CreatePaymentRepository(
        IReadOnlyList<Payment>? data = null)
    {
        var mock = new Mock<IPaymentRepository>();
        SetupBaseCalls(mock, data ?? []);
        return mock;
    }

    public static Mock<IRefundRepository> CreateRefundRepository(
        IReadOnlyList<Refund>? data = null)
    {
        var mock = new Mock<IRefundRepository>();
        SetupBaseCalls(mock, data ?? []);
        return mock;
    }

    public static Mock<ICouponRepository> CreateCouponRepository(
        IReadOnlyList<Coupon>? data = null)
    {
        var mock = new Mock<ICouponRepository>();
        SetupBaseCalls(mock, data ?? []);
        return mock;
    }

    public static Mock<IWalletRepository> CreateWalletRepository(
        IReadOnlyList<Wallet>? data = null)
    {
        var mock = new Mock<IWalletRepository>();
        SetupBaseCalls(mock, data ?? []);
        return mock;
    }

    public static Mock<ILedgerRepository> CreateLedgerRepository(
        IReadOnlyList<Ledger>? data = null)
    {
        var mock = new Mock<ILedgerRepository>();
        SetupBaseCalls(mock, data ?? []);
        return mock;
    }

    public static Mock<ISettlementRepository> CreateSettlementRepository(
        IReadOnlyList<SettlementBatch>? data = null)
    {
        var mock = new Mock<ISettlementRepository>();
        SetupBaseCalls(mock, data ?? []);
        return mock;
    }

    private static void SetupBaseCalls<TEntity, TRepo>(Mock<TRepo> mock, IReadOnlyList<TEntity> data)
        where TRepo : class, IRepository<TEntity>
        where TEntity : BaseEntity
    {
        mock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Guid id, CancellationToken _) => data.FirstOrDefault(e => e.Id == id));

        mock.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(data);

        mock.Setup(r => r.FindAsync(It.IsAny<Expression<Func<TEntity, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Expression<Func<TEntity, bool>> predicate, CancellationToken _) =>
                data.AsQueryable().Where(predicate).ToList());

        mock.Setup(r => r.AddAsync(It.IsAny<TEntity>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((TEntity entity, CancellationToken _) => entity);

        mock.Setup(r => r.CountAsync(It.IsAny<Expression<Func<TEntity, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Expression<Func<TEntity, bool>>? predicate, CancellationToken _) =>
                predicate == null ? data.Count : data.AsQueryable().Count(predicate));

        mock.Setup(r => r.AnyAsync(It.IsAny<Expression<Func<TEntity, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Expression<Func<TEntity, bool>> predicate, CancellationToken _) =>
                data.AsQueryable().Any(predicate));
    }
}
