using System.Net.Http.Headers;
using System.Net.Http;
using System.Threading.Tasks;

namespace Booking.IntegrationTests
{
    public static class AuthenticatedHttpClientFactory
    {
        public static HttpClient CreateClientWithJwt(string jwtToken)
        {
            var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken);
            return client;
        }
    }
}
