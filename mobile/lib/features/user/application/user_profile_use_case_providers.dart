import 'package:flutter_riverpod/flutter_riverpod.dart';

import 'package:sports_gurukul/features/user/application/usecases/delete_profile_photo.dart';
import 'package:sports_gurukul/features/user/application/usecases/get_current_profile.dart';
import 'package:sports_gurukul/features/user/application/usecases/get_profile_photo.dart';
import 'package:sports_gurukul/features/user/application/usecases/update_preferences.dart';
import 'package:sports_gurukul/features/user/application/usecases/update_profile.dart';
import 'package:sports_gurukul/features/user/application/usecases/upload_profile_photo.dart';
import 'package:sports_gurukul/features/user/infrastructure/user_profile_infrastructure_providers.dart';

final getCurrentProfileProvider = Provider<GetCurrentProfile>(
  (ref) => GetCurrentProfile(ref.watch(userProfileRepositoryProvider)),
);

final updateProfileProvider = Provider<UpdateProfile>(
  (ref) => UpdateProfile(ref.watch(userProfileRepositoryProvider)),
);

final updatePreferencesProvider = Provider<UpdatePreferences>(
  (ref) => UpdatePreferences(ref.watch(userProfileRepositoryProvider)),
);

final uploadProfilePhotoProvider = Provider<UploadProfilePhoto>(
  (ref) => UploadProfilePhoto(ref.watch(userProfileRepositoryProvider)),
);

final getProfilePhotoProvider = Provider<GetProfilePhoto>(
  (ref) => GetProfilePhoto(ref.watch(userProfileRepositoryProvider)),
);

final deleteProfilePhotoProvider = Provider<DeleteProfilePhoto>(
  (ref) => DeleteProfilePhoto(ref.watch(userProfileRepositoryProvider)),
);
