using Xunit;
using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using Newtonsoft.Json;
using System.Text;
using SportsGurukul.Application.Features.Bookings.Commands.CreateBooking;
using SportsGurukul.Application.Features.Bookings.Commands.UpdateBooking;
using SportsGurukul.Application.Features.Bookings.Queries.GetBookingById;

namespace Booking.IntegrationTests
{
    public class BookingTests : BaseIntegrationTest
    {
        public BookingTests(TestWebApplicationFactory factory) : base(factory)
        {
        }

        [Fact]
        public async Task CreateBooking_ReturnsSuccessStatusCodeAndBookingId()
        {
            // Arrange
            var command = new CreateBookingCommand
            {
                AthleteId = Guid.NewGuid(),
                CoachId = Guid.NewGuid(),
                ActivityId = Guid.NewGuid(),
                StartTime = DateTime.UtcNow.AddHours(1),
                EndTime = DateTime.UtcNow.AddHours(2),
                DurationMinutes = 60,
                Status = SportsGurukul.Domain.Enums.BookingStatus.Pending
            };
            var jsonContent = new StringContent(JsonConvert.SerializeObject(command), Encoding.UTF8, "application/json");

            // Act
            var response = await HttpClient.PostAsync("/api/v1/bookings", jsonContent);

            // Assert
            response.EnsureSuccessStatusCode();
            var responseString = await response.Content.ReadAsStringAsync();
            var bookingId = JsonConvert.DeserializeObject<Guid>(responseString);
            bookingId.Should().NotBeEmpty();
        }
    }
}
