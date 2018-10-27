using IdentityModel.Client;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;

namespace WidgetApi.BasicAuthenticationHandler
{
    public interface IBasicAuthenticationService
    {
        Task<bool> IsValidUserAsync(BasicAuthenticationOptions options, string username, string password);
    }

    public class BasicAuthenticationService : IBasicAuthenticationService
    {
        public Task<bool> IsValidUserAsync(BasicAuthenticationOptions options, string username, string password)
        {
            var discoveryClient = new DiscoveryClient(options.DiscoveryUrl);
            var discoveryResponse = discoveryClient.GetAsync().Result;
            if (discoveryResponse.IsError)
            {
                return Task.FromResult(false);
            }

            var tokenClient = new TokenClient(discoveryResponse.TokenEndpoint, username, password);
            var tokenResponse = tokenClient.RequestClientCredentialsAsync(options.Scope).Result;
            if (tokenResponse.IsError)
            {
                return Task.FromResult(false);
            }

            return Task.FromResult(true);
        }
    }
}
