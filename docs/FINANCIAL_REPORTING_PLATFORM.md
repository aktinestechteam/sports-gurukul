# Financial Reporting & Reconciliation Platform

## Architecture

The Financial Reporting & Reconciliation Platform is a reusable library for financial data analysis, reporting, reconciliation, and export, following Clean Architecture, CQRS, and MediatR patterns.

```
┌──────────────────────────────────────────────────────────────────┐
│                       Consumers (API Layer)                       │
│         FinancialDashboardController | ReportController           │
├──────────────────────────────────────────────────────────────────┤
│                    Application Layer (CQRS)                       │
│  ┌──────────────────────────────────────────────────────────┐    │
│  │                    MediatR Queries                        │    │
│  │  FinancialDashboard | RevenueSummary | OutstandingSummary │    │
│  │  LedgerQuery | SettlementSummary | TaxSummary | Reconcile │    │
│  └──────────────────────────────────────────────────────────┘    │
├──────────────────────────────────────────────────────────────────┤
│                    FinancialReporting Platform                    │
│  ┌──────────────────────────────────────────────────────────┐    │
│  │                   Dashboard Module                        │    │
│  │  8 KPIs: Revenue, Payments, Refunds, Outstanding,        │    │
│  │  Settlements, Wallet, Scholarships, Coupons              │    │
│  ├──────────────────────────────────────────────────────────┤    │
│  │                   Reports Module                          │    │
│  │  19 Report Types: Revenue, Daily/Monthly/Yearly,         │    │
│  │  Outstanding, PaymentSuccess, FailedPayments, Refund,    │    │
│  │  Settlement, Ledger, Journal, Wallet, Coupons,           │    │
│  │  Scholarship, Tax, GST, Academy/Coach/Athlete            │    │
│  ├──────────────────────────────────────────────────────────┤    │
│  │                   Analytics Module                        │    │
│  │  10 Engines: RevenueTrends, PaymentTrends, RefundTrends, │    │
│  │  CollectionEfficiency, OutstandingAging, PaymentMethod,  │    │
│  │  GatewaySuccess, SettlementPerformance, Scholarship,     │    │
│  │  CouponEffectiveness                                     │    │
│  ├──────────────────────────────────────────────────────────┤    │
│  │                Reconciliation Module                      │    │
│  │  5 Types: Bank, Gateway, Invoice, Settlement, Ledger     │    │
│  ├──────────────────────────────────────────────────────────┤    │
│  │                   Export Module                           │    │
│  │  Excel (Stub), CSV (Stub), PDF (Stub), ReportGenerator   │    │
│  ├──────────────────────────────────────────────────────────┤    │
│  │                   Caching Module                          │    │
│  │  In-Memory (Redis abstraction), 5 Cache Regions,         │    │
│  │  Absolute + Sliding Expiration                           │    │
│  ├──────────────────────────────────────────────────────────┤    │
│  │                   Security Module                         │    │
│  │  RBAC (8 permissions, 4 roles), Audit Logging,           │    │
│  │  Sensitive Data Masking                                  │    │
│  └──────────────────────────────────────────────────────────┘    │
├──────────────────────────────────────────────────────────────────┤
│              Infrastructure Layer (Stub / Replaceable)            │
│  In-Memory Storage | Stub Excel/CSV/PDF | ConcurrentDictionary   │
└──────────────────────────────────────────────────────────────────┘
```

## Module Interfaces

### IDashboardService
8 Key Performance Indicators for the financial dashboard:

| Method | Returns |
|--------|---------|
| `GetDashboardAsync` | `FinancialDashboard` (all 8 KPIs) |
| `GetRevenueKpiAsync` | `RevenueKpi` |
| `GetPaymentKpiAsync` | `PaymentKpi` |
| `GetRefundKpiAsync` | `RefundKpi` |
| `GetOutstandingKpiAsync` | `OutstandingKpi` |
| `GetSettlementKpiAsync` | `SettlementKpi` |
| `GetWalletKpiAsync` | `WalletKpi` |
| `GetScholarshipKpiAsync` | `ScholarshipKpi` |
| `GetCouponKpiAsync` | `CouponKpi` |

### IReportService
19 Report Types:

