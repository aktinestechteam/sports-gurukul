import 'dart:io';

import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:image_picker/image_picker.dart';

import 'package:sports_gurukul/app/theme/colors/app_colors.dart';
import 'package:sports_gurukul/app/theme/spacing/app_spacing.dart';
import 'package:sports_gurukul/features/user/domain/entities/user_profile.dart';
import 'package:sports_gurukul/features/user/presentation/providers/profile_controller.dart';
import 'package:sports_gurukul/features/user/presentation/widgets/profile_messages.dart';
import 'package:sports_gurukul/l10n/generated/app_localizations.dart';
import 'package:sports_gurukul/shared/cards/glass_card.dart';

/// Profile photo header with pick-and-upload integration.
///
/// Displays the current avatar (or an initials placeholder) and offers
/// upload / remove actions via an image picker. All network work is
/// delegated to [ProfileController]; this widget renders UI only.
class ProfilePhotoPicker extends ConsumerStatefulWidget {
  const ProfilePhotoPicker({
    required this.profile,
    super.key,
  });

  final UserProfile profile;

  @override
  ConsumerState<ProfilePhotoPicker> createState() => _ProfilePhotoPickerState();
}

class _ProfilePhotoPickerState extends ConsumerState<ProfilePhotoPicker> {
  bool _busy = false;

  Future<void> _pickAndUpload() async {
    final picker = ImagePicker();
    final picked = await picker.pickImage(
      source: ImageSource.gallery,
      maxWidth: 1200,
      maxHeight: 1200,
      imageQuality: 85,
    );
    if (picked == null || !mounted) {
      return;
    }
    setState(() => _busy = true);
    final result = await ref
        .read(profileControllerProvider.notifier)
        .uploadPhoto(File(picked.path));
    if (!mounted) {
      return;
    }
    setState(() => _busy = false);
    result.when(
      onSuccess: () {
        ScaffoldMessenger.of(context).showSnackBar(
          SnackBar(
            content: Text(
              AppLocalizations.of(context).profilePhotoUploaded,
            ),
          ),
        );
      },
      onFailure: (failure) {
        ScaffoldMessenger.of(context).showSnackBar(
          SnackBar(
            content: Text(ProfileMessages.failure(context, failure)),
          ),
        );
      },
    );
  }

  Future<void> _removePhoto() async {
    setState(() => _busy = true);
    final result = await ref
        .read(profileControllerProvider.notifier)
        .deletePhoto();
    if (!mounted) {
      return;
    }
    setState(() => _busy = false);
    result.when(
      onSuccess: () {
        ScaffoldMessenger.of(context).showSnackBar(
          SnackBar(
            content: Text(
              AppLocalizations.of(context).profilePhotoRemoved,
            ),
          ),
        );
      },
      onFailure: (failure) {
        ScaffoldMessenger.of(context).showSnackBar(
          SnackBar(
            content: Text(ProfileMessages.failure(context, failure)),
          ),
        );
      },
    );
  }

  @override
  Widget build(BuildContext context) {
    final l10n = AppLocalizations.of(context);
    final hasPhoto =
        widget.profile.profileImageUrl != null &&
        widget.profile.profileImageUrl!.isNotEmpty;

    return GlassCard(
      child: Column(
        children: <Widget>[
          Stack(
            alignment: Alignment.bottomRight,
            children: <Widget>[
              _Avatar(
                imageUrl: widget.profile.profileImageUrl,
                name: widget.profile.fullName,
              ),
              if (_busy)
                const Positioned(
                  right: AppSpacing.none,
                  bottom: AppSpacing.none,
                  child: SizedBox(
                    width: 28,
                    height: 28,
                    child: CircularProgressIndicator(strokeWidth: 2.5),
                  ),
                )
              else
                Positioned(
                  right: AppSpacing.none,
                  bottom: AppSpacing.none,
                  child: IconButton.filled(
                    icon: const Icon(Icons.photo_camera_outlined, size: 18),
                    tooltip: l10n.profileChangePhoto,
                    onPressed: _busy ? null : _pickAndUpload,
                  ),
                ),
            ],
          ),
          const SizedBox(height: AppSpacing.lg),
          Text(
            widget.profile.fullName,
            textAlign: TextAlign.center,
            style: const TextStyle(
              fontSize: 20,
              fontWeight: FontWeight.w700,
              color: AppColors.surface,
            ),
          ),
          const SizedBox(height: AppSpacing.xs),
          Text(
            widget.profile.email,
            textAlign: TextAlign.center,
            style: const TextStyle(
              fontSize: 14,
              color: AppColors.grey300,
            ),
          ),
          if (hasPhoto) ...<Widget>[
            const SizedBox(height: AppSpacing.lg),
            TextButton.icon(
              onPressed: _busy ? null : _removePhoto,
              style: TextButton.styleFrom(foregroundColor: AppColors.danger),
              icon: const Icon(Icons.delete_outline, size: 18),
              label: Text(l10n.profileRemovePhoto),
            ),
          ],
        ],
      ),
    );
  }
}

/// Circular avatar backed by the profile image URL or initials.
class _Avatar extends StatelessWidget {
  const _Avatar({required this.imageUrl, required this.name});

  final String? imageUrl;
  final String name;

  @override
  Widget build(BuildContext context) {
    const size = 96.0;
    final hasImage = imageUrl != null && imageUrl!.isNotEmpty;
    final initials = name
        .split(' ')
        .where((part) => part.isNotEmpty)
        .take(2)
        .map((part) => part[0].toUpperCase())
        .join();

    return Container(
      width: size,
      height: size,
      decoration: const BoxDecoration(
        shape: BoxShape.circle,
        gradient: LinearGradient(
          begin: Alignment.topLeft,
          end: Alignment.bottomRight,
          colors: <Color>[AppColors.primary400, AppColors.violet500],
        ),
      ),
      clipBehavior: Clip.antiAlias,
      alignment: Alignment.center,
      child: hasImage
          ? Image.network(
              imageUrl!,
              width: size,
              height: size,
              fit: BoxFit.cover,
              errorBuilder: (_, _, _) => _InitialsLabel(initials: initials),
              loadingBuilder: (context, child, progress) {
                if (progress == null) {
                  return child;
                }
                return const Center(
                  child: SizedBox(
                    width: 24,
                    height: 24,
                    child: CircularProgressIndicator(strokeWidth: 2),
                  ),
                );
              },
            )
          : _InitialsLabel(initials: initials),
    );
  }
}

class _InitialsLabel extends StatelessWidget {
  const _InitialsLabel({required this.initials});

  final String initials;

  @override
  Widget build(BuildContext context) {
    return Text(
      initials.isEmpty ? '?' : initials,
      style: const TextStyle(
        color: AppColors.surface,
        fontSize: 32,
        fontWeight: FontWeight.w700,
      ),
    );
  }
}
