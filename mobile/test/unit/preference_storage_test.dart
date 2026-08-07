import 'package:flutter_test/flutter_test.dart';
import 'package:shared_preferences/shared_preferences.dart';
import 'package:sports_gurukul/core/storage/preference_storage.dart';

void main() {
  TestWidgetsFlutterBinding.ensureInitialized();

  final storage = PreferenceStorage();

  setUp(() {
    SharedPreferences.setMockInitialValues(<String, Object>{});
  });

  group('PreferenceStorage', () {
    test('round-trips all supported value types', () async {
      await storage.writeString('name', 'Rohit');
      await storage.writeInt('age', 21);
      await storage.writeBool('active', value: true);
      await storage.writeDouble('score', 99.5);

      expect(await storage.readString('name'), 'Rohit');
      expect(await storage.readInt('age'), 21);
      expect(await storage.readBool('active'), isTrue);
      expect(await storage.readDouble('score'), 99.5);
    });

    test('returns null for missing keys', () async {
      expect(await storage.readString('missing'), isNull);
      expect(await storage.readInt('missing'), isNull);
      expect(await storage.readBool('missing'), isNull);
      expect(await storage.readDouble('missing'), isNull);
    });

    test('delete removes a key and clear empties the store', () async {
      await storage.writeString('name', 'Rohit');
      await storage.delete('name');
      expect(await storage.readString('name'), isNull);

      await storage.writeString('other', 'x');
      await storage.clear();
      expect(await storage.readString('other'), isNull);
    });
  });
}
