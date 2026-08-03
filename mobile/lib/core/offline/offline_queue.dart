/// Offline-first queue scaffold.
///
/// P003 wiring only: no tables or queue operations exist yet. In P004 this
/// becomes a drift-backed outbox that persists mutations made while offline
/// and replays them once connectivity returns.
abstract final class OfflineQueue {
  /// Opens the queue for use. Currently a no-op; the queue is not backed by a
  /// table until the P004 outbox lands.
  static Future<void> open() async {}
}
