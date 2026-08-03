---
title: Local Database Architecture
module: Implementation
platform: Flutter
database: Drift (SQLite)
version: 1.0
status: Approved
owner: Mobile Architecture Team
---

# Local Database Architecture

> Defines the official local database architecture for Sports Gurukul using Drift (SQLite). The database is the primary source of data for the mobile application and supports offline-first operations, synchronization, encryption, and high-performance queries.

---

# Table of Contents

1. Overview
2. Objectives
3. Architecture
4. Database Design Principles
5. Database Structure
6. Core Tables
7. Feature Tables
8. Relationships
9. DAO Pattern
10. Migration Strategy
11. Sync Metadata
12. Offline Queue
13. Indexing
14. Encryption
15. Performance
16. Backup Strategy
17. Testing
18. Acceptance Criteria

---

# 1. Overview

The local database provides

✓ Offline First

✓ Fast Reads

✓ Background Synchronization

✓ Local Search

✓ Queue Persistence

✓ Conflict Resolution Support

UI always reads from SQLite.

---

# 2. Objectives

Provide

- High Performance
- Data Integrity
- Offline Availability
- Secure Storage
- Easy Migration
- Incremental Sync

---

# 3. Architecture

```text
Flutter UI

↓

Riverpod

↓

Repository

↓

DAO

↓

Drift Database

↓

SQLite
```

The database never communicates directly with the UI.

---

# 4. Design Principles

✓ Normalize transactional data

✓ Denormalize dashboard summaries where beneficial

✓ Indexed Queries

✓ Immutable IDs

✓ Soft Delete

✓ Version Tracking

✓ Sync Metadata

✓ Optimistic Updates

---

# 5. Database Structure

```text
database/

database.dart

tables/

daos/

migrations/

converters/

seed/

```

---

# 6. Core Tables

## Users

Stores

- User ID
- Name
- Email
- Mobile
- Role
- Status

---

## Profile

Stores

- Athlete Details
- Academy
- Sport
- Position
- Preferences

---

## Settings

Stores

- Theme
- Language
- Notification Settings
- Privacy Preferences

---

## SyncMetadata

Stores

- Entity
- Last Sync Time
- Version
- Sync Status
- Checksum (optional)

---

# 7. Feature Tables

Training

Attendance

Performance

Tournament

Events

Payments

Wallet

Notifications

Achievements

Leaderboard

Medical

Documents

Messages

Tasks

Calendar

AI Conversations

Support Tickets

Downloads

Each feature owns its own tables.

---

# 8. Example Training Table

```text
Training

id

title

coachId

academyId

startTime

endTime

status

lastModified

version
```

---

# 9. Attendance Table

```text
Attendance

id

trainingId

athleteId

status

checkInTime

checkOutTime

syncStatus
```

---

# 10. Documents Table

Stores

Metadata only

- Document ID
- Name
- Type
- Local Path
- Remote URL
- Version
- Status

Binary files remain in secure file storage.

---

# 11. AI Conversation Table

Stores

Conversation ID

Prompt

Response Summary (optional)

Timestamp

Status

Conversation metadata only when required by user settings and privacy policy.

---

# 12. Offline Queue

Stores

```text
QueueId

Entity

Operation

Payload

CreatedAt

RetryCount

Status
```

Operations

Insert

Update

Delete

Upload

---

# 13. Relationships

```text
Athlete

↓

Training

↓

Attendance

↓

Performance

↓

Achievements
```

Foreign keys enabled.

Cascade rules defined per entity.

---

# 14. DAO Pattern

Each feature owns

```
TrainingDao

AttendanceDao

PaymentDao

MedicalDao

DocumentDao
```

DAO Responsibilities

CRUD

Filtering

Pagination

Transactions

Search

No business logic.

---

# 15. Transactions

Use transactions for

Attendance

Payments

Medical Updates

Sync Operations

Document Metadata

Guarantee atomic writes.

---

# 16. Migration Strategy

Every schema change requires

Migration

Version Increment

Backward Compatibility

Migration Tests

Example

```text
Version 7

↓

Version 8

↓

Add column

↓

Populate data

↓

Validate
```

Never delete production data automatically.

---

# 17. Sync Metadata

Every table includes

```
lastModified

version

syncStatus

deletedAt (nullable)

```

Sync Status

Pending

Synced

Failed

Conflict

---

# 18. Indexing Strategy

Indexes on

Primary Key

Foreign Keys

Search Columns

Sync Status

Last Modified

Date Fields

Avoid unnecessary indexes.

---

# 19. Search Strategy

Use SQLite indexes for

Training

Documents

Messages

Notifications

Support full-text search (FTS) where justified, such as messages or knowledge base articles.

---

# 20. Encryption

Protect

Medical Data

Documents Metadata

Authentication Tokens (stored in Secure Storage)

Sensitive Preferences

SQLite encryption should be enabled if required by security policy.

---

# 21. Cleanup Policy

Automatically remove

Expired Cache

Old Notifications

Temporary Downloads

Completed Sync Queue Entries

Retain audit-relevant data according to business rules.

---

# 22. Backup Strategy

User Data

↓

Server Sync

↓

Cloud Backup

Local database is recoverable through synchronization rather than device-only backups.

---

# 23. Performance Targets

Database Open

<100 ms

Simple Query

<20 ms

Insert

<20 ms

Transaction

<100 ms

Bulk Insert (100 records)

<300 ms

---

# 24. Testing

Validate

CRUD Operations

Transactions

Migrations

Indexes

Conflict Resolution

Offline Queue

Corruption Recovery

---

# 25. Folder Structure

```text
database/

database.dart

tables/

users.dart

profile.dart

training.dart

attendance.dart

payments.dart

medical.dart

documents.dart

sync_metadata.dart

offline_queue.dart

daos/

training_dao.dart

attendance_dao.dart

payment_dao.dart

medical_dao.dart

document_dao.dart

migrations/

migration_v1.dart

migration_v2.dart

seed/

demo_data.dart
```

---

# 26. Acceptance Criteria

✓ Offline-first architecture

✓ Feature-owned tables

✓ DAO pattern implemented

✓ Indexed queries

✓ Sync metadata available

✓ Offline queue supported

✓ Migration strategy defined

✓ Encryption strategy documented

✓ Performance targets achieved

✓ Fully testable

---

# Related Documents

07-Navigation.md

08-State-Management.md

Repository Pattern

Offline Synchronization

Riverpod Architecture

---

# Future Enhancements

- Incremental database synchronization
- Database compression
- Intelligent cache eviction
- Background vacuum and optimization
- Read-only replicas for analytics
- Multi-account database partitioning

---

# End of Document
