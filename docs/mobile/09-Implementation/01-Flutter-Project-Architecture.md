---
title: Flutter Enterprise Project Architecture
module: Implementation
platform: Flutter
architecture: Clean Architecture + Feature First + Riverpod
version: 1.0
status: Approved
owner: Mobile Architecture Team
---

# Flutter Enterprise Project Architecture

> This document defines the official project architecture for the Sports Gurukul mobile ecosystem. All applications (Athlete, Parent, Coach, Academy Admin, Super Admin) must follow this architecture.

---

# Table of Contents

1. Objectives
2. Architecture Principles
3. High-Level Architecture
4. Folder Structure
5. Feature Structure
6. Shared Modules
7. Application Layers
8. Dependency Rules
9. Package Strategy
10. Naming Conventions
11. Build Configuration
12. Environment Management
13. Coding Standards
14. Acceptance Criteria

---

# 1. Objectives

The architecture should provide

✓ Scalability

✓ Maintainability

✓ Testability

✓ Modular Design

✓ Offline First

✓ High Performance

✓ Easy Feature Development

---

# 2. Architecture Principles

Feature First

Clean Architecture

SOLID

Repository Pattern

Dependency Injection

Offline First

Immutable State

Composition over Inheritance

Single Responsibility

---

# 3. High-Level Architecture

```text
Presentation

↓

Application

↓

Domain

↓

Infrastructure

↓

External Services
```

---

# 4. Complete Folder Structure

```text
lib/

app/

core/

shared/

features/

assets/

l10n/

main.dart
```

---

# app/

Contains application bootstrap

```text
app/

app.dart

router/

theme/

localization/

startup/

environment/

config/

flavors/
```

Responsibilities

• Application startup

• Dependency registration

• Routing

• Themes

• Global configuration

---

# core/

Reusable infrastructure

```text
core/

api/

authentication/

database/

network/

storage/

security/

logging/

analytics/

cache/

errors/

exceptions/

constants/

extensions/

utilities/

services/

interceptors/

permissions/

sync/

```

Nothing in core should depend on a feature.

---

# shared/

Reusable UI

```text
shared/

widgets/

dialogs/

forms/

buttons/

cards/

charts/

animations/

navigation/

layouts/

theme/

icons/

design_system/
```

Shared widgets must remain business-independent.

---

# features/

Business functionality

```text
features/

authentication/

dashboard/

training/

attendance/

performance/

tournaments/

events/

payments/

wallet/

documents/

medical/

communication/

notifications/

achievements/

leaderboard/

profile/

settings/

support/

ai/

```

Every feature follows the same internal structure.

---

# assets/

```text
assets/

images/

icons/

animations/

fonts/

videos/

certificates/

```

---

# l10n/

```text
l10n/

app_en.arb

app_hi.arb

app_mr.arb

...
```

---

# 5. Feature Structure

Example

```text
training/

presentation/

application/

domain/

infrastructure/

```

---

## Presentation

Contains UI only

```text
presentation/

pages/

screens/

widgets/

providers/

controllers/

dialogs/

```

Never call APIs directly.

---

## Application

Contains business orchestration

```text
application/

services/

usecases/

commands/

queries/

```

Coordinates domain logic.

---

## Domain

Pure business rules

```text
domain/

entities/

repositories/

value_objects/

failures/

```

No Flutter dependency.

---

## Infrastructure

External implementations

```text
infrastructure/

repositories/

datasources/

models/

mappers/

api/

local/

```

Handles

REST

SQLite

Secure Storage

Caching

File System

---

# 6. Dependency Rules

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

Infrastructure

↓

Presentation

Domain

↓

Flutter Widgets

Presentation

↓

Database

---

# 7. Module Communication

Features never access each other's database.

Communication

```text
Feature A

↓

Shared Service

↓

Feature B
```

Never

```text
Feature A

↓

Feature B Repository
```

---

# 8. Package Strategy

Internal packages

```text
packages/

sg_design_system/

sg_core/

sg_api/

sg_analytics/

sg_testing/

```

Allows reuse across

Athlete App

Parent App

Coach App

Admin Portal

---

# 9. Dependency Injection

Recommended

Riverpod

```text
Provider

↓

Repository

↓

Datasource

↓

API
```

Avoid

Global Singletons

---

# 10. Environment Configuration

```text
Development

QA

UAT

Production
```

Each environment has

API URL

Analytics

Feature Flags

Logging Level

---

# 11. Configuration Files

```text
.env.dev

.env.qa

.env.uat

.env.prod
```

Never commit secrets.

---

# 12. Naming Standards

Screens

```
TrainingPage
```

Widgets

```
TrainingCard
```

Providers

```
TrainingProvider
```

Repositories

```
TrainingRepository
```

Models

```
TrainingDto
```

Entities

```
Training
```

Use Cases

```
GetTrainingSchedule
```

---

# 13. Feature Registration

Every feature registers

Routes

Providers

Repositories

Localization

Permissions

Analytics Events

---

# 14. Build Order

```text
Core

↓

Shared

↓

Infrastructure

↓

Features

↓

App
```

---

# 15. Error Handling

Every feature returns

```text
Success

or

Failure
```

Never throw unhandled exceptions to UI.

---

# 16. Logging

Every feature logs

API Calls

Errors

Performance

Sync

Authentication

Navigation

---

# 17. Testing Structure

Every feature includes

```text
test/

unit/

widget/

integration/

fixtures/

```

---

# 18. Performance Rules

No feature loads unnecessary data.

Lazy loading required.

Pagination required.

Caching required.

Background sync supported.

---

# 19. Security Rules

Never

Store passwords

Store OTP

Log JWT

Log medical data

Log payment information

---

# 20. Acceptance Criteria

✓ Feature-first architecture

✓ Clean Architecture enforced

✓ Shared UI components

✓ Modular design

✓ Offline-first

✓ Testable

✓ Secure

✓ Scalable

✓ Easy onboarding

✓ Enterprise ready

---

# Related Documents

02-Clean-Architecture.md

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

- Plugin architecture
- Dynamic feature loading
- Micro-frontend support
- Cross-platform shared packages
- Independent feature versioning

---

# End of Document
