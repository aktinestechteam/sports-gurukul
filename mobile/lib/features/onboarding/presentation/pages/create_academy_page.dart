import 'package:flutter/material.dart';
import 'package:go_router/go_router.dart';

import 'package:sports_gurukul/app/router/route_paths.dart';
import 'package:sports_gurukul/app/theme/colors/app_gradients.dart';
import 'package:sports_gurukul/features/onboarding/presentation/widgets/academy_flow_placeholder.dart';
import 'package:sports_gurukul/l10n/generated/app_localizations.dart';

/// Navigation placeholder for the Create Academy flow.
///
/// The full create-academy experience lands in a later sprint; this page only
/// wires the route so the onboarding navigation has a real target.
class CreateAcademyPage extends StatelessWidget {
  const CreateAcademyPage({super.key});

  @override
  Widget build(BuildContext context) {
    final l10n = AppLocalizations.of(context);
    return AcademyFlowPlaceholder(
      icon: Icons.school_rounded,
      gradient: AppGradients.ocean,
      title: l10n.academyCreateTitle,
      message: l10n.academyCreateMessage,
      onBack: () => context.go(RoutePaths.dashboard),
    );
  }
}
