# Sports Gurukul

AI-first digital sports ecosystem connecting athletes, coaches, academies, parents, scouts, sponsors and tournament organizers on a unified platform.

## Repository Structure

```
sportsgurukul/
├── backend/          # ASP.NET Core 9 — Clean Architecture
├── ai-services/      # FastAPI (Python) — AI Coach, RAG, LangGraph
├── mobile/           # Flutter — Athlete / Coach mobile app
├── web-admin/        # React + Vite + TypeScript — Admin portal
├── docs/             # Architecture, PRD, API specs
├── .github/          # CI / CD workflows
├── docker-compose.yml
└── README.md
```

## Technology Stack

| Layer | Technology |
|-------|-----------|
| Mobile | Flutter (Riverpod) |
| Admin | React 19 + Vite + TypeScript |
| Backend | ASP.NET Core 9 (.NET 9) — Clean Architecture |
| AI Services | FastAPI + LangGraph + RAG |
| Database | PostgreSQL 16 |
| Cache | Redis 7 |
| Vector DB | Qdrant |
| Containers | Docker + Docker Compose |

## Getting Started

### Prerequisites

- .NET 9 SDK
- Node.js 18+ / npm
- Python 3.12+
- Flutter 3.x (for mobile)
- Docker & Docker Compose (optional)

### Quick Start

```bash
# 1. Clone the repository
git clone https://github.com/<org>/sportsgurukul.git
cd sportsgurukul

# 2. Start infrastructure (PostgreSQL + Redis)
docker compose up -d postgres redis

# 3. Backend
cd backend
dotnet restore
dotnet build
dotnet run --project src/SportsGurukul.Api

# 4. AI Service
cd ../ai-services
python -m venv .venv
.venv\Scripts\activate        # Windows
pip install -r requirements.txt
uvicorn app.main:app --reload --port 8000

# 5. Web Admin
cd ../web-admin
npm install
npm run dev
```

## Backend — Clean Architecture

| Project | Responsibility |
|---------|---------------|
| `SportsGurukul.Domain` | Entities, value objects, domain events |
| `SportsGurukul.Application` | Use-cases, DTOs, validators, interfaces |
| `SportsGurukul.Infrastructure` | EF Core, repositories, external services |
| `SportsGurukul.Api` | Controllers, middleware, DI, startup |
| `SportsGurukul.UnitTests` | Unit tests (xUnit) |
| `SportsGurukul.IntegrationTests` | Integration tests (xUnit) |

## CI / CD

GitHub Actions pipeline runs on every push and pull request:

- Restore → Build → Test → Lint

## License

Proprietary — All rights reserved.
