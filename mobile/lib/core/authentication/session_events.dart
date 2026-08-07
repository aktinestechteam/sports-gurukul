import 'dart:async';

/// Broadcast channel for session lifecycle events.
///
/// Emits an event when the stored session becomes invalid outside the auth
/// flow (for example when the `AuthInterceptor` cannot refresh an expired
/// access token). The auth controller listens to this stream and transitions
/// to the unauthenticated state; widgets never subscribe directly.
class SessionEvents {
  final StreamController<void> _controller = StreamController<void>.broadcast();

  /// Stream of session-expiry notifications.
  Stream<void> get onSessionExpired => _controller.stream;

  /// Signals that the current session has expired and must be discarded.
  void expireSession() {
    if (!_controller.isClosed) {
      _controller.add(null);
    }
  }

  /// Releases the underlying stream controller.
  void dispose() => _controller.close();
}
