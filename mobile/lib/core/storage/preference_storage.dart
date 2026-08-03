import 'package:shared_preferences/shared_preferences.dart';

/// Plain key-value storage for non-sensitive preferences.
///
/// Holds settings such as theme, language and onboarding state. Secrets
/// (tokens, credentials) must go through `SecureStorage` instead - the two
/// never share a key namespace.
class PreferenceStorage {
  /// Reads a value by [key], or `null` when absent.
  Future<String?> readString(String key) async =>
      (await _prefs()).getString(key);

  /// Writes [value] under [key].
  Future<void> writeString(String key, String value) async {
    await (await _prefs()).setString(key, value);
  }

  /// Reads an integer by [key], or `null` when absent.
  Future<int?> readInt(String key) async => (await _prefs()).getInt(key);

  /// Writes [value] under [key].
  Future<void> writeInt(String key, int value) async {
    await (await _prefs()).setInt(key, value);
  }

  /// Reads a boolean by [key], or `null` when absent.
  Future<bool?> readBool(String key) async => (await _prefs()).getBool(key);

  /// Writes [value] under [key].
  Future<void> writeBool(String key, {required bool value}) async {
    await (await _prefs()).setBool(key, value);
  }

  /// Reads a double by [key], or `null` when absent.
  Future<double?> readDouble(String key) async =>
      (await _prefs()).getDouble(key);

  /// Writes [value] under [key].
  Future<void> writeDouble(String key, double value) async {
    await (await _prefs()).setDouble(key, value);
  }

  /// Removes the entry stored under [key].
  Future<void> delete(String key) async {
    await (await _prefs()).remove(key);
  }

  /// Removes every entry held in shared preferences.
  Future<void> clear() async {
    await (await _prefs()).clear();
  }

  Future<SharedPreferences> _prefs() => SharedPreferences.getInstance();
}
