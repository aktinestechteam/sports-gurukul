using System.Linq.Expressions;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using SportsGurukul.Api;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Features.UserManagement.DTOs;
using SportsGurukul.Domain.Common;
using SportsGurukul.Domain.Entities;
using SportsGurukul.Domain.Entities.Finance;
using SportsGurukul.Domain.Entities.Notification;
using SportsGurukul.Domain.Enums;
using SportsGurukul.Infrastructure.Authentication;
using SportsGurukul.Infrastructure.Storage;

namespace SportsGurukul.IntegrationTests;

public class TestApplicationFactory : WebApplicationFactory<ApiMarker>
{
    private readonly List<User> _users = new();
    private readonly List<Role> _roles = new();
    private readonly List<EmailVerificationToken> _verificationTokens = new();
    private readonly List<PasswordResetToken> _resetTokens = new();
    private readonly List<RefreshToken> _refreshTokens = new();
    private readonly List<UserRole> _userRoles = new();

    public TestApplicationFactory()
    {
        _roles.Add(new Role
        {
            Id = Guid.NewGuid(),
            Name = "Athlete",
            RoleType = RoleType.Athlete,
            RolePermissions = new List<RolePermission>()
        });
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            var toRemove = services.Where(s =>
                s.ServiceType == typeof(IUserRepository) ||
                s.ServiceType == typeof(IEmailVerificationTokenRepository) ||
                s.ServiceType == typeof(IPasswordResetTokenRepository) ||
                s.ServiceType == typeof(IRefreshTokenRepository) ||
                s.ServiceType == typeof(IRoleRepository) ||
                s.ServiceType == typeof(IPermissionRepository) ||
                s.ServiceType == typeof(IUserRoleRepository) ||
                s.ServiceType == typeof(IUserProfileRepository) ||
                s.ServiceType == typeof(IUnitOfWork) ||
                s.ServiceType == typeof(IApplicationDbContext) ||
                s.ServiceType == typeof(IEmailService) ||
                s.ServiceType == typeof(IPasswordHasher) ||
                s.ServiceType == typeof(IJwtTokenService) ||
                s.ServiceType == typeof(ICurrentUser)).ToList();
            foreach (var s in toRemove) services.Remove(s);

            services.AddSingleton<InMemoryUnitOfWork>();
            services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<InMemoryUnitOfWork>());
            services.AddScoped<IApplicationDbContext>(sp => sp.GetRequiredService<InMemoryUnitOfWork>());
            services.AddSingleton<MockEmailService>();
            services.AddSingleton<IEmailService>(sp => sp.GetRequiredService<MockEmailService>());
            services.AddScoped<IPasswordHasher, MockPasswordHasher>();
            services.AddScoped<IJwtTokenService, MockJwtTokenService>();
            services.AddScoped<ICurrentUser>(_ => new MockCurrentUser(null));

            services.AddScoped<IUserRepository>(_ => new InMemoryUserRepository(_users));
            services.AddScoped<IEmailVerificationTokenRepository>(_ => new InMemoryVerificationTokenRepository(_verificationTokens));
            services.AddScoped<IPasswordResetTokenRepository>(_ => new InMemoryResetTokenRepository(_resetTokens));
            services.AddScoped<IRefreshTokenRepository>(_ => new InMemoryRefreshTokenRepository(_refreshTokens));
            services.AddScoped<IRoleRepository>(_ => new InMemoryRoleRepository(_roles));
            services.AddScoped<IPermissionRepository>(_ => new InMemoryPermissionRepository());
            services.AddScoped<IUserRoleRepository>(_ => new InMemoryUserRoleRepository(_userRoles));

            var userProfiles = new List<UserProfile>();
            var contactInfos = new List<ContactInformation>();
            var addresses = new List<Address>();
            var userPreferences = new List<UserPreference>();
            services.AddScoped<IUserProfileRepository>(_ => new InMemoryUserProfileRepository(userProfiles, contactInfos, addresses, userPreferences));
            services.AddScoped<IRepository<ContactInformation>>(_ => new InMemoryGenericRepository<ContactInformation>(contactInfos));
            services.AddScoped<IRepository<Address>>(_ => new InMemoryGenericRepository<Address>(addresses));
            services.AddScoped<IRepository<UserPreference>>(_ => new InMemoryGenericRepository<UserPreference>(userPreferences));

            var userFiles = new List<UserFile>();
            services.AddScoped<IFileRepository>(_ => new InMemoryFileRepository(userFiles));
            services.AddScoped<IFileStorageService, InMemoryFileStorageService>();

            services.Configure<StorageOptions>(opts =>
            {
                opts.Provider = StorageProvider.Local;
                opts.BasePath = "test-uploads";
            });
        });
    }

    public void SeedUser(User user) => _users.Add(user);
    public void SeedRole(Role role) => _roles.Add(role);
    public List<User> GetUsers() => _users;
    public List<Role> GetRoles() => _roles;
    public List<RefreshToken> GetRefreshTokens() => _refreshTokens;
    public List<UserRole> GetUserRoles() => _userRoles;
    public List<EmailVerificationToken> GetVerificationTokens() => _verificationTokens;
    public List<PasswordResetToken> GetResetTokens() => _resetTokens;

    public MockEmailService GetEmailService() =>
        Services.GetRequiredService<MockEmailService>();
}

