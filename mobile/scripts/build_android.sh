#!/usr/bin/env bash
# Sports Gurukul Mobile - Android release build.
set -euo pipefail

cd "$(dirname "$0")/.."

echo "==> Building Android release APK"
flutter build apk --release

echo "==> Build complete: build/app/outputs/flutter-apk/app-release.apk"
