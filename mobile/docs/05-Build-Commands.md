# Build Commands

Status: **Approved baseline** · Sprint 0 (P001)

All commands run from the `mobile/` directory.

## Dependencies & Localization

```bash
flutter pub get          # Resolve dependencies
flutter gen-l10n         # Regenerate localizations from ARB files
```

## Development Run

```bash
flutter run                          # Run on the selected device
flutter run -d chrome                # Web (debug)
flutter run -d windows               # Windows desktop (debug)
flutter run --release                # Release mode
```

## Verification

```bash
dart format .                        # Format all Dart code
dart format --output=none --set-exit-if-changed lib test integration_test
                                     # Format check (CI)
flutter analyze                      # Static analysis (zero warnings target)
flutter test                         # Unit + widget tests
flutter test test/unit               # Unit tests only
flutter test test/widget             # Widget tests only
flutter test integration_test        # Device smoke test (needs device/emulator)
```

Quality gate in one shot (bash on macOS/Linux/Git-Bash):

```bash
./scripts/check.sh
```

## Builds

```bash
flutter build apk --release          # Android APK
flutter build appbundle --release    # Android App Bundle (Play Store)
flutter build ios --release          # iOS (macOS + Xcode required)
flutter build web                    # Web build
flutter build windows                # Windows desktop build
```

Android build artifacts:
`build/app/outputs/flutter-apk/app-release.apk`

## Tests on a Specific Device

```bash
flutter devices                      # List available devices
flutter test integration_test -d <device-id>
```

## Clean

```bash
flutter clean                        # Remove build artifacts
flutter pub get                      # Re-resolve after clean
flutter gen-l10n                     # Regenerate localizations
```

## Notes

- `flutter gen-l10n` runs automatically during builds because
  `flutter: generate: true` is set in `pubspec.yaml`.
- The integration smoke test launches the real app and asserts the
  placeholder dashboard renders; it requires a device or emulator.