public class InMemoryUnitOfWork : IUnitOfWork, IApplicationDbContext
{
    public DbSet<TrainingCertificate> Certificates { get; } = null!;
    public DbSet<Tournament> Tournaments { get; } = null!;
    public DbSet<TournamentMatch> TournamentMatches { get; } = null!;
    public DbSet<TournamentRegistration> TournamentRegistrations { get; } = null!;
    public DbSet<TournamentBracket> TournamentBrackets { get; } = null!;
    public DbSet<TournamentRanking> TournamentRankings { get; } = null!;
    public DbSet<TournamentParticipant> TournamentParticipants { get; } = null!;
    public DbSet<TournamentFixture> TournamentFixtures { get; } = null!;
    public DbSet<TournamentResult> TournamentResults { get; } = null!;
    public DbSet<TournamentAward> TournamentAwards { get; } = null!;
    public DbSet<TournamentOfficial> TournamentOfficials { get; } = null!;

    // Event Management
    public DbSet<Event> Events { get; } = null!;
    public DbSet<EventTypeEntity> EventTypes { get; } = null!;
    public DbSet<EventCategory> EventCategories { get; } = null!;
    public DbSet<EventSchedule> EventSchedules { get; } = null!;
    public DbSet<EventVenue> EventVenues { get; } = null!;
    public DbSet<EventRegistration> EventRegistrations { get; } = null!;
    public DbSet<EventParticipant> EventParticipants { get; } = null!;
    public DbSet<EventSpeaker> EventSpeakers { get; } = null!;
    public DbSet<EventCoach> EventCoaches { get; } = null!;
    public DbSet<EventVolunteer> EventVolunteers { get; } = null!;
    public DbSet<EventSponsor> EventSponsors { get; } = null!;
    public DbSet<EventSession> EventSessions { get; } = null!;
    public DbSet<EventAgenda> EventAgendas { get; } = null!;
    public DbSet<EventTicket> EventTickets { get; } = null!;
    public DbSet<EventAttendance> EventAttendances { get; } = null!;
    public DbSet<EventCertificate> EventCertificates { get; } = null!;
    public DbSet<EventFeedback> EventFeedbacks { get; } = null!;
    public DbSet<EventMedia> EventMedia { get; } = null!;
    public DbSet<EventDocument> EventDocuments { get; } = null!;
    public DbSet<EventAnnouncement> EventAnnouncements { get; } = null!;

    // Event Search & Discovery
    public DbSet<EventSavedSearch> EventSavedSearches { get; } = null!;
    public DbSet<EventRecentSearch> EventRecentSearches { get; } = null!;

    // Finance Domain
    public DbSet<Invoice> Invoices { get; } = null!;
    public DbSet<InvoiceItem> InvoiceItems { get; } = null!;
    public DbSet<InvoiceTax> InvoiceTaxes { get; } = null!;
    public DbSet<InvoiceDiscount> InvoiceDiscounts { get; } = null!;
    public DbSet<InvoicePayment> InvoicePayments { get; } = null!;
    public DbSet<Payment> Payments { get; } = null!;
    public DbSet<PaymentMethod> PaymentMethods { get; } = null!;
    public DbSet<PaymentTransaction> PaymentTransactions { get; } = null!;
    public DbSet<Refund> Refunds { get; } = null!;
    public DbSet<RefundItem> RefundItems { get; } = null!;
    public DbSet<Wallet> Wallets { get; } = null!;
    public DbSet<WalletTransaction> WalletTransactions { get; } = null!;
    public DbSet<Ledger> Ledgers { get; } = null!;
    public DbSet<LedgerEntry> LedgerEntries { get; } = null!;
    public DbSet<Journal> Journals { get; } = null!;
    public DbSet<JournalEntry> JournalEntries { get; } = null!;
    public DbSet<FeeStructure> FeeStructures { get; } = null!;
    public DbSet<FeeCategory> FeeCategories { get; } = null!;
    public DbSet<Scholarship> Scholarships { get; } = null!;
    public DbSet<DiscountPolicy> DiscountPolicies { get; } = null!;
    public DbSet<Coupon> Coupons { get; } = null!;
    public DbSet<CouponUsage> CouponUsages { get; } = null!;
    public DbSet<TaxConfiguration> TaxConfigurations { get; } = null!;
    public DbSet<PaymentGateway> PaymentGateways { get; } = null!;
    public DbSet<GatewayTransaction> GatewayTransactions_ { get; } = null!;
    public DbSet<Settlement> Settlements { get; } = null!;
    public DbSet<SettlementBatch> SettlementBatches { get; } = null!;
    public DbSet<PaymentReminder> PaymentReminders { get; } = null!;
    public DbSet<Receipt> Receipts { get; } = null!;
    public DbSet<CreditNote> CreditNotes { get; } = null!;
    public DbSet<DebitNote> DebitNotes { get; } = null!;
    public DbSet<FinancialAudit> FinancialAudits { get; } = null!;

