import 'package:connectivity_plus/connectivity_plus.dart';
import 'package:flutter_test/flutter_test.dart';
import 'package:sports_gurukul/core/connectivity/connectivity_service.dart';

void main() {
  ConnectivityService serviceFor(List<ConnectivityResult> results) =>
      ConnectivityService(
        check: () async => results,
        changes: Stream<List<ConnectivityResult>>.value(results),
      );

  group('ConnectivityService', () {
    test('classifies mobile, wifi, ethernet, vpn and satellite as online', () {
      for (final result in <ConnectivityResult>[
        ConnectivityResult.mobile,
        ConnectivityResult.wifi,
        ConnectivityResult.ethernet,
        ConnectivityResult.vpn,
        ConnectivityResult.satellite,
      ]) {
        expect(
          ConnectivityService.hasUsableConnection(<ConnectivityResult>[result]),
          isTrue,
        );
      }
    });

    test('classifies none, bluetooth and other as offline', () {
      for (final result in <ConnectivityResult>[
        ConnectivityResult.none,
        ConnectivityResult.bluetooth,
        ConnectivityResult.other,
      ]) {
        expect(
          ConnectivityService.hasUsableConnection(<ConnectivityResult>[result]),
          isFalse,
        );
      }
    });

    test('isOnline reflects any usable result in the list', () async {
      final service = serviceFor(<ConnectivityResult>[
        ConnectivityResult.none,
        ConnectivityResult.wifi,
      ]);
      expect(await service.isOnline, isTrue);
    });

    test('isOnline is false when every result is unusable', () async {
      final service = serviceFor(<ConnectivityResult>[
        ConnectivityResult.none,
        ConnectivityResult.bluetooth,
      ]);
      expect(await service.isOnline, isFalse);
    });

    test('emits online state from the connectivity stream', () async {
      final service = serviceFor(<ConnectivityResult>[
        ConnectivityResult.mobile,
      ]);
      await expectLater(service.onConnectivityChanged, emits(true));
    });
  });
}
