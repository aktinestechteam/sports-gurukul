import 'dart:ui';

import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';

import 'package:sports_gurukul/app/theme/app_shadow.dart';
import 'package:sports_gurukul/app/theme/colors/app_colors.dart';
import 'package:sports_gurukul/app/theme/colors/app_gradients.dart';
import 'package:sports_gurukul/app/theme/spacing/app_spacing.dart';
import 'package:sports_gurukul/features/authentication/presentation/providers/auth_controller.dart';
import 'package:sports_gurukul/l10n/generated/app_localizations.dart';
import 'package:sports_gurukul/shared/animations/entrance.dart';
import 'package:sports_gurukul/shared/animations/spring_press.dart';
import 'package:sports_gurukul/shared/cards/glass_card.dart';
import 'package:sports_gurukul/shared/cards/gradient_card.dart';
import 'package:sports_gurukul/shared/charts/gradient_progress.dart';
import 'package:sports_gurukul/shared/layouts/aurora_background.dart';
import 'package:sports_gurukul/shared/widgets/animated_tab_bar.dart';

/// Vibrant home dashboard built on the glass/gradient design language.
///
/// Composes [AuroraBackground] with [GlassCard] stats, [GradientCard] quick
/// actions and an [AnimatedTabBar] switching content through an
/// [AnimatedSwitcher]. All radii, gradients, shadows and colors come from the
/// design tokens in `app/theme`.
class DashboardPage extends ConsumerWidget {
  const DashboardPage({super.key});

  @override
  Widget build(BuildContext context, WidgetRef ref) {
    final l10n = AppLocalizations.of(context);
    final state = ref.watch(authControllerProvider);
    final session = state is AuthAuthenticated ? state.session : null;
    final name = session?.user.fullName.trim() ?? '';
    final firstName = name.isEmpty ? '' : name.split(' ').first;

    return Scaffold(
      backgroundColor: Colors.transparent,
      body: AuroraBackground(
        child: SafeArea(
          bottom: false,
          child: RepaintBoundary(
            child: SingleChildScrollView(
              physics: const BouncingScrollPhysics(),
              padding: const EdgeInsets.fromLTRB(
                AppSpacing.xl,
                AppSpacing.md,
                AppSpacing.xl,
                AppSpacing.xxxl,
              ),
              child: Column(
                crossAxisAlignment: CrossAxisAlignment.start,
                children: <Widget>[
                  _DashboardHeader(
                    greeting: _greeting(l10n, firstName),
                    name: name,
                    onLogout: () =>
                        ref.read(authControllerProvider.notifier).logout(),
                  ),
                  const SizedBox(height: AppSpacing.xxl),
                  Entrance(
                    delay: const Duration(milliseconds: 100),
                    child: _StatsRow(l10n: l10n),
                  ),
                  const SizedBox(height: AppSpacing.xxxl),
                  Entrance(
                    delay: const Duration(milliseconds: 200),
                    child: _QuickActions(l10n: l10n),
                  ),
                  const SizedBox(height: AppSpacing.xxxl),
                  Entrance(
                    delay: const Duration(milliseconds: 300),
                    child: _DashboardTabs(l10n: l10n),
                  ),
                ],
              ),
            ),
          ),
        ),
      ),
    );
  }
}

String _greeting(AppLocalizations l10n, String name) {
  final hour = DateTime.now().hour;
  if (hour < 12) return l10n.dashboardGreetingMorning(name);
  if (hour < 17) return l10n.dashboardGreetingAfternoon(name);
  return l10n.dashboardGreetingEvening(name);
}

/// Greeting row with the user avatar and logout affordance.
class _DashboardHeader extends StatelessWidget {
  const _DashboardHeader({
    required this.greeting,
    required this.name,
    required this.onLogout,
  });

  final String greeting;
  final String name;
  final VoidCallback onLogout;

  @override
  Widget build(BuildContext context) {
    final initials = _initials(name);
    return Row(
      children: <Widget>[
        Expanded(
          child: Column(
            crossAxisAlignment: CrossAxisAlignment.start,
            children: <Widget>[
              Text(
                greeting,
                maxLines: 1,
                overflow: TextOverflow.ellipsis,
                style: Theme.of(context).textTheme.headlineSmall?.copyWith(
                  color: AppColors.surface,
                  fontWeight: FontWeight.w800,
                ),
              ),
              const SizedBox(height: AppSpacing.xs),
              Text(
                AppLocalizations.of(context).dashboardHeaderSubtitle,
                maxLines: 1,
                overflow: TextOverflow.ellipsis,
                style: Theme.of(context).textTheme.bodyMedium?.copyWith(
                  color: AppColors.grey300,
                ),
              ),
            ],
          ),
        ),
        _GlassCircle(
          size: 44,
          child: Text(
            initials,
            style: Theme.of(context).textTheme.titleSmall?.copyWith(
              color: AppColors.surface,
              fontWeight: FontWeight.w800,
            ),
          ),
        ),
        const SizedBox(width: AppSpacing.sm),
        _GlassCircle(
          size: 44,
          onTap: onLogout,
          tooltip: AppLocalizations.of(context).authLogout,
          child: const Icon(
            Icons.logout_rounded,
            color: AppColors.surface,
            size: 20,
          ),
        ),
      ],
    );
  }

