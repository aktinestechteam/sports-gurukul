using Xunit;

namespace SportsGurukul.Finance.IntegrationTests.Fixtures;

[CollectionDefinition("Finance")]
public class FinanceTestCollection : ICollectionFixture<FinanceWebApplicationFactory>
{
}
