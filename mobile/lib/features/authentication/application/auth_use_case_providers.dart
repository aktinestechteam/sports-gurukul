import 'package:flutter_riverpod/flutter_riverpod.dart';

import 'package:sports_gurukul/features/authentication/application/usecases/forgot_password.dart';
import 'package:sports_gurukul/features/authentication/application/usecases/login_user.dart';
import 'package:sports_gurukul/features/authentication/application/usecases/logout_user.dart';
import 'package:sports_gurukul/features/authentication/application/usecases/refresh_session.dart';
import 'package:sports_gurukul/features/authentication/application/usecases/register_user.dart';
import 'package:sports_gurukul/features/authentication/application/usecases/reset_password.dart';
import 'package:sports_gurukul/features/authentication/application/usecases/send_verification_email.dart';
import 'package:sports_gurukul/features/authentication/application/usecases/verify_email.dart';
import 'package:sports_gurukul/features/authentication/infrastructure/auth_infrastructure_providers.dart';

final loginUserProvider = Provider<LoginUser>(
  (ref) => LoginUser(ref.watch(authRepositoryProvider)),
);

final registerUserProvider = Provider<RegisterUser>(
  (ref) => RegisterUser(ref.watch(authRepositoryProvider)),
);

final refreshSessionProvider = Provider<RefreshSession>(
  (ref) => RefreshSession(ref.watch(authRepositoryProvider)),
);

final logoutUserProvider = Provider<LogoutUser>(
  (ref) => LogoutUser(ref.watch(authRepositoryProvider)),
);

final forgotPasswordProvider = Provider<ForgotPassword>(
  (ref) => ForgotPassword(ref.watch(authRepositoryProvider)),
);

final resetPasswordProvider = Provider<ResetPassword>(
  (ref) => ResetPassword(ref.watch(authRepositoryProvider)),
);

final sendVerificationEmailProvider = Provider<SendVerificationEmail>(
  (ref) => SendVerificationEmail(ref.watch(authRepositoryProvider)),
);

final verifyEmailProvider = Provider<VerifyEmail>(
  (ref) => VerifyEmail(ref.watch(authRepositoryProvider)),
);
