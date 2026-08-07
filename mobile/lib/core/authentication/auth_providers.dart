import 'package:flutter_riverpod/flutter_riverpod.dart';

import 'package:sports_gurukul/core/authentication/session_events.dart';
import 'package:sports_gurukul/core/authentication/token_store.dart';
import 'package:sports_gurukul/core/storage/storage_providers.dart';

/// Provides the shared JWT token store backed by secure storage.
final tokenStoreProvider = Provider<TokenStore>(
  (ref) => TokenStore(storage: ref.watch(secureStorageProvider)),
);

/// Provides the broadcast channel for session-expiry notifications.
final sessionEventsProvider = Provider<SessionEvents>((_) => SessionEvents());
