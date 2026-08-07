import 'package:drift/drift.dart';
import 'package:drift_flutter/drift_flutter.dart';

part 'app_database.g.dart';

/// Application-wide drift database.
///
/// P003 wiring only: no tables are defined yet. The database is constructed
/// through [driftDatabase], which resolves a native database on mobile/desktop
/// and a web database on the web. Real tables and the migration history land
/// in P004.
@DriftDatabase()
class AppDatabase extends _$AppDatabase {
  AppDatabase() : super(driftDatabase(name: 'sports_gurukul'));

  @override
  int get schemaVersion => 1;

  @override
  MigrationStrategy get migration => MigrationStrategy(
    onCreate: (m) async {
      await m.createAll();
    },
    onUpgrade: (m, from, to) async {
      // Migration steps are appended here as the schema evolves (P004).
    },
    beforeOpen: (details) async {
      await customStatement('PRAGMA foreign_keys = ON');
    },
  );
}
