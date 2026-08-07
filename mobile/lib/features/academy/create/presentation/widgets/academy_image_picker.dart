import 'dart:typed_data';

import 'package:flutter/material.dart';
import 'package:image_picker/image_picker.dart';

import 'package:sports_gurukul/app/theme/colors/app_colors.dart';
import 'package:sports_gurukul/app/theme/radius/app_radius.dart';
import 'package:sports_gurukul/app/theme/spacing/app_spacing.dart';
import 'package:sports_gurukul/l10n/generated/app_localizations.dart';

/// A pick-and-preview image field for the branding step.
///
/// Lets the user choose a [label] image (logo or cover) from the gallery and
/// previews the local [file]. When the flow edits an existing academy, an
/// optional [fallbackUrl] previews the currently stored image until a new one
/// is picked. Branding images are uploaded to the academy right after
/// creation/update and then shown on the academy-admin dashboard header.
class AcademyImagePicker extends StatelessWidget {
  const AcademyImagePicker({
    required this.label,
    required this.helpText,
    required this.file,
    required this.onPicked,
    required this.onRemoved,
    this.fallbackUrl,
    this.square = false,
    super.key,
  });

  final String label;
  final String helpText;
  final XFile? file;

  /// URL of the currently stored image, shown when no local [file] has been
  /// picked yet (edit flow prefill).
  final String? fallbackUrl;

  final ValueChanged<XFile> onPicked;
  final VoidCallback onRemoved;
  final bool square;

  Future<void> _pick(BuildContext context) async {
    final picker = ImagePicker();
    final picked = await picker.pickImage(
      source: ImageSource.gallery,
      maxWidth: 1600,
      maxHeight: square ? 1600 : 900,
      imageQuality: 90,
    );
    if (picked == null) {
      return;
    }
    onPicked(picked);
  }

  @override
  Widget build(BuildContext context) {
    final l10n = AppLocalizations.of(context);
    final hasFile = file != null;
    final previewSize = square ? 96.0 : 144.0;

    return Container(
      padding: const EdgeInsets.all(AppSpacing.md),
      decoration: BoxDecoration(
        borderRadius: BorderRadius.circular(AppRadius.large),
        border: Border.all(color: AppColors.glassBorderHi),
      ),
      child: Row(
        children: <Widget>[
          Container(
            width: previewSize,
            height: square ? previewSize : previewSize * 0.56,
            decoration: BoxDecoration(
              borderRadius: BorderRadius.circular(AppRadius.medium),
              color: AppColors.loginFieldFill,
            ),
            clipBehavior: Clip.antiAlias,
            child: hasFile
                ? _PreviewImage(file: file!)
                : fallbackUrl != null
                ? _NetworkPreviewImage(url: fallbackUrl!)
                : const Icon(
                    Icons.image_outlined,
                    color: AppColors.grey500,
                    size: 28,
                  ),
          ),
          const SizedBox(width: AppSpacing.lg),
          Expanded(
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: <Widget>[
                Text(
                  label,
                  style: Theme.of(context).textTheme.titleSmall?.copyWith(
                    color: AppColors.surface,
                    fontWeight: FontWeight.w700,
                  ),
                ),
                const SizedBox(height: AppSpacing.xs),
                Text(
                  helpText,
                  style: Theme.of(context).textTheme.bodySmall?.copyWith(
                    color: AppColors.grey300,
                    height: 1.35,
                  ),
                ),
                const SizedBox(height: AppSpacing.sm),
                Row(
                  children: <Widget>[
                    TextButton.icon(
                      onPressed: () => _pick(context),
                      icon: const Icon(Icons.photo_library_outlined, size: 18),
                      label: Text(
                        hasFile
                            ? l10n.academyReplaceImage
                            : l10n.academyChooseImage,
                      ),
                      style: TextButton.styleFrom(
                        foregroundColor: AppColors.blue500,
                      ),
                    ),
                    if (hasFile)
                      TextButton.icon(
                        onPressed: onRemoved,
                        icon: const Icon(Icons.delete_outline, size: 18),
                        label: Text(l10n.academyRemoveImage),
                        style: TextButton.styleFrom(
                          foregroundColor: AppColors.danger,
                        ),
                      ),
                  ],
                ),
              ],
            ),
          ),
        ],
      ),
    );
  }
}

/// Loads a picked image into memory for preview on any platform.
class _PreviewImage extends StatefulWidget {
  const _PreviewImage({required this.file});

  final XFile file;

  @override
  State<_PreviewImage> createState() => _PreviewImageState();
}

class _PreviewImageState extends State<_PreviewImage> {
  late Future<Uint8List> _bytes;

  @override
  void initState() {
    super.initState();
    _bytes = widget.file.readAsBytes();
  }

  @override
  void didUpdateWidget(_PreviewImage oldWidget) {
    super.didUpdateWidget(oldWidget);
    if (oldWidget.file.path != widget.file.path) {
      _bytes = widget.file.readAsBytes();
    }
  }

  @override
  Widget build(BuildContext context) {
    return FutureBuilder<Uint8List>(
      future: _bytes,
      builder: (context, snapshot) {
        final bytes = snapshot.data;
        if (bytes == null) {
          return const Center(
            child: Icon(
              Icons.image_outlined,
              color: AppColors.grey500,
              size: 28,
            ),
          );
        }
        return Image.memory(bytes, fit: BoxFit.cover, gaplessPlayback: true);
      },
    );
  }
}

/// Loads an existing remote image for preview, falling back to the placeholder
/// icon when the URL cannot be loaded.
class _NetworkPreviewImage extends StatelessWidget {
  const _NetworkPreviewImage({required this.url});

  final String url;

  @override
  Widget build(BuildContext context) {
    return Image.network(
      url,
      fit: BoxFit.cover,
      gaplessPlayback: true,
      errorBuilder: (_, _, _) => const Center(
        child: Icon(
          Icons.image_outlined,
          color: AppColors.grey500,
          size: 28,
        ),
      ),
    );
  }
}
