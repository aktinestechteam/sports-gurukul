---
title: Athlete Payments Module
module: Athlete
screen: Payments
platform: Flutter
backend: Finance Platform
version: 1.0
status: Draft
owner: Finance Product Team
---

# 💳 Athlete Payments Module

> The Payments Module enables athletes and parents to manage academy fees, tournament registrations, event payments, subscriptions, scholarships, discounts, receipts, refunds, and payment history from a unified financial dashboard.

---

# Table of Contents

1. Overview
2. Business Goals
3. User Journey
4. Payment Dashboard
5. Outstanding Fees
6. Invoice Details
7. Payment Methods
8. Payment Confirmation
9. Receipts
10. Refunds
11. Scholarships
12. Discounts
13. Installments
14. Payment History
15. AI Financial Assistant
16. API Integration
17. State Management
18. Offline Strategy
19. Security
20. Analytics
21. Acceptance Criteria

---

# 1. Overview

The Payments Module manages every financial interaction between the athlete and academy.

Supports

- Academy Fees
- Tournament Fees
- Event Fees
- Merchandise
- Camps
- Coaching Packages
- Wallet
- Refunds
- Scholarships
- Discounts

---

# 2. Business Goals

Increase

- Digital Payments
- On-time Fee Collection
- AutoPay Adoption
- Payment Transparency

Reduce

- Manual Collections
- Payment Queries
- Missed Due Dates

---

# 3. User Journey

```text
Dashboard

↓

Payments

↓

Outstanding Fees

↓

Invoice Details

↓

Select Payment Method

↓

Payment Gateway

↓

Success

↓

Receipt

↓

Dashboard
```

---

# 4. Payment Dashboard

Displays

- Outstanding Amount
- Upcoming Due Date
- Paid This Year
- Wallet Balance
- Active Scholarships
- Available Discounts
- Recent Payments

API

```
GET /api/v1/finance/dashboard
```

---

# Dashboard Layout

```
Outstanding Amount

↓

Upcoming Due

↓

Quick Pay

↓

Recent Transactions

↓

Scholarships

↓

Discounts

↓

Payment History

↓

AI Financial Insight
```

---

# 5. Outstanding Fees

Displays

- Invoice Number
- Category
- Amount
- Due Date
- Status

Categories

- Academy Fees
- Tournament Fees
- Camp Fees
- Uniform
- Equipment

API

```
GET /api/v1/finance/invoices
```

---

# 6. Invoice Details

Displays

- Invoice Number
- Academy
- Description
- Items
- Taxes
- Discounts
- Scholarship
- Final Amount
- Due Date

Actions

- Download PDF
- Share
- Pay Now

API

```
GET /api/v1/finance/invoices/{id}
```

---

# 7. Payment Methods

Supports

UPI

Credit Card

Debit Card

Net Banking

Wallet

AutoPay

Future

International Cards

Apple Pay

Google Pay

---

API

```
GET /api/v1/finance/payment-methods
```

---

# 8. Payment Gateway

Workflow

```text
Invoice

↓

Choose Method

↓

Gateway

↓

OTP

↓

Payment

↓

Confirmation

↓

Receipt
```

---

API

```
POST /api/v1/finance/payments
```

---

# 9. Payment Success

Displays

```
✓ Payment Successful

Transaction ID

Amount Paid

Date

Receipt

Download

Share

Done
```

---

# 10. Receipts

Supports

PDF

Email

Share

Download

Print

API

```
GET /api/v1/finance/receipts
```

---

# 11. Refunds

Displays

- Refund Status
- Amount
- Reason
- Expected Date

API

```
GET /api/v1/finance/refunds
```

---

# 12. Scholarships

Displays

- Scholarship Name
- Amount
- Percentage
- Validity
- Eligibility

API

```
GET /api/v1/finance/scholarships
```

---

# 13. Discounts

Displays

- Early Bird
- Sibling Discount
- Seasonal Offers
- Coupon Codes

API

```
GET /api/v1/finance/discounts
```

---

# 14. Installment Plans

