---
title: Developer Onboarding Guide
module: Implementation
platform: Flutter
version: 1.0
status: Approved
owner: Mobile Architecture Team
---

# Developer Onboarding Guide

> This guide helps new developers quickly set up their development environment, understand the project architecture, follow engineering standards, and contribute effectively to the Sports Gurukul mobile applications.

---

# Table of Contents

1. Welcome
2. Prerequisites
3. Required Software
4. Project Overview
5. Repository Structure
6. Environment Setup
7. Running the Project
8. Flutter Flavors
9. Development Workflow
10. Testing
11. Debugging
12. Code Contribution
13. Release Workflow
14. Troubleshooting
15. Learning Path
16. Acceptance Criteria

---

# 1. Welcome

Welcome to the Sports Gurukul Mobile Engineering Team.

Project Goals

✓ Enterprise Grade

✓ Offline First

✓ High Performance

✓ Secure

✓ Scalable

✓ AI Ready

---

# 2. Prerequisites

Knowledge

- Dart
- Flutter
- Git
- REST APIs
- Clean Architecture
- Riverpod

Recommended

- SQLite
- CQRS
- Azure DevOps
- GitHub

---

# 3. Required Software

Flutter SDK

Latest stable version approved by the project

Dart SDK

Bundled with Flutter

Android Studio

Latest stable

Xcode (macOS)

Latest supported version

Visual Studio Code

Recommended Extensions

- Dart
- Flutter
- Error Lens
- GitLens
- Code Spell Checker

Git

Latest LTS

---

# 4. Clone Repository

```bash
git clone https://github.com/<organization>/sports-gurukul-mobile.git

cd sports-gurukul-mobile
```

---

# 5. Install Dependencies

```bash
flutter pub get
```

Verify

```bash
flutter doctor
```

All checks should pass before development begins.

---

# 6. Project Structure

```text
lib/

app/

core/

shared/

features/

l10n/

assets/

test/
```

Read these documents first

- Flutter Project Architecture
- Clean Architecture
- Riverpod Architecture
- Repository Pattern
- Coding Standards

---

# 7. Environment Setup

Copy

```text
.env.dev.example
```

Create

```text
.env.dev
```

Configure

API URL

Environment

Analytics

Feature Flags

Do not commit environment files containing secrets.

---

# 8. Flutter Flavors

Available

Development

QA

UAT

Production

Run Development

```bash
flutter run --flavor dev
```

Build Production

```bash
flutter build appbundle --flavor prod
```

---

# 9. Running the Application

Android

```bash
flutter run
```

iOS

```bash
flutter run
```

Web (if enabled)

```bash
flutter run -d chrome
```

---

# 10. Project Architecture

```text
Presentation

↓

Application

↓

Domain

↓

Infrastructure
```

Read flow

```text
UI

↓

Provider

↓

Use Case

↓

Repository

↓

Local Database / API
```

---

# 11. Adding a New Feature

Create

```text
features/

feature_name/

presentation/

application/

domain/

infrastructure/
```

Register

- Routes
- Providers
- Repositories
- Localization
- Analytics Events
- Permissions (if applicable)

---

# 12. Development Workflow

```text
Create Branch

↓

Implement Feature

↓

Run Tests

↓

Update Documentation

↓

Open Pull Request

↓

Review

↓

Merge
```

---

# 13. Branch Naming

Examples

```
feature/attendance-sync

feature/ai-chat

fix/payment-timeout

refactor/profile

hotfix/login
```

---

# 14. Commit Messages

Examples

```
feat(training): add attendance workflow

fix(profile): resolve image upload

refactor(api): simplify retry interceptor

docs: update onboarding guide

test(training): add repository tests
```

---

# 15. Running Tests

All Tests

```bash
flutter test
```

Coverage

```bash
flutter test --coverage
```

Integration

```bash
flutter test integration_test
```

Analyze

```bash
flutter analyze
```

---

# 16. Before Opening a PR

Checklist

✓ Analyzer passes

✓ Tests pass

✓ Documentation updated

✓ Localization updated

✓ Accessibility reviewed

✓ No debug logs

✓ No secrets committed

---

# 17. Debugging Tools

Use

Flutter DevTools

Riverpod Inspector (when available)

Network Logging

Performance Overlay

Memory Profiler

Timeline

Avoid leaving debug instrumentation enabled in production builds.

---

# 18. Logging

Use structured logging.

Log

Feature

Duration

Correlation ID

Result

Never log

Passwords

JWT

OTP

Medical Data

Payment Information

---

# 19. Common Commands

Clean

```bash
flutter clean
```

Dependencies

```bash
flutter pub get
```

Upgrade

```bash
flutter pub upgrade
```

Analyze

```bash
flutter analyze
```

Format

```bash
dart format .
```

---

# 20. Troubleshooting

Flutter Doctor Issues

```bash
flutter doctor
```

Dependency Problems

```bash
flutter pub get
```

Build Cache

```bash
flutter clean
```

iOS Pods

```bash
cd ios

pod install
```

Android Gradle

```bash
./gradlew clean
```

---

# 21. Learning Path

Week 1

- Project Setup
- Architecture
- Git Workflow

Week 2

- Riverpod
- Repositories
- Drift
- Dio

Week 3

- Offline Sync
- Navigation
- Security

Week 4

- AI Module
- Performance
- Testing
- CI/CD

---

# 22. Coding Standards

Every developer must read

- Coding Standards
- Clean Architecture
- Testing Strategy
- Security Guidelines

Compliance is mandatory.

---

# 23. Communication

Use approved project channels for

- Daily updates
- Architecture discussions
- Production incidents
- Release planning
- Technical decisions

Architecture decisions should be documented through Architecture Decision Records (ADRs).

---

# 24. First Task

Recommended onboarding task

- Fix a small UI issue
- Add a localization string
- Write a unit test
- Review one Pull Request

This helps new developers understand the workflow before implementing larger features.

---

# 25. Acceptance Criteria

✓ Environment configured

✓ Project builds successfully

✓ Tests execute successfully

✓ Coding standards understood

✓ Architecture reviewed

✓ Development workflow understood

✓ Able to implement a feature independently

✓ Ready for code review

---

# Related Documents

Flutter Project Architecture

Clean Architecture

Riverpod Architecture

Repository Pattern

Local Database

Navigation

State Management Standards

Coding Standards

Testing Strategy

CI/CD & Release Management

---

# Future Enhancements

- Interactive onboarding portal
- Sample feature implementation
- Video walkthroughs
- Architecture playground
- Automated development environment setup
- AI-powered onboarding assistant
- Internal knowledge base integration

---

# End of Document
