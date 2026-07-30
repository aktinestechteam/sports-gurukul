# Finance Domain

## Overview

The Finance Domain provides the core financial infrastructure for the Sports Gurukul platform. It manages invoicing, payments, refunds, wallets, ledgers, journals, fee structures, coupons, settlements, and financial audit trails.

This domain is designed as a reusable platform module consumed by all business modules: Academy, Athlete, Coach, Training Programs, Events, Tournament Registration, Membership, Merchandise, and future Subscription Services.

---

## Aggregate Boundaries

| Aggregate Root | Entities | Description |
|---|---|---|
| **Invoice** | Invoice, InvoiceItem, InvoiceTax, InvoiceDiscount, InvoicePayment, PaymentReminder, CreditNote, DebitNote | Central billing document with line items, taxes, discounts, payment allocations, reminders, and correction notes |
| **Payment** | Payment, PaymentTransaction, GatewayTransaction, Receipt | Payment processing with transaction log, gateway interactions, and receipt generation |
| **Refund** | Refund, RefundItem | Full/partial refund processing against payments |
| **Wallet** | Wallet, WalletTransaction | Digital wallet with balance tracking and transaction history per user |
| **Ledger** | Ledger, LedgerEntry | Chart of accounts with double-entry ledger entries |
| **Journal** | Journal, JournalEntry | Accounting journals with period-based entries |
| **Coupon** | Coupon, CouponUsage | Discount coupons with usage tracking per user |
| **Settlement** | SettlementBatch, Settlement | Batch settlement of payments to gateways |
| **FeeStructure** | FeeStructure, FeeCategory | Configurable fee structures categorized by type |
| **Standalone** | Scholarship, DiscountPolicy, TaxConfiguration, PaymentGateway, PaymentMethod, FinancialAudit | Master data and audit entities |

---

## Entity Relationships

```
Invoice
├── Athlete (FK: AthleteId, nullable)
├── Academy (FK: AcademyId, nullable)
├── Event (FK: EventId, nullable)
├── Tournament (FK: TournamentId, nullable)
├── TrainingProgram (FK: TrainingProgramId, nullable)
├── AcademyMembership (FK: MembershipId, nullable)
├── InvoiceItem (1:N, Cascade)
├── InvoiceTax (1:N, Cascade)
├── InvoiceDiscount (1:N, Cascade)
├── InvoicePayment (1:N, Cascade) → Payment
├── PaymentReminder (1:N, Cascade)
├── CreditNote (1:N, Cascade)
└── DebitNote (1:N, Cascade)

Payment
├── Invoice (FK: InvoiceId, nullable)
├── PaymentGateway (FK: GatewayId, nullable)
├── InvoicePayment (1:N, Cascade) → Invoice
├── PaymentTransaction (1:N, Cascade)
├── GatewayTransaction (1:N, Cascade)
├── Refund (1:N, Cascade)
├── Receipt (1:N, Cascade)
└── Settlement (1:N, Cascade)

Refund
├── Payment (FK: PaymentId, Cascade)
└── RefundItem (1:N, Cascade)

Wallet
├── User (FK: UserId, Unique)
└── WalletTransaction (1:N, Cascade)

Ledger
└── LedgerEntry (1:N, Cascade)

Journal
└── JournalEntry (1:N, Cascade)

Coupon
└── CouponUsage (1:N, Cascade) → User

SettlementBatch
└── Settlement (1:N, Cascade) → Payment

FeeStructure
├── Sport (FK: SportId, nullable)
├── Academy (FK: AcademyId, nullable)
└── FeeCategory (FK: FeeCategoryId, nullable)
```

---

## Enumerations

| Enum | Values |
|---|---|
| **InvoiceStatus** | Draft, Issued, PartiallyPaid, Paid, Cancelled, Overdue |
| **PaymentStatus** | Pending, Authorized, Captured, Failed, Cancelled, Refunded |
| **RefundStatus** | Requested, Approved, Rejected, Completed |
| **PaymentMethod** | Cash, Card, UPI, NetBanking, Wallet, Cheque, BankTransfer |
| **DiscountType** | Percentage, Flat |
| **LedgerType** | Asset, Liability, Income, Expense, Equity |
| **JournalStatus** | Draft, Posted, Cancelled |
| **TransactionType** | Debit, Credit, Refund, Fee, Adjustment |
| **FeeFrequency** | OneTime, Monthly, Quarterly, HalfYearly, Yearly |
| **SettlementStatus** | Pending, InProgress, Completed, Failed |
| **PaymentReminderType** | Email, SMS, PushNotification |
| **CreditNoteStatus** | Draft, Issued, Applied, Cancelled |
| **DebitNoteStatus** | Draft, Issued, Applied, Cancelled |