    // Notification Domain
    public DbSet<Domain.Entities.Notification.Notification> Notifications { get; } = null!;
    public DbSet<NotificationRecipient> NotificationRecipients { get; } = null!;
    public DbSet<NotificationTemplate> NotificationTemplates { get; } = null!;
    public DbSet<TemplateVersion> TemplateVersions { get; } = null!;
    public DbSet<TemplateVariable> TemplateVariables { get; } = null!;
    public DbSet<NotificationChannel> NotificationChannels { get; } = null!;
    public DbSet<NotificationProvider> NotificationProviders { get; } = null!;
    public DbSet<NotificationPreference> NotificationPreferences { get; } = null!;
    public DbSet<NotificationSubscription> NotificationSubscriptions { get; } = null!;
    public DbSet<NotificationSchedule> NotificationSchedules { get; } = null!;
    public DbSet<NotificationQueue> NotificationQueue { get; } = null!;
    public DbSet<NotificationAttachment> NotificationAttachments { get; } = null!;
    public DbSet<NotificationDelivery> NotificationDeliveries { get; } = null!;
    public DbSet<NotificationRetry> NotificationRetries { get; } = null!;
    public DbSet<NotificationBatch> NotificationBatches { get; } = null!;
    public DbSet<NotificationCampaign> NotificationCampaigns { get; } = null!;
    public DbSet<NotificationEvent> NotificationEvents { get; } = null!;
    public DbSet<NotificationAudit> NotificationAudits { get; } = null!;

    // AI & Intelligence Domain
    public DbSet<Domain.Entities.AI.AIProvider> AIProviders { get; } = null!;
    public DbSet<Domain.Entities.AI.AIModel> AIModels { get; } = null!;
    public DbSet<Domain.Entities.AI.AIAssistant> AIAssistants { get; } = null!;
    public DbSet<Domain.Entities.AI.Conversation> Conversations { get; } = null!;
    public DbSet<Domain.Entities.AI.ConversationMessage> ConversationMessages { get; } = null!;
    public DbSet<Domain.Entities.AI.ConversationMemory> ConversationMemories { get; } = null!;
    public DbSet<Domain.Entities.AI.PromptTemplate> PromptTemplates { get; } = null!;
    public DbSet<Domain.Entities.AI.PromptVersion> PromptVersions { get; } = null!;
    public DbSet<Domain.Entities.AI.KnowledgeBase> KnowledgeBases { get; } = null!;
    public DbSet<Domain.Entities.AI.KnowledgeSource> KnowledgeSources { get; } = null!;
    public DbSet<Domain.Entities.AI.KnowledgeDocument> KnowledgeDocuments { get; } = null!;
    public DbSet<Domain.Entities.AI.Embedding> Embeddings { get; } = null!;
    public DbSet<Domain.Entities.AI.EmbeddingChunk> EmbeddingChunks { get; } = null!;
    public DbSet<Domain.Entities.AI.VectorIndex> VectorIndices { get; } = null!;
    public DbSet<Domain.Entities.AI.SemanticSearchRequest> SemanticSearchRequests { get; } = null!;
    public DbSet<Domain.Entities.AI.SemanticSearchResult> SemanticSearchResults { get; } = null!;
    public DbSet<Domain.Entities.AI.ToolDefinition> ToolDefinitions { get; } = null!;
    public DbSet<Domain.Entities.AI.ToolExecution> ToolExecutions { get; } = null!;
    public DbSet<Domain.Entities.AI.WorkflowDefinition> WorkflowDefinitions { get; } = null!;
    public DbSet<Domain.Entities.AI.WorkflowExecution> WorkflowExecutions { get; } = null!;
    public DbSet<Domain.Entities.AI.AgentDefinition> AgentDefinitions { get; } = null!;
    public DbSet<Domain.Entities.AI.AgentExecution> AgentExecutions { get; } = null!;
    public DbSet<Domain.Entities.AI.AIAuditLog> AIAuditLogs { get; } = null!;
    public DbSet<Domain.Entities.AI.AITokenUsage> AITokenUsages { get; } = null!;
    public DbSet<Domain.Entities.AI.AIModelConfiguration> AIModelConfigurations { get; } = null!;
    public DbSet<Domain.Entities.AI.AIRoutingPolicy> AIRoutingPolicies { get; } = null!;

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default) => Task.FromResult(1);
    public void Dispose() { }
}

public class MockEmailService : IEmailService
{
    public List<(string To, string Subject, string HtmlBody)> SentEmails { get; } = new();

    public Task SendAsync(string to, string subject, string htmlBody, CancellationToken cancellationToken = default)
    {
        SentEmails.Add((to, subject, htmlBody));
        return Task.CompletedTask;
    }
}

public class MockPasswordHasher : IPasswordHasher
{
    public string HashPassword(string password)
    {
        using var sha256 = SHA256.Create();
        var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password + "test-salt"));
        return Convert.ToBase64String(bytes);
    }

    public bool VerifyPassword(string hashedPassword, string providedPassword)
    {
        return HashPassword(providedPassword) == hashedPassword;
    }
}