| Method | Returns |
|--------|---------|
| `GenerateRevenueReportAsync` | `RevenueReport` |
| `GenerateDailyCollectionReportAsync` | `DailyCollectionReport` |
| `GenerateMonthlyCollectionReportAsync` | `MonthlyCollectionReport` |
| `GenerateYearlyRevenueReportAsync` | `YearlyRevenueReport` |
| `GenerateOutstandingInvoicesReportAsync` | `OutstandingInvoicesReport` |
| `GeneratePaymentSuccessReportAsync` | `PaymentSuccessReport` |
| `GenerateFailedPaymentsReportAsync` | `FailedPaymentsReport` |
| `GenerateRefundReportAsync` | `RefundReport` |
| `GenerateSettlementReportAsync` | `SettlementReport` |
| `GenerateLedgerReportAsync` | `LedgerReport` |
| `GenerateJournalReportAsync` | `JournalReport` |
| `GenerateWalletTransactionsReportAsync` | `WalletTransactionsReport` |
| `GenerateCouponUsageReportAsync` | `CouponUsageReport` |
| `GenerateScholarshipReportAsync` | `ScholarshipReport` |
| `GenerateTaxReportAsync` | `TaxReport` |
| `GenerateGstReportAsync` | `GstReport` |
| `GenerateAcademyRevenueReportAsync` | `AcademyRevenueReport` |
| `GenerateCoachRevenueReportAsync` | `CoachRevenueReport` |
| `GenerateAthletePaymentReportAsync` | `AthletePaymentReport` |

### IAnalyticsService
10 Analytics Engines:

| Method | Returns |
|--------|---------|
| `GetRevenueTrendsAsync` | `RevenueTrendsResult` |
| `GetPaymentTrendsAsync` | `PaymentTrendsResult` |
| `GetRefundTrendsAsync` | `RefundTrendsResult` |
| `GetCollectionEfficiencyAsync` | `CollectionEfficiencyResult` |
| `GetOutstandingAgingAsync` | `OutstandingAgingResult` |
| `GetPaymentMethodDistributionAsync` | `PaymentMethodDistributionResult` |
| `GetGatewaySuccessRateAsync` | `GatewaySuccessRateResult` |
| `GetSettlementPerformanceAsync` | `SettlementPerformanceResult` |
| `GetScholarshipImpactAsync` | `ScholarshipImpactResult` |
| `GetCouponEffectivenessAsync` | `CouponEffectivenessResult` |

### IReconciliationService
5 Reconciliation Types:

| Method | Returns |
|--------|---------|
| `ReconcileBankAsync` | `BankReconciliationResult` |
| `ReconcileGatewayAsync` | `GatewayReconciliationResult` |
| `ReconcileInvoicesAsync` | `InvoiceReconciliationResult` |
| `ReconcileSettlementsAsync` | `SettlementReport` |
| `ReconcileLedgerAsync` | `LedgerReconciliationResult` |
| `DetectDifferencesAsync` | `ExceptionReport` |
| `GenerateExceptionReportAsync` | `ExceptionReport` |

### IExportService

| Method | Description |
|--------|-------------|
| `ExportAsync<T>` | Export to any format by `ReportFormat` enum |
| `ExportToExcelAsync<T>` | Export to Excel (.xlsx) |
| `ExportToCsvAsync<T>` | Export to CSV (.csv) |
| `ExportToPdfAsync<T>` | Export to PDF (.pdf) |

### IFinancialReportGenerator
High-level report generation with export:

| Method | Description |
|--------|-------------|
| `GenerateReportAsync` | Generate report data from `ReportRequest` |
| `GenerateAndExportAsync` | Generate + export to specified format |

### IFinancialCacheService
Redis-abstraction caching:

| Method | Description |
|--------|-------------|
| `GetAsync<T>` | Get cached value |
| `SetAsync<T>` | Cache a value with absolute + sliding expiration |
| `RemoveAsync` | Evict a cache key |
| `ExistsAsync` | Check if key exists and not expired |
| `BuildKey` | Build namespaced cache key (`fin:{region}:{id}`) |

### IFinancialAuditService

| Method | Description |
|--------|-------------|
| `LogAsync` | Record audit log entry |
| `GetAuditLogsAsync` | Query audit logs by date range / user |
| `GetAuditLogsByResourceAsync` | Query audit logs by resource |
| `HasPermissionAsync` | RBAC permission check |
| `MaskSensitiveData` | Mask string (shows last N chars) |

## Dashboard KPIs

| KPI | Metrics |
|-----|---------|
| Revenue | Total, Monthly, Daily, Growth%, Projected, BySource |
| Payments | Total TX, Success/Failed, Rate%, ATV, Volume, ByGateway |
| Refunds | Count, Amount, Rate%, Pending, AvgTime |
| Outstanding | Count, Amount, Overdue, Aging (0-30/31-60/61-90/90+) |
| Settlements | Pending, Amount, Completed, AvgTime, ByGateway |
| Wallet | Balance, Active Wallets, Credits, Debits, Today TX |
| Scholarships | Count, Amount, Active, AvgValue, ByType |
| Coupons | Used, Discount, AvgDiscount, Active, MostUsed |

## MediatR Queries

