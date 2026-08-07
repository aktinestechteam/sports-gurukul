#!/usr/bin/env bash
# Sports Gurukul Mobile - quality gate.
# Formats, analyzes and runs the test suite. Fails on any violation.
set -euo pipefail

cd "$(dirname "$0")/.."

echo "==> Formatting check"
dart format --output=none --set-exit-if-changed lib test integration_test

echo "==> Static analysis"
flutter analyze

echo "==> Running tests"
flutter test

echo "==> Quality gate passed"
