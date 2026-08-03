using Microsoft.EntityFrameworkCore;
using SportsGurukul.Domain.Entities;
using SportsGurukul.Domain.Entities.AI;
using SportsGurukul.Domain.Entities.Finance;
using SportsGurukul.Domain.Entities.Notification;

namespace SportsGurukul.Application.Common.Interfaces;

public interface IApplicationDbContext
{
    DbSet<TrainingCertificate> Certificates { get; }
    DbSet<Tournament> Tournaments { get; }
    DbSet<TournamentMatch> TournamentMatches { get; }
    DbSet<TournamentRegistration> TournamentRegistrations { get; }
    DbSet<TournamentBracket> TournamentBrackets { get; }
    DbSet<TournamentRanking> TournamentRankings { get; }
    DbSet<TournamentParticipant> TournamentParticipants { get; }
    DbSet<TournamentFixture> TournamentFixtures { get; }
    DbSet<TournamentResult> TournamentResults { get; }
    DbSet<TournamentAward> TournamentAwards { get; }
    DbSet<TournamentOfficial> TournamentOfficials { get; }

    // Event Management
    DbSet<Event> Events { get; }
    DbSet<EventTypeEntity> EventTypes { get; }
    DbSet<EventCategory> EventCategories { get; }
    DbSet<EventSchedule> EventSchedules { get; }
    DbSet<EventVenue> EventVenues { get; }
    DbSet<EventRegistration> EventRegistrations { get; }
    DbSet<EventParticipant> EventParticipants { get; }
    DbSet<EventSpeaker> EventSpeakers { get; }
    DbSet<EventCoach> EventCoaches { get; }
    DbSet<EventVolunteer> EventVolunteers { get; }
    DbSet<EventSponsor> EventSponsors { get; }
    DbSet<EventSession> EventSessions { get; }
    DbSet<EventAgenda> EventAgendas { get; }
    DbSet<EventTicket> EventTickets { get; }
    DbSet<EventAttendance> EventAttendances { get; }
    DbSet<EventCertificate> EventCertificates { get; }
    DbSet<EventFeedback> EventFeedbacks { get; }
    DbSet<EventMedia> EventMedia { get; }
    DbSet<EventDocument> EventDocuments { get; }
    DbSet<EventAnnouncement> EventAnnouncements { get; }

    // Event Search & Discovery
    DbSet<EventSavedSearch> EventSavedSearches { get; }
    DbSet<EventRecentSearch> EventRecentSearches { get; }

    // Finance Domain
    DbSet<Invoice> Invoices { get; }
    DbSet<InvoiceItem> InvoiceItems { get; }
    DbSet<InvoiceTax> InvoiceTaxes { get; }
    DbSet<InvoiceDiscount> InvoiceDiscounts { get; }
    DbSet<InvoicePayment> InvoicePayments { get; }
    DbSet<Payment> Payments { get; }
    DbSet<PaymentMethod> PaymentMethods { get; }
    DbSet<PaymentTransaction> PaymentTransactions { get; }
    DbSet<Refund> Refunds { get; }
    DbSet<RefundItem> RefundItems { get; }
    DbSet<Wallet> Wallets { get; }
    DbSet<WalletTransaction> WalletTransactions { get; }
    DbSet<Ledger> Ledgers { get; }
    DbSet<LedgerEntry> LedgerEntries { get; }
    DbSet<Journal> Journals { get; }
    DbSet<JournalEntry> JournalEntries { get; }
    DbSet<FeeStructure> FeeStructures { get; }
    DbSet<FeeCategory> FeeCategories { get; }
    DbSet<Scholarship> Scholarships { get; }
    DbSet<DiscountPolicy> DiscountPolicies { get; }
    DbSet<Coupon> Coupons { get; }
    DbSet<CouponUsage> CouponUsages { get; }
    DbSet<TaxConfiguration> TaxConfigurations { get; }
    DbSet<PaymentGateway> PaymentGateways { get; }
    DbSet<GatewayTransaction> GatewayTransactions_ { get; }
    DbSet<Settlement> Settlements { get; }
    DbSet<SettlementBatch> SettlementBatches { get; }
    DbSet<PaymentReminder> PaymentReminders { get; }
    DbSet<Receipt> Receipts { get; }
    DbSet<CreditNote> CreditNotes { get; }
    DbSet<DebitNote> DebitNotes { get; }
    DbSet<FinancialAudit> FinancialAudits { get; }

    // Notification Domain
    DbSet<Domain.Entities.Notification.Notification> Notifications { get; }
    DbSet<NotificationRecipient> NotificationRecipients { get; }
    DbSet<NotificationTemplate> NotificationTemplates { get; }
    DbSet<TemplateVersion> TemplateVersions { get; }
    DbSet<TemplateVariable> TemplateVariables { get; }
    DbSet<NotificationChannel> NotificationChannels { get; }
    DbSet<NotificationProvider> NotificationProviders { get; }
    DbSet<NotificationPreference> NotificationPreferences { get; }
    DbSet<NotificationSubscription> NotificationSubscriptions { get; }
    DbSet<NotificationSchedule> NotificationSchedules { get; }
    DbSet<NotificationQueue> NotificationQueue { get; }
    DbSet<NotificationAttachment> NotificationAttachments { get; }
    DbSet<NotificationDelivery> NotificationDeliveries { get; }
    DbSet<NotificationRetry> NotificationRetries { get; }
    DbSet<NotificationBatch> NotificationBatches { get; }
    DbSet<NotificationCampaign> NotificationCampaigns { get; }
    DbSet<NotificationEvent> NotificationEvents { get; }
    DbSet<NotificationAudit> NotificationAudits { get; }

    // AI & Intelligence Platform
    DbSet<AIProvider> AIProviders { get; }
    DbSet<AIModel> AIModels { get; }
    DbSet<AIAssistant> AIAssistants { get; }
    DbSet<PromptTemplate> PromptTemplates { get; }
    DbSet<PromptVersion> PromptVersions { get; }
    DbSet<Conversation> Conversations { get; }
    DbSet<ConversationMessage> ConversationMessages { get; }
    DbSet<ConversationMemory> ConversationMemories { get; }
    DbSet<KnowledgeBase> KnowledgeBases { get; }
    DbSet<KnowledgeSource> KnowledgeSources { get; }
    DbSet<KnowledgeDocument> KnowledgeDocuments { get; }
    DbSet<Embedding> Embeddings { get; }
    DbSet<EmbeddingChunk> EmbeddingChunks { get; }
    DbSet<VectorIndex> VectorIndexes { get; }
    DbSet<SemanticSearchRequest> SemanticSearchRequests { get; }
    DbSet<SemanticSearchResult> SemanticSearchResults { get; }
    DbSet<ToolDefinition> ToolDefinitions { get; }
    DbSet<ToolExecution> ToolExecutions { get; }
    DbSet<WorkflowDefinition> WorkflowDefinitions { get; }
    DbSet<WorkflowExecution> WorkflowExecutions { get; }
    DbSet<AgentDefinition> AgentDefinitions { get; }
    DbSet<AgentExecution> AgentExecutions { get; }
    DbSet<AIAuditLog> AIAuditLogs { get; }
    DbSet<AITokenUsage> AITokenUsages { get; }
    DbSet<AIModelConfiguration> AIModelConfigurations { get; }
    DbSet<AIRoutingPolicy> AIRoutingPolicies { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
