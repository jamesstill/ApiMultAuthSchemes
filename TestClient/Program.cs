using System;
using System.Globalization;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using IdentityModel.Client;

namespace TestClient
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Test client for Ocelot, IdentityServer4, and an internal ASP.NET Core 2.1 API");
            Console.WriteLine(Environment.NewLine);

            TokenAuthenticationAsync().GetAwaiter().GetResult();
            BasicAuthenticationAsync().GetAwaiter().GetResult();
            HMACAuthenticationAsync().GetAwaiter().GetResult();

            Console.WriteLine("Press ENTER to quit.");
            Console.ReadLine();
        }

        private static async Task TokenAuthenticationAsync()
        {
            try
            {
                Console.WriteLine("Token Authentication Example");
                Console.WriteLine("Calling IdentityServer4 discovery endpoint...");
                Console.WriteLine(Environment.NewLine);

                var discoveryUrl = "https://localhost:44386/";
                var discoveryClient = new DiscoveryClient(discoveryUrl);
                var discoveryResponse = await discoveryClient.GetAsync();
                if (discoveryResponse.IsError)
                {
                    throw new Exception("Failed to get discovery response!");
                }

                Console.WriteLine("Calling IdentityServer4 authorize endpoint to get request token...");
                Console.WriteLine(Environment.NewLine);

                var uri = "https://localhost:44336/api/v1/widget";
                var clientId = "WidgetClient";
                var clientSecret = "p@ssw0rd";
                var scope = "widgetapi";
                var tokenClient = new TokenClient(discoveryResponse.TokenEndpoint, clientId, clientSecret);
                var tokenResponse = await tokenClient.RequestClientCredentialsAsync(scope);
                if (tokenResponse.IsError)
                {
                    throw new Exception("Authentication failed!");
                }

                Console.WriteLine("Calling Ocelot endpoint with bearer token to get widgets...");
                Console.WriteLine(Environment.NewLine);

                var gatewayClient = new HttpClient();
                gatewayClient.SetBearerToken(tokenResponse.AccessToken);
                var response = await gatewayClient.GetAsync(uri);
                if (response.IsSuccessStatusCode)
                {
                    Console.WriteLine("Received HTTP 200 from API. Writing widgets to console...");
                    Console.WriteLine(Environment.NewLine);

                    var content = await response.Content.ReadAsStringAsync();
                    Console.WriteLine(content);
                }
                else
                {
                    Console.WriteLine("Response was unsuccessful with status code: " + response.StatusCode);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            Console.WriteLine(Environment.NewLine);
        }

        private static async Task BasicAuthenticationAsync()
        {
            try
            {
                await Task.Delay(1000); // Ocelot rate limit in place
                Console.WriteLine("Basic Authentication Example");
                Console.WriteLine(Environment.NewLine);

                var uri = "https://localhost:44336/api/v1/widget";
                var clientId = "WidgetClient";
                var clientSecret = "p@ssw0rd";
                var credentials = Convert.ToBase64String(Encoding.ASCII.GetBytes(clientId + ":" + clientSecret));

                var gatewayClient = new HttpClient();
                gatewayClient.DefaultRequestHeaders.Clear();
                gatewayClient.DefaultRequestHeaders.Add("Authorization", "Basic " + credentials);
                var response = await gatewayClient.GetAsync(uri);
                if (response.IsSuccessStatusCode)
                {
                    Console.WriteLine("Received HTTP 200 from API. Writing widgets to console...");
                    Console.WriteLine(Environment.NewLine);

                    var content = await response.Content.ReadAsStringAsync();
                    Console.WriteLine(content);
                }
                else
                {
                    Console.WriteLine("Response was unsuccessful with status code: " + response.StatusCode);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            Console.WriteLine(Environment.NewLine);
        }

        private static async Task HMACAuthenticationAsync()
        {
            try
            {
                await Task.Delay(1000); // Ocelot rate limit in place
                Console.WriteLine("HMAC Authentication Example");
                Console.WriteLine(Environment.NewLine);

                var uri = "https://localhost:44336/api/v1/widget";
                var clientId = "WidgetClient";
                var clientSecret = "1F92F130-D8E5-45D6-9792-9138F29920E0";
                var authenticationHeaderName = "Authentication";
                var timestampHeaderName = "Timestamp";
                var timestamp = DateTime.UtcNow;
                var timestampValue = timestamp.ToString("o", CultureInfo.InvariantCulture);
                var ticks = timestamp.Ticks.ToString(CultureInfo.InvariantCulture);

                string hashValue;
                var key = Encoding.UTF8.GetBytes(clientSecret);
                using (var hmac = new HMACSHA256(key))
                {
                    var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(ticks));
                    var hashString = Convert.ToBase64String(hash);
                    hashValue = string.Format("{0}:{1}", clientId, hashString);
                }

                var gatewayClient = new HttpClient();
                gatewayClient.DefaultRequestHeaders.Clear();
                gatewayClient.DefaultRequestHeaders.Add(timestampHeaderName, timestampValue);
                gatewayClient.DefaultRequestHeaders.Add(authenticationHeaderName, hashValue);
                var response = await gatewayClient.GetAsync(uri);
                if (response.IsSuccessStatusCode)
                {
                    Console.WriteLine("Received HTTP 200 from API. Writing widgets to console...");
                    Console.WriteLine(Environment.NewLine);

                    var content = await response.Content.ReadAsStringAsync();
                    Console.WriteLine(content);
                }
                else
                {
                    Console.WriteLine("Response was unsuccessful with status code: " + response.StatusCode);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            Console.WriteLine(Environment.NewLine);
        }
    }
}
