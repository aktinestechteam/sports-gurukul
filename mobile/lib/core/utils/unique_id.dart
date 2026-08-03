import 'package:uuid/uuid.dart';

/// Generates unique identifiers.
///
/// Wraps `package:uuid` so call sites never depend on the concrete package.
/// v4 identifiers are used for correlation IDs, offline mutations and other
/// records that must be unique without a server round-trip.
class UniqueId {
  UniqueId({Uuid? uuid}) : _uuid = uuid ?? const Uuid();

  final Uuid _uuid;

  /// Returns a random (version 4) unique identifier.
  String v4() => _uuid.v4();
}