public class MockJwtTokenService : IJwtTokenService
{
    private const string TestSigningKey = "Test-Secret-Key-At-Least-32-Characters-Long!!";
    private const string TestIssuer = "SportsGurukul";
    private const string TestAudience = "SportsGurukul";

    public string GenerateAccessToken(User user, IReadOnlyList<string> roles, IReadOnlyList<string> permissions)
    {
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new(JwtRegisteredClaimNames.Email, user.Email),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new("full_name", user.FullName)
        };

        foreach (var role in roles)
            claims.Add(new Claim(ClaimTypes.Role, role));

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(TestSigningKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: TestIssuer,
            audience: TestAudience,
            claims: claims,
            expires: DateTime.UtcNow.AddHours(1),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public string GenerateRefreshToken()
    {
        var bytes = new byte[64];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(bytes);
        return Convert.ToBase64String(bytes);
    }
}

public class MockCurrentUser : ICurrentUser
{
    public Guid? UserId { get; }
    public string? Email { get; }
    public bool IsAuthenticated => UserId.HasValue;
    public IReadOnlyList<string> Roles => Array.Empty<string>();
    public IReadOnlyList<string> Permissions => Array.Empty<string>();

    public MockCurrentUser(Guid? userId)
    {
        UserId = userId;
    }
}

public class InMemoryUserRepository : IUserRepository
{
    private readonly List<User> _users;
    public InMemoryUserRepository(List<User> users) => _users = users;

    public Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => Task.FromResult(_users.FirstOrDefault(u => u.Id == id));

    public Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
        => Task.FromResult(_users.FirstOrDefault(u => u.Email == email));

    public Task<User?> GetByPhoneNumberAsync(string phoneNumber, CancellationToken cancellationToken = default)
        => Task.FromResult(_users.FirstOrDefault(u => u.PhoneNumber == phoneNumber));

    public Task<User?> GetByEmailOrPhoneAsync(string emailOrPhone, CancellationToken cancellationToken = default)
        => Task.FromResult(_users.FirstOrDefault(u => u.Email == emailOrPhone || u.PhoneNumber == emailOrPhone));

    public Task<IReadOnlyList<User>> GetByStatusAsync(UserStatus status, CancellationToken cancellationToken = default)
        => Task.FromResult<IReadOnlyList<User>>(_users.Where(u => u.Status == status).ToList());

    public void Update(User user) { }

    public Task<User> AddAsync(User entity, CancellationToken cancellationToken = default)
    {
        _users.Add(entity);
        return Task.FromResult(entity);
    }

    public Task<IReadOnlyList<User>> GetAllAsync(CancellationToken cancellationToken = default)
        => Task.FromResult<IReadOnlyList<User>>(_users);

    public Task<IReadOnlyList<User>> FindAsync(System.Linq.Expressions.Expression<Func<User, bool>> predicate, CancellationToken cancellationToken = default)
        => Task.FromResult<IReadOnlyList<User>>(_users.Where(predicate.Compile()).ToList());

    public Task<int> CountAsync(System.Linq.Expressions.Expression<Func<User, bool>>? predicate = null, CancellationToken cancellationToken = default)
        => Task.FromResult(predicate is null ? _users.Count : _users.Count(predicate.Compile()));

    public Task<bool> AnyAsync(System.Linq.Expressions.Expression<Func<User, bool>> predicate, CancellationToken cancellationToken = default)
        => Task.FromResult(_users.Any(predicate.Compile()));

    public void Remove(User entity) => _users.Remove(entity);
}

public class InMemoryVerificationTokenRepository : IEmailVerificationTokenRepository
{
    private readonly List<EmailVerificationToken> _tokens;
    public InMemoryVerificationTokenRepository(List<EmailVerificationToken> tokens) => _tokens = tokens;

    public Task<EmailVerificationToken?> GetByTokenAsync(string token, CancellationToken cancellationToken = default)
        => Task.FromResult(_tokens.FirstOrDefault(t => t.Token == token && !t.IsDeleted));

    public Task<EmailVerificationToken?> GetActiveTokenByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
        => Task.FromResult(_tokens.FirstOrDefault(t =>
            t.UserId == userId && !t.IsDeleted && t.UsedAt == null && t.ExpiresAt > DateTime.UtcNow));

    public Task<int> InvalidateAllUserTokensAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var active = _tokens.Where(t => t.UserId == userId && !t.IsDeleted && t.UsedAt == null).ToList();
        foreach (var token in active) token.UsedAt = DateTime.UtcNow;
        return Task.FromResult(active.Count);
    }

    public Task<EmailVerificationToken?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => Task.FromResult(_tokens.FirstOrDefault(t => t.Id == id));

    public Task<IReadOnlyList<EmailVerificationToken>> GetAllAsync(CancellationToken cancellationToken = default)
        => Task.FromResult<IReadOnlyList<EmailVerificationToken>>(_tokens);

