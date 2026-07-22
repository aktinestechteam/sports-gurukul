import 'package:flutter_test/flutter_test.dart';
import 'package:sports_gurukul/app/app.dart';

void main() {
  testWidgets('App renders', (WidgetTester tester) async {
    await tester.pumpWidget(const SportsGurukulApp());
    expect(find.text('Sports Gurukul'), findsOneWidget);
  });
}
