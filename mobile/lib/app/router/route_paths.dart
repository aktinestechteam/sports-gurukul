import 'package:sports_gurukul/core/constants/route_constants.dart';

/// Route paths for the Sports Gurukul application.
///
/// Type alias over the core [RouteConstants] so feature and router code can
/// keep using the shorter `RoutePaths` name while paths stay centralized in
/// `core/constants`. Feature routes are registered by their owning feature.
typedef RoutePaths = RouteConstants;
