---
title: Reference Feature Implementation Guide
module: Implementation Playbooks
feature: Attendance
version: 1.0
status: Approved
owner: Mobile Architecture Team
---

# Reference Feature Implementation

> This document demonstrates how to implement a complete feature using the Sports Gurukul architecture. Every new feature should follow the same implementation pattern.

---

# Table of Contents

1. Overview
2. Folder Structure
3. Domain Layer
4. Infrastructure Layer
5. Local Database
6. Repository
7. Application Layer
8. Riverpod
9. UI Layer
10. Routing
11. Localization
12. Analytics
13. Testing
14. Checklist

---

# 1. Overview

Example Feature

Attendance

Architecture

Presentation

↓

Application

↓

Domain

↓

Infrastructure

↓

API + Drift

---

# 2. Folder Structure

```text
features/

attendance/

presentation/

application/

domain/

infrastructure/

test/
```

---

# 3. Domain

Create

```
Attendance

AttendanceRepository

AttendanceFailure

AttendanceStatus

MarkAttendanceUseCase

GetAttendanceHistoryUseCase
```

No Flutter imports.

---

# 4. Infrastructure

Create

```
AttendanceDto

AttendanceMapper

AttendanceRemoteDatasource

AttendanceLocalDatasource

AttendanceRepositoryImpl
```

---

# 5. Database

Create

```
attendance_table.dart

attendance_dao.dart
```

Fields

```
id

athleteId

trainingId

checkInTime

checkOutTime

status

syncStatus

version

lastModified
```

---

# 6. Repository

Read

Memory Cache

↓

SQLite

↓

Remote API

Write

SQLite

↓

Sync Queue

↓

Background Upload

---

# 7. Application

Use Cases

```
MarkAttendance

GetAttendanceHistory

SyncAttendance

GetAttendanceStatistics
```

Each use case performs one business action.

---

# 8. Riverpod

Files

```
attendance_provider.dart

attendance_notifier.dart

attendance_state.dart

attendance_selectors.dart
```

Provider Flow

```text
UI

↓

Notifier

↓

Use Case

↓

Repository
```

---

# 9. UI

Screens

AttendancePage

AttendanceHistoryPage

AttendanceDetailPage

Widgets

AttendanceCard

AttendanceSummary

AttendanceTimeline

AttendanceFilter

---

# 10. Navigation

Register

```
/attendance

/attendance/:id

/history
```

Protect with authentication guard.

---

# 11. Localization

Keys

```
attendance.title

attendance.mark

attendance.success

attendance.failed

attendance.syncPending
```

---

# 12. Analytics

Track

```
attendance_screen_opened

attendance_marked

attendance_sync_started

attendance_sync_completed

attendance_sync_failed
```

---

# 13. Offline

Workflow

```text
User

↓

Mark Attendance

↓

SQLite

↓

Queue

↓

Sync

↓

Server

↓

Update Local Database
```

---

# 14. Testing

Unit

Repository

Use Cases

Widget

Attendance Page

Attendance Card

Integration

Attendance Flow

Offline Queue

Retry

---

# 15. Performance

Target

Attendance Load

<200 ms

Attendance Save

<100 ms

Offline Save

<50 ms

Sync

Background

---

# 16. Security

Validate

Authentication

Authorization

Input

Timestamp

GPS (if enabled)

Do not trust client-side timestamps without server validation.

---

# 17. Checklist

✓ Feature follows folder structure

✓ Domain independent

✓ Repository implemented

✓ Offline supported

✓ Localization complete

✓ Analytics integrated

✓ Tests written

✓ Documentation updated

✓ Code review approved

✓ Production ready

---

# End of Document