  static String _initials(String name) {
    final parts = name.split(RegExp(r'\s+')).where((p) => p.isNotEmpty);
    final letters = parts.map((p) => p[0].toUpperCase());
    return letters.take(2).join();
  }
}

/// Circular glass surface; optionally tappable with a tooltip.
class _GlassCircle extends StatelessWidget {
  const _GlassCircle({
    required this.size,
    required this.child,
    this.onTap,
    this.tooltip,
  });

  final double size;
  final Widget child;
  final VoidCallback? onTap;
  final String? tooltip;

  @override
  Widget build(BuildContext context) {
    final circle = ClipOval(
      child: BackdropFilter(
        filter: ImageFilter.blur(sigmaX: 10, sigmaY: 10),
        child: Container(
          width: size,
          height: size,
          decoration: const BoxDecoration(
            shape: BoxShape.circle,
            color: AppColors.glassFill,
            border: Border.fromBorderSide(
              BorderSide(color: AppColors.glassBorderLo),
            ),
          ),
          alignment: Alignment.center,
          child: child,
        ),
      ),
    );
    if (onTap == null) return circle;
    return Tooltip(
      message: tooltip ?? '',
      child: SpringPress(
        onPressed: onTap,
        scaleDown: 0.9,
        child: circle,
      ),
    );
  }
}

/// Row of three glass stat cards, each with a gradient progress meter.
class _StatsRow extends StatelessWidget {
  const _StatsRow({required this.l10n});

  final AppLocalizations l10n;

  @override
  Widget build(BuildContext context) {
    return Row(
      children: <Widget>[
        Expanded(
          child: _StatCard(
            label: l10n.dashboardStatActiveDays,
            value: '18/28',
            progress: 0.64,
            gradient: AppGradients.emerald,
          ),
        ),
        const SizedBox(width: AppSpacing.md),
        Expanded(
          child: _StatCard(
            label: l10n.dashboardStatAvgIntensity,
            value: '82%',
            progress: 0.82,
            gradient: AppGradients.sunset,
          ),
        ),
        const SizedBox(width: AppSpacing.md),
        Expanded(
          child: _StatCard(
            label: l10n.dashboardStatRecovery,
            value: '74%',
            progress: 0.74,
            gradient: AppGradients.aurora,
          ),
        ),
      ],
    );
  }
}

class _StatCard extends StatelessWidget {
  const _StatCard({
    required this.label,
    required this.value,
    required this.progress,
    required this.gradient,
  });

  final String label;
  final String value;
  final double progress;
  final LinearGradient gradient;

  @override
  Widget build(BuildContext context) {
    return GlassCard(
      padding: const EdgeInsets.all(AppSpacing.lg),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: <Widget>[
          Text(
            label,
            maxLines: 1,
            overflow: TextOverflow.ellipsis,
            style: Theme.of(context).textTheme.bodySmall?.copyWith(
              color: AppColors.grey300,
              fontWeight: FontWeight.w600,
            ),
          ),
          const SizedBox(height: AppSpacing.sm),
          Text(
            value,
            style: Theme.of(context).textTheme.headlineSmall?.copyWith(
              color: AppColors.surface,
              fontWeight: FontWeight.w800,
            ),
          ),
          const SizedBox(height: AppSpacing.md),
          GradientProgress(
            value: progress,
            gradient: gradient,
            height: 8,
            shadows: const <BoxShadow>[
              BoxShadow(
                color: Color(0x44FFFFFF),
                blurRadius: 10,
                offset: Offset(0, 3),
              ),
            ],
          ),
        ],
      ),
    );
  }
}

/// Two-by-two grid of gradient quick-action cards.
class _QuickActions extends StatelessWidget {
  const _QuickActions({required this.l10n});

  final AppLocalizations l10n;

