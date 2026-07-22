# Architecture Overview

```mermaid
flowchart LR
Mobile --> API
Web --> API
API --> PostgreSQL
API --> Redis
API --> AI
AI --> VectorDB
API --> Storage
```
