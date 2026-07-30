using Microsoft.EntityFrameworkCore;
using SportsGurukul.Domain.Entities;
using SportsGurukul.Domain.Entities.Finance;
using SportsGurukul.Domain.Enums;
using SportsGurukul.Domain.Enums.Finance;
using SportsGurukul.Infrastructure.Persistence;

namespace SportsGurukul.Finance.IntegrationTests.Seed;

public class FinanceDataSeeder
{
    private readonly ApplicationDbContext _dbContext;

    public FinanceDataSeeder(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task SeedAsync()
    {
        if (await _dbContext.Roles.AnyAsync())
            return;

        var now = DateTime.UtcNow;

        var roles = new List<Role>
        {
            new() { Id = FinanceTestIds.AdminRoleId, Name = "Admin", RoleType = RoleType.Admin },
            new() { Id = FinanceTestIds.AcademyRoleId, Name = "Academy Admin", RoleType = RoleType.Academy },
            new() { Id = FinanceTestIds.CoachRoleId, Name = "Coach", RoleType = RoleType.Coach },
            new() { Id = FinanceTestIds.AthleteRoleId, Name = "Athlete", RoleType = RoleType.Athlete }
        };
        _dbContext.Roles.AddRange(roles);

        var users = new List<User>
        {
            new() { Id = FinanceTestIds.AdminUserId, FullName = "Finance Admin", Email = FinanceTestIds.AdminEmail, PhoneNumber = "+919100000001", PasswordHash = "hash", Status = UserStatus.Active, AuthMethod = AuthenticationMethod.EmailPassword, IsEmailVerified = true, CreatedAt = now },
            new() { Id = FinanceTestIds.AcademyUserId, FullName = "Academy Admin", Email = FinanceTestIds.AcademyEmail, PhoneNumber = "+919100000002", PasswordHash = "hash", Status = UserStatus.Active, AuthMethod = AuthenticationMethod.EmailPassword, IsEmailVerified = true, CreatedAt = now },
            new() { Id = FinanceTestIds.CoachUserId, FullName = "Test Coach", Email = FinanceTestIds.CoachEmail, PhoneNumber = "+919100000003", PasswordHash = "hash", Status = UserStatus.Active, AuthMethod = AuthenticationMethod.EmailPassword, IsEmailVerified = true, CreatedAt = now },
            new() { Id = FinanceTestIds.AthleteUserId, FullName = "Test Athlete", Email = FinanceTestIds.AthleteEmail, PhoneNumber = "+919100000004", PasswordHash = "hash", Status = UserStatus.Active, AuthMethod = AuthenticationMethod.EmailPassword, IsEmailVerified = true, CreatedAt = now }
        };
        _dbContext.Users.AddRange(users);

        _dbContext.UserRoles.AddRange(
            new UserRole { UserId = FinanceTestIds.AdminUserId, RoleId = FinanceTestIds.AdminRoleId, AssignedAt = now },
            new UserRole { UserId = FinanceTestIds.AcademyUserId, RoleId = FinanceTestIds.AcademyRoleId, AssignedAt = now },
            new UserRole { UserId = FinanceTestIds.CoachUserId, RoleId = FinanceTestIds.CoachRoleId, AssignedAt = now },
            new UserRole { UserId = FinanceTestIds.AthleteUserId, RoleId = FinanceTestIds.AthleteRoleId, AssignedAt = now }
        );

        var wallet = new Wallet
        {
            Id = FinanceTestIds.TestWalletId,
            UserId = FinanceTestIds.AthleteUserId,
            Balance = 10000m,
            Currency = "INR",
            IsActive = true,
            CreatedAt = now
        };
        _dbContext.Wallets.Add(wallet);

        var coupon = new Coupon
        {
            Id = FinanceTestIds.TestCouponId,
            Code = "WELCOME10",
            Type = DiscountType.Percentage,
            Value = 10m,
            MaxUsage = 100,
            CurrentUsage = 0,
            IsActive = true,
            ValidFrom = now.AddDays(-30),
            ValidTo = now.AddDays(30),
            MinOrderAmount = 500m,
            MaxDiscountAmount = 500m,
            CreatedAt = now
        };
        _dbContext.Coupons.Add(coupon);

        var ledger = new Ledger
        {
            Id = FinanceTestIds.TestLedgerId,
            Name = "Revenue from Services",
            Code = "REV-001",
            Type = LedgerType.Income,
            Description = "Primary revenue ledger",
            IsActive = true,
            CreatedAt = now
        };
        _dbContext.Ledgers.Add(ledger);

        var scholarship = new Scholarship
        {
            Id = FinanceTestIds.TestScholarshipId,
            Name = "Athlete Scholarship",
            DiscountPercentage = 25m,
            MaxAmount = 25000m,
            IsActive = true,
            ValidFrom = now.AddDays(-30),
            ValidTo = now.AddDays(335),
            CreatedAt = now
        };
        _dbContext.Scholarships.Add(scholarship);

        await _dbContext.SaveChangesAsync();
    }
}
