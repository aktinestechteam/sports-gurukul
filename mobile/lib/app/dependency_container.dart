import 'package:flutter_riverpod/flutter_riverpod.dart' show ProviderScope;

/// Composition root for the application's dependency graph.
///
/// Riverpod providers are the dependency container - there is no global
/// service locator. Providers live in their owning module (theme, router,
/// and later repositories/datasources) and are composed here for the
/// production [ProviderScope] and for test overrides.
///
/// Sprint P002: placeholder. Concrete providers are registered inline at
/// their declaration sites and composed in `app/bootstrap.dart`. Note:
/// Riverpod 3 keeps the `Override` type internal, so override lists are
/// passed to `ProviderScope.overrides` at the call site rather than being
/// typed here.
abstract final class DependencyContainer {
  const DependencyContainer();
}
