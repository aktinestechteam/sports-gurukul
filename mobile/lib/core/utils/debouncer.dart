import 'dart:async';

import 'package:flutter/foundation.dart';
import 'package:sports_gurukul/core/constants/duration_constants.dart';

/// Delays an action until input has been quiet for [duration].
///
/// Each [schedule] call cancels any pending call. Use it for search
/// fields, form validation and other rapid-fire events to avoid doing work
/// on every keystroke. Not thread-safe, but safe to use from a single
/// isolate / UI thread.
class Debouncer {
  Debouncer({this.duration = DurationConstants.debounceDefault});

  /// Quiet window before the action runs.
  final Duration duration;

  Timer? _timer;
  VoidCallback? _action;

  /// Whether an action is currently scheduled.
  bool get isPending => _timer?.isActive ?? false;

  /// Schedules [action] to run after [duration] of inactivity.
  void schedule(VoidCallback action) {
    _timer?.cancel();
    _action = action;
    _timer = Timer(duration, () {
      _action = null;
      action();
    });
  }

  /// Runs the pending action immediately, if any.
  void flush() {
    final action = _action;
    if (action == null) {
      return;
    }
    _timer?.cancel();
    _timer = null;
    _action = null;
    action();
  }

  /// Cancels the pending action, if any.
  void cancel() {
    _timer?.cancel();
    _timer = null;
    _action = null;
  }
}
