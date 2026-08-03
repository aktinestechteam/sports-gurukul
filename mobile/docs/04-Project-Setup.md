# Project Setup

Status: **Approved baseline** · Sprint 0 (P001)

## 1. Prerequisites

| Tool | Version | Notes |
| ---- | ------- | ----- |
| Flutter SDK | 3.44.x stable | Bundles Dart 3.12.x |
| Dart SDK | 3.12.x | Comes with Flutter |
| Android SDK | min SDK 26, recommended 35 | For Android builds |
| Xcode | 16+ | macOS only, for iOS builds |
| Git | any recent | Repository access |

Verify with:

```bash
flutter --version
flutter doctor
```

## 2. Getting the Code

```bash
git clone <repo-url> SportsGurukul
cd SportsGurukul/mobile
```

## 3. One-Time Setup

From the `mobile/` directory:

```bash
# Windows PowerShell
flutter pub get
flutter gen-l10n
```

Or use the setup script (bash on macOS/Linux/Git-Bash):

```bash
./scripts/setup.sh
```

## 4. Running the App

Connect a device/emulator or use a desktop/web target, then:

```bash
flutter run
```

Expected result: splash screen → placeholder dashboard showing
**"Project Initialized Successfully"**.

## 5. Localization

Localization sources are `lib/l10n/app_{en,hi,mr}.arb`. Regenerate after
changing an ARB file:

```bash
flutter gen-l10n
```

The generated classes land in `lib/l10n/generated/` and are regenerated
automatically on build (`flutter: generate: true`).

## 6. Environment Configuration

Sprint 0 uses the `development` environment default
(`lib/app/config/app_environment.dart`). Build flavors
(`sportsgurukul_dev/qa/uat/prod`) and environment-specific configuration
land with the API sprint (P002). **Never commit secrets**; use environment
configuration and secure storage.

## 7. Repository Layout

- `mobile/` — Flutter application (this project)
- `backend/` — ASP.NET Core API (already completed; contract via Swagger)
- `ai-services/` — Python AI services
- `web-admin/` — React admin portal
- `docs/` — Product and technical specifications
