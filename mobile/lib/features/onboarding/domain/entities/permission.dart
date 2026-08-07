import 'package:sports_gurukul/features/onboarding/domain/entities/user_role.dart';

/// Capabilities granted to an authenticated user.
///
/// Permissions are derived locally from the resolved [UserRole]; the backend
/// authorizes requests server-side, this set only drives UI affordances
/// (visible actions and route choices).
enum Permission {
  /// Reach the application dashboard.
  viewDashboard,

  /// View and edit the user's own profile.
  manageOwnProfile,

  /// Browse coaches, academies and tournaments.
  exploreApplication,

  /// Start the "Create My Academy" flow.
  createAcademy,

  /// Start the "Join Existing Academy" flow.
  joinAcademy,

  /// Administer an academy and its members.
  manageAcademy,

  /// Schedule and manage training sessions.
  manageSessions,

  /// Manage academy athletes.
  manageAthletes,

  /// Manage academy coaches.
  manageCoaches,

  /// Create and manage bookings.
  manageBookings,

  /// Manage tournaments and registrations.
  manageTournaments,

  /// Process payments and subscriptions.
  managePayments,

  /// View analytics and statistics.
  viewAnalytics,
}

/// Permissions every authenticated user holds.
const Set<Permission> basePermissions = <Permission>{
  Permission.viewDashboard,
  Permission.manageOwnProfile,
  Permission.exploreApplication,
};

/// The permissions granted to [role] (`null` = unknown role).
Set<Permission> permissionsForRole(UserRole? role) {
  return switch (role) {
    UserRole.superAdmin || UserRole.admin || UserRole.academy => <Permission>{
      ...basePermissions,
      Permission.createAcademy,
      Permission.joinAcademy,
      Permission.manageAcademy,
      Permission.manageSessions,
      Permission.manageAthletes,
      Permission.manageCoaches,
      Permission.manageBookings,
      Permission.manageTournaments,
      Permission.managePayments,
      Permission.viewAnalytics,
    },
    UserRole.coach => <Permission>{
      ...basePermissions,
      Permission.manageSessions,
      Permission.viewAnalytics,
    },
    UserRole.athlete || UserRole.parent => <Permission>{
      ...basePermissions,
      Permission.joinAcademy,
      Permission.manageBookings,
    },
    UserRole.scout => <Permission>{
      ...basePermissions,
      Permission.joinAcademy,
      Permission.viewAnalytics,
    },
    UserRole.sponsor => <Permission>{
      ...basePermissions,
      Permission.managePayments,
      Permission.viewAnalytics,
    },
    UserRole.aiAdministrator => <Permission>{
      ...basePermissions,
      Permission.viewAnalytics,
    },
    null => basePermissions,
  };
}