    public Task<IReadOnlyList<EmailVerificationToken>> FindAsync(System.Linq.Expressions.Expression<Func<EmailVerificationToken, bool>> predicate, CancellationToken cancellationToken = default)
        => Task.FromResult<IReadOnlyList<EmailVerificationToken>>(_tokens.Where(predicate.Compile()).ToList());

    public Task<EmailVerificationToken> AddAsync(EmailVerificationToken entity, CancellationToken cancellationToken = default)
    {
        _tokens.Add(entity);
        return Task.FromResult(entity);
    }

    public void Update(EmailVerificationToken entity) { }
    public void Remove(EmailVerificationToken entity) => _tokens.Remove(entity);

    public Task<int> CountAsync(System.Linq.Expressions.Expression<Func<EmailVerificationToken, bool>>? predicate = null, CancellationToken cancellationToken = default)
        => Task.FromResult(predicate is null ? _tokens.Count : _tokens.Count(predicate.Compile()));

    public Task<bool> AnyAsync(System.Linq.Expressions.Expression<Func<EmailVerificationToken, bool>> predicate, CancellationToken cancellationToken = default)
        => Task.FromResult(_tokens.Any(predicate.Compile()));
}

public class InMemoryResetTokenRepository : IPasswordResetTokenRepository
{
    private readonly List<PasswordResetToken> _tokens;
    public InMemoryResetTokenRepository(List<PasswordResetToken> tokens) => _tokens = tokens;

    public Task<PasswordResetToken?> GetByTokenAsync(string token, CancellationToken cancellationToken = default)
        => Task.FromResult(_tokens.FirstOrDefault(t => t.Token == token && !t.IsDeleted));

    public Task<PasswordResetToken?> GetActiveTokenByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
        => Task.FromResult(_tokens.FirstOrDefault(t =>
            t.UserId == userId && !t.IsDeleted && t.UsedAt == null && t.ExpiresAt > DateTime.UtcNow));

    public Task<int> InvalidateAllUserTokensAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var active = _tokens.Where(t => t.UserId == userId && !t.IsDeleted && t.UsedAt == null).ToList();
        foreach (var token in active) token.UsedAt = DateTime.UtcNow;
        return Task.FromResult(active.Count);
    }

    public Task<PasswordResetToken?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => Task.FromResult(_tokens.FirstOrDefault(t => t.Id == id));

    public Task<IReadOnlyList<PasswordResetToken>> GetAllAsync(CancellationToken cancellationToken = default)
        => Task.FromResult<IReadOnlyList<PasswordResetToken>>(_tokens);

    public Task<IReadOnlyList<PasswordResetToken>> FindAsync(System.Linq.Expressions.Expression<Func<PasswordResetToken, bool>> predicate, CancellationToken cancellationToken = default)
        => Task.FromResult<IReadOnlyList<PasswordResetToken>>(_tokens.Where(predicate.Compile()).ToList());

    public Task<PasswordResetToken> AddAsync(PasswordResetToken entity, CancellationToken cancellationToken = default)
    {
        _tokens.Add(entity);
        return Task.FromResult(entity);
    }

    public void Update(PasswordResetToken entity) { }
    public void Remove(PasswordResetToken entity) => _tokens.Remove(entity);

    public Task<int> CountAsync(System.Linq.Expressions.Expression<Func<PasswordResetToken, bool>>? predicate = null, CancellationToken cancellationToken = default)
        => Task.FromResult(predicate is null ? _tokens.Count : _tokens.Count(predicate.Compile()));

    public Task<bool> AnyAsync(System.Linq.Expressions.Expression<Func<PasswordResetToken, bool>> predicate, CancellationToken cancellationToken = default)
        => Task.FromResult(_tokens.Any(predicate.Compile()));
}

public class InMemoryRefreshTokenRepository : IRefreshTokenRepository
{
    private readonly List<RefreshToken> _tokens;
    public InMemoryRefreshTokenRepository(List<RefreshToken> tokens) => _tokens = tokens;

    public Task<RefreshToken?> GetByTokenAsync(string token, CancellationToken cancellationToken = default)
        => Task.FromResult(_tokens.FirstOrDefault(t => t.Token == token && !t.IsDeleted));

    public Task<IReadOnlyList<RefreshToken>> GetActiveTokensByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
        => Task.FromResult<IReadOnlyList<RefreshToken>>(_tokens.Where(t =>
            t.UserId == userId && !t.IsDeleted && t.RevokedAt == null && t.ExpiresAt > DateTime.UtcNow).ToList());

    public Task<int> RevokeAllUserTokensAsync(Guid userId, string? replacedByToken, CancellationToken cancellationToken = default)
    {
        var active = _tokens.Where(t => t.UserId == userId && !t.IsDeleted && t.RevokedAt == null).ToList();
        foreach (var token in active)
        {
            token.RevokedAt = DateTime.UtcNow;
            token.ReplacedByToken = replacedByToken;
        }
        return Task.FromResult(active.Count);
    }

    public Task<RefreshToken?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => Task.FromResult(_tokens.FirstOrDefault(t => t.Id == id));

