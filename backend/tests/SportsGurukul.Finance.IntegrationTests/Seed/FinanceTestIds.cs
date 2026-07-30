namespace SportsGurukul.Finance.IntegrationTests.Seed;

public static class FinanceTestIds
{
    public static readonly Guid AdminRoleId = Guid.Parse("10000000-0000-0000-0000-000000000001");
    public static readonly Guid AcademyRoleId = Guid.Parse("10000000-0000-0000-0000-000000000002");
    public static readonly Guid CoachRoleId = Guid.Parse("10000000-0000-0000-0000-000000000003");
    public static readonly Guid AthleteRoleId = Guid.Parse("10000000-0000-0000-0000-000000000004");

    public static readonly Guid AdminUserId = Guid.Parse("20000000-0000-0000-0000-000000000001");
    public static readonly Guid AcademyUserId = Guid.Parse("20000000-0000-0000-0000-000000000002");
    public static readonly Guid CoachUserId = Guid.Parse("20000000-0000-0000-0000-000000000003");
    public static readonly Guid AthleteUserId = Guid.Parse("20000000-0000-0000-0000-000000000004");

    public static readonly Guid TestInvoiceId = Guid.Parse("30000000-0000-0000-0000-000000000001");
    public static readonly Guid TestPaymentId = Guid.Parse("30000000-0000-0000-0000-000000000002");
    public static readonly Guid TestRefundId = Guid.Parse("30000000-0000-0000-0000-000000000003");
    public static readonly Guid TestWalletId = Guid.Parse("30000000-0000-0000-0000-000000000004");
    public static readonly Guid TestCouponId = Guid.Parse("30000000-0000-0000-0000-000000000005");
    public static readonly Guid TestSettlementId = Guid.Parse("30000000-0000-0000-0000-000000000006");
    public static readonly Guid TestLedgerId = Guid.Parse("30000000-0000-0000-0000-000000000007");
    public static readonly Guid TestScholarshipId = Guid.Parse("30000000-0000-0000-0000-000000000008");

    public const string AdminEmail = "finance-admin@sportsgurukul.com";
    public const string AcademyEmail = "finance-academy@sportsgurukul.com";
    public const string CoachEmail = "finance-coach@sportsgurukul.com";
    public const string AthleteEmail = "finance-athlete@sportsgurukul.com";

    public const string JwtSigningKey = "Finance-Integration-Test-Signing-Key-At-Least-32-Characters!!";
    public const string JwtIssuer = "SportsGurukul.Finance.Test";
    public const string JwtAudience = "SportsGurukul.Finance.Test";
}