---

## Repositories

| Interface | Implementation | Aggregate |
|---|---|---|
| `IInvoiceRepository` | `InvoiceRepository` | Invoice |
| `IPaymentRepository` | `PaymentRepository` | Payment |
| `IRefundRepository` | `RefundRepository` | Refund |
| `IWalletRepository` | `WalletRepository` | Wallet |
| `ILedgerRepository` | `LedgerRepository` | Ledger |
| `ICouponRepository` | `CouponRepository` | Coupon |
| `ISettlementRepository` | `SettlementRepository` | SettlementBatch |

All repositories extend `IRepository<T>` / `Repository<T>` which provides standard CRUD operations with soft-delete filtering.

---

## Persistence Layer

- **DbContext**: `ApplicationDbContext` (finance DbSets added)
- **Configurations**: 24 `IEntityTypeConfiguration<T>` classes under `Configurations/Finance/`
- **Conventions**: All entities use `BaseEntity` with `CreatedAt`, `UpdatedAt`, `IsDeleted` audit fields
- **Soft Delete**: Global query filters on all entities (`HasQueryFilter(e => !e.IsDeleted)`)
- **Concurrency**: `RowVersion` byte[] with `IsRowVersion()` on Invoice and Payment
- **Indexes**: Foreign key indexes, unique indexes on business keys (InvoiceNumber, PaymentReference, etc.), composite indexes for common queries

---

## Seed Data

| Table | Records |
|---|---|
| **FeeCategories** | Registration Fee, Tuition Fee, Tournament Fee, Membership Fee, Facility Fee, Equipment Fee |
| **PaymentMethods** | Cash, Card, UPI, Net Banking, Wallet, Cheque, Bank Transfer |
| **TaxConfigurations** | GST 5%, GST 12%, GST 18%, GST 28% |

---

## Cross-Cutting Platform Services

The Finance Domain integrates with existing platform services:

| Platform Service | Integration Point |
|---|---|
| **Identity Platform** | `User` entity reference in Wallet, CouponUsage; `CreatedBy`/`UpdatedBy` audit fields |
| **Audit Platform** | `FinancialAudit` entity for domain-specific audit trail (entity type, action, changes, IP) |
| **Notification Platform** | `PaymentReminder` entity for sending payment reminders via Email/SMS/Push |
| **Document Platform** | Invoice PDF generation, Receipt generation (document storage integration via `ReferenceType`/`ReferenceId` on InvoiceItem) |
| **Reference Data Platform** | FeeCategory, TaxConfiguration, PaymentGateway, PaymentMethod as reference data consumed by business modules |

---

## Extension Points

1. **Payment Gateway Integration**: `PaymentGateway` entity stores gateway configuration (jsonb); `GatewayTransaction` logs raw request/response; extend by implementing gateway-specific handlers
2. **Invoice Numbering**: Custom numbering strategies can be implemented at the application layer using `IInvoiceRepository`
3. **Fee Computation**: `FeeStructure` with `FeeCategory` provides flexible fee configuration; extend with custom pricing rules
4. **Tax Engine**: `TaxConfiguration` supports multiple tax types; extend with region-specific tax calculation
5. **Discount Policies**: `DiscountPolicy` and `Coupon` provide percentage/flat discounts; extend with stackable/conditional rules
6. **Settlement Engine**: `SettlementBatch` + `Settlement` support batch payment settlement to gateways; extend with reconciliation workflows
7. **Wallet Top-Up**: `Wallet` + `WalletTransaction` support credit/debit operations; extend with auto-topup rules

---

## Migration

- **Migration**: `20260729041852_AddFinanceDomain`
- Creates 24 tables with full foreign key relationships, indexes, and seed data
- Applies to PostgreSQL via Npgsql