Displays

- Installment Schedule
- Paid
- Remaining
- Due Dates

Supports

Auto Debit

Manual Payment

API

```
GET /api/v1/finance/installments
```

---

# 15. Payment History

Displays

- Transaction ID
- Amount
- Date
- Status
- Payment Method

Supports

Search

Filter

Export

API

```
GET /api/v1/finance/history
```

---

# 16. AI Financial Assistant

Provides

- Upcoming Due Reminder
- Best Payment Option
- Scholarship Suggestions
- Installment Recommendation
- Payment Summary

Example

```
🤖 AI Insight

You have ₹2,500 due in 5 days.

Pay before Friday to avoid late fees.

You may be eligible for a sibling discount.
```

API

```
POST /api/v1/ai/finance
```

---

# Flutter Widget Tree

```text
Scaffold

CustomScrollView

SliverAppBar

OutstandingCard

InvoiceList

QuickPayCard

PaymentMethodSheet

TransactionHistory

ScholarshipCard

DiscountCard

AIInsightCard

BottomNavigationBar
```

---

# Riverpod Providers

```
FinanceProvider

InvoiceProvider

PaymentProvider

ReceiptProvider

ScholarshipProvider

DiscountProvider

RefundProvider

AIProvider
```

---

# API Summary

| API                          | Purpose                |
| ---------------------------- | ---------------------- |
| GET /finance/dashboard       | Finance Dashboard      |
| GET /finance/invoices        | Outstanding Fees       |
| GET /finance/invoices/{id}   | Invoice Details        |
| POST /finance/payments       | Make Payment           |
| GET /finance/payment-methods | Payment Methods        |
| GET /finance/receipts        | Receipts               |
| GET /finance/history         | Payment History        |
| GET /finance/refunds         | Refunds                |
| GET /finance/scholarships    | Scholarships           |
| GET /finance/discounts       | Discounts              |
| GET /finance/installments    | Installments           |
| POST /ai/finance             | AI Financial Assistant |

---

# Offline Behaviour

Available

- Previous Receipts
- Invoice Cache
- Payment History
- Scholarships

Unavailable

- New Payments
- Payment Gateway
- Refund Requests

---

# Notifications

Notify Athlete

- Payment Due
- Payment Successful
- Payment Failed
- Refund Processed
- Scholarship Approved
- New Discount
- Installment Due

---

# Security

- JWT Authentication
- PCI-DSS Compliant Payment Flow
- Tokenized Card Storage (Gateway Managed)
- Certificate Pinning
- Secure Storage
- Device Validation
- Fraud Detection Hooks
- Audit Logging

No card details are stored within Sports Gurukul.

---

# Analytics

Track

```
payments_opened

invoice_viewed

payment_started

payment_success

payment_failed

receipt_downloaded

refund_viewed

scholarship_opened

discount_applied

ai_finance_opened
```

---

# Performance Goals

Dashboard

<500 ms

Invoice Details

<300 ms

Payment Gateway Launch

<1 second

Receipt Download

<2 seconds

---

# Accessibility

Supports

- Screen Reader
- VoiceOver
- TalkBack
- Dynamic Font
- High Contrast
- Keyboard Navigation

---

# Acceptance Criteria

✓ Outstanding fees displayed

✓ Invoice details available

✓ Multiple payment methods supported

✓ Secure payment processing

✓ Receipts downloadable

✓ Refund tracking available

✓ Scholarships visible

✓ Installments supported

✓ AI financial guidance available

✓ Fully integrated with Finance Platform

---

# Related Backend Modules

Finance Platform

Identity Platform

Notification Platform

Document Platform

AI Platform

Analytics Platform

---

# Future Enhancements

- Subscription management
- Family payment dashboard
- Multi-child consolidated billing
- EMI calculator
- International payment support
- Payment widgets for parent app
- Voice-assisted payments

---

# Next Documents

09-Wallet.md

10-Notifications.md

11-Profile.md

12-Settings.md

13-Documents.md

14-Medical.md

15-Achievements.md

---

**End of Document**
