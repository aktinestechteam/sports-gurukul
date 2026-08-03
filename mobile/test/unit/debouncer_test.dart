import 'package:flutter_test/flutter_test.dart';
import 'package:sports_gurukul/core/utils/debouncer.dart';

void main() {
  group('Debouncer', () {
    testWidgets('coalesces rapid schedules into one call', (tester) async {
      final debouncer = Debouncer();
      var calls = 0;

      debouncer
        ..schedule(() => calls++)
        ..schedule(() => calls++)
        ..schedule(() => calls++);

      expect(calls, 0);
      expect(debouncer.isPending, isTrue);

      await tester.pump(const Duration(milliseconds: 299));
      expect(calls, 0);

      await tester.pump(const Duration(milliseconds: 1));
      expect(calls, 1);
      expect(debouncer.isPending, isFalse);
    });

    testWidgets('cancel drops the pending action', (tester) async {
      final debouncer = Debouncer();
      var calls = 0;

      debouncer
        ..schedule(() => calls++)
        ..cancel();

      expect(debouncer.isPending, isFalse);
      await tester.pump(const Duration(seconds: 1));
      expect(calls, 0);
    });

    testWidgets('flush runs the pending action immediately', (tester) async {
      final debouncer = Debouncer();
      var calls = 0;

      debouncer
        ..schedule(() => calls++)
        ..flush();

      expect(calls, 1);
      expect(debouncer.isPending, isFalse);

      await tester.pump(const Duration(seconds: 1));
      expect(calls, 1);
    });

    testWidgets('flush is a no-op when nothing is pending', (tester) async {
      final debouncer = Debouncer()..flush();
      expect(debouncer.isPending, isFalse);
    });
  });
}