  @override
  Widget build(BuildContext context) {
    return Column(
      crossAxisAlignment: CrossAxisAlignment.start,
      children: <Widget>[
        _SectionTitle(l10n.dashboardQuickActions),
        const SizedBox(height: AppSpacing.lg),
        Row(
          children: <Widget>[
            Expanded(
              child: GradientCard(
                title: l10n.dashboardActionBookSession,
                icon: Icons.event_available_rounded,
                gradient: AppGradients.ocean,
                shadows: AppShadow.glowAurora,
                onPressed: () {},
              ),
            ),
            const SizedBox(width: AppSpacing.md),
            Expanded(
              child: GradientCard(
                title: l10n.dashboardActionFindCoach,
                icon: Icons.person_search_rounded,
                gradient: AppGradients.emerald,
                shadows: AppShadow.glowEmerald,
                onPressed: () {},
              ),
            ),
          ],
        ),
        const SizedBox(height: AppSpacing.md),
        Row(
          children: <Widget>[
            Expanded(
              child: GradientCard(
                title: l10n.dashboardActionLeaderboards,
                icon: Icons.emoji_events_rounded,
                gradient: AppGradients.sunset,
                shadows: AppShadow.glowSunset,
                onPressed: () {},
              ),
            ),
            const SizedBox(width: AppSpacing.md),
            Expanded(
              child: GradientCard(
                title: l10n.dashboardActionTournaments,
                icon: Icons.sports_soccer_rounded,
                gradient: AppGradients.primary,
                shadows: AppShadow.glowPrimary,
                onPressed: () {},
              ),
            ),
          ],
        ),
      ],
    );
  }
}

/// Tabbed dashboard content switching through an [AnimatedSwitcher].
class _DashboardTabs extends StatefulWidget {
  const _DashboardTabs({required this.l10n});

  final AppLocalizations l10n;

  @override
  State<_DashboardTabs> createState() => _DashboardTabsState();
}

class _DashboardTabsState extends State<_DashboardTabs> {
  int _index = 0;

  @override
  Widget build(BuildContext context) {
    final l10n = widget.l10n;
    return Column(
      crossAxisAlignment: CrossAxisAlignment.start,
      children: <Widget>[
        AnimatedTabBar(
          labels: <String>[
            l10n.dashboardTabOverview,
            l10n.dashboardTabTraining,
            l10n.dashboardTabInsights,
          ],
          index: _index,
          onChanged: (value) => setState(() => _index = value),
        ),
        const SizedBox(height: AppSpacing.xl),
        AnimatedSwitcher(
          duration: const Duration(milliseconds: 350),
          switchInCurve: Curves.easeOutCubic,
          switchOutCurve: Curves.easeInCubic,
          transitionBuilder: (child, animation) {
            return FadeTransition(
              opacity: animation,
              child: SlideTransition(
                position: Tween<Offset>(
                  begin: const Offset(0.04, 0),
                  end: Offset.zero,
                ).animate(animation),
                child: child,
              ),
            );
          },
          child: KeyedSubtree(
            key: ValueKey<int>(_index),
            child: _TabContent(index: _index, l10n: l10n),
          ),
        ),
      ],
    );
  }
}

/// Content shown for the currently active dashboard tab.
class _TabContent extends StatelessWidget {
  const _TabContent({required this.index, required this.l10n});

  final int index;
  final AppLocalizations l10n;

  @override
  Widget build(BuildContext context) {
    return switch (index) {
      0 => _WeeklyTargetCard(l10n: l10n),
      1 => _UpcomingSessions(l10n: l10n),
      _ => _Insights(l10n: l10n),
    };
  }
}

class _WeeklyTargetCard extends StatelessWidget {
  const _WeeklyTargetCard({required this.l10n});

  final AppLocalizations l10n;

  @override
  Widget build(BuildContext context) {
    return GlassCard(
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: <Widget>[
          Row(
            children: <Widget>[
              const Icon(
                Icons.track_changes_rounded,
                color: AppColors.cyan400,
                size: 20,
              ),
              const SizedBox(width: AppSpacing.sm),
              Text(
                l10n.dashboardWeeklyTarget,
                style: Theme.of(context).textTheme.titleMedium?.copyWith(
                  color: AppColors.surface,
                  fontWeight: FontWeight.w700,
                ),
              ),
              const Spacer(),
              Text(
                '78%',
                style: Theme.of(context).textTheme.titleLarge?.copyWith(
                  color: AppColors.surface,
                  fontWeight: FontWeight.w800,
                ),
              ),
            ],
          ),
          const SizedBox(height: AppSpacing.lg),
          const GradientProgress(
            value: 0.78,
            gradient: AppGradients.aurora,
            height: 12,
            shadows: AppShadow.glowAurora,
          ),
          const SizedBox(height: AppSpacing.lg),
          Text(
            l10n.dashboardWeeklyTargetDone,
            style: Theme.of(context).textTheme.bodyMedium?.copyWith(
              color: AppColors.grey300,
            ),
          ),
        ],
      ),
    );
  }
}

