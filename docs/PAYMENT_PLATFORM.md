# Payment Gateway Platform

## Architecture

The Payment Gateway Platform is a reusable, provider-agnostic payment processing library following Clean Architecture and SOLID principles.

```
┌─────────────────────────────────────────────────────┐
│                   API Layer                          │
│           PaymentGatewayController                   │
├─────────────────────────────────────────────────────┤
│              Application Layer                       │
│   IPaymentService (existing Finance module)          │
├─────────────────────────────────────────────────────┤
│           PaymentGateway Platform                    │
│  ┌──────────────────────────────────────────┐       │
│  │            Factory Layer                  │       │
│  │    PaymentGatewayFactory                  │       │
│  │    IPaymentGatewayFactory                 │       │
│  ├──────────────────────────────────────────┤       │
│  │           Adapter Layer                   │       │
│  │  ┌──────┐ ┌──────┐ ┌──────┐ ┌──────┐   │       │
│  │  │Razor │ │Stripe│ │Cash  │ │PayU  │   │       │
│  │  │pay   │ │      │ │free  │ │      │   │       │
│  │  └──────┘ └──────┘ └──────┘ └──────┘   │       │
│  ├──────────────────────────────────────────┤       │
│  │           Engine Layer                    │       │
│  │  ┌─────────┐ ┌──────┐ ┌─────────┐       │       │
│  │  │ Billing │ │ Tax  │ │Discount │       │       │
│  │  │ Engine  │ │Engine│ │ Engine  │       │       │
│  │  └─────────┘ └──────┘ └─────────┘       │       │
│  ├──────────────────────────────────────────┤       │
│  │           Security Layer                  │       │
│  │  Webhook Sig | Idempotency | Replay      │       │
│  │  Protection  | Tokenization | Encrypt    │       │
│  └──────────────────────────────────────────┘       │
├─────────────────────────────────────────────────────┤
│               Infrastructure Layer                   │
│           HttpClient | Gateway Config                │
└─────────────────────────────────────────────────────┘
```

## Provider Interfaces

### IPaymentGateway
Main interface for all payment operations:

| Method | Description |
|--------|-------------|
| `CreateOrderAsync` | Create a new payment order |
| `AuthorizePaymentAsync` | Authorize a payment (2-step) |
| `CapturePaymentAsync` | Capture an authorized payment |
| `GetPaymentStatusAsync` | Check payment status |
| `RefundPaymentAsync` | Process a refund |
| `CancelPaymentAsync` | Cancel pending payment |
| `VoidPaymentAsync` | Void an authorization |
| `RetryPaymentAsync` | Retry a failed payment |
| `VerifyWebhookSignatureAsync` | Validate webhook signature |
| `SavePaymentMethodAsync` | Tokenize a payment method |
| `DeletePaymentMethodAsync` | Remove a tokenized method |

### IPaymentGatewayFactory
Provider resolution and registration:

| Method | Description |
|--------|-------------|
| `GetGateway(provider)` | Resolve gateway by provider name |
| `GetDefaultGateway()` | Get the default provider |
| `GetRegisteredProviders()` | List all registered providers |
| `IsProviderSupported(name)` | Check if provider exists |
| `RegisterProvider(name, gateway)` | Register a provider |

### IPaymentProvider
Low-level provider contract for each gateway.

### IPaymentWebhookHandler
Webhook event processing with event delegation via C# events.

### IPaymentSignatureValidator
HMAC-SHA256 / MD5 signature generation and validation.

### IPaymentTokenService
Payment method tokenization (create, read, delete, mask, encrypt).

### IPaymentReconciliationService
Payment reconciliation, discrepancy handling, settlement submission.

## Gateway Adapters

| Adapter | Provider | Auth | Signature |
|---------|----------|------|-----------|
| `RazorpayGateway` | Razorpay | Basic Auth (Key:Secret) | HMAC-SHA256 |
| `StripeGateway` | Stripe | Bearer Token | HMAC-SHA256 |
| `CashfreeGateway` | Cashfree | x-client-id + x-client-secret | HMAC-SHA256 |
| `PayUGateway` | PayU | Hash-based | SHA-512 |
| `PayPalGateway` | PayPal | OAuth2 Bearer | HMAC-SHA256 |

All adapters return `PaymentOrderResponse` / `PaymentStatusResponse` regardless of provider.

## Provider Flow

```
Client                          API                          Gateway
  │                              │                              │
  │  POST /api/v1/payments       │                              │
  │  (provider=Razorpay)         │                              │
  ├─────────────────────────────►│                              │
  │                              │  PaymentGatewayFactory       │
  │                              │  .GetGateway("Razorpay")     │
  │                              │  ─────────────────────────   │
  │                              │                              │
  │                              │  POST /orders (Razorpay API) │
  │                              ├─────────────────────────────►│
  │                              │                              │
  │                              │◄─────────────────────────────│
  │                              │  { id: "order_xxx",         │
  │                              │    status: "created",        │
  │                              │    amount: 100000 }          │
  │◄─────────────────────────────│                              │
  │  { gatewayOrderId,           │                              │
  │    paymentPageUrl,           │                              │
  │    status: "created" }       │                              │
```

## Webhook Flow

```
Gateway                          API                         Handler
  │                              │                              │
  │  POST /api/v1/payments/      │                              │
  │       webhook/{provider}     │                              │
  ├─────────────────────────────►│                              │
  │                              │  1. Replay Protection Check  │
  │                              │  2. Signature Validation     │
  │                              │  3. Idempotency Check        │
  │                              │  4. Event Type Routing       │
  │                              ├─────────────────────────────►│
  │                              │                              │
  │                              │◄─────────────────────────────│
  │                              │  WebhookResult               │
  │◄─────────────────────────────│                              │
  │  200 OK                      │                              │
```

