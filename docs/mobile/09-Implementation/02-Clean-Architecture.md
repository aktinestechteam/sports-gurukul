---
title: Flutter Clean Architecture Guide
module: Implementation
platform: Flutter
architecture: Clean Architecture
version: 1.0
status: Approved
owner: Mobile Architecture Team
---

# Flutter Enterprise Clean Architecture

> Defines the official Clean Architecture implementation for Sports Gurukul mobile applications. Every feature must follow this architecture to ensure maintainability, scalability, and testability.

---

# Table of Contents

1. Overview
2. Design Goals
3. Architecture Layers
4. Dependency Rule
5. Feature Architecture
6. Data Flow
7. CQRS Integration
8. Layer Responsibilities
9. Mapping Strategy
10. Error Handling
11. Dependency Injection
12. Offline First
13. Testing Strategy
14. Anti-Patterns
15. Acceptance Criteria

---

# 1. Overview

The application follows

✓ Clean Architecture

✓ Feature First

✓ CQRS

✓ Repository Pattern

✓ Offline First

✓ Riverpod

✓ Dependency Injection

Every feature is independent.

---

# 2. Design Goals

The architecture should provide

- Separation of Concerns
- Independent Features
- Easy Testing
- Maintainability
- Replaceable Infrastructure
- Reusable Business Logic
- Framework Independence

---

# 3. Architecture Layers

```text
┌──────────────────────────────┐
│        Presentation          │
└──────────────▲───────────────┘
               │
┌──────────────┴───────────────┐
│        Application           │
└──────────────▲───────────────┘
               │
┌──────────────┴───────────────┐
│          Domain              │
└──────────────▲───────────────┘
               │
┌──────────────┴───────────────┐
│      Infrastructure          │
└──────────────────────────────┘
```

Dependencies only point inward.

---

# 4. Dependency Rule

Allowed

```text
Presentation

↓

Application

↓

Domain

↓

Infrastructure
```

Not Allowed

Infrastructure → Presentation

Domain → Flutter

Application → UI Widgets

Presentation → Database

---

# 5. Feature Folder Structure

Example

```text
features/

training/

presentation/

application/

domain/

infrastructure/
```

---

# 6. Presentation Layer

Contains only UI logic.

Folders

```text
presentation/

pages/

widgets/

providers/

controllers/

dialogs/

state/

```

Responsibilities

- Render UI
- User interaction
- Navigation
- Form validation
- Loading/Error states

Never

- Call APIs
- Execute SQL
- Perform business calculations

---

# 7. Application Layer

Coordinates use cases.

```text
application/

usecases/

commands/

queries/

services/

```

Responsibilities

- Execute workflows
- Coordinate repositories
- Handle transactions
- Apply business policies

Example

```
RegisterTournament

↓

Validate Athlete

↓

Check Eligibility

↓

Call Repository

↓

Return Result
```

---

# 8. Domain Layer

Pure business rules.

Folders

```text
domain/

entities/

repositories/

value_objects/

events/

failures/

```

Contains

- Entities
- Interfaces
- Business Rules
- Value Objects

Never

- Flutter
- HTTP
- SQLite
- JSON
- Dio

---

# 9. Infrastructure Layer

Implements interfaces.

```text
infrastructure/

api/

datasources/

repositories/

models/

mappers/

database/

cache/

```

Responsibilities

- REST APIs
- SQLite
- Secure Storage
- File Storage
- Caching
- DTO Conversion

---

# 10. Complete Data Flow

```text
UI

↓

Riverpod Provider

↓

Use Case

↓

Repository Interface

↓

Repository Implementation

↓

Remote Data Source

↓

REST API

↓

.NET Backend
```

Offline

```text
UI

↓

Use Case

↓

Repository

↓

SQLite

↓

Sync Engine

↓

REST API
```

---

# 11. CQRS Integration

Query

```text
GetTrainingSchedule
```

Returns

Training List

Command

```text
MarkAttendance
```

Changes State

Every write operation

↓

Command

Every read operation

↓

Query

---

# 12. Repository Pattern

Presentation never knows

API

SQLite

Cache

Repository decides

Remote

↓

Local

↓

Cache

Example

```text
TrainingRepository

↓

API

or

SQLite

or

Cache
```

---

# 13. Mapping Strategy

Backend DTO

↓

Mapper

↓

Domain Entity

↓

Mapper

↓

Presentation Model

Never expose DTOs to the UI.

---

# 14. Riverpod Integration

Presentation

```text
ConsumerWidget

↓

TrainingProvider

↓

GetTrainingUseCase

↓

Repository
```

Providers never contain API code.

---

# 15. Error Handling

All use cases return

```dart
Result<Success, Failure>
```

Failure Types

NetworkFailure

ValidationFailure

AuthenticationFailure

PermissionFailure

BusinessFailure

UnknownFailure

UI never receives exceptions directly.

---

# 16. Offline First Strategy

Read

```text
SQLite

↓

Background Sync

↓

Server Update
```

Write

```text
SQLite

↓

Queue

↓

Background Upload

↓

Server
```

UI always reads local data.

---

# 17. Dependency Injection

Recommended

Riverpod

Example

```text
Provider

↓

Repository

↓

Datasource

↓

API Client
```

Avoid

Global service locators.

---

# 18. Sequence Diagram

```text
User

↓

Screen

↓

Provider

↓

Use Case

↓

Repository

↓

Remote Data Source

↓

API

↓

Repository

↓

Provider

↓

UI
```

---

# 19. Feature Communication

Never

```text
Attendance

↓

Training Repository
```

Instead

```text
Attendance

↓

Shared Service

↓

Training
```

---

# 20. Caching Strategy

Repository checks

```text
Memory Cache

↓

SQLite

↓

API
```

Update

SQLite

↓

UI

↓

Background Refresh

---

# 21. Logging

Every Use Case logs

Start

End

Duration

Failure

Correlation ID

Never log sensitive information.

---

# 22. Testing Strategy

Presentation

Widget Tests

Application

Unit Tests

Domain

Pure Unit Tests

Infrastructure

Mock APIs

Integration Tests

---

# 23. Anti-Patterns

❌ API calls from widgets

❌ Business logic in UI

❌ DTOs in Presentation

❌ SQL in Providers

❌ Shared mutable state

❌ Global singletons

❌ Circular dependencies

❌ Massive repositories

---

# 24. Example Feature Flow

```text
Athlete taps

Attendance

↓

AttendanceProvider

↓

MarkAttendanceUseCase

↓

AttendanceRepository

↓

Remote API

↓

Local SQLite

↓

Sync Queue

↓

Updated UI
```

---

# 25. Acceptance Criteria

✓ Every feature follows four-layer architecture

✓ Domain has zero Flutter dependency

✓ Repository pattern implemented

✓ CQRS respected

✓ Offline-first supported

✓ Riverpod integrated

✓ Testable architecture

✓ Modular implementation

✓ No circular dependencies

✓ Enterprise ready

---

# Related Documents

03-Riverpod-Architecture.md

04-Dio-API-Architecture.md

05-Repository-Pattern.md

06-Local-Database.md

07-Navigation.md

08-State-Management.md

09-Coding-Standards.md

10-Developer-Onboarding.md

---

# Future Enhancements

- Modular feature packages
- Plugin-based feature loading
- Code generation for repositories
- Event-driven domain architecture
- Feature dependency analyzer
- Automatic architecture validation

---

# End of Document
