# Folder Structure

Status: **Approved baseline** · Sprint 0 (P001)

```
mobile/
├── analysis_options.yaml       Lint configuration (flutter_lints + project rules)
├── l10n.yaml                   gen_l10n configuration
├── pubspec.yaml                Dependencies & app metadata
├── README.md
├── assets/
│   ├── images/                 App imagery (WebP preferred)
│   ├── icons/                  Brand and feature icons
│   ├── animations/             Lottie / JSON animations
│   ├── fonts/                  Bundled fonts (Inter planned)
│   ├── videos/                 Video assets
│   └── certificates/           Certificates (pinning, planned)
├── docs/                       Project documentation (this set)
├── integration_test/           Device-level smoke tests
├── lib/
│   ├── main.dart               Entry point
│   ├── app/                    Application bootstrap
│   │   ├── app.dart            Root widget (MaterialApp.router)
│   │   ├── bootstrap/          Startup sequence + splash screen
│   │   ├── config/             Environment + app config
│   │   ├── localization/       Localization composition (ARB sources in lib/l10n)
│   │   ├── router/             GoRouter config, names, paths, guards
│   │   └── theme/              Design tokens + Material 3 themes
│   ├── core/                   Reusable infrastructure
│   │   ├── api/                HTTP client, interceptors (P002)
│   │   ├── authentication/     Auth services & storage (P002)
│   │   ├── database/           Drift database (database sprint)
│   │   ├── network/            Connectivity (connectivity_plus)
│   │   ├── storage/            Secure & prefs storage
│   │   ├── security/           Pinning, biometrics
│   │   ├── logging/            Structured logging
│   │   ├── analytics/          Analytics + telemetry
│   │   ├── cache/              Memory cache
│   │   ├── errors/             Error types & result wrappers
│   │   ├── exceptions/         Exception hierarchy
│   │   ├── constants/          App-wide constants
│   │   ├── extensions/         Dart/Flutter extensions
│   │   ├── utilities/          Stateless helpers
│   │   ├── services/           Cross-cutting services
│   │   ├── interceptors/       (Reserved) HTTP interceptors
│   │   ├── permissions/        Runtime permission handling
│   │   └── sync/               Offline sync engine
│   ├── shared/                 Reusable, business-independent UI
│   │   ├── widgets/  dialogs/  forms/  buttons/  cards/  charts/
│   │   ├── animations/  navigation/  layouts/  theme/  icons/
│   │   └── design_system/      Design-system components
│   ├── features/               Feature modules (presentation/application/domain/infrastructure)
│   │   ├── authentication/     (P002)
│   │   ├── dashboard/          Splash → placeholder dashboard (P001)
│   │   ├── profile/            (later sprint)
│   │   ├── athlete/            Role-specific (later)
│   │   ├── coach/              Role-specific (later)
│   │   ├── academy/            Role-specific (later)
│   │   ├── booking/            (later)
│   │   ├── tournament/         (later)
│   │   ├── notification/       (later)
│   │   ├── ai/                 (later)
│   │   └── settings/           (later)
│   └── l10n/
│       ├── app_en.arb          English (template)
│       ├── app_hi.arb          Hindi
│       ├── app_mr.arb          Marathi
│       └── generated/          Generated AppLocalizations
├── scripts/                    CI/dev workflow scripts
│   ├── setup.sh                pub get + gen-l10n
│   ├── check.sh                format + analyze + test
│   └── build_android.sh / build_ios.sh
└── test/
    ├── unit/                   Pure Dart unit tests
    ├── widget/                 Widget tests
    ├── integration/            (device tests; currently in integration_test/)
    └── fixtures/               Test fixtures
```

## Decisions & Rationale

1. **Feature list (P001)** — `authentication`, `dashboard`, `profile`,
   `athlete`, `coach`, `academy`, `booking`, `tournament`, `notification`,
   `ai`, `settings` are created now. Remaining features from the approved
   architecture (`training`, `attendance`, `performance`, `payments`, etc.)
   are scaffolded at the start of their own sprints to avoid dead empty
   folders (YAGNI).
2. **`app/bootstrap/`** maps to the approved `app/startup/`; startup +
   environment concerns fold into `app/config/` for Sprint 0.
3. **`core/` and `shared/` subfolders** follow the approved
   `01-Flutter-Project-Architecture.md` layout exactly.
4. **Empty folders are tracked with `.gitkeep`** so the architecture shape
   is reviewable in version control.
