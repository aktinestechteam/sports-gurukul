import 'package:go_router/go_router.dart' show GoRouter;

/// Navigation service contract for imperative navigation needs.
///
/// Most navigation is declarative through the [GoRouter] configuration in
/// `app/router/app_router.dart`. A service is provided for the rare cases
/// that require imperative navigation (deep-link handling, auth-driven
/// redirects, logout). Implemented when authentication lands.
///
/// Reference: docs/mobile/09-Implementation/07-Navigation.md
// ignore: one_member_abstracts - placeholder contract, gains members in P004
abstract class NavigationService {
  /// Resolves a deep-link URI to a route path, or null when unsupported.
  String? resolveDeepLink(Uri uri);
}
