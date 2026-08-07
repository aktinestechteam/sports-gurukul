import 'package:flutter/material.dart';
import 'package:go_router/go_router.dart';

import 'package:sports_gurukul/app/router/route_paths.dart';
import 'package:sports_gurukul/app/theme/colors/app_gradients.dart';
import 'package:sports_gurukul/features/onboarding/presentation/widgets/academy_flow_placeholder.dart';
import 'package:sports_gurukul/l10n/generated/app_localizations.dart';

/// Navigation placeholder for the Join Academy flow.
///
/// The full join-academy experience lands in a later sprint; this page only
/// wires the route so the onboarding navigation has a real target.
class JoinAcademyPage extends StatelessWidget {
  const JoinAcademyPage({super.key});

  @override
  Widget build(BuildContext context) {
    final l10n = AppLocalizations.of(context);
    return AcademyFlowPlaceholder(
      icon: Icons.groups_rounded,
      gradient: AppGradients.emerald,
      title: l10n.academyJoinTitle,
      message: l10n.academyJoinMessage,
      onBack: () => context.go(RoutePaths.dashboard),
    );
  }
}
