import 'package:flutter/widgets.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';

import 'package:sports_gurukul/app/app.dart';
import 'package:sports_gurukul/app/app_initializer.dart';
import 'package:sports_gurukul/app/dependency_container.dart'
    show DependencyContainer;

/// Application entry point invoked by `main()`.
///
/// Runs the ordered startup initialization ([AppInitializer]), then mounts
/// the root widget inside the application-wide [ProviderScope]. Test and
/// environment overrides are applied to this scope as providers land
/// (see [DependencyContainer]).
abstract final class AppBootstrap {
  static Future<void> run() async {
    WidgetsFlutterBinding.ensureInitialized();
    await AppInitializer.initialize();
    runApp(
      const ProviderScope(
        child: SportsGurukulApp(),
      ),
    );
  }
}
