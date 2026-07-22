# Runtime Architecture

Client Apps
  ↓
ASP.NET Core API
  ├── PostgreSQL
  ├── Redis
  └── FastAPI AI Service
         ├── LangGraph
         ├── Vector DB
         └── LLM Provider
