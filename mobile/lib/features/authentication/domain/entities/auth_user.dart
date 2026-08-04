import 'package:flutter/foundation.dart';

/// An authenticated user's public identity.
///
/// Derived from the backend `AuthResponse`/`LoginResponse` contract: user id,
/// email, full name and assigned roles. Roles drive role-based navigation
/// later; the domain entity is intentionally free of tokens.
@immutable
class AuthUser {
  const AuthUser({
    required this.id,
    required this.email,
    required this.fullName,
    required this.roles,
  });

  /// Backend user id (`Guid` serialized as a string).
  final String id;

  /// The user's email address (used as the login identifier).
  final String email;

  /// The user's display name.
  final String fullName;

  /// Roles assigned to the user (e.g. `Athlete`, `Coach`).
  final List<String> roles;

  @override
  bool operator ==(Object other) =>
      other is AuthUser &&
      other.id == id &&
      other.email == email &&
      other.fullName == fullName &&
      _listEquals(other.roles, roles);

  @override
  int get hashCode => Object.hash(id, email, fullName, Object.hashAll(roles));

  @override
  String toString() =>
      'AuthUser(id: $id, email: $email, fullName: $fullName, roles: $roles)';
}

bool _listEquals(List<String> a, List<String> b) =>
    a.length == b.length &&
    a.every((value) => b.contains(value)) &&
    b.every((value) => a.contains(value));
