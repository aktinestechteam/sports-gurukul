/// Roles the backend assigns to users (the `RoleType` domain enum).
///
/// Backend values are PascalCase strings (e.g. `SuperAdmin`, `Athlete`);
/// [UserRole.fromName] normalizes the common spellings. Registration creates
/// every account with the default `Athlete` role
/// ([UserRole.defaultRegistrationRoleName]), so a role alone never proves
/// academy membership (see `UserState`).
enum UserRole {
  superAdmin,
  admin,
  academy,
  coach,
  athlete,
  parent,
  scout,
  sponsor,
  aiAdministrator;

  /// Name of the role auto-assigned by the backend at registration.
  static const String defaultRegistrationRoleName = 'Athlete';

  /// Parses [name] into a [UserRole], or `null` when the value is unknown.
  static UserRole? fromName(String name) => switch (name.trim().toLowerCase()) {
    'superadmin' || 'super-admin' || 'super_admin' => UserRole.superAdmin,
    'admin' => UserRole.admin,
    'academy' => UserRole.academy,
    'coach' => UserRole.coach,
    'athlete' => UserRole.athlete,
    'parent' => UserRole.parent,
    'scout' => UserRole.scout,
    'sponsor' => UserRole.sponsor,
    'aiadministrator' ||
    'ai-admin' ||
    'ai_administrator' => UserRole.aiAdministrator,
    _ => null,
  };

  /// Whether this role can administer the whole platform.
  bool get isPlatformAdministrator =>
      this == UserRole.superAdmin || this == UserRole.admin;

  /// Whether this is the role assigned by default on registration.
  bool get isDefaultRegistrationRole => this == UserRole.athlete;
}