### Webhook Event Mapping

| Gateway Event | Platform Event |
|---------------|----------------|
| `payment.success` / `payment_intent.succeeded` | `PaymentSuccess` |
| `payment.failed` / `payment_intent.payment_failed` | `PaymentFailed` |
| `payment.captured` / `capture.completed` | `PaymentCaptured` |
| `payment.authorized` | `PaymentAuthorized` |
| `refund.completed` / `refund.created` | `RefundCompleted` |
| `refund.failed` | `RefundFailed` |
| `dispute.created` | `DisputeCreated` |
| `dispute.resolved` | `DisputeResolved` |
| `chargeback` | `Chargeback` |

## Billing Flow

```
BillingService
  ├── GenerateInvoice → Line items + Tax → InvoiceResult
  ├── GenerateInvoiceWithTax → Line items + GST → InvoiceResult with CGST/SGST/IGST
  ├── CalculateLateFee → Overdue amount + days → LateFeeResult
  ├── GenerateInstallmentPlan → Amount + installments → Schedule[]
  ├── CalculatePenalty → Amount + days + rate → Penalty amount
  └── IsWithinGracePeriod → Due date + grace days → bool
```

## Security

| Feature | Implementation |
|---------|---------------|
| Webhook Signature Validation | HMAC-SHA256 (Razorpay, Stripe, Cashfree, PayPal), MD5 (PayU) |
| Timestamp Validation | Rejects webhooks older than 5 minutes |
| Replay Attack Protection | Nonce store deduplication, timestamp drift detection |
| Idempotency | In-memory store with configurable TTL (default 24h) |
| Sensitive Data Encryption | AES-256 encryption for stored tokens |
| Payment Tokenization | No raw card/PII storage, tokenized references only |
| No CVV Storage | CVV never stored - prohibited by design |

## Extension Points

| Interface | Purpose | Default |
|-----------|---------|---------|
| `IFraudDetectionService` | Fraud assessment hooks | Stub (no-op) |
| `IRiskAssessmentService` | Risk scoring hooks | Stub (no-op) |
| `ISubscriptionBillingService` | Recurring billing profiles | Stub (extend for implementation) |
| `IRecurringInvoiceService` | Recurring invoice generation | Stub (extend for implementation) |
| `IPaymentReconciliationService` | Payment reconciliation | Stub (extend for implementation) |
| `IDiscountHandler` | Custom discount logic | Coupon, Scholarship, Promotion stubs |

### Adding a New Gateway Provider

1. Create a class extending `GatewayAdapterBase`
2. Implement all abstract methods
3. Add registration in `DependencyInjection.cs`
4. Add config mapping in `PaymentGatewayOptions`

## Performance Targets

| Operation | Target |
|-----------|--------|
| Create Order | <250ms |
| Capture Payment | <200ms |
| Webhook Processing | <100ms |
| Refund | <300ms |
| Status Check | <150ms |

## Files Created

```
backend/src/SportsGurukul.Platform.PaymentGateway/
├── SportsGurukul.Platform.PaymentGateway.csproj
├── DependencyInjection.cs
├── Interfaces/
│   ├── IPaymentGateway.cs
│   ├── IPaymentGatewayFactory.cs
│   ├── IPaymentProvider.cs
│   ├── IPaymentWebhookHandler.cs
│   ├── IPaymentSignatureValidator.cs
│   ├── IPaymentTokenService.cs
│   └── IPaymentReconciliationService.cs
├── Models/
│   ├── PaymentOrderRequest.cs
│   ├── WebhookPayload.cs
│   └── BillingModels.cs
├── Factory/
│   └── PaymentGatewayFactory.cs
├── Adapters/
│   ├── GatewayAdapterBase.cs
│   ├── RazorpayGateway.cs
│   ├── StripeGateway.cs
│   ├── CashfreeGateway.cs
│   ├── PayUGateway.cs
│   └── PayPalGateway.cs
├── Security/
│   ├── WebhookSignatureValidator.cs
│   ├── IdempotencyService.cs
│   ├── ReplayProtectionService.cs
│   ├── PaymentTokenService.cs
│   ├── SensitiveDataEncryptor.cs
│   └── PaymentWebhookHandler.cs
├── Billing/
│   ├── IBillingService.cs
│   └── BillingService.cs
├── Subscription/
│   ├── ISubscriptionBillingService.cs
│   └── IRecurringInvoiceService.cs
├── Tax/
│   ├── ITaxEngine.cs
│   └── TaxEngine.cs
├── Discount/
│   ├── IDiscountEngine.cs
│   └── DiscountEngine.cs
├── Fraud/
│   ├── IFraudDetectionService.cs
│   └── IRiskAssessmentService.cs
└── Accounting/
    ├── IAccountingService.cs
    └── AccountingService.cs

backend/src/SportsGurukul.Api/Controllers/V1/
└── PaymentGatewayController.cs

backend/tests/SportsGurukul.Platform.PaymentGateway.Tests/
├── SportsGurukul.Platform.PaymentGateway.Tests.csproj
├── GatewayFactoryTests.cs
├── SignatureValidationTests.cs
├── IdempotencyTests.cs
├── ReplayProtectionTests.cs
├── WebhookHandlerTests.cs
├── GatewayAdapterTests.cs
├── BillingEngineTests.cs
├── TaxEngineTests.cs
└── DiscountEngineTests.cs
```
