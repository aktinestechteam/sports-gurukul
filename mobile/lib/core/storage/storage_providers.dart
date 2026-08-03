import 'package:flutter_riverpod/flutter_riverpod.dart';

import 'package:sports_gurukul/core/storage/preference_storage.dart';
import 'package:sports_gurukul/core/storage/secure_storage.dart';

/// Provides the secure key-value store for tokens and secrets.
final secureStorageProvider = Provider<SecureStorage>((_) => SecureStorage());

/// Provides the shared-preferences store for non-sensitive settings.
final preferenceStorageProvider = Provider<PreferenceStorage>(
  (_) => PreferenceStorage(),
);
