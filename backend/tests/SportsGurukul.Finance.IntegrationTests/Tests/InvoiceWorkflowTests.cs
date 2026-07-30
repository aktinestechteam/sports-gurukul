using FluentAssertions;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.FinanceManagement.Commands.Invoice;
using SportsGurukul.Application.Features.FinanceManagement.DTOs;
using SportsGurukul.Application.Features.FinanceManagement.Queries;
using SportsGurukul.Domain.Enums.Finance;
using SportsGurukul.Finance.IntegrationTests.Fixtures;
using SportsGurukul.Finance.IntegrationTests.Helpers;
using SportsGurukul.Finance.IntegrationTests.Seed;
using Xunit;

namespace SportsGurukul.Finance.IntegrationTests.Tests;

[Collection("Finance")]
public class InvoiceWorkflowTests : FinanceTestBase
{
    public InvoiceWorkflowTests(FinanceWebApplicationFactory factory) : base(factory)
    {
    }

    [Fact]
    public async Task CreateInvoice_WithValidData_ReturnsSuccess()
    {
        var command = new CreateInvoiceCommand(
            Description: "Monthly training fees",
            DueDate: DateTime.UtcNow.AddDays(30),
            Currency: "INR",
            AthleteId: FinanceTestIds.AthleteUserId,
            AcademyId: FinanceTestIds.AcademyUserId,
            LineItems: new List<CreateInvoiceLineItemDto>
            {
                new("Training sessions", "Service", null, 1, 5000m, null)
            },
            CouponCode: null,
            ScholarshipId: null);

        var result = await Mediator.Send(command);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value!.InvoiceNumber.Should().NotBeNullOrWhiteSpace();
        result.Value.AthleteId.Should().Be(FinanceTestIds.AthleteUserId);
        result.Value.Status.Should().Be(InvoiceStatus.Draft);
    }

    [Fact]
    public async Task CreateInvoice_WithInvalidData_ReturnsFailure()
    {
        var command = new CreateInvoiceCommand(
            Description: null,
            DueDate: null,
            Currency: "INR",
            AthleteId: null,
            AcademyId: null,
            LineItems: new List<CreateInvoiceLineItemDto>(),
            CouponCode: null,
            ScholarshipId: null);

        var result = await Mediator.Send(command);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().NotBeNullOrWhiteSpace();
    }

    [Fact]
    public async Task GetInvoiceById_ReturnsCorrectData()
    {
        var createResult = await Mediator.Send(new CreateInvoiceCommand(
            Description: "Equipment charges",
            DueDate: DateTime.UtcNow.AddDays(15),
            Currency: "INR",
            AthleteId: FinanceTestIds.AthleteUserId,
            AcademyId: FinanceTestIds.AcademyUserId,
            LineItems: new List<CreateInvoiceLineItemDto>
            {
                new("Cricket kit", "Equipment", null, 1, 12000m, null)
            },
            CouponCode: null,
            ScholarshipId: null));
        createResult.IsSuccess.Should().BeTrue();

        var query = new GetInvoiceByIdQuery(createResult.Value!.Id);

        var result = await Mediator.Send(query);

        result.IsSuccess.Should().BeTrue();
        result.Value!.Id.Should().Be(createResult.Value.Id);
        result.Value.Description.Should().Be("Equipment charges");
        result.Value.TotalAmount.Should().Be(12000m);
    }

    [Fact]
    public async Task IssueInvoice_ChangesStatus()
    {
        var createResult = await Mediator.Send(new CreateInvoiceCommand(
            Description: "Issue test",
            DueDate: DateTime.UtcNow.AddDays(10),
            Currency: "INR",
            AthleteId: FinanceTestIds.AthleteUserId,
            AcademyId: FinanceTestIds.AcademyUserId,
            LineItems: new List<CreateInvoiceLineItemDto>
            {
                new("Test service", "Service", null, 1, 3000m, null)
            },
            CouponCode: null,
            ScholarshipId: null));
        createResult.IsSuccess.Should().BeTrue();

        var command = new IssueInvoiceCommand(createResult.Value!.Id);

        var result = await Mediator.Send(command);

        result.IsSuccess.Should().BeTrue();
        result.Value!.Status.Should().Be(InvoiceStatus.Issued);
        result.Value.IssuedAt.Should().NotBeNull();
    }

    [Fact]
    public async Task CancelInvoice_ChangesStatusToCancelled()
    {
        var createResult = await Mediator.Send(new CreateInvoiceCommand(
            Description: "Cancel test",
            DueDate: DateTime.UtcNow.AddDays(5),
            Currency: "INR",
            AthleteId: FinanceTestIds.AthleteUserId,
            AcademyId: FinanceTestIds.AcademyUserId,
            LineItems: new List<CreateInvoiceLineItemDto>
            {
                new("Cancellable service", "Service", null, 1, 2000m, null)
            },
            CouponCode: null,
            ScholarshipId: null));
        createResult.IsSuccess.Should().BeTrue();

        var command = new CancelInvoiceCommand(createResult.Value!.Id, "Customer requested cancellation");

        var result = await Mediator.Send(command);

        result.IsSuccess.Should().BeTrue();
        result.Value!.Status.Should().Be(InvoiceStatus.Cancelled);
    }

