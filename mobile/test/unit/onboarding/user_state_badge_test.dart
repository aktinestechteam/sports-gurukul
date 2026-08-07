import 'package:flutter_test/flutter_test.dart';
import 'package:sports_gurukul/features/onboarding/domain/entities/user_state.dart';
import 'package:sports_gurukul/features/onboarding/presentation/widgets/user_state_badge.dart';
import 'package:sports_gurukul/l10n/generated/app_localizations_en.dart';

import '../../helpers/onboarding_test_helper.dart';

/// Verifies the reusable state-driven role badge resolver.
///
/// The badge must be driven by the resolved [UserState], never by the raw
/// backend role: a brand-new account (default `Athlete` registration role)
/// must be labelled "New User", not "Athlete".
void main() {
  final l10n = AppLocalizationsEn();

  test('a brand-new user shows New User, never Athlete', () {
    final badge = resolveUserStateBadge(l10n, testNewUserSession());
    expect(badge.label, 'New User');
    expect(badge.role, isNull);
  });

  test('an academy admin shows Academy Admin with the academy tint', () {
    final badge = resolveUserStateBadge(l10n, testAcademyAdminSession());
    expect(badge.label, 'Academy Admin');
    expect(badge.role, isNotNull);
  });

  test('a coach shows Coach with the coach tint', () {
    final badge = resolveUserStateBadge(l10n, testMemberSession());
    expect(badge.label, 'Coach');
    expect(badge.role, isNotNull);
  });

  test('an academy-assigned athlete shows Athlete', () {
    final badge = resolveUserStateBadge(l10n, testAthleteMemberSession());
    expect(badge.label, 'Athlete');
    expect(badge.role, isNotNull);
  });

  test('a pending membership shows Pending Approval with a neutral tint', () {
    final badge = resolveUserStateBadge(l10n, testPendingApprovalSession());
    expect(badge.label, 'Pending Approval');
    expect(badge.role, isNull);
  });
}
