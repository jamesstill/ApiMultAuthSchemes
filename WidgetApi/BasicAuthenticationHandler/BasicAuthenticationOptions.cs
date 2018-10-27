using Microsoft.AspNetCore.Authentication;

namespace WidgetApi.BasicAuthenticationHandler
{
    public class BasicAuthenticationOptions : AuthenticationSchemeOptions
    {
        public string DiscoveryUrl { get; set; }
        public string Scope { get; set; }
    }
}
