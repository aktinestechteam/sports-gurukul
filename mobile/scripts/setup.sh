#!/usr/bin/env bash
# Sports Gurukul Mobile - project setup.
# Fetches dependencies and generates localizations.
set -euo pipefail

cd "$(dirname "$0")/.."

echo "==> Fetching dependencies"
flutter pub get

echo "==> Generating localizations"
flutter gen-l10n

echo "==> Setup complete"
