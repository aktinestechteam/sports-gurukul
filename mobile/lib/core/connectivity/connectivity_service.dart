import 'package:connectivity_plus/connectivity_plus.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';

/// Provides the shared connectivity monitor.
final connectivityServiceProvider = Provider<ConnectivityService>(
  (_) => ConnectivityService(),
);

/// Observes the active network connectivity.
///
/// Wraps `connectivity_plus` so call sites never depend on the concrete
/// package and the offline/online classification lives in one place.
class ConnectivityService {
  ConnectivityService({
    Future<List<ConnectivityResult>> Function()? check,
    Stream<List<ConnectivityResult>>? changes,
  }) : _check = check ?? _defaultCheck,
       _onConnectivityChanged = changes ?? _defaultConnectivityChanged();

  static Future<List<ConnectivityResult>> _defaultCheck() =>
      Connectivity().checkConnectivity();

  static Stream<List<ConnectivityResult>> _defaultConnectivityChanged() =>
      Connectivity().onConnectivityChanged;

  final Future<List<ConnectivityResult>> Function() _check;
  final Stream<List<ConnectivityResult>> _onConnectivityChanged;

  /// Stream of online/offline transitions.
  ///
  /// Emits `true` when the device has any usable connection and `false`
  /// otherwise.
  Stream<bool> get onConnectivityChanged =>
      _onConnectivityChanged.map(hasUsableConnection);

  /// Whether the device currently has a usable connection.
  Future<bool> get isOnline async {
    final results = await _check();
    return hasUsableConnection(results);
  }

  /// Whether any of the [results] represents a usable connection.
  static bool hasUsableConnection(List<ConnectivityResult> results) =>
      results.any(_isUsableResult);

  static bool _isUsableResult(ConnectivityResult result) => switch (result) {
    ConnectivityResult.mobile ||
    ConnectivityResult.wifi ||
    ConnectivityResult.ethernet ||
    ConnectivityResult.vpn ||
    ConnectivityResult.satellite => true,
    ConnectivityResult.bluetooth ||
    ConnectivityResult.none ||
    ConnectivityResult.other => false,
  };
}