| Query | Handler | Returns |
|-------|---------|---------|
| `FinancialDashboardQuery` | `FinancialDashboardQueryHandler` | `FinancialDashboard` |
| `RevenueSummaryQuery` | `RevenueSummaryQueryHandler` | `RevenueReport` |
| `OutstandingSummaryQuery` | `OutstandingSummaryQueryHandler` | `OutstandingInvoicesReport` |
| `SettlementSummaryQuery` | `SettlementSummaryQueryHandler` | `SettlementReport` |
| `TaxSummaryQuery` | `TaxSummaryQueryHandler` | `TaxReport` |
| `LedgerQuery` | `LedgerQueryHandler` | `LedgerReport` |
| `ReconciliationQuery` | `ReconciliationQueryHandler` | `ReconciliationResult` |

## Enums

| Enum | Values |
|------|--------|
| `FinancialEntityType` | Academy, Coach, Athlete, Parent, Sponsor, Tournament, Platform |
| `TransactionType` | Payment, Refund, Settlement, Fee, Commission, Discount, Scholarship, WalletCredit, WalletDebit |
| `InvoiceStatus` | Draft, Sent, Paid, Overdue, Cancelled, Refunded |
| `PaymentStatus` | Created, Authorized, Captured, Failed, Refunded, PartiallyRefunded, Disputed |
| `SettlementStatus` | Pending, Initiated, Completed, Failed, Disputed |
| `ReconciliationStatus` | Matched, Unmatched, Discrepancy, Pending, Exception |
| `ReportFormat` | Excel, Csv, Pdf |
| `ReportType` | Revenue, DailyCollection, MonthlyCollection, YearlyRevenue, OutstandingInvoices, PaymentSuccess, FailedPayments, Refund, Settlement, Ledger, Journal, WalletTransactions, CouponUsage, Scholarship, Tax, Gst, AcademyRevenue, CoachRevenue, AthletePayment |
| `ReconciliationType` | Bank, Gateway, Invoice, Settlement, Ledger |
| `CacheRegion` | Dashboard, RevenueSummary, MonthlyReports, TaxSummary, Analytics |

## Security (RBAC)

### Permissions

| Permission | Description |
|------------|-------------|
| `financial.dashboard.view` | View financial dashboard |
| `financial.reports.view` | View financial reports |
| `financial.reports.export` | Export financial reports |
| `financial.reconciliation.view` | View reconciliation results |
| `financial.reconciliation.run` | Run reconciliation |
| `financial.analytics.view` | View analytics |
| `financial.audit.view` | View audit logs |
| `financial.settings.manage` | Manage financial settings |

### Roles

| Role | Permissions |
|------|-------------|
| `FinanceTeam` | All permissions |
| `AcademyAdmin` | Dashboard, Reports, Reconciliation (view only) |
| `Management` | Dashboard, Reports, Analytics |
| `Auditor` | Reports (view), Reconciliation (view), Audit (view) |

## Configuration

### `FinancialReportingOptions`

| Option | Default | Description |
|--------|---------|-------------|
| `EnableCaching` | `true` | Enable in-memory caching |
| `DashboardCacheDurationMinutes` | `5` | Dashboard cache TTL |
| `ReportCacheDurationMinutes` | `10` | Report cache TTL |
| `EnableAuditLogging` | `true` | Enable audit logging |
| `EnableSensitiveDataMasking` | `true` | Enable PII masking |

### DI Registration

```csharp
// In Program.cs or Service Collection setup:
services.AddFinancialReportingPlatform(options =>
{
    options.EnableCaching = true;
    options.DashboardCacheDurationMinutes = 5;
    options.ReportCacheDurationMinutes = 10;
    options.EnableAuditLogging = true;
    options.EnableSensitiveDataMasking = true;
});
```

All services are registered as singletons:
- `IDashboardService` → `DashboardService`
- `IReportService` → `ReportService`
- `IAnalyticsService` → `AnalyticsService`
- `IReconciliationService` → `ReconciliationService`
- `IExportService` → `ExportService`
- `IExcelExportService` → `StubExcelExportService`
- `ICsvExportService` → `StubCsvExportService`
- `IPdfExportService` → `StubPdfExportService`
- `IFinancialCacheService` → `FinancialCacheService`
- `IFinancialAuditService` → `FinancialAuditService`
- `IFinancialReportGenerator` → `FinancialReportGenerator`

## Caching

In-memory `ConcurrentDictionary`-based cache with Redis-like abstraction:

```
Cache Key: fin:{CacheRegion}:{Identifier}
  Example: fin:dashboard:default
           fin:revenue:2026-01

CacheEntry:
  ├── Value (object)
  ├── CreatedAt (DateTime)
  ├── AbsoluteExpiration (TimeSpan) — default 5 min
  └── SlidingExpiration (TimeSpan) — default 2 min
```

Regions: `Dashboard`, `RevenueSummary`, `MonthlyReports`, `TaxSummary`, `Analytics`

## Reconciliation Types

