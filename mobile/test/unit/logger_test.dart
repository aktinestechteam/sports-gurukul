import 'package:flutter_test/flutter_test.dart';
import 'package:sports_gurukul/core/logging/app_logger.dart';
import 'package:sports_gurukul/core/logging/debug_logger.dart';
import 'package:sports_gurukul/core/logging/log_level.dart';
import 'package:sports_gurukul/core/logging/logger.dart';
import 'package:sports_gurukul/core/logging/release_logger.dart';

class RecordingLogger extends Logger {
  final List<LogLevel> levels = [];
  final List<Object?> messages = [];

  @override
  bool isEnabled(LogLevel level) => true;

  @override
  void log(
    LogLevel level,
    Object? message, {
    Object? error,
    StackTrace? stackTrace,
  }) {
    levels.add(level);
    messages.add(message);
  }
}

void main() {
  group('LogLevel', () {
    test('orders levels by severity', () {
      expect(LogLevel.trace.priority, lessThan(LogLevel.debug.priority));
      expect(LogLevel.warning.priority, greaterThan(LogLevel.info.priority));
      expect(LogLevel.fatal.priority, greaterThan(LogLevel.error.priority));
    });

    test('isAtLeast compares thresholds', () {
      expect(LogLevel.warning.isAtLeast(LogLevel.warning), isTrue);
      expect(LogLevel.info.isAtLeast(LogLevel.warning), isFalse);
      expect(LogLevel.error.isAtLeast(LogLevel.info), isTrue);
    });
  });

  group('DebugLogger', () {
    test('enables every level', () {
      final logger = DebugLogger();
      for (final level in LogLevel.values) {
        expect(logger.isEnabled(level), isTrue);
      }
    });

    test('emits without throwing for every level', () {
      DebugLogger()
        ..trace('t')
        ..debug('d')
        ..info('i')
        ..warning('w')
        ..error('e')
        ..fatal('f');
    });
  });

  group('ReleaseLogger', () {
    test('filters verbose levels', () {
      final logger = ReleaseLogger();
      expect(logger.isEnabled(LogLevel.debug), isFalse);
      expect(logger.isEnabled(LogLevel.info), isFalse);
      expect(logger.isEnabled(LogLevel.warning), isTrue);
      expect(logger.isEnabled(LogLevel.error), isTrue);
      expect(logger.isEnabled(LogLevel.fatal), isTrue);
    });

    test('emits warning and above without throwing', () {
      ReleaseLogger()
        ..warning('w')
        ..error('e')
        ..fatal('f');
    });
  });

  group('AppLogger', () {
    final recording = RecordingLogger();

    setUp(() {
      AppLogger.configure(recording);
    });

    tearDown(() {
      AppLogger.configure(DebugLogger());
    });

    test('routes every level through the configured logger', () {
      AppLogger.t('t');
      AppLogger.d('d');
      AppLogger.i('i');
      AppLogger.w('w');
      AppLogger.e('e');
      AppLogger.f('f');

      expect(recording.messages, ['t', 'd', 'i', 'w', 'e', 'f']);
      expect(recording.levels, [
        LogLevel.trace,
        LogLevel.debug,
        LogLevel.info,
        LogLevel.warning,
        LogLevel.error,
        LogLevel.fatal,
      ]);
    });
  });
}