class _UpcomingSessions extends StatelessWidget {
  const _UpcomingSessions({required this.l10n});

  final AppLocalizations l10n;

  @override
  Widget build(BuildContext context) {
    return Column(
      children: <Widget>[
        _SessionRow(
          icon: Icons.directions_run_rounded,
          gradient: AppGradients.ocean,
          title: l10n.dashboardSessionEveningRun,
          time: l10n.dashboardSessionEveningRunTime,
        ),
        const SizedBox(height: AppSpacing.md),
        _SessionRow(
          icon: Icons.fitness_center_rounded,
          gradient: AppGradients.sunset,
          title: l10n.dashboardSessionSquadStrength,
          time: l10n.dashboardSessionSquadStrengthTime,
        ),
      ],
    );
  }
}

class _SessionRow extends StatelessWidget {
  const _SessionRow({
    required this.icon,
    required this.gradient,
    required this.title,
    required this.time,
  });

  final IconData icon;
  final LinearGradient gradient;
  final String title;
  final String time;

  @override
  Widget build(BuildContext context) {
    return GlassCard(
      padding: const EdgeInsets.all(AppSpacing.lg),
      child: Row(
        children: <Widget>[
          Container(
            width: 46,
            height: 46,
            decoration: BoxDecoration(
              shape: BoxShape.circle,
              gradient: gradient,
            ),
            child: Icon(icon, color: AppColors.surface, size: 22),
          ),
          const SizedBox(width: AppSpacing.md),
          Expanded(
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: <Widget>[
                Text(
                  title,
                  style: Theme.of(context).textTheme.titleSmall?.copyWith(
                    color: AppColors.surface,
                    fontWeight: FontWeight.w700,
                  ),
                ),
                const SizedBox(height: 2),
                Text(
                  time,
                  style: Theme.of(context).textTheme.bodySmall?.copyWith(
                    color: AppColors.grey300,
                  ),
                ),
              ],
            ),
          ),
          const Icon(
            Icons.chevron_right_rounded,
            color: AppColors.grey400,
          ),
        ],
      ),
    );
  }
}

class _Insights extends StatelessWidget {
  const _Insights({required this.l10n});

  final AppLocalizations l10n;

  @override
  Widget build(BuildContext context) {
    return Column(
      children: <Widget>[
        _InsightRow(
          icon: Icons.monitor_heart_rounded,
          color: AppColors.cyan400,
          text: l10n.dashboardInsightRecovery,
        ),
        const SizedBox(height: AppSpacing.md),
        _InsightRow(
          icon: Icons.speed_rounded,
          color: AppColors.accent,
          text: l10n.dashboardInsightIntensity,
        ),
        const SizedBox(height: AppSpacing.md),
        _InsightRow(
          icon: Icons.bedtime_rounded,
          color: AppColors.violet300,
          text: l10n.dashboardInsightRestDay,
        ),
      ],
    );
  }
}

class _InsightRow extends StatelessWidget {
  const _InsightRow({
    required this.icon,
    required this.color,
    required this.text,
  });

  final IconData icon;
  final Color color;
  final String text;

  @override
  Widget build(BuildContext context) {
    return GlassCard(
      padding: const EdgeInsets.all(AppSpacing.lg),
      child: Row(
        children: <Widget>[
          Container(
            width: 40,
            height: 40,
            decoration: BoxDecoration(
              shape: BoxShape.circle,
              color: color.withValues(alpha: 0.18),
            ),
            child: Icon(icon, color: color, size: 20),
          ),
          const SizedBox(width: AppSpacing.md),
          Expanded(
            child: Text(
              text,
              style: Theme.of(context).textTheme.bodyMedium?.copyWith(
                color: AppColors.surface,
                fontWeight: FontWeight.w600,
              ),
            ),
          ),
        ],
      ),
    );
  }
}

/// Uppercase section heading used across dashboard sections.
class _SectionTitle extends StatelessWidget {
  const _SectionTitle(this.text);

  final String text;

  @override
  Widget build(BuildContext context) {
    return Text(
      text,
      style: Theme.of(context).textTheme.titleLarge?.copyWith(
        color: AppColors.surface,
        fontWeight: FontWeight.w800,
      ),
    );
  }
}