| Type | Source vs Target |
|------|-----------------|
| Bank | Bank statement vs System transactions |
| Gateway | Gateway records vs System records |
| Invoice | Invoices issued vs Payments received |
| Settlement | Expected settlement vs Actual settlement |
| Ledger | System balance vs Ledger balance |

## Data Flow

```
Client                          Platform
  │                                │
  │  Request Dashboard / Report    │
  ├───────────────────────────────►│
  │                                │
  │        ┌──────────────────┐    │
  │        │ Caching Layer     │    │
  │        │ Check cache key   │    │
  │        │ ──────────────    │    │
  │        │ Hit → return      │    │
  │        │ Miss → continue   │    │
  │        └──────────────────┘    │
  │                                │
  │        ┌──────────────────┐    │
  │        │ Service Layer     │    │
  │        │ DashboardService  │    │
  │        │ ReportService     │    │
  │        │ AnalyticsService  │    │
  │        │ ReconcileService  │    │
  │        └──────────────────┘    │
  │                                │
  │        ┌──────────────────┐    │
  │        │ Export Layer      │    │
  │        │ ExportService     │    │
  │        │ → Excel/CSV/PDF  │    │
  │        └──────────────────┘    │
  │                                │
  │        ┌──────────────────┐    │
  │        │ Security Layer    │    │
  │        │ Audit Logging     │    │
  │        │ Data Masking      │    │
  │        │ Permission Check  │    │
  │        └──────────────────┘    │
  │                                │
  │◄───────────────────────────────│
  │  Dashboard / Report / Export   │
```

## Performance Targets

| Operation | Target |
|-----------|--------|
| Dashboard Load | <300ms |
| Report Generation | <1s |
| Analytics Query | <500ms |
| Reconciliation | <2s |
| Export (all formats) | <5s |
| Cache Hit | <10ms |

## Package Dependencies

| Package | Version |
|---------|---------|
| `Microsoft.Extensions.DependencyInjection.Abstractions` | 9.0.0 |
| `Microsoft.Extensions.Logging.Abstractions` | 9.0.0 |
| `Microsoft.Extensions.Options` | 9.0.0 |
| `Microsoft.Extensions.Configuration.Abstractions` | 9.0.0 |
| `MediatR` | 12.4.1 |

## Files Created

```
backend/src/SportsGurukul.Platform.FinancialReporting/
├── SportsGurukul.Platform.FinancialReporting.csproj
├── DependencyInjection.cs
├── Models/
│   ├── CommonModels.cs          (Enums: EntityType, Transaction, Status, Report, Format)
│   ├── DashboardModels.cs       (FinancialDashboard + 8 KPI classes)
│   ├── ReportModels.cs          (19 Report types + line items)
│   ├── AnalyticsModels.cs       (10 Analytics result classes + TrendDataPoint)
│   ├── ReconciliationModels.cs  (5 Reconciliation types + differences/exceptions)
│   ├── ExportModels.cs          (ExportRequest, ExportResult)
│   ├── CacheModels.cs           (CacheOptions, CacheRegion)
│   └── SecurityModels.cs        (AuditLogEntry, Permissions, Roles)
├── Interfaces/
│   ├── IDashboardService.cs
│   ├── IReportService.cs
│   ├── IAnalyticsService.cs
│   ├── IReconciliationService.cs
│   ├── IExportService.cs
│   ├── IFinancialCacheService.cs
│   ├── IFinancialAuditService.cs
│   └── IFinancialReportGenerator.cs
├── Queries/
│   ├── FinancialDashboardQuery.cs
│   ├── RevenueSummaryQuery.cs
│   ├── OutstandingSummaryQuery.cs
│   ├── SettlementSummaryQuery.cs
│   ├── TaxSummaryQuery.cs
│   ├── LedgerQuery.cs
│   └── ReconciliationQuery.cs
├── Dashboard/
│   └── DashboardService.cs
├── Reports/
│   └── ReportService.cs
├── Analytics/
│   └── AnalyticsService.cs
├── Reconciliation/
│   └── ReconciliationService.cs
├── Exports/
│   ├── ExportService.cs
│   └── FinancialReportGenerator.cs
├── Caching/
│   └── CacheService.cs
└── Security/
    └── FinancialAuditService.cs

backend/tests/SportsGurukul.Platform.FinancialReporting.Tests/
├── SportsGurukul.Platform.FinancialReporting.Tests.csproj
├── DashboardServiceTests.cs
├── ReportServiceTests.cs
├── AnalyticsServiceTests.cs
├── ReconciliationServiceTests.cs
├── ExportServiceTests.cs
├── CacheServiceTests.cs
├── FinancialAuditServiceTests.cs
├── FinancialReportGeneratorTests.cs
├── QueryHandlerTests.cs
└── PerformanceTests.cs
```
