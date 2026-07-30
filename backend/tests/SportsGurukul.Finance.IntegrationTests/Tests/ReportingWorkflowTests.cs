using FluentAssertions;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.FinanceManagement.Queries;
using SportsGurukul.Finance.IntegrationTests.Fixtures;
using SportsGurukul.Finance.IntegrationTests.Helpers;
using Xunit;

namespace SportsGurukul.Finance.IntegrationTests.Tests;

[Collection("Finance")]
public class ReportingWorkflowTests : FinanceTestBase
{
    public ReportingWorkflowTests(FinanceWebApplicationFactory factory) : base(factory) { }

    [Fact]
    public async Task GetRevenueReport_ReturnsRevenueData()
    {
        var result = await SendAsync(new GetRevenueQuery(null, null));

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
    }

    [Fact]
    public async Task GetFinanceDashboard_ReturnsDashboardData()
    {
        var result = await SendAsync(new GetFinanceDashboardQuery());

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
    }

    [Fact]
    public async Task GetFinanceReports_ReturnsReportData()
    {
        var result = await SendAsync(new GetFinanceReportsQuery(null, null));

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
    }

    [Fact]
    public async Task FinanceSearch_ReturnsSearchResults()
    {
        var result = await SendAsync(new FinanceSearchQuery(
            null, null, null, null, 1, 20));

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
    }
}
