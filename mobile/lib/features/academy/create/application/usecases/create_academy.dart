import 'package:sports_gurukul/core/result/result.dart';
import 'package:sports_gurukul/features/academy/create/domain/entities/academy.dart';
import 'package:sports_gurukul/features/academy/create/domain/entities/create_academy_params.dart';
import 'package:sports_gurukul/features/academy/create/domain/repositories/create_academy_repository.dart';

/// Creates a new academy from the wizard's collected [CreateAcademyParams].
class CreateAcademy {
  const CreateAcademy(this._repository);

  final CreateAcademyRepository _repository;

  Future<Result<Academy>> call(CreateAcademyParams params) =>
      _repository.createAcademy(params);
}