    [Fact]
    public async Task MarkInvoiceAsPaid_UpdatesAmountPaid()
    {
        var createResult = await Mediator.Send(new CreateInvoiceCommand(
            Description: "Paid invoice test",
            DueDate: DateTime.UtcNow.AddDays(10),
            Currency: "INR",
            AthleteId: FinanceTestIds.AthleteUserId,
            AcademyId: FinanceTestIds.AcademyUserId,
            LineItems: new List<CreateInvoiceLineItemDto>
            {
                new("Paid service", "Service", null, 1, 8000m, null)
            },
            CouponCode: null,
            ScholarshipId: null));
        createResult.IsSuccess.Should().BeTrue();

        var issueResult = await Mediator.Send(new IssueInvoiceCommand(createResult.Value!.Id));
        issueResult.IsSuccess.Should().BeTrue();

        var command = new MarkInvoiceAsPaidCommand(createResult.Value.Id);

        var result = await Mediator.Send(command);

        result.IsSuccess.Should().BeTrue();
        result.Value!.Status.Should().Be(InvoiceStatus.Paid);
        result.Value.PaidAmount.Should().Be(createResult.Value.TotalAmount);
        result.Value.PaidAt.Should().NotBeNull();
    }

    [Fact]
    public async Task SearchInvoices_ReturnsFilteredResults()
    {
        await Mediator.Send(new CreateInvoiceCommand(
            Description: "Searchable invoice Alpha",
            DueDate: DateTime.UtcNow.AddDays(7),
            Currency: "INR",
            AthleteId: FinanceTestIds.AthleteUserId,
            AcademyId: FinanceTestIds.AcademyUserId,
            LineItems: new List<CreateInvoiceLineItemDto>
            {
                new("Search test", "Service", null, 1, 1500m, null)
            },
            CouponCode: null,
            ScholarshipId: null));

        var query = new SearchInvoicesQuery(
            SearchTerm: "Searchable invoice",
            Status: null,
            AthleteId: FinanceTestIds.AthleteUserId,
            AcademyId: null,
            FromDate: null,
            ToDate: null,
            Page: 1,
            PageSize: 20);

        var result = await Mediator.Send(query);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeEmpty();
        result.Value.Should().Contain(i => i.AthleteName == "Test Athlete");
    }

    [Fact]
    public async Task GetOutstandingInvoices_ReturnsUnpaid()
    {
        var createResult = await Mediator.Send(new CreateInvoiceCommand(
            Description: "Outstanding invoice",
            DueDate: DateTime.UtcNow.AddDays(-1),
            Currency: "INR",
            AthleteId: FinanceTestIds.AthleteUserId,
            AcademyId: FinanceTestIds.AcademyUserId,
            LineItems: new List<CreateInvoiceLineItemDto>
            {
                new("Overdue service", "Service", null, 1, 4000m, null)
            },
            CouponCode: null,
            ScholarshipId: null));
        createResult.IsSuccess.Should().BeTrue();

        await Mediator.Send(new IssueInvoiceCommand(createResult.Value!.Id));

        var query = new GetOutstandingInvoicesQuery();

        var result = await Mediator.Send(query);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Contain(i => i.Id == createResult.Value.Id);
    }

    [Fact]
    public async Task GetInvoiceReceipt_ReturnsReceiptData()
    {
        var createResult = await Mediator.Send(new CreateInvoiceCommand(
            Description: "Receipt test",
            DueDate: DateTime.UtcNow.AddDays(20),
            Currency: "INR",
            AthleteId: FinanceTestIds.AthleteUserId,
            AcademyId: FinanceTestIds.AcademyUserId,
            LineItems: new List<CreateInvoiceLineItemDto>
            {
                new("Receipt item", "Service", null, 2, 2500m, null)
            },
            CouponCode: null,
            ScholarshipId: null));
        createResult.IsSuccess.Should().BeTrue();

        await Mediator.Send(new IssueInvoiceCommand(createResult.Value!.Id));
        await Mediator.Send(new MarkInvoiceAsPaidCommand(createResult.Value.Id));

        var query = new GetInvoiceReceiptQuery(createResult.Value.Id);

        var result = await Mediator.Send(query);

        result.IsSuccess.Should().BeTrue();
        result.Value!.InvoiceNumber.Should().NotBeNullOrWhiteSpace();
        result.Value.TotalAmount.Should().BeGreaterThan(0);
        result.Value.LineItems.Should().NotBeEmpty();
    }
}