    public Task<IReadOnlyList<RefreshToken>> GetAllAsync(CancellationToken cancellationToken = default)
        => Task.FromResult<IReadOnlyList<RefreshToken>>(_tokens);

    public Task<IReadOnlyList<RefreshToken>> FindAsync(System.Linq.Expressions.Expression<Func<RefreshToken, bool>> predicate, CancellationToken cancellationToken = default)
        => Task.FromResult<IReadOnlyList<RefreshToken>>(_tokens.Where(predicate.Compile()).ToList());

    public Task<RefreshToken> AddAsync(RefreshToken entity, CancellationToken cancellationToken = default)
    {
        _tokens.Add(entity);
        return Task.FromResult(entity);
    }

    public void Update(RefreshToken entity) { }
    public void Remove(RefreshToken entity) => _tokens.Remove(entity);

    public Task<int> CountAsync(System.Linq.Expressions.Expression<Func<RefreshToken, bool>>? predicate = null, CancellationToken cancellationToken = default)
        => Task.FromResult(predicate is null ? _tokens.Count : _tokens.Count(predicate.Compile()));

    public Task<bool> AnyAsync(System.Linq.Expressions.Expression<Func<RefreshToken, bool>> predicate, CancellationToken cancellationToken = default)
        => Task.FromResult(_tokens.Any(predicate.Compile()));
}

public class InMemoryRoleRepository : IRoleRepository
{
    private readonly List<Role> _roles;
    public InMemoryRoleRepository(List<Role> roles) => _roles = roles;

    public Task<Role?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => Task.FromResult(_roles.FirstOrDefault(r => r.Id == id));

    public Task<Role?> GetByNameAsync(string name, CancellationToken cancellationToken = default)
        => Task.FromResult(_roles.FirstOrDefault(r => r.Name == name));

    public Task<Role?> GetByRoleTypeAsync(RoleType roleType, CancellationToken cancellationToken = default)
        => Task.FromResult(_roles.FirstOrDefault(r => r.RoleType == roleType));

    public Task<IReadOnlyList<Role>> GetAllAsync(CancellationToken cancellationToken = default)
        => Task.FromResult<IReadOnlyList<Role>>(_roles);

    public Task<IReadOnlyList<Role>> GetAllWithPermissionsAsync(CancellationToken cancellationToken = default)
        => Task.FromResult<IReadOnlyList<Role>>(_roles);

    public Task<IReadOnlyList<Role>> FindAsync(System.Linq.Expressions.Expression<Func<Role, bool>> predicate, CancellationToken cancellationToken = default)
        => Task.FromResult<IReadOnlyList<Role>>(_roles.Where(predicate.Compile()).ToList());

    public Task<Role> AddAsync(Role entity, CancellationToken cancellationToken = default)
    {
        _roles.Add(entity);
        return Task.FromResult(entity);
    }

    public void Update(Role entity) { }
    public void Remove(Role entity) { }

    public Task<int> CountAsync(System.Linq.Expressions.Expression<Func<Role, bool>>? predicate = null, CancellationToken cancellationToken = default)
        => Task.FromResult(predicate is null ? _roles.Count : _roles.Count(predicate.Compile()));

    public Task<bool> AnyAsync(System.Linq.Expressions.Expression<Func<Role, bool>> predicate, CancellationToken cancellationToken = default)
        => Task.FromResult(_roles.Any(predicate.Compile()));
}

public class InMemoryPermissionRepository : IPermissionRepository
{
    public Task<Permission?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) => Task.FromResult<Permission?>(null);
    public Task<Permission?> GetByNameAsync(string name, CancellationToken cancellationToken = default) => Task.FromResult<Permission?>(null);
    public Task<IReadOnlyList<Permission>> GetAllAsync(CancellationToken cancellationToken = default) => Task.FromResult<IReadOnlyList<Permission>>(new List<Permission>());
    public Task<IReadOnlyList<Permission>> GetByModuleAsync(string module, CancellationToken cancellationToken = default) => Task.FromResult<IReadOnlyList<Permission>>(new List<Permission>());
    public Task<IReadOnlyList<Permission>> FindAsync(System.Linq.Expressions.Expression<Func<Permission, bool>> predicate, CancellationToken cancellationToken = default) => Task.FromResult<IReadOnlyList<Permission>>(new List<Permission>());
    public Task<Permission> AddAsync(Permission entity, CancellationToken cancellationToken = default) => Task.FromResult(entity);
    public void Update(Permission entity) { }
    public void Remove(Permission entity) { }
    public Task<int> CountAsync(System.Linq.Expressions.Expression<Func<Permission, bool>>? predicate = null, CancellationToken cancellationToken = default) => Task.FromResult(0);
    public Task<bool> AnyAsync(System.Linq.Expressions.Expression<Func<Permission, bool>> predicate, CancellationToken cancellationToken = default) => Task.FromResult(false);
}

public class InMemoryUserRoleRepository : IUserRoleRepository
{
    private readonly List<UserRole> _userRoles;
    public InMemoryUserRoleRepository(List<UserRole> userRoles) => _userRoles = userRoles;

