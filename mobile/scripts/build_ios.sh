#!/usr/bin/env bash
# Sports Gurukul Mobile - iOS release build.
# Requires macOS with Xcode. Skipped automatically on other platforms.
set -euo pipefail

cd "$(dirname "$0")/.."

if [[ "$(uname -s)" != "Darwin" ]]; then
  echo "iOS builds require macOS with Xcode. Skipping."
  exit 0
fi

echo "==> Building iOS release"
flutter build ios --release --no-codesign

echo "==> Build complete"
