---
title: Sports Gurukul Mobile API Integration Guide
version: 1.0
status: Draft
owner: Solution Architecture Team
backend: ASP.NET Core (.NET 9)
frontend: Flutter
state_management: Riverpod
http_client: Dio
authentication: JWT + Refresh Token
---

# 🔗 Sports Gurukul Mobile API Integration Guide

> Defines standards, architecture, authentication, networking, error handling, DTO mapping, offline synchronization, and API integration between the Flutter mobile applications and the Sports Gurukul backend.

---

# Table of Contents

1. Purpose
2. Backend Architecture
3. API Standards
4. Base URLs
5. Authentication
6. HTTP Client
7. API Repository Pattern
8. DTO Mapping
9. Feature Integration
10. Pagination
11. File Upload
12. Streaming APIs
13. WebSocket
14. Error Handling
15. Retry Strategy
16. Caching
17. Offline Sync
18. Logging
19. Security
20. Acceptance Criteria

---

# 1. Purpose

This document defines a single integration standard for all Flutter applications.

Every feature must communicate with the backend through the Repository layer.

UI must never call REST APIs directly.

---

# 2. Backend Architecture

```text
Flutter UI

↓

Riverpod Provider

↓

Application Service

↓

Repository

↓

Dio HTTP Client

↓

API Gateway

↓

ASP.NET Core

↓

CQRS

↓

Domain

↓

Database
```

---

# 3. API Standards

## Base Path

```
/api/v1/
```

Future versions

```
/api/v2/
```

---

## Content Type

```
application/json
```

---

## Encoding

```
UTF-8
```

---

## Date Format

```
ISO-8601 UTC
```

Example

```
2026-08-01T10:30:45Z
```

---

# 4. Environment Configuration

## Development

```
https://dev-api.sportsgurukul.com
```

## QA

```
https://qa-api.sportsgurukul.com
```

## Staging

```
https://staging-api.sportsgurukul.com
```

## Production

```
https://api.sportsgurukul.com
```

---

# 5. Authentication

Authentication uses JWT Access Token + Refresh Token.

### Login Flow

```text
Login

↓

POST /api/v1/auth/login

↓

Access Token

↓

Refresh Token

↓

Secure Storage

↓

Authenticated Requests
```

---

### HTTP Headers

```
Authorization: Bearer <AccessToken>

Content-Type: application/json

Accept: application/json
```

---

### Refresh Token

```
POST /api/v1/auth/refresh-token
```

Automatically invoked before token expiration.

---

# 6. Dio HTTP Client

Singleton configuration

```dart
class ApiClient {
  final Dio dio;
}
```

Interceptors

- Authentication
- Refresh Token
- Logging
- Retry
- Connectivity
- Correlation ID

---

Timeouts

Connect

30 seconds

Receive

60 seconds

Send

60 seconds

---

# 7. Repository Pattern

UI must never use Dio directly.

Correct flow

```text
UI

↓

Riverpod

↓

Repository

↓

API Client
```

Example

```dart
TrainingRepository

AttendanceRepository

PaymentRepository

NotificationRepository

AIRepository
```

---

# 8. DTO Mapping

Every API response must be mapped to immutable models.

Never expose JSON directly to widgets.

```text
JSON

↓

DTO

↓

Domain Model

↓

UI Model
```

---

# 9. Feature Integration

## Authentication

```
POST /auth/login

POST /auth/logout

POST /auth/refresh-token

GET /profile
```

---

## Dashboard

```
GET /dashboard
```

---

## Athlete

```
GET /athletes/me

PUT /athletes/me

GET /athletes/me/statistics
```

---

## Attendance

```
GET /attendance

POST /attendance/check-in

POST /attendance/check-out

POST /attendance/leave-request
```

---

## Training

```
GET /training

GET /training/{id}

GET /training/today

POST /training/feedback
```

---

## Performance

```
GET /performance

GET /performance/history

GET /performance/goals
```

---

## Tournament

```
GET /tournaments

GET /tournaments/{id}

POST /tournaments/register
```

---

## Events

```
GET /events

POST /events/register
```

---

## Finance

```
GET /finance/invoices

GET /finance/receipts

POST /finance/payments

GET /finance/wallet
```

---

## Notifications

```
GET /notifications

PUT /notifications/read

DELETE /notifications/{id}
```

---

## AI Platform

```
POST /ai/chat

GET /ai/conversations

GET /ai/prompts

POST /ai/feedback
```

---

# 10. Standard Response Model

```json
{
  "success": true,
  "message": "Operation completed successfully.",
  "data": {},
  "errors": [],
  "traceId": "abc123"
}
```

---

# 11. Pagination

Supported parameters

```
page

pageSize

sort

filter

search
```

Example

```
GET /training?page=1&pageSize=20
```

---

# 12. File Upload

Supports

- Images
- PDF
- Certificates
- Medical Reports
- Videos

Use

```
multipart/form-data
```

Progress indicator required.

---

# 13. Streaming APIs

Supported for AI

```
POST /ai/chat/stream
```

Use Server-Sent Events (SSE).

Display incremental tokens.

---

# 14. WebSocket

Used for

- Chat
- Live Match Scores
- Coach Presence
- Notification Updates

---

# 15. Error Handling

| Status | Meaning       | UI Action                |
| ------ | ------------- | ------------------------ |
| 400    | Validation    | Show validation message  |
| 401    | Unauthorized  | Refresh token/Login      |
| 403    | Forbidden     | Access denied            |
| 404    | Not Found     | Show empty state         |
| 409    | Conflict      | Reload latest data       |
| 422    | Business Rule | Display business message |
| 429    | Rate Limited  | Retry later              |
| 500    | Server Error  | Retry option             |

---

# 16. Retry Strategy

Retry automatically for

- Network failure
- Timeout
- HTTP 503

Do not retry

- HTTP 400
- HTTP 401
- HTTP 403
- HTTP 422

Use exponential backoff.

---

# 17. Caching

Cache

- Dashboard
- Training
- Attendance
- Profile
- Notifications
- AI History

Never cache

- JWT
- Payments in progress
- Passwords

---

# 18. Offline Synchronization

Queue offline actions

Examples

- Attendance
- Feedback
- Profile updates

Synchronize automatically when connectivity returns.

Conflict resolution policy

```
Server Wins
```

unless business rules require manual resolution.

---

# 19. Logging

Log

- Request ID
- Response Time
- HTTP Status
- API Name

Never log

- Passwords
- JWT
- Refresh Token
- Payment Details
- AI Prompts containing sensitive data

---

# 20. Security

Use

- HTTPS only
- Certificate Pinning
- Secure Storage
- JWT Authentication
- Refresh Tokens
- Biometric Login
- Device Binding (future)
- Root/Jailbreak Detection

---

# 21. Folder Structure

```text
lib/

core/network/

api_client.dart

interceptors/

repositories/

authentication/

training/

attendance/

payments/

notifications/

ai/

models/

dto/

mappers/
```

---

# 22. API Integration Checklist

- Repository pattern only
- Dio singleton
- JWT interceptor
- Refresh token interceptor
- Retry interceptor
- DTO mapping
- Immutable models
- Pagination support
- Offline queue
- Correlation ID
- Structured logging
- Error mapping
- Secure storage
- Feature-based repositories

---

# Related Documents

- 00-Mobile-App-Vision.md
- 01-Design-System.md
- 02-Information-Architecture.md
- 03-Navigation-Architecture.md
- 05-State-Management.md

---

**End of Document**
