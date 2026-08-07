# Sports Gurukul Mobile

Enterprise Flutter application for the Sports Gurukul ecosystem. Role-based
application serving **Athlete**, **Parent**, **Coach**, **Academy** and
**Super Admin** users from a single codebase.

**Tagline:** Train • Compete • Excel

## Status

- **Sprint 0 (P001–P004):** Foundation complete — architecture, folder
  structure, theming, localization, routing, strict linting (very_good_analysis),
  codegen (freezed/json), network layer (dio + interceptors), Drift scaffold,
  storage facades, utilities, testing tooling, and the `.ai/` AI governance
  knowledge base.
- Business features, authentication and API integration are **out of scope**
  for Sprint 0 and begin with Sprint 1 (P005+).

## Quick Start

```bash
# From the mobile/ directory
flutter pub get
flutter gen-l10n
dart run build_runner build
flutter test
flutter analyze
flutter run
```

The app boots to a splash screen and routes to a placeholder dashboard
confirming a successful bootstrap:

> **Sports Gurukul — Project Initialized Successfully**

## AI Governance

> Read the entire `.ai/` directory (repository root) before making any change.
> `PROMPT_TEMPLATE.md` is the mandatory template for AI prompts.

## Stack

| Concern      | Choice                                                        |
| ------------ | ------------------------------------------------------------- |
| Language     | Dart / Flutter (Material 3)                                   |
| Architecture | Clean Architecture + Feature First (Approved mobile docs)     |
| DI & State   | Riverpod 3.x (only; no Provider/Bloc/GetX)                    |
| Navigation   | go_router 17.x (centralized routes + guards)                  |
| Localization | flutter_localizations + gen_l10n (en, hi, mr)                 |
| API          | Dio 5.x (ApiClient + RequestId→Auth→Logging→Retry interceptors)|
| Database     | Drift 2.x (offline-first, migrations scaffolded)              |
| Storage      | flutter_secure_storage (secrets) + shared_preferences (prefs) |
| Models       | freezed + json_serializable (build_runner)                    |
| Linting      | very_good_analysis (strict; analyze must be clean)            |
| Testing      | flutter_test + mocktail + built-in goldens + coverage         |
| Logging      | package:logger via AppLogger (print() banned)                 |

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
| [Coding Standards](docs/07-Coding-Standards.md) | Authoritative coding rules |
| [Feature Development Guide](docs/08-Feature-Development-Guide.md) | How to build a feature |
| [Engineering Standards](docs/09-EngineeringStandards.md) | Linting, logging, DoD |
| [Dependency Guide](docs/10-DependencyGuide.md) | Package inventory + add policy |
| [Naming Convention](docs/11-NamingConvention.md) | File/class/layer naming |
| [Git Workflow](docs/12-GitWorkflow.md) | Branching, commits, release |
| [Package Decision Log](docs/13-PackageDecisionLog.md) | Why each package was chosen |

Authoritative product and technical specifications live in the repository
root under `docs/mobile/`; AI governance under `.ai/`.

## License

Proprietary. All rights reserved.
