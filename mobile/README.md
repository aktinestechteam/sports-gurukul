# Sports Gurukul Mobile

Enterprise Flutter application for the Sports Gurukul ecosystem. Role-based
application serving **Athlete**, **Parent**, **Coach**, **Academy** and
**Super Admin** users from a single codebase.

**Tagline:** Train • Compete • Excel

## Status

- **Sprint 0 (P001):** Project foundation — architecture, folder structure,
  theming, localization, routing and CI-ready tooling.
- Business features, authentication and API integration are **out of scope**
  for Sprint 0 and will be delivered in subsequent sprints (P002 onward).

## Quick Start

```bash
# From the mobile/ directory
flutter pub get
flutter gen-l10n
flutter run
```

The app boots to a splash screen and routes to a placeholder dashboard
confirming a successful bootstrap:

> **Sports Gurukul — Project Initialized Successfully**

## Stack

| Concern      | Choice                                                        |
| ------------ | ------------------------------------------------------------- |
| Language     | Dart / Flutter (Material 3)                                   |
| Architecture | Clean Architecture + Feature First (Approved mobile docs)     |
| DI & State   | Riverpod 3.x                                                  |
| Navigation   | go_router (role-based routing planned)                        |
| Localization | flutter_localizations + gen_l10n (en, hi, mr)                 |
| Offline      | Offline-first ready (Drift/SQLite planned for database sprint)|
| API          | Dio (added in P002 with the API layer)                        |

## Documentation

| Document | Purpose |
| -------- | ------- |
| [README](README.md) | This file |
| [Architecture Overview](docs/01-Architecture-Overview.md) | Layers, patterns, decisions |
| [Folder Structure](docs/02-Folder-Structure.md) | Full `lib/` tree and rationale |
| [Development Workflow](docs/03-Development-Workflow.md) | Branching, sprints, quality gates |
| [Project Setup](docs/04-Project-Setup.md) | Prerequisites and environment setup |
| [Build Commands](docs/05-Build-Commands.md) | Run, test, analyze, build commands |
| [Contribution Guidelines](docs/06-Contribution-Guidelines.md) | Coding standards and PR process |

Authoritative product and technical specifications live in the repository
root under `docs/mobile/`.

## License

Proprietary. All rights reserved.
