# DATABASE

Status: **Adopted** - Owner: Chief Software Architect

## 1. Rule

**Drift is the only local database.** No raw sqflite, isar, hive, or manual
SQLite.

Current: `drift ^2.34.3` + `drift_flutter ^0.3.1` + `drift_dev ^2.34.0`.

## 2. Offline-first

- The app is offline-first: reads work from the local Drift store; writes are
  queued when offline and synced when connectivity returns.
- Connectivity is observed through `ConnectivityService`
  (`lib/core/connectivity/connectivity_service.dart`).
- Offline mutations go through `OfflineQueue` (`lib/core/offline/`) - the
  outbox table and replay logic land with feature work (P005+).

## 3. DAO pattern

- Tables, `@DriftDatabase`, and generated data classes live in
  `core/database/` (`app_database.dart` + `.g.dart`).
- Data-access objects (DAOs) encapsulate queries; repositories in feature
  `infrastructure/` depend on DAOs, not on raw queries.
- Database code never leaks into `presentation/`.

## 4. Migrations

- `AppDatabase.schemaVersion` starts at `1`; a `MigrationStrategy` is defined
  (onCreate / onUpgrade / beforeOpen with `PRAGMA foreign_keys = ON`).
- Every schema change: bump `schemaVersion` and add a step in `onUpgrade`.
  **Never mutate existing tables in place across releases.**
- Validate migrations on web build target and desktop tests.

## 5. Conflict resolution

- Write conflicts between local changes and server state are resolved at the
  repository boundary: prefer server data for shared entities, local pending
  mutations for user-created data; merge rules per aggregate documented in the
  feature spec. See `docs/mobile/08-Platform/02-Offline-Synchronization.md`.

## 6. Storage separation

- Secrets/tokens: `SecureStorage` (keychain) - never in Drift.
- Non-sensitive preferences: `PreferenceStorage` (shared_preferences).
- Drift is for structured domain data that needs queries, relations and
  offline sync. Do not use it as a general key-value bag.

## 7. Reference

- `docs/mobile/09-Implementation/06-Local-Database.md`
- `mobile/docs/13-PackageDecisionLog.md` (drift decision)