    public Task AddAsync(UserRole userRole, CancellationToken cancellationToken = default)
    {
        _userRoles.Add(userRole);
        return Task.CompletedTask;
    }

    public Task<IReadOnlyList<UserRole>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
        => Task.FromResult<IReadOnlyList<UserRole>>(_userRoles.Where(ur => ur.UserId == userId).ToList());

    public Task RemoveByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        _userRoles.RemoveAll(ur => ur.UserId == userId);
        return Task.CompletedTask;
    }
}

public class InMemoryGenericRepository<T> : IRepository<T> where T : BaseEntity
{
    private readonly List<T> _entities;
    public InMemoryGenericRepository(List<T> entities) => _entities = entities;

    public Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => Task.FromResult(_entities.FirstOrDefault(e => e.Id == id && !e.IsDeleted));

    public Task<IReadOnlyList<T>> GetAllAsync(CancellationToken cancellationToken = default)
        => Task.FromResult<IReadOnlyList<T>>(_entities.Where(e => !e.IsDeleted).ToList());

    public Task<IReadOnlyList<T>> FindAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
        => Task.FromResult<IReadOnlyList<T>>(_entities.Where(e => !e.IsDeleted).Where(predicate.Compile()).ToList());

    public Task<T> AddAsync(T entity, CancellationToken cancellationToken = default)
    {
        _entities.Add(entity);
        return Task.FromResult(entity);
    }

    public void Update(T entity) { }

    public void Remove(T entity) => entity.IsDeleted = true;

    public Task<int> CountAsync(Expression<Func<T, bool>>? predicate = null, CancellationToken cancellationToken = default)
    {
        var query = _entities.Where(e => !e.IsDeleted);
        if (predicate is not null) query = query.Where(predicate.Compile());
        return Task.FromResult(query.Count());
    }

    public Task<bool> AnyAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
        => Task.FromResult(_entities.Where(e => !e.IsDeleted).Any(predicate.Compile()));
}

public class InMemoryUserProfileRepository : IUserProfileRepository
{
    private readonly List<UserProfile> _profiles;
    private readonly List<ContactInformation> _contacts;
    private readonly List<Address> _addresses;
    private readonly List<UserPreference> _preferences;

    public InMemoryUserProfileRepository(
        List<UserProfile> profiles,
        List<ContactInformation> contacts,
        List<Address> addresses,
        List<UserPreference> preferences)
    {
        _profiles = profiles;
        _contacts = contacts;
        _addresses = addresses;
        _preferences = preferences;
    }

    public Task<UserProfile?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => Task.FromResult(_profiles.FirstOrDefault(p => p.Id == id && !p.IsDeleted));

    public Task<UserProfile?> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
        => Task.FromResult(_profiles.FirstOrDefault(p => p.UserId == userId && !p.IsDeleted));

    public Task<UserProfile?> GetWithAddressesAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var profile = _profiles.FirstOrDefault(p => p.UserId == userId && !p.IsDeleted);
        if (profile is not null)
            profile.Addresses = _addresses.Where(a => a.UserProfileId == profile.Id && !a.IsDeleted).ToList();
        return Task.FromResult(profile);
    }

    public Task<UserProfile?> GetWithContactInformationAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var profile = _profiles.FirstOrDefault(p => p.UserId == userId && !p.IsDeleted);
        if (profile is not null)
            profile.ContactInformation = _contacts.FirstOrDefault(c => c.UserProfileId == profile.Id && !c.IsDeleted);
        return Task.FromResult(profile);
    }

    public Task<UserProfile?> GetFullProfileAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var profile = _profiles.FirstOrDefault(p => p.UserId == userId && !p.IsDeleted);
        if (profile is not null)
        {
            profile.Addresses = _addresses.Where(a => a.UserProfileId == profile.Id && !a.IsDeleted).ToList();
            profile.ContactInformation = _contacts.FirstOrDefault(c => c.UserProfileId == profile.Id && !c.IsDeleted);
            profile.UserPreference = _preferences.FirstOrDefault(p => p.UserProfileId == profile.Id && !p.IsDeleted);
        }
        return Task.FromResult(profile);
    }

    public Task<(IReadOnlyList<UserListDto> Users, int TotalCount)> SearchProfilesAsync(UserSearchRequest request, CancellationToken cancellationToken = default)
    {
        var profiles = _profiles.Where(p => !p.IsDeleted).ToList();
        return Task.FromResult<(IReadOnlyList<UserListDto>, int)>((new List<UserListDto>(), profiles.Count));
    }

    public Task<UserProfile?> GetDeletedByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
        => Task.FromResult(_profiles.FirstOrDefault(p => p.UserId == userId && p.IsDeleted));

    public Task<IReadOnlyList<UserProfile>> GetAllAsync(CancellationToken cancellationToken = default)
        => Task.FromResult<IReadOnlyList<UserProfile>>(_profiles.Where(p => !p.IsDeleted).ToList());

    public Task<IReadOnlyList<UserProfile>> FindAsync(Expression<Func<UserProfile, bool>> predicate, CancellationToken cancellationToken = default)
        => Task.FromResult<IReadOnlyList<UserProfile>>(_profiles.Where(p => !p.IsDeleted).Where(predicate.Compile()).ToList());

    public Task<UserProfile> AddAsync(UserProfile entity, CancellationToken cancellationToken = default)
    {
        _profiles.Add(entity);
        return Task.FromResult(entity);
    }

    public void Update(UserProfile entity) { }

    public void Remove(UserProfile entity) => entity.IsDeleted = true;

    public Task<int> CountAsync(Expression<Func<UserProfile, bool>>? predicate = null, CancellationToken cancellationToken = default)
    {
        var query = _profiles.Where(p => !p.IsDeleted);
        if (predicate is not null) query = query.Where(predicate.Compile());
        return Task.FromResult(query.Count());
    }

    public Task<bool> AnyAsync(Expression<Func<UserProfile, bool>> predicate, CancellationToken cancellationToken = default)
        => Task.FromResult(_profiles.Where(p => !p.IsDeleted).Any(predicate.Compile()));
}

