import 'package:flutter_test/flutter_test.dart';
import 'package:sports_gurukul/shared/models/sample_model.dart';

void main() {
  group('SampleModel', () {
    const model = SampleModel(id: 'id-1', name: 'sample', count: 3);

    test('exposes fields', () {
      expect(model.id, 'id-1');
      expect(model.name, 'sample');
      expect(model.count, 3);
    });

    test('supports copyWith', () {
      final copy = model.copyWith(name: 'updated');
      expect(copy.name, 'updated');
      expect(copy.id, 'id-1');
    });

    test('respects value equality', () {
      expect(model, const SampleModel(id: 'id-1', name: 'sample', count: 3));
      expect(model, isNot(const SampleModel(id: 'id-2', name: 'sample')));
    });

    test('round-trips through JSON', () {
      final json = model.toJson();
      expect(SampleModel.fromJson(json), model);
    });
  });
}
