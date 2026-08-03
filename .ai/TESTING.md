# TESTING

Status: **Adopted** - Owner: Chief Software Architect

Testing strategy for the Flutter app.
Detail: `docs/mobile/08-Platform/09-Testing-Strategy.md`.

## 1. Test layers

| Layer | Location | Covers |
| --- | --- | --- |
| Unit | `test/unit/` | Pure logic: mappers, DTOs, models, facades, use cases, error mapping |
| Widget | `test/widget/` | Widgets, pages, provider wiring, goldens |
| Integration | `test/integration/` | Full flows across app shell + services |
| Golden | `test/widget/goldens/` | Visual regression via `matchesGoldenFile` |

Support folders: `test/fixtures/` (data), `test/helpers/` (pump helpers),
`test/mocks/` (mocktail mocks).

## 2. Rules

- **New behaviour ships with tests.** No test = not done.
- Unit-test pure logic; widget-test user-visible behaviour; keep assertions
  meaningful (not just "renders").
- Test error and empty states, not just the happy path.
- Use `mocktail` for mocks (`test/mocks/`); prefer real/fake lightweight
  doubles over mocks where feasible.
- Providers are overridden at the `ProviderScope` in widget tests
  (Riverpod 3: pass override lists at the call site).
- `SharedPreferences.setMockInitialValues` for preference tests; storage
  facades accept injected backends for mocking.

## 3. Golden tests

- Use Flutter's built-in `matchesGoldenFile` (golden_toolkit is
  discontinued - see `DECISIONS.md`).
- Goldens render with the Ahem test font and are **platform-specific**;
  regenerate with
  `flutter test --update-goldens test/widget/dashboard_golden_test.dart`.
- Commit goldens; treat golden changes as deliberate visual changes.

## 4. Coverage

- Run `flutter test --coverage` -> `coverage/lcov.info` (gitignored).
- Coverage is a signal, not the goal: prefer meaningful assertions over
  chasing 100%. Establish per-module coverage gates as CI lands.

## 5. Command summary

```bash
dart format --set-exit-if-changed lib test integration_test
flutter analyze
flutter test
flutter test --coverage
flutter build web
```

## 6. Reference

- `docs/mobile/08-Platform/09-Testing-Strategy.md`
- `mobile/docs/13-PackageDecisionLog.md` (mocktail, golden)
- `REVIEW_CHECKLIST.md`, `DEFINITION_OF_DONE.md`