public class InMemoryFileRepository : IFileRepository
{
    private readonly List<UserFile> _files;
    public InMemoryFileRepository(List<UserFile> files) => _files = files;

    public Task<UserFile?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => Task.FromResult(_files.FirstOrDefault(f => f.Id == id && !f.IsDeleted));

    public Task<IReadOnlyList<UserFile>> GetAllAsync(CancellationToken cancellationToken = default)
        => Task.FromResult<IReadOnlyList<UserFile>>(_files.Where(f => !f.IsDeleted).ToList());

    public Task<IReadOnlyList<UserFile>> FindAsync(Expression<Func<UserFile, bool>> predicate, CancellationToken cancellationToken = default)
        => Task.FromResult<IReadOnlyList<UserFile>>(_files.Where(f => !f.IsDeleted).Where(predicate.Compile()).ToList());

    public Task<UserFile> AddAsync(UserFile entity, CancellationToken cancellationToken = default)
    {
        _files.Add(entity);
        return Task.FromResult(entity);
    }

    public void Update(UserFile entity) { }

    public void Remove(UserFile entity) => entity.IsDeleted = true;

    public Task<int> CountAsync(Expression<Func<UserFile, bool>>? predicate = null, CancellationToken cancellationToken = default)
    {
        var query = _files.Where(f => !f.IsDeleted);
        if (predicate is not null) query = query.Where(predicate.Compile());
        return Task.FromResult(query.Count());
    }

    public Task<bool> AnyAsync(Expression<Func<UserFile, bool>> predicate, CancellationToken cancellationToken = default)
        => Task.FromResult(_files.Where(f => !f.IsDeleted).Any(predicate.Compile()));

    public Task<UserFile?> GetByUserIdAndTypeAsync(Guid userId, FileType fileType, CancellationToken cancellationToken = default)
        => Task.FromResult(_files.FirstOrDefault(f => f.UserId == userId && f.FileType == fileType && !f.IsDeleted));

    public Task<IReadOnlyList<UserFile>> GetAllByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
        => Task.FromResult<IReadOnlyList<UserFile>>(_files.Where(f => f.UserId == userId && !f.IsDeleted).OrderByDescending(f => f.CreatedAt).ToList());

    public Task<UserFile?> GetActiveProfilePhotoAsync(Guid userId, CancellationToken cancellationToken = default)
        => Task.FromResult(_files.FirstOrDefault(f => f.UserId == userId && f.FileType == FileType.ProfilePhoto && !f.IsDeleted));
}

public class InMemoryFileStorageService : IFileStorageService
{
    private readonly Dictionary<string, byte[]> _storedFiles = new();

    public Task<FileStorageResult> UploadAsync(Stream fileStream, string fileName, string contentType, FileCategory category, CancellationToken cancellationToken = default)
    {
        var storedFileName = $"{Guid.NewGuid()}{Path.GetExtension(fileName)}";
        using var ms = new MemoryStream();
        fileStream.CopyTo(ms);
        var content = ms.ToArray();
        _storedFiles[storedFileName] = content;

        return Task.FromResult(new FileStorageResult
        {
            StoredFileName = storedFileName,
            StoragePath = $"/uploads/{category.ToString().ToLowerInvariant()}/{storedFileName}",
            PublicUrl = $"/uploads/{category.ToString().ToLowerInvariant()}/{storedFileName}",
            FileSize = content.Length
        });
    }

    public Task<Stream?> GetAsync(string storagePath, CancellationToken cancellationToken = default)
    {
        var fileName = Path.GetFileName(storagePath);
        if (!_storedFiles.TryGetValue(fileName, out var content))
            return Task.FromResult<Stream?>(null);

        return Task.FromResult<Stream?>(new MemoryStream(content));
    }

    public Task<bool> DeleteAsync(string storagePath, CancellationToken cancellationToken = default)
    {
        var fileName = Path.GetFileName(storagePath);
        return Task.FromResult(_storedFiles.Remove(fileName));
    }

    public string GetPublicUrl(string storagePath) => storagePath;
}
