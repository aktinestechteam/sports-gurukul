# Database Design

Version: 1.0

## Overview
Primary database: PostgreSQL

## Design Principles
- UUID primary keys
- Soft deletes
- Audit columns
- Optimistic concurrency
- Foreign key integrity

## Core Domains
- Identity
- Athlete
- Coach
- Academy
- Booking
- Payment
- Tournament
- Marketplace
- Community
- Notifications
- Analytics

## Common Columns
- Id (UUID)
- CreatedOn
- CreatedBy
- UpdatedOn
- UpdatedBy
- IsDeleted
- Version

## Key Tables
- Users
- Roles
- Athletes
- Coaches
- Academies
- Bookings
- Payments
- Tournaments
- Products
- Orders
- Notifications
- AuditLogs

## Index Strategy
- PK indexes
- FK indexes
- Composite indexes for search
- Full-text search where applicable

## ERD (High Level)

Users --> Athletes
Users --> Coaches
Academies --> Coaches
Academies --> Athletes
Athletes --> Bookings
Bookings --> Payments
Tournaments --> Registrations

## Backup Strategy
- Daily full backup
- Point-in-time recovery
- Read replicas

## Future
- Table partitioning
- Archival policies
- Data warehouse integration
