# Enterprise ERD

```mermaid
erDiagram
    USERS ||--o{ USERROLES : has
    ROLES ||--o{ USERROLES : contains
    USERS ||--|| ATHLETES : owns
    USERS ||--|| COACHES : owns
    ACADEMIES ||--o{ COACHES : employs
```

## Design Notes
- UUID primary keys
- Soft deletes
- Audit columns on all business tables
- Foreign key integrity
- Index frequently queried columns
- Partition large transactional tables in future
