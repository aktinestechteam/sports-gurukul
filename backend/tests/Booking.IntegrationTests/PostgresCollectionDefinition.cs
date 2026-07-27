namespace Booking.IntegrationTests;

[CollectionDefinition("Postgres")]
public sealed class PostgresCollectionDefinition : ICollectionFixture<TestWebApplicationFactory>
{
}
