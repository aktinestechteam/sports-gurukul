---
title: Flutter Coding Standards & Best Practices
module: Implementation
platform: Flutter
version: 1.0
status: Approved
owner: Mobile Architecture Team
---

# Flutter Coding Standards & Best Practices

> Defines the official coding standards, naming conventions, project organization, review process, and engineering best practices for Sports Gurukul mobile applications.

---

# Table of Contents

1. Overview
2. Engineering Principles
3. Dart Standards
4. Flutter Standards
5. Naming Conventions
6. File Organization
7. Error Handling
8. Logging Standards
9. Documentation Standards
10. Performance Guidelines
11. Security Practices
12. Dependency Management
13. Code Review Checklist
14. Git Standards
15. Definition of Done
16. Acceptance Criteria

---

# 1. Overview

Every line of code should be

✓ Readable

✓ Testable

✓ Maintainable

✓ Secure

✓ Performant

✓ Consistent

Code is written for humans first, computers second.

---

# 2. Engineering Principles

Follow

- SOLID
- DRY (Don't Repeat Yourself)
- KISS (Keep It Simple)
- YAGNI (You Aren't Gonna Need It)
- Composition over Inheritance
- Fail Fast
- Clean Architecture

---

# 3. Dart Standards

Use

- Null Safety
- Final variables by default
- Const constructors where possible
- Named parameters
- Trailing commas for multi-line widgets
- Strong typing

Avoid

- `dynamic`
- Force unwrap (`!`) unless justified
- Long methods
- Deep nesting

Target

Method Length

<40 lines

Class Length

<300 lines

Cyclomatic Complexity

<10

---

# 4. Flutter Standards

Widgets

Prefer

StatelessWidget

ConsumerWidget

Small reusable widgets

Avoid

Large build methods

Nested widgets exceeding reasonable readability

Business logic inside widgets

---

# 5. Naming Conventions

Classes

```
TrainingRepository
```

Files

```
training_repository.dart
```

Variables

```
trainingList
```

Constants

```
maxRetryCount
```

Private Members

```
_trainingRepository
```

Enums

```
TrainingStatus
```

Extensions

```
DateTimeExtensions
```

---

# 6. Folder Organization

```text
feature/

presentation/

application/

domain/

infrastructure/
```

One responsibility per file.

Avoid utility classes that become dumping grounds.

---

# 7. State Management Rules

UI

↓

Provider

↓

Use Case

↓

Repository

↓

Datasource

Never bypass layers.

---

# 8. Error Handling

Never expose

Stack Trace

Raw Exception

Database Error

HTTP Error

To the UI.

Instead

Map to

```
Failure
```

Display

Friendly message

Retry option

---

# 9. Logging Standards

Log

Feature

Method

Duration

Correlation ID

Result

Never log

Passwords

OTP

JWT

Medical Data

Payment Data

PII

Use structured logging.

---

# 10. Documentation

Every public class

Must include

Purpose

Responsibilities

Usage Notes (when needed)

Complex business rules should be documented with comments explaining "why", not "what".

---

# 11. Performance Guidelines

Use

const widgets

ListView.builder

GridView.builder

Pagination

Lazy loading

Memoization where appropriate

Avoid

Repeated API calls

Unnecessary rebuilds

Large synchronous work on UI thread

---

# 12. Security Practices

Never

Hardcode API Keys

Commit Secrets

Store Passwords

Store OTP

Disable TLS Validation

Always

Validate Inputs

Use Secure Storage

Mask Sensitive Logs

---

# 13. Dependency Management

Use only approved packages.

Every dependency must

Have active maintenance

Support null safety

Be compatible with the target Flutter version

Be reviewed for licensing and security

Review dependencies quarterly.

---

# 14. Linting

Required

```
flutter analyze
```

Recommended

```
dart_code_metrics
```

No analyzer warnings before merge.

---

# 15. Code Review Checklist

Architecture

✓ Clean Architecture respected

✓ Feature boundaries maintained

Code Quality

✓ Naming conventions followed

✓ Readable methods

✓ No duplication

Testing

✓ Unit tests added

✓ Widget tests updated

Security

✓ No secrets

✓ Input validation

Performance

✓ Minimal rebuilds

✓ Pagination

Documentation

✓ Public APIs documented

---

# 16. Git Commit Standards

Format

```
type(scope): summary
```

Examples

```
feat(training): add attendance sync

fix(payment): resolve retry issue

refactor(profile): simplify repository

docs(api): update authentication guide

test(training): add provider tests
```

Types

- feat
- fix
- refactor
- docs
- test
- chore
- perf
- ci
- build

---

# 17. Pull Request Standards

Every PR must include

- Summary
- Related Work Item / Issue
- Screenshots (if UI changes)
- Test Evidence
- Rollback Impact
- Breaking Changes (if any)

Checklist

✓ Tests Passed

✓ Analyzer Clean

✓ Documentation Updated

✓ Review Approved

---

# 18. Definition of Done (DoD)

A feature is complete only if

✓ Requirements implemented

✓ Architecture followed

✓ Unit tests written

✓ Widget tests updated

✓ Integration tests pass (where applicable)

✓ Documentation updated

✓ Localization complete

✓ Accessibility verified

✓ Performance reviewed

✓ Security review completed

✓ Code review approved

✓ CI pipeline green

---

# 19. Common Anti-Patterns

❌ Business logic inside widgets

❌ API calls from UI

❌ Duplicate code

❌ Circular dependencies

❌ Massive classes

❌ God objects

❌ Global mutable state

❌ Hardcoded strings

❌ Hardcoded colors

❌ Copy-paste implementations

---

# 20. Performance Targets

Analyzer Warnings

0

Critical Bugs

0

Unit Test Coverage

> 90%

Widget Test Coverage

Critical components

PR Review Time

<2 business days

CI Build

<15 minutes

---

# 21. Folder Naming

Use

```
snake_case
```

Examples

```
training_repository.dart

attendance_provider.dart

medical_dashboard_page.dart
```

---

# 22. Import Ordering

Order imports as follows

1. Dart SDK
2. Flutter SDK
3. Third-party packages
4. Internal packages
5. Relative imports

Separate each group with a blank line.

---

# 23. Localization Standards

Never hardcode user-visible text.

Use localization keys

```
context.l10n.trainingTitle
```

Avoid inline strings in widgets.

---

# 24. Accessibility Standards

Every interactive component should provide

Semantic label

Accessible focus

Minimum touch target

Keyboard support where applicable

---

# 25. Acceptance Criteria

✓ Coding conventions documented

✓ Naming standards defined

✓ Security rules enforced

✓ Performance practices documented

✓ Code review checklist established

✓ Definition of Done defined

✓ Git standards documented

✓ Localization standards enforced

✓ Accessibility included

✓ Enterprise ready

---

# Related Documents

Flutter Project Architecture

Clean Architecture

Riverpod Architecture

Navigation

Testing Strategy

CI/CD & Release Management

---

# Future Enhancements

- AI-assisted code review
- Architecture rule validation
- Automated dependency health reports
- Secure coding scorecards
- Static architecture compliance checks
- Repository-wide coding metrics dashboard

---

# End of Document
