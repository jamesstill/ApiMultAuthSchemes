using Microsoft.AspNetCore.Authentication;

namespace WidgetApi.HMACAuthenticationHandler
{
    public class HMACAuthenticationOptions : AuthenticationSchemeOptions
    {
        public string Key { get; set; }
    }
}